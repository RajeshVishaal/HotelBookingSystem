using Common.Dto;
using InventoryService.Application.Dto;
using InventoryService.Domain.Entities;
using InventoryService.Infrastructure.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace InventoryService.Infrastructure.Data.Repositories;

public class HotelRepository : IHotelRepository
{
    private readonly AppDbContext _db;

    public HotelRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<Hotel?> GetByIdAsync(Guid hotelId)
    {
        return await _db.Hotels
            .Include(h => h.Rooms)
            .FirstOrDefaultAsync(h => h.Id == hotelId);
    }

    public async Task<PaginatedResponse<HotelSearchResult>> SearchHotelsAsync(
        string? name,
        DateOnly? fromDate,
        DateOnly? toDate,
        int? guests,
        int skip,
        int take)
    {
        var query = _db.Hotels.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(name))
        {
            var filter = $"%{name.ToLower()}%";
            query = query.Where(h =>
                EF.Functions.ILike(h.Name, filter) ||
                EF.Functions.ILike(h.City, filter));
        }

        var totalCount = await query.CountAsync();

        var hotels = await query
            .OrderBy(h => h.Name)
            .Skip(skip)
            .Take(take)
            .Select(h => new HotelSearchResult
            {
                HotelId = h.Id,
                Name = h.Name,
                City = h.City,
                Country = h.Country,
                AddressLine = h.AddressLine,
                ImageUrl = h.ImageUrl,
                AverageRating = h.AverageRating,
                TotalReviews = h.TotalReviews ?? 0,
                Comment = h.Comment ?? string.Empty
            })
            .AsSingleQuery()
            .ToListAsync();

        if (!fromDate.HasValue || !toDate.HasValue || !guests.HasValue)
            return await BuildHotelResponseWithoutAvailabilityAsync(hotels, totalCount, skip, take);

        var totalNights = (toDate.Value.ToDateTime(TimeOnly.MinValue) - fromDate.Value.ToDateTime(TimeOnly.MinValue))
            .Days;
        if (totalNights <= 0)
            return EmptyHotelResponse(skip, take);

        var hotelIds = hotels.Select(h => h.HotelId).ToList();

        var availableRooms = await (
            from rc in _db.RoomCategories.AsNoTracking()
            join rt in _db.RoomTypes.AsNoTracking() on rc.RoomTypeId equals rt.Id
            join rp in _db.RoomPricings.AsNoTracking() on rc.Id equals rp.RoomCategoryId
            where hotelIds.Contains(rc.HotelId)
                  && _db.RoomAvailabilities
                      .Where(a => a.Date >= fromDate && a.Date < toDate && a.TotalCount - a.TotalBooked > 0)
                      .GroupBy(a => a.RoomCategoryId)
                      .Where(g => g.Select(a => a.Date).Distinct().Count() == totalNights)
                      .Select(g => g.Key)
                      .Contains(rc.Id)
            select new
            {
                rc.HotelId,
                rc.Id,
                RoomTypeName = rt.Name,
                rt.MaximumGuests,
                rp.BaseRate
            }).ToListAsync();

        var result = hotels
            .Select(h =>
            {
                var roomCategories = availableRooms
                    .Where(r => r.HotelId == h.HotelId)
                    .Select(r => new RoomSearchResult
                    {
                        Id = r.Id,
                        RoomTypeName = r.RoomTypeName,
                        MaximumGuests = r.MaximumGuests,
                        BaseRate = r.BaseRate * totalNights,
                        Info = $"{guests} guests, {totalNights} night{(totalNights == 1 ? "" : "s")}"
                    })
                    .ToList();

                if (!roomCategories.Any())
                    return null;

                return new HotelSearchResult
                {
                    HotelId = h.HotelId,
                    Name = h.Name,
                    City = h.City,
                    Country = h.Country,
                    AddressLine = h.AddressLine,
                    ImageUrl = h.ImageUrl,
                    AverageRating = h.AverageRating,
                    TotalReviews = h.TotalReviews,
                    Comment = h.Comment,
                    RoomCategories = roomCategories
                };
            })
            .Where(h => h != null)
            .ToList()!;

        return BuildPagedResponse(result, totalCount, skip, take);
    }

    private async Task<PaginatedResponse<HotelSearchResult>> BuildHotelResponseWithoutAvailabilityAsync(
        List<HotelSearchResult> hotels,
        int totalCount,
        int skip,
        int take)
    {
        var hotelIds = hotels.Select(h => h.HotelId).ToList();

        var roomData = await (
            from rc in _db.RoomCategories.AsNoTracking()
            join rt in _db.RoomTypes.AsNoTracking() on rc.RoomTypeId equals rt.Id
            join rp in _db.RoomPricings.AsNoTracking() on rc.Id equals rp.RoomCategoryId
            where hotelIds.Contains(rc.HotelId)
            select new
            {
                rc.HotelId,
                RoomCategoryId = rc.Id,
                RoomTypeName = rt.Name,
                rt.MaximumGuests,
                rp.BaseRate
            }).ToListAsync();

        var hotelsWithRooms = hotels.Select(h => new HotelSearchResult
        {
            HotelId = h.HotelId,
            Name = h.Name,
            City = h.City,
            Country = h.Country,
            AddressLine = h.AddressLine,
            ImageUrl = h.ImageUrl,
            AverageRating = h.AverageRating,
            TotalReviews = h.TotalReviews,
            Comment = h.Comment,
            RoomCategories = roomData
                .Where(r => r.HotelId == h.HotelId)
                .Select(r => new RoomSearchResult
                {
                    Id = r.RoomCategoryId,
                    RoomTypeName = r.RoomTypeName,
                    MaximumGuests = r.MaximumGuests,
                    BaseRate = r.BaseRate,
                    Info = $"{r.MaximumGuests} guests"
                })
                .ToList()
        }).ToList();

        return BuildPagedResponse(hotelsWithRooms, totalCount, skip, take);
    }


    public async Task<RoomReservationReceipt> ReserveRoomsAsync(
        Guid hotelId,
        List<RoomReservationItem> rooms,
        DateOnly checkIn,
        DateOnly checkOut)
    {
        using var transaction = await _db.Database.BeginTransactionAsync();

        try
        {
            var totalNights = (checkOut.ToDateTime(TimeOnly.MinValue) - checkIn.ToDateTime(TimeOnly.MinValue)).Days;
            if (totalNights <= 0)
                throw new InvalidOperationException("Invalid date range.");

            var categoryIds = rooms.Select(r => r.RoomCategoryId).Distinct().ToList();
            var pricingMap = await _db.RoomPricings
                .Where(p => categoryIds.Contains(p.RoomCategoryId))
                .ToDictionaryAsync(p => p.RoomCategoryId, p => p.BaseRate);

            foreach (var room in rooms)
                for (var date = checkIn; date < checkOut; date = date.AddDays(1))
                {
                    var success = false;
                    var retries = 3;

                    while (!success && retries > 0)
                        try
                        {
                            var availability = await _db.RoomAvailabilities
                                .FirstOrDefaultAsync(a =>
                                    a.HotelId == hotelId &&
                                    a.RoomCategoryId == room.RoomCategoryId &&
                                    a.Date == date);

                            if (availability == null)
                                throw new InvalidOperationException($"No availability data for {date}.");

                            var available = availability.TotalCount - availability.TotalBooked;

                            if (available < room.Quantity)
                            {
                                var message = available switch
                                {
                                    0 => $"No rooms left for category {room.RoomCategoryId} on {date}.",
                                    1 => $"Only 1 room left for category {room.RoomCategoryId} on {date}.",
                                    _ => $"Only {available} rooms left for category {room.RoomCategoryId} on {date}."
                                };
                                throw new InvalidOperationException(message);
                            }

                            availability.TotalBooked += room.Quantity;

                            await _db.SaveChangesAsync();
                            success = true;
                        }
                        catch (DbUpdateConcurrencyException)
                        {
                            retries--;
                            if (retries == 0)
                                throw new InvalidOperationException(
                                    $"Concurrency conflict for room {room.RoomCategoryId} on {date}.");

                            // Detach to retry safely
                            foreach (var entry in _db.ChangeTracker.Entries())
                                entry.State = EntityState.Detached;
                        }
                }

            var totalCost = 0m;
            var reservedRooms = new List<ReservedRoomDetail>();

            foreach (var room in rooms)
            {
                var pricePerNight = pricingMap.TryGetValue(room.RoomCategoryId, out var rate)
                    ? rate
                    : 0m;

                var subtotal = pricePerNight * room.Quantity * totalNights;
                totalCost += subtotal;

                reservedRooms.Add(new ReservedRoomDetail(
                    room.RoomCategoryId,
                    room.Quantity,
                    pricePerNight,
                    subtotal
                ));
            }

            await transaction.CommitAsync();

            return new RoomReservationReceipt
            {
                HotelId = hotelId,
                CheckIn = checkIn,
                CheckOut = checkOut,
                TotalCost = totalCost,
                Rooms = reservedRooms
            };
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }       

    public async Task<HotelDetails?> GetHotelDetailsAsync(
        Guid hotelId,
        DateOnly? fromDate,
        DateOnly? toDate,
        int? guests)
    {
        var hasDates = fromDate.HasValue && toDate.HasValue;
        var totalNights = hasDates
            ? (toDate.Value.ToDateTime(TimeOnly.MinValue) - fromDate.Value.ToDateTime(TimeOnly.MinValue)).Days
            : 0;

        Dictionary<Guid, List<Application.Dto.RoomAvailability>> availabilityMap = new();
        HashSet<Guid> availableCategoryIds = new();

        if (hasDates)
        {
            var availability = await _db.RoomAvailabilities
                .AsNoTracking()
                .Where(a => a.HotelId == hotelId && a.Date >= fromDate && a.Date < toDate)
                .Select(a => new
                {
                    a.RoomCategoryId,
                    a.Date,
                    AvailableCount = a.TotalCount - a.TotalBooked
                })
                .ToListAsync();

            if (availability.Count > 0)
            {
                availabilityMap = availability
                    .GroupBy(a => a.RoomCategoryId)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(a => new Application.Dto.RoomAvailability
                        {
                            Date = a.Date,
                            AvailableCount = a.AvailableCount
                        }).ToList());

                var requiredDates = Enumerable.Range(0, totalNights)
                    .Select(offset => fromDate.Value.AddDays(offset))
                    .ToList();

                availableCategoryIds = availabilityMap
                    .Where(g =>
                        requiredDates.All(date =>
                            g.Value.Any(av => av.Date == date && av.AvailableCount > 0)))
                    .Select(g => g.Key)
                    .ToHashSet();
            }
            else
            {
                hasDates = false;
            }
        }

        var hotel = await _db.Hotels
            .AsNoTracking()
            .Include(h => h.HotelFacilities)
            .Where(h => h.Id == hotelId)
            .Select(h => new
            {
                h.Id,
                h.Name,
                h.City,
                h.Country,
                h.AddressLine,
                h.ImageUrl,
                h.AverageRating,
                h.TotalReviews,
                h.Comment,
                Facilities = h.HotelFacilities.Select(f => f.Name).ToList(),
                Rooms = _db.RoomCategories
                    .Where(rc => rc.HotelId == h.Id &&
                                 (!hasDates || availableCategoryIds.Contains(rc.Id) || !availableCategoryIds.Any()))
                    .Select(rc => new
                    {
                        rc.Id,
                        rc.ImageUrls,
                        Facilities = rc.RoomFacilities.Select(f => f.Name).ToList(),
                        Beds = rc.BedConfigs.Select(b => new
                        {
                            BedTypeName = b.BedType.Name,
                            b.BedCount
                        }).ToList(),
                        BaseRate = rc.Pricing != null ? rc.Pricing.BaseRate : 0m,
                        RoomTypeName = rc.RoomType.Name,
                        rc.RoomType.MaximumGuests
                    })
                    .ToList()
            })
            .FirstOrDefaultAsync();

        if (hotel == null)
            return null;

        var roomDetails = hotel.Rooms.Select(rc => new Application.Dto.RoomCategory
        {
            Id = rc.Id,
            RoomTypeName = rc.RoomTypeName,
            MaximumGuests = rc.MaximumGuests,
            BaseRate = rc.BaseRate,
            ImageUrls = rc.ImageUrls ?? new List<string>(),
            Facilities = rc.Facilities ?? new List<string>(),
            Beds = rc.Beds.Select(b => new BedConfig
            {
                BedTypeName = b.BedTypeName,
                BedCount = b.BedCount
            }).ToList(),
            Availability = availabilityMap.TryGetValue(rc.Id, out var availList)
                ? availList
                : new List<Application.Dto.RoomAvailability>(),
            Info = guests.HasValue && hasDates
                ? $"{guests} guests, {totalNights} night{(totalNights == 1 ? "" : "s")}"
                : string.Empty
        }).ToList();

        var reviews = await _db.Reviews
            .AsNoTracking()
            .Where(r => r.HotelId == hotelId)
            .OrderByDescending(r => r.CreatedAt)
            .Take(10)
            .Select(r => new Application.Dto.Review
            {
                Rating = r.Rating,
                Comment = r.Comment,
                CreatedAt = r.CreatedAt,
                AuthorName = r.UserId.ToString()
            })
            .ToListAsync();

        var result = new HotelDetails
        {
            HotelId = hotel.Id,
            HotelName = hotel.Name,
            City = hotel.City,
            Country = hotel.Country,
            AddressLine = hotel.AddressLine,
            ImageUrl = hotel.ImageUrl,
            AverageRating = hotel.AverageRating,
            TotalReviews = hotel.TotalReviews ?? 0,
            Comment = hotel.Comment ?? string.Empty,
            Facilities = hotel.Facilities,
            Rooms = roomDetails,
            Reviews = reviews
        };

        return result;
    }


    public async Task<BookingSummaryResponse> GetBookingSummaryAsync(
        Guid hotelId,
        List<RoomReservationItem> rooms,
        DateOnly checkIn,
        DateOnly checkOut,
        int guests
    )
    {
        var totalNights = (checkOut.ToDateTime(TimeOnly.MinValue) - checkIn.ToDateTime(TimeOnly.MinValue)).Days;
        if (totalNights <= 0)
            throw new InvalidOperationException("Invalid date range.");

        var categoryIds = rooms.Select(r => r.RoomCategoryId).Distinct().ToList();

        var hotel = await _db.Hotels
                        .AsNoTracking()
                        .Where(h => h.Id == hotelId)
                        .Select(h => new HotelSummary
                        {
                            HotelId = h.Id,
                            Id = h.Id,
                            Name = h.Name,
                            City = h.City,
                            Country = h.Country,
                            ImageUrl = h.ImageUrl,
                            AverageRating = h.AverageRating,
                            TotalReviews = h.TotalReviews ?? 0,
                            Comment = h.Comment ?? string.Empty
                        })
                        .FirstOrDefaultAsync()
                    ?? throw new ArgumentException($"Hotel with ID {hotelId} not found.");

        var roomData = await (
            from rc in _db.RoomCategories.AsNoTracking()
            join rt in _db.RoomTypes.AsNoTracking() on rc.RoomTypeId equals rt.Id
            join rp in _db.RoomPricings.AsNoTracking() on rc.Id equals rp.RoomCategoryId
            where rc.HotelId == hotelId && categoryIds.Contains(rc.Id)
            select new
            {
                rc.Id,
                RoomTypeName = rt.Name,
                rp.BaseRate,
                rc.ImageUrls,
                Facilities = rc.RoomFacilities.Select(f => f.Name).ToList()
            }
        ).ToListAsync();

        var roomSummaries = new List<BookingRoomSummary>();
        var totalCost = 0m;

        foreach (var selected in rooms)
        {
            var data = roomData.FirstOrDefault(r => r.Id == selected.RoomCategoryId)
                       ?? throw new InvalidOperationException($"Room category {selected.RoomCategoryId} not found.");

            var subtotal = data.BaseRate * selected.Quantity * totalNights;
            totalCost += subtotal;

            roomSummaries.Add(new BookingRoomSummary
            {
                RoomCategoryId = selected.RoomCategoryId,
                RoomTypeName = data.RoomTypeName,
                Quantity = selected.Quantity,
                PricePerNight = data.BaseRate,
                Subtotal = subtotal,
                Images = data.ImageUrls ?? new List<string>(),
                Facilities = data.Facilities
            });
        }

        var summary = new BookingSummary
        {
            HotelId = hotelId,
            CheckIn = checkIn,
            CheckOut = checkOut,
            TotalNights = totalNights,
            Guests = guests,
            Rooms = roomSummaries,
            TotalCost = totalCost
        };

        return new BookingSummaryResponse
        {
            Hotel = hotel,
            Summary = summary
        };
    }

    private static PaginatedResponse<HotelSearchResult> EmptyHotelResponse(int skip, int take)
    {
        return new PaginatedResponse<HotelSearchResult>
        {
            Items = Array.Empty<HotelSearchResult>(),
            PageNo = skip / take + 1,
            PageSize = take,
            TotalRecords = 0,
            HasNext = false,
            HasPrevious = skip > 0
        };
    }

    private static PaginatedResponse<HotelSearchResult> BuildPagedResponse(List<HotelSearchResult> data,
        int totalCount,
        int skip,
        int take)
    {
        return new PaginatedResponse<HotelSearchResult>
        {
            Items = data,
            PageNo = skip / take + 1,
            PageSize = take,
            TotalRecords = totalCount,
            HasNext = skip + take < totalCount,
            HasPrevious = skip > 0
        };
    }
}
using Bogus;
using InventoryService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InventoryService.Infrastructure.Data.Seeders;

public static class DBInitializer
{
    public static async Task SeedAsync(AppDbContext db)
    {
        if (await db.Hotels.AnyAsync()) return;

        var tx = await db.Database.BeginTransactionAsync();
        try
        {
            var random = new Random();
            var faker = new Faker("en_GB");

            var roomTypes = new[]
            {
                new RoomType { Name = "Single Room", MaximumGuests = 1 },
                new RoomType { Name = "Double Room", MaximumGuests = 2 },
                new RoomType { Name = "Deluxe Room", MaximumGuests = 3 }
            };
            await db.RoomTypes.AddRangeAsync(roomTypes);

            var bedTypes = new[]
            {
                new BedType { Name = "Single Bed" },
                new BedType { Name = "Double Bed" },
                new BedType { Name = "King Bed" }
            };
            await db.BedTypes.AddRangeAsync(bedTypes);

            var facilities = new[]
            {
                new Facility { Name = "Free Wi-Fi" },
                new Facility { Name = "Air Conditioning" },
                new Facility { Name = "24-Hour Reception" },
                new Facility { Name = "On-site Parking" },
                new Facility { Name = "Gym" },
                new Facility { Name = "Indoor Pool" },
                new Facility { Name = "Restaurant & Bar" },
                new Facility { Name = "Pet Friendly" },
                new Facility { Name = "Laundry Service" },
                new Facility { Name = "Airport Shuttle" }
            };
            await db.Facilities.AddRangeAsync(facilities);
            await db.SaveChangesAsync();

            var hotelPoster = "https://i.ibb.co/DgGRkhMd/poster.jpg";

            var roomImages = new List<string>
            {
                "https://i.ibb.co/ynTVGvLD/295688312.jpg",
                "https://i.ibb.co/PZ1G6NmC/295688325.jpg",
                "https://i.ibb.co/jZDcy1Hv/295688342.jpg",
                "https://i.ibb.co/mptHVZG/295688351.jpg",
                "https://i.ibb.co/s92mS4sN/295688389.jpg",
                "https://i.ibb.co/Nnxmzxwc/295688415.jpg"
            };

            var cities = new[]
            {
                "London", "Manchester", "Birmingham", "Liverpool", "Leeds", "Glasgow",
                "Edinburgh", "Newcastle upon Tyne", "Bristol", "Nottingham", "Sheffield",
                "Cardiff", "Oxford", "Cambridge", "York", "Bath", "Brighton", "Reading",
                "Aberdeen", "Inverness", "Dundee", "Belfast"
            };

            var londonDistricts = new List<string>
            {
                "Westminster", "Kensington", "Chelsea", "Mayfair", "Soho",
                "Camden Town", "Greenwich", "Canary Wharf", "Hammersmith",
                "Paddington", "Shoreditch", "Clapham", "Notting Hill",
                "Chiswick", "Richmond", "Hampstead", "Wimbledon", "Ealing",
                "Islington", "South Bank", "King’s Cross", "Marylebone",
                "Bloomsbury", "Fitzrovia", "Whitechapel", "Docklands"
            };

            var hotelNames = new List<string>
            {
                "The Savoy London", "The Ritz London", "Hiltex Westminster", "Mayfair Continental",
                "Soho Executive Suites", "Chelsea Comfort Inn", "Greenwich View Hotel",
                "Canary Wharf Premier", "Camden Townhouse", "Shoreditch Grand", "Notting Hill Residence",
                "The Parliament Hotel", "The London Grand", "Westminster Plaza", "Kensington Court Hotel",
                "The Royal Borough Suites", "South Bank Palace", "Ealing Skyline", "Marylebone Manor",
                "Hampstead Boutique", "Richmond Park View", "The Regent’s Place", "Whitechapel Loft Hotel",
                "Wimbledon Retreat", "Clapham Villa", "Docklands Executive Suites", "Islington Charm Hotel",
                "Paddington Crown", "The Thames View", "The Piccadilly Royal", "London Central Luxe",
                "Kensington Elite Stay", "Notting Hill Hideaway", "Bloomsbury Grand", "The Oxford Street Hotel",
                "Victoria Palace Inn", "Chiswick Grove Hotel", "The Marble Arch Suites", "Fitzrovia Square Hotel",

                "Manchester Haven", "Piccadilly Suites", "Northern Lights Hotel", "The Lowry Stay",
                "City Central Hotel", "Canal Street View", "Deansgate Deluxe", "Victoria Grand Manchester",
                "Salford Quays Residences", "Spinningfields View", "Old Trafford Executive Hotel",
                "Trafford Park Inn", "Stockport Gateway Suites",

                "Tyne View Inn", "Baltic Heights", "The Quayside Hotel", "Cathedral View Newcastle",
                "Grey Street Boutique", "The Monument Place", "Ouseburn Lofts", "Jesmond Park Hotel",
                "Gateshead View", "Angel of the North Suites",

                "Liverpool Bayfront", "Albert Dock Suites", "Riverside Premier", "Anfield Lodge",
                "Merseyview Hotel", "Lime Street Residence", "Cavern Quarter Hotel", "Echo Arena Inn",
                "Sefton Park Retreat", "Baltic Triangle Hotel",

                "The Caledonian", "Royal Mile Hotel", "The Edinburgh Loft", "The Georgian Plaza",
                "Old Town Suites", "Castle View Hotel", "Princes Street Grand", "Haymarket Hub",
                "Glasgow Riverside", "Merchant City Grand", "Buchanan Street Hotel", "Kelvingrove Inn",
                "Aberdeen Harbour Suites", "Union Street Residence", "Dundee Waterfront Hotel",
                "Inverness Highlands Retreat",

                "Bullring Central", "The Mailbox Hotel", "New Street Plaza", "Birmingham Heights",
                "Jewellery Quarter Hotel", "Leeds Skyline Towers", "Victoria Gate Hotel Leeds",
                "Sheffield Park View", "Nottingham Central Grand", "Derby Luxe Stay",

                "Bristol Harbour Grand", "Harbourside Inn Bristol", "Clifton Hill View", "Bath Crescent Hotel",
                "Brighton Seaside Lodge", "Oxford Dreaming Inn", "Cambridge Riverbank Suites",
                "Reading Central Plaza", "Portsmouth Harbour Hotel", "Southampton Marina Suites",

                "Cardiff Bayfront", "Belfast City Grand", "Causeway Coastal Retreat",
                "Snowdonia View Hotel", "Llandudno Bay Hotel", "Bangor Harbour Lodge",

                "York Minster View", "Durham Heritage Inn", "Chester City Boutique",
                "Canterbury Cathedral Suites", "Stratford-upon-Avon Manor", "Winchester Grand Hotel"
            };

            var hotels = hotelNames.Select(name =>
            {
                var city = faker.PickRandom(cities);
                var address = city == "London"
                    ? $"{faker.Address.StreetAddress()}, {faker.PickRandom(londonDistricts)}"
                    : faker.Address.StreetAddress();

                var avg = faker.Random.Decimal(6.5m, 9.5m);
                var comment = avg switch
                {
                    >= 9.0m => "Wonderful stay! Guests love everything about this hotel.",
                    >= 8.0m => "Very good — popular among couples and business travellers.",
                    >= 7.0m => "Good comfort and service, ideal for short stays.",
                    _ => "Decent budget option with basic amenities."
                };

                var hotel = new Hotel
                {
                    Name = name,
                    City = city,
                    Country = "United Kingdom",
                    AddressLine = address,
                    PostalCode = faker.Address.ZipCode("SW# #AA"),
                    ImageUrl = hotelPoster,
                    AverageRating = avg,
                    TotalReviews = faker.Random.Int(100, 2000),
                    Comment = comment,
                    Summary = faker.Lorem.Sentence(10, 15),
                    HotelFacilities = facilities
                        .OrderBy(_ => Guid.NewGuid())
                        .Take(random.Next(4, 8))
                        .ToList()
                };

                return hotel;
            }).ToList();

            await db.Hotels.AddRangeAsync(hotels);
            await db.SaveChangesAsync();

            var categories = new List<RoomCategory>();
            foreach (var hotel in hotels)
            foreach (var type in roomTypes)
            {
                var category = new RoomCategory
                {
                    HotelId = hotel.Id,
                    RoomTypeId = type.Id,
                    TotalCount = random.Next(5, 20),
                    RoomTypeName = type.Name,
                    ImageUrls = roomImages.OrderBy(_ => Guid.NewGuid()).Take(3).ToList(),
                    RoomFacilities = facilities
                        .OrderBy(_ => Guid.NewGuid())
                        .Take(random.Next(3, 6))
                        .ToList()
                };

                categories.Add(category);
            }

            await db.RoomCategories.AddRangeAsync(categories);
            await db.SaveChangesAsync();

            var pricings = new List<RoomPricing>();
            var seasonalRates = new List<SeasonalRate>();

            foreach (var category in categories)
            {
                var baseRate = faker.Random.Decimal(70, 300);
                var pricing = new RoomPricing
                {
                    BaseRate = baseRate,
                    RoomCategoryId = category.Id
                };
                pricings.Add(pricing);
            }

            await db.RoomPricings.AddRangeAsync(pricings);
            await db.SaveChangesAsync();

            foreach (var pricing in pricings)
            {
                seasonalRates.Add(new SeasonalRate
                {
                    RoomPricingId = pricing.Id,
                    Rate = pricing.BaseRate + faker.Random.Decimal(20, 80),
                    StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(15)),
                    EndDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(35))
                });
                seasonalRates.Add(new SeasonalRate
                {
                    RoomPricingId = pricing.Id,
                    Rate = pricing.BaseRate + faker.Random.Decimal(10, 50),
                    StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(45)),
                    EndDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(60))
                });
            }

            await db.SeasonalRates.AddRangeAsync(seasonalRates);

            var bedConfigs = new List<RoomBedConfig>();
            foreach (var category in categories)
            {
                var beds = bedTypes.OrderBy(_ => Guid.NewGuid()).Take(2).ToList();
                foreach (var bed in beds)
                    bedConfigs.Add(new RoomBedConfig
                    {
                        BedTypeId = bed.Id,
                        BedCount = random.Next(1, 3),
                        RoomCategoryId = category.Id
                    });
            }

            await db.RoomBedConfigs.AddRangeAsync(bedConfigs);
            await db.SaveChangesAsync();


            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var availabilities = new List<RoomAvailability>();
            foreach (var category in categories)
                for (var i = 0; i < 365; i++)
                {
                    var total = category.TotalCount;
                    var booked = random.Next(0, total);
                    availabilities.Add(new RoomAvailability
                    {
                        HotelId = category.HotelId,
                        RoomCategoryId = category.Id,
                        Date = today.AddDays(i),
                        TotalBooked = booked,
                        TotalCount = total,
                        IsAvailable = booked < total
                    });
                }

            await db.RoomAvailabilities.AddRangeAsync(availabilities);
            await db.SaveChangesAsync();


            var detailedReviews = new List<(decimal rating, string comment)>
            {
                (9.5m,
                    "Absolutely wonderful stay! The staff went above and beyond to make us feel welcome. The room was spotless, the bed extremely comfortable, and breakfast was freshly cooked every morning. We loved the central location — just a short walk from restaurants and shops."),
                (8.9m,
                    "Very good experience overall. The reception team was friendly and check-in was smooth. The hotel felt modern and clean, though the Wi-Fi signal could have been stronger in the upper floors. Would definitely stay again."),
                (9.2m,
                    "Great atmosphere and super attentive staff. The breakfast buffet was excellent with plenty of options. The room had a beautiful city view, and even though it was near the main street, noise insulation was perfect."),
                (8.6m,
                    "We stayed here for a weekend getaway and were pleasantly surprised. The interiors were modern, the shower had great water pressure, and everything felt new. Only minor downside was that parking spaces filled up quickly in the evening."),
                (9.3m,
                    "This hotel exceeded expectations — classy décor, very comfortable bedding, and fast room service. The in-house restaurant served some of the best food we’ve had in a hotel chain. Highly recommended for business travellers."),
                (8.7m,
                    "Clean, quiet, and well-maintained. Staff were polite and always ready to help. The location is perfect for exploring nearby attractions, and we appreciated the late check-out option."),
                (9.0m,
                    "Fantastic service throughout our stay. Receptionist remembered our names, which was a nice touch. The bar area was cozy and perfect for winding down after a long day. The room lighting was warm and relaxing."),
                (8.5m,
                    "Good hotel for a quick city break. Rooms were spacious and tidy, and the beds were very comfortable. The gym could use a few more machines, but everything else was top-notch."),
                (9.1m,
                    "We loved every bit of our stay! The breakfast spread was amazing — fresh pastries, fruit, and made-to-order omelets. The staff even packed us a takeaway box when we had an early flight."),
                (8.8m,
                    "Stylish hotel with a welcoming vibe. The lounge area was a great place to work and have coffee. The bathroom amenities were premium quality. Definitely worth the price."),
                (9.4m,
                    "Our room was beautifully decorated with a modern touch. The view over the river was stunning at night. Service was exceptional, and the restaurant dinner menu was delicious."),
                (8.3m,
                    "Stayed for two nights while attending a conference. The Wi-Fi was fast, the meeting rooms were well equipped, and the staff managed everything professionally. Breakfast could start earlier, though."),
                (9.6m,
                    "This was our second stay and it was even better than the first. The upgraded suite was spotless, and the room service menu was extensive. Staff remembered our previous preferences — impressive attention to detail."),
                (8.4m,
                    "Nice mid-range hotel with a comfortable atmosphere. Rooms were clean and the AC worked perfectly. The pool was small but spotless. Would recommend for short stays."),
                (9.0m,
                    "Wonderful experience! The bed was like sleeping on a cloud, and the blackout curtains made it easy to sleep in. The hotel bar served great cocktails, and the breakfast buffet was generous."),
                (8.2m,
                    "Good overall value. Check-in was fast, rooms were clean, and towels fresh. Some furniture showed minor wear, but it didn’t affect comfort. Conveniently located near public transport."),
                (9.3m,
                    "One of the best stays I’ve had recently. Friendly staff, delicious breakfast, and spotless rooms. The gym and sauna were a great addition after a long day of sightseeing."),
                (8.9m,
                    "Great price for what you get. The lobby smells amazing, and the front-desk staff were genuinely warm. Room temperature was perfect and Wi-Fi stable — ideal for remote work."),
                (9.1m,
                    "Hotel has a premium feel without being overpriced. Room decor was tasteful and bed extremely comfortable. Loved the rainfall shower and complimentary coffee machine."),
                (8.7m,
                    "Very pleasant stay. Staff were accommodating when we requested a quiet room. The area felt safe, and we found great restaurants nearby. Would happily return next time we’re in town.")
            };

            var reviewEntities = new List<Review>();

            foreach (var hotel in hotels)
            foreach (var (rating, comment) in detailedReviews)
                reviewEntities.Add(new Review
                {
                    HotelId = hotel.Id,
                    Rating = rating,
                    Comment = comment,
                    UserId = Guid.Parse("289feefb-1fbb-4979-b121-c7fde74a347e")
                });

            await db.Reviews.AddRangeAsync(reviewEntities);
            await db.SaveChangesAsync();

            await tx.CommitAsync();
            Console.WriteLine("dataset seeded successfully!");
        }
        catch (Exception ex)
        {
            await tx.RollbackAsync();
            Console.WriteLine($"Seeding failed: {ex.Message}");
            throw;
        }
    }
}
using System.ComponentModel.DataAnnotations;

namespace InventoryService.Application.Dto;

public abstract class BaseHotel
{
    public Guid HotelId { get; set; }
    public string Name { get; set; } = default!;
    public string City { get; set; } = default!;
    public string Country { get; set; } = default!;
    public string? AddressLine { get; set; }
    public string? ImageUrl { get; set; }
    public decimal AverageRating { get; set; }
    public int TotalReviews { get; set; }
    public string? Comment { get; set; }
}

public abstract class BaseRoom
{
    public Guid Id { get; set; }
    public string RoomTypeName { get; set; } = default!;
    public int MaximumGuests { get; set; }
    public decimal BaseRate { get; set; }
}

public abstract class BaseDateRange
{
    [Required] public DateOnly CheckIn { get; set; }
    [Required] public DateOnly CheckOut { get; set; }
}

public abstract class BaseReservationRequest : BaseDateRange
{
    [Required] public Guid HotelId { get; set; }

    [Required]
    [MinLength(1, ErrorMessage = "At least one room must be specified.")]
    public List<RoomReservationItem> Rooms { get; set; } = new();

    [Required]
    [Range(1, 100, ErrorMessage = "Guests must be between 1 and 100.")]
    public int Guests { get; set; }
}

public class HotelSummary : BaseHotel
{
    public Guid Id { get; set; }
}

public class HotelSearchResult : BaseHotel
{
    public List<RoomSearchResult> RoomCategories { get; set; } = new();
}

public class HotelDetails : BaseHotel
{
    public string HotelName
    {
        get => Name;
        set => Name = value;
    }

    public List<string> Facilities { get; set; } = new();
    public List<RoomCategory> Rooms { get; set; } = new();
    public List<Review> Reviews { get; set; } = new();
}

public class RoomSearchResult : BaseRoom
{
    public string Info { get; set; } = string.Empty;
}

public class RoomCategory : BaseRoom
{
    public List<string> ImageUrls { get; set; } = new();
    public List<string> Facilities { get; set; } = new();
    public List<BedConfig> Beds { get; set; } = new();
    public List<RoomAvailability>? Availability { get; set; }
    public string Info { get; set; } = string.Empty;
}

public class BedConfig
{
    public string BedTypeName { get; set; } = default!;
    public int BedCount { get; set; }
}

public class RoomAvailability
{
    public DateOnly Date { get; set; }
    public int AvailableCount { get; set; }
}

public class Review
{
    public decimal Rating { get; set; }
    public string Comment { get; set; } = default!;
    public DateTime CreatedAt { get; set; }
    public string AuthorName { get; set; } = default!;
}

public class BookingSummaryRequest : BaseReservationRequest
{
}

public class BookingSummaryResponse
{
    public HotelSummary Hotel { get; set; } = new();
    public BookingSummary Summary { get; set; } = new();
}

public class BookingSummary : BaseDateRange
{
    public Guid HotelId { get; set; }
    public int TotalNights { get; set; }
    public int Guests { get; set; }
    public List<BookingRoomSummary> Rooms { get; set; } = new();
    public decimal TotalCost { get; set; }
}

public class BookingRoomSummary
{
    public Guid RoomCategoryId { get; set; }
    public string RoomTypeName { get; set; } = default!;
    public int Quantity { get; set; }
    public decimal PricePerNight { get; set; }
    public decimal Subtotal { get; set; }
    public List<string>? Images { get; set; }
    public List<string>? Facilities { get; set; }
}

public class RoomReservationRequest : BaseReservationRequest
{
    [Required] public Guid UserId { get; set; }
}

public record RoomReservationItem(Guid RoomCategoryId, int Quantity);

public class RoomReservationReceipt : BaseDateRange
{
    public Guid HotelId { get; set; }
    public decimal TotalCost { get; set; }
    public List<ReservedRoomDetail> Rooms { get; set; } = new();
}

public record ReservedRoomDetail(
    Guid RoomCategoryId,
    int Quantity,
    decimal PricePerNight,
    decimal Subtotal
);

public class AvailabilityCheckRequest : BaseDateRange
{
    [Required] public Guid HotelId { get; set; }
    [Required] public List<Guid> RoomCategoryIds { get; set; } = new();

    [Required]
    [Range(1, 100, ErrorMessage = "Guests must be between 1 and 100.")]
    public int Guests { get; set; }
}

public class AvailabilityCheckResponse
{
    public bool Available { get; set; }
}
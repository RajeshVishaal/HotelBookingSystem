using System.ComponentModel.DataAnnotations;

namespace BookingService.Application.Dto;

public record ReservationRequest(
    [Required] Guid HotelId,
    [Required] Guid UserId,
    [Required] [Range(1, 100)] int Guests,
    [Required] [MinLength(1)] List<ReservationRoomItem> Rooms,
    [Required] DateOnly CheckIn,
    [Required] DateOnly CheckOut
);

public record ReservationRoomItem(
    [Required] Guid RoomCategoryId,
    [Required] [Range(1, 100)] int Quantity
);

public class ReservationReceipt
{
    public Guid HotelId { get; set; }
    public DateOnly CheckIn { get; set; }
    public DateOnly CheckOut { get; set; }
    public decimal TotalCost { get; set; }
    public List<ReservedRoomDetail> Rooms { get; set; } = new();
}

public record ReservedRoomDetail(
    Guid RoomCategoryId,
    int Quantity,
    decimal PricePerNight,
    decimal Subtotal
);

public class HotelSummary
{
    public Guid HotelId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
}

public class BookingDetailsResponse
{
    public string BookingReference { get; set; } = default!;
    public Guid HotelId { get; set; }
    public string HotelName { get; set; } = string.Empty;
    public string HotelImageUrl { get; set; } = string.Empty;
    public Guid UserId { get; set; }
    public DateOnly CheckIn { get; set; }
    public DateOnly CheckOut { get; set; }
    public int Guests { get; set; }
    public decimal TotalCost { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<ReservedRoom> Rooms { get; set; } = new();
}

public record ReservedRoom
{
    public Guid RoomCategoryId { get; set; }
    public int Quantity { get; set; }
    public decimal BaseRate { get; set; }
    public decimal Subtotal { get; set; }
}
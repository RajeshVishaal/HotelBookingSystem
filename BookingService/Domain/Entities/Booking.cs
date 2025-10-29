using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Common.Entities;

namespace BookingService.Domain.Entities;

[Table("Bookings")]
public class Booking : BaseEntity<Guid>
{
    [Required] [StringLength(50)] public string BookingReference { get; set; } = default!;

    [Required] [StringLength(255)] public string IdempotencyKey { get; set; } = default!;

    [Required] public Guid HotelId { get; set; }

    [Required] public Guid UserId { get; set; }

    [Required] [DataType(DataType.Date)] public DateOnly CheckIn { get; set; }

    [Required] [DataType(DataType.Date)] public DateOnly CheckOut { get; set; }

    [Required]
    [DataType(DataType.Currency)]
    public decimal TotalCost { get; set; }

    [Required] [Range(1, 50)] public int Guests { get; set; }

    public ICollection<BookingRoomInfo> Rooms { get; set; } = new List<BookingRoomInfo>();
}
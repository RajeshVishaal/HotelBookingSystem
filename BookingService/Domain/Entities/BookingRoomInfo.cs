using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Common.Entities;

namespace BookingService.Domain.Entities;

[Table("BookingRoomInfo")]
public class BookingRoomInfo : BaseEntity<Guid>
{
    public Guid BookingId { get; set; }
    [Required] public Guid RoomCategoryId { get; set; }

    [Required] [Range(1, 10)] public int Quantity { get; set; }

    [Required]
    [DataType(DataType.Currency)]
    public decimal BaseRate { get; set; }

    [Required]
    [DataType(DataType.Currency)]
    public decimal Subtotal { get; set; }

    [ForeignKey(nameof(BookingId))] public Booking Booking { get; set; } = null!;
}
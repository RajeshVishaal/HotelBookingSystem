using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Common.Entities;

namespace InventoryService.Domain.Entities;

[Table("RoomAvailabilities")]
public class RoomAvailability : BaseEntity<Guid>
{
    [Required] public Guid HotelId { get; set; }

    [ForeignKey(nameof(HotelId))] public Hotel Hotel { get; set; } = null!;

    [Required] public Guid RoomCategoryId { get; set; }

    [ForeignKey(nameof(RoomCategoryId))] public RoomCategory RoomCategory { get; set; } = null!;

    [Required] [DataType(DataType.Date)] public DateOnly Date { get; set; }

    [Required] [Range(0, 1000)] public int TotalCount { get; set; }

    [Required] [Range(0, 1000)] public int TotalBooked { get; set; }

    [Required] public bool IsAvailable { get; set; }

    [Timestamp] public uint RowVersion { get; set; }
}
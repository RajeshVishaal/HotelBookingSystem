using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Common.Entities;

namespace InventoryService.Domain.Entities;

[Table("RoomBedConfigs")]
public class RoomBedConfig : BaseEntity<Guid>
{
    [Required] public Guid BedTypeId { get; set; }

    [Required] public Guid RoomCategoryId { get; set; }

    [ForeignKey(nameof(RoomCategoryId))] public RoomCategory RoomCategory { get; set; } = null!;

    [ForeignKey(nameof(BedTypeId))] public BedType BedType { get; set; } = null!;

    [Required] [Range(1, 10)] public int BedCount { get; set; }
}
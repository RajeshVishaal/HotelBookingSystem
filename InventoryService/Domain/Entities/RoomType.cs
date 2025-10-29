using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Common.Entities;

namespace InventoryService.Domain.Entities;

[Table("RoomTypes")]
public class RoomType : BaseEntity<Guid>
{
    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string Name { get; set; } = null!;

    [Required] [Range(1, 20)] public int MaximumGuests { get; set; }
}
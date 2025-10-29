using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Common.Entities;

namespace InventoryService.Domain.Entities;

[Table("BedTypes")]
public class BedType : BaseEntity<Guid>
{
    [Required]
    [StringLength(50, MinimumLength = 2)]
    public string Name { get; set; } = null!;
}
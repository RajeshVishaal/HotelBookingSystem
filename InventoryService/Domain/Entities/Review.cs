using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Common.Entities;

namespace InventoryService.Domain.Entities;

[Table("Reviews")]
public class Review : BaseEntity<Guid>
{
    public Guid? UserId { get; set; }

    [Required] public Guid HotelId { get; set; }

    [ForeignKey(nameof(HotelId))] public Hotel Hotel { get; set; } = null!;

    [Required] [Range(0, 5)] public decimal Rating { get; set; }

    [Required]
    [StringLength(2000, MinimumLength = 5)]
    public string Comment { get; set; } = null!;
}
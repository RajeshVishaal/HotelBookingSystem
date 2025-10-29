using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Common.Entities;

namespace InventoryService.Domain.Entities;

[Table("SeasonalRates")]
public class SeasonalRate : BaseEntity<Guid>
{
    [Required] public Guid RoomPricingId { get; set; }

    [ForeignKey(nameof(RoomPricingId))] public RoomPricing RoomPricing { get; set; } = null!;

    [Required]
    [DataType(DataType.Currency)]
    [Range(0.01, 999999.99)]
    public decimal Rate { get; set; }

    [Required] [DataType(DataType.Date)] public DateOnly StartDate { get; set; }

    [Required] [DataType(DataType.Date)] public DateOnly EndDate { get; set; }
}
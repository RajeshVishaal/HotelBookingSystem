using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Common.Entities;

namespace InventoryService.Domain.Entities;

[Table("RoomPricings")]
public class RoomPricing : BaseEntity<Guid>
{
    [Required]
    [DataType(DataType.Currency)]
    [Range(0.01, 999999.99)]
    public decimal BaseRate { get; set; }

    [Required] public Guid RoomCategoryId { get; set; }

    [ForeignKey(nameof(RoomCategoryId))] public RoomCategory RoomCategory { get; set; } = null!;

    public ICollection<SeasonalRate> SeasonalRates { get; set; } = new List<SeasonalRate>();

    public decimal GetRateFor(DateOnly date)
    {
        return SeasonalRates.FirstOrDefault(s => date >= s.StartDate && date <= s.EndDate)?.Rate ?? BaseRate;
    }
}
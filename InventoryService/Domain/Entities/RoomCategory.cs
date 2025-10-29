using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Common.Entities;

namespace InventoryService.Domain.Entities;

[Table("RoomCategories")]
public class RoomCategory : BaseEntity<Guid>
{
    [Required] public Guid HotelId { get; set; }

    [ForeignKey(nameof(HotelId))] public Hotel Hotel { get; set; } = null!;

    [Required] public Guid RoomTypeId { get; set; }

    [ForeignKey(nameof(RoomTypeId))] public RoomType RoomType { get; set; } = null!;

    [StringLength(100)] public string? RoomTypeName { get; set; }

    [Required] [Range(1, 1000)] public int TotalCount { get; set; }

    public List<string> ImageUrls { get; set; } = new();

    public RoomPricing Pricing { get; set; } = null!;

    public ICollection<RoomBedConfig> BedConfigs { get; set; } = new List<RoomBedConfig>();
    public ICollection<Facility> RoomFacilities { get; set; } = new List<Facility>();
}
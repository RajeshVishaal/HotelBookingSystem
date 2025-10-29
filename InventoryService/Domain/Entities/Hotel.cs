using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Common.Entities;

namespace InventoryService.Domain.Entities;

[Table("Hotels")]
public class Hotel : BaseEntity<Guid>
{
    [Required]
    [StringLength(200, MinimumLength = 2)]
    public string Name { get; set; } = null!;

    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string City { get; set; } = null!;

    [Required]
    [StringLength(500, MinimumLength = 5)]
    public string AddressLine { get; set; } = null!;

    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string Country { get; set; } = null!;

    [StringLength(20)] public string? PostalCode { get; set; }

    [DataType(DataType.ImageUrl)]
    [StringLength(500)]
    public string? ImageUrl { get; set; }

    [Range(0, 5)] public decimal AverageRating { get; set; }

    [Range(0, int.MaxValue)] public int? TotalReviews { get; set; }

    [StringLength(1000)] public string? Comment { get; set; }

    [StringLength(2000)] public string? Summary { get; set; }

    public ICollection<Review> Reviews { get; set; } = new List<Review>();
    public ICollection<RoomCategory> Rooms { get; set; } = new List<RoomCategory>();
    public ICollection<Facility> HotelFacilities { get; set; } = new List<Facility>();
}
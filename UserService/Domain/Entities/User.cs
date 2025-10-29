using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Common.Entities;

namespace UserService.Domain.Entities;

[Table("Users")]
public class User : BaseEntity<Guid>
{
    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string FirstName { get; set; } = null!;

    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string LastName { get; set; } = null!;

    [Required]
    [DataType(DataType.EmailAddress)]
    [StringLength(255, MinimumLength = 5)]
    public string EmailAddress { get; set; } = null!;

    [Required] [StringLength(500)] public string PasswordHash { get; set; } = null!;
}
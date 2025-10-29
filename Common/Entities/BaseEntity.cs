using System.ComponentModel.DataAnnotations;

namespace Common.Entities;

public abstract class BaseEntity<T>
{
    protected BaseEntity()
    {
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    [Key] public T Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
namespace webapi.Models;

using System.ComponentModel.DataAnnotations;

public abstract class BaseModel
{
    [Key]
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
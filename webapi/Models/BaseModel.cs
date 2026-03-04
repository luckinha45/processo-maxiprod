namespace webapi.Models;

using System.ComponentModel.DataAnnotations;

/// <summary>
/// Adiciona id, created_at e updated_at para as Models que a utilizarem
/// </summary>
public abstract class BaseModel
{
    [Key]
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
using System.ComponentModel.DataAnnotations;

namespace Ecommerce_Api.Models;

public class Category
{
    // Primari Key
    [Key]
    public int Id { get; set; }
    [Required]
    public string Name { get; set; } = string.Empty;
    [Required]
    public DateTime CreationDate { get; set; }
}
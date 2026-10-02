using System.ComponentModel.DataAnnotations;

namespace Ecommerce_Api.Models.Dtos;

public class CreateCategoryDto
{
    [Required (ErrorMessage = "Name is required")]
    [MaxLength(50, ErrorMessage = "Name cannot exceed 50 characters")]
    [MinLength(3, ErrorMessage = "Name must have at least 3 characters")]
    public string Name { get; set; } = string.Empty;
    
}
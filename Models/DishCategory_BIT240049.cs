using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace BIT240049_Mid.Models;

[Table("BIT240049_DishCategories")]
public class DishCategory_BIT240049
{
    [Key]
    [Column("BIT240049_Id")]
    public int Id { get; set; }

    [Column("BIT240049_Name")]
    [Display(Name = "Name")]
    [Required(ErrorMessage = "NameRequired")]
    [StringLength(100, ErrorMessage = "TextTooLong")]
    public string Name { get; set; } = "";

    [Column("BIT240049_Description")]
    [Display(Name = "Description")]
    [StringLength(500, ErrorMessage = "TextTooLong")]
    public string? Description { get; set; }

    // Mot DishCategory co nhieu Dish
    [ValidateNever]
    public List<Dish_BIT240049> Dishes { get; set; } = new();
}

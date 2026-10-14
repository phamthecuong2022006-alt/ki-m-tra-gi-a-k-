using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace BIT240049_Mid.Models;

[Table("BIT240049_DishImages")]
public class DishImage_BIT240049
{
    [Key]
    [Column("BIT240049_Id")]
    public int Id { get; set; }

    [Column("BIT240049_ImageUrl")]
    [Display(Name = "ImageUrl")]
    [Required(ErrorMessage = "ImageUrlRequired")]
    [StringLength(500, ErrorMessage = "TextTooLong")]
    public string ImageUrl { get; set; } = "";

    [Column("BIT240049_IsThumbnail")]
    [Display(Name = "IsThumbnail")]
    public bool IsThumbnail { get; set; }

    [Column("BIT240049_DishId")]
    public int DishId { get; set; }

    [ValidateNever]
    public Dish_BIT240049? Dish { get; set; }
}

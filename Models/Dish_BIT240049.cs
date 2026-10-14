using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace BIT240049_Mid.Models;

[Table("BIT240049_Dishes")]
public class Dish_BIT240049
{
    [Key]
    [Column("BIT240049_Id")]
    public int Id { get; set; }

    [Column("BIT240049_Name")]
    [Display(Name = "Name")]
    [Required(ErrorMessage = "NameRequired")]
    [StringLength(150, ErrorMessage = "TextTooLong")]
    public string Name { get; set; } = "";

    [Column("BIT240049_Price", TypeName = "decimal(18,2)")]
    [Display(Name = "Price")]
    [Range(0.01, 1000000000000.0, ErrorMessage = "PriceInvalid")]   // Price > 0
    public decimal Price { get; set; }

    [Column("BIT240049_PreparationTime")]
    [Display(Name = "PreparationTime")]
    [Range(1, 100000, ErrorMessage = "PrepInvalid")]                // PreparationTime > 0
    public int PreparationTime { get; set; }

    [Column("BIT240049_IsAvailable")]
    [Display(Name = "IsAvailable")]
    public bool IsAvailable { get; set; } = true;

    [Column("BIT240049_Description")]
    [Display(Name = "Description")]
    [StringLength(1000, ErrorMessage = "TextTooLong")]
    public string? Description { get; set; }

    [Column("BIT240049_DishCategoryId")]
    [Display(Name = "Category")]
    [Range(1, int.MaxValue, ErrorMessage = "CategoryRequired")]
    public int DishCategoryId { get; set; }

    // Moi Dish thuoc dung mot DishCategory
    [ValidateNever]
    public DishCategory_BIT240049? DishCategory { get; set; }

    // Mot Dish co nhieu DishImage
    [ValidateNever]
    public List<DishImage_BIT240049> Images { get; set; } = new();

    /// <summary>Anh dai dien; neu chua chon thi lay anh dau tien; null neu mon chua co anh.</summary>
    [NotMapped]
    public DishImage_BIT240049? Thumbnail =>
        Images.FirstOrDefault(i => i.IsThumbnail) ?? Images.OrderBy(i => i.Id).FirstOrDefault();
}

using BIT240049_Mid.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace BIT240049_Mid.ViewModels;

/// <summary>Dieu kien tim kiem / loc / sap xep (lay tu query string) + ket qua.</summary>
public class DishIndexViewModel
{
    public string? Search { get; set; }
    public int? CategoryId { get; set; }
    public string? Status { get; set; }       // "", "available", "unavailable"
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public string? Sort { get; set; }         // "", "price_asc", "price_desc", "time_asc"

    [BindNever, ValidateNever]
    public List<Dish_BIT240049> Dishes { get; set; } = new();

    [BindNever, ValidateNever]
    public List<DishCategory_BIT240049> Categories { get; set; } = new();
}

public class CategoryListItem
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string? Description { get; set; }
    public int DishCount { get; set; }
}

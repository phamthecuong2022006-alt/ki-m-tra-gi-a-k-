using BIT240049_Mid.Data;
using BIT240049_Mid.Models;
using BIT240049_Mid.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace BIT240049_Mid.Controllers;

public class DishesController : AppController
{
    private readonly AppDbContext _db;
    private readonly IStringLocalizer<SharedResource> _L;

    public DishesController(AppDbContext db, IStringLocalizer<SharedResource> localizer)
    {
        _db = db;
        _L = localizer;
    }

    // ------------------------------------------------------------------
    // Chuc nang 2: danh sach, tim kiem, loc, sap xep (loc o tang database)
    // ------------------------------------------------------------------
    public async Task<IActionResult> Index([FromQuery] DishIndexViewModel vm)
    {
        vm.Search = vm.Search?.Trim();

        // Khoang gia khong hop le: am hoac min > max
        var priceInvalid = vm.MinPrice < 0 || vm.MaxPrice < 0 || vm.MinPrice > vm.MaxPrice;
        if (priceInvalid)
        {
            ModelState.AddModelError(string.Empty, _L["PriceRangeInvalid"]);
        }

        var query = _db.Dishes
            .AsNoTracking()
            .Include(d => d.DishCategory)
            .Include(d => d.Images)
            .AsQueryable();

        var search = vm.Search;
        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(d => d.Name.Contains(search));
        }

        if (vm.CategoryId.HasValue)
        {
            var categoryId = vm.CategoryId.Value;
            query = query.Where(d => d.DishCategoryId == categoryId);
        }

        if (vm.Status == "available")
        {
            query = query.Where(d => d.IsAvailable);
        }
        else if (vm.Status == "unavailable")
        {
            query = query.Where(d => !d.IsAvailable);
        }

        if (!priceInvalid)
        {
            if (vm.MinPrice.HasValue)
            {
                var min = vm.MinPrice.Value;
                query = query.Where(d => d.Price >= min);
            }
            if (vm.MaxPrice.HasValue)
            {
                var max = vm.MaxPrice.Value;
                query = query.Where(d => d.Price <= max);
            }
        }

        query = vm.Sort switch
        {
            "price_asc" => query.OrderBy(d => d.Price).ThenBy(d => d.Id),
            "price_desc" => query.OrderByDescending(d => d.Price).ThenBy(d => d.Id),
            "time_asc" => query.OrderBy(d => d.PreparationTime).ThenBy(d => d.Id),
            _ => query.OrderBy(d => d.Id)
        };

        vm.Dishes = await query.ToListAsync();
        vm.Categories = await _db.DishCategories.AsNoTracking().OrderBy(c => c.Name).ToListAsync();
        return View(vm);
    }

    // ------------------------------------------------------------------
    // Detail (nhieu anh)
    // ------------------------------------------------------------------
    public async Task<IActionResult> Detail(int id)
    {
        var dish = await _db.Dishes
            .AsNoTracking()
            .Include(d => d.DishCategory)
            .Include(d => d.Images.OrderByDescending(i => i.IsThumbnail).ThenBy(i => i.Id))
            .FirstOrDefaultAsync(d => d.Id == id);

        if (dish == null)
        {
            return NotFoundPage("DishNotFound");
        }
        return View(dish);
    }

    // ------------------------------------------------------------------
    // Create
    // ------------------------------------------------------------------
    public async Task<IActionResult> Create()
    {
        await LoadCategoriesAsync(0);
        return View(new Dish_BIT240049 { IsAvailable = true });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        [Bind("Name,Price,PreparationTime,IsAvailable,Description,DishCategoryId")] Dish_BIT240049 dish)
    {
        dish.Name = dish.Name?.Trim() ?? "";
        dish.Description = string.IsNullOrWhiteSpace(dish.Description) ? null : dish.Description.Trim();

        await ValidateDishAsync(dish, excludeId: 0);

        if (ModelState.IsValid)
        {
            _db.Dishes.Add(dish);
            try
            {
                await _db.SaveChangesAsync();
                TempData["Success"] = _L["SavedSuccess"].Value;
                return RedirectToAction(nameof(Detail), new { id = dish.Id });
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError(string.Empty, _L["DbError"]);
            }
        }

        await LoadCategoriesAsync(dish.DishCategoryId);
        return View(dish);
    }

    // ------------------------------------------------------------------
    // Edit
    // ------------------------------------------------------------------
    public async Task<IActionResult> Edit(int id)
    {
        var dish = await _db.Dishes.AsNoTracking().FirstOrDefaultAsync(d => d.Id == id);
        if (dish == null)
        {
            return NotFoundPage("DishNotFound");
        }

        await LoadCategoriesAsync(dish.DishCategoryId);
        return View(dish);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        int id,
        [Bind("Name,Price,PreparationTime,IsAvailable,Description,DishCategoryId")] Dish_BIT240049 dish)
    {
        var existing = await _db.Dishes.FirstOrDefaultAsync(d => d.Id == id);
        if (existing == null)
        {
            return NotFoundPage("DishNotFound");
        }

        dish.Id = id;
        dish.Name = dish.Name?.Trim() ?? "";
        dish.Description = string.IsNullOrWhiteSpace(dish.Description) ? null : dish.Description.Trim();

        await ValidateDishAsync(dish, excludeId: id);

        if (ModelState.IsValid)
        {
            existing.Name = dish.Name;
            existing.Price = dish.Price;
            existing.PreparationTime = dish.PreparationTime;
            existing.IsAvailable = dish.IsAvailable;
            existing.Description = dish.Description;
            existing.DishCategoryId = dish.DishCategoryId;

            try
            {
                await _db.SaveChangesAsync();
                TempData["Success"] = _L["SavedSuccess"].Value;
                return RedirectToAction(nameof(Detail), new { id });
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError(string.Empty, _L["DbError"]);
            }
        }

        await LoadCategoriesAsync(dish.DishCategoryId);
        return View(dish);
    }

    // ------------------------------------------------------------------
    // Delete mon an (anh bi xoa theo do cascade)
    // ------------------------------------------------------------------
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var dish = await _db.Dishes.FirstOrDefaultAsync(d => d.Id == id);
        if (dish == null)
        {
            return NotFoundPage("DishNotFound");
        }

        _db.Dishes.Remove(dish);
        try
        {
            await _db.SaveChangesAsync();
            TempData["Success"] = _L["DeletedSuccess"].Value;
        }
        catch (DbUpdateException)
        {
            TempData["Error"] = _L["DbError"].Value;
        }
        return RedirectToAction(nameof(Index));
    }

    // ------------------------------------------------------------------
    // Chuc nang 4: quan ly anh
    // ------------------------------------------------------------------
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> AddImage(int id, string? imageUrl, bool isThumbnail)
    {
        if (!await _db.Dishes.AnyAsync(d => d.Id == id))
        {
            return NotFoundPage("DishNotFound");
        }

        imageUrl = imageUrl?.Trim();
        if (string.IsNullOrEmpty(imageUrl))
        {
            TempData["Error"] = _L["ImageUrlRequired"].Value;
            return RedirectToAction(nameof(Detail), new { id });
        }

        var validUrl = Uri.TryCreate(imageUrl, UriKind.Absolute, out var uri)
                       && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
        if (!validUrl || imageUrl.Length > 500)
        {
            TempData["Error"] = _L["ImageUrlInvalid"].Value;
            return RedirectToAction(nameof(Detail), new { id });
        }

        await using var tx = await _db.Database.BeginTransactionAsync();

        if (isThumbnail)
        {
            // Moi mon toi da 1 anh dai dien: bo co o cac anh cu truoc
            await _db.DishImages
                .Where(i => i.DishId == id && i.IsThumbnail)
                .ExecuteUpdateAsync(s => s.SetProperty(i => i.IsThumbnail, false));
        }

        _db.DishImages.Add(new DishImage_BIT240049
        {
            DishId = id,
            ImageUrl = imageUrl,
            IsThumbnail = isThumbnail
        });
        await _db.SaveChangesAsync();
        await tx.CommitAsync();

        TempData["Success"] = _L["ImageAdded"].Value;
        return RedirectToAction(nameof(Detail), new { id });
    }

    // id = Id cua DishImage
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> SetThumbnail(int id)
    {
        var image = await _db.DishImages.AsNoTracking().FirstOrDefaultAsync(i => i.Id == id);
        if (image == null)
        {
            return NotFoundPage("ImageNotFound");
        }

        await using var tx = await _db.Database.BeginTransactionAsync();

        await _db.DishImages
            .Where(i => i.DishId == image.DishId && i.IsThumbnail && i.Id != id)
            .ExecuteUpdateAsync(s => s.SetProperty(i => i.IsThumbnail, false));

        await _db.DishImages
            .Where(i => i.Id == id)
            .ExecuteUpdateAsync(s => s.SetProperty(i => i.IsThumbnail, true));

        await tx.CommitAsync();

        TempData["Success"] = _L["ThumbnailChanged"].Value;
        return RedirectToAction(nameof(Detail), new { id = image.DishId });
    }

    // id = Id cua DishImage
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteImage(int id)
    {
        var image = await _db.DishImages.FirstOrDefaultAsync(i => i.Id == id);
        if (image == null)
        {
            return NotFoundPage("ImageNotFound");
        }

        var dishId = image.DishId;
        _db.DishImages.Remove(image);
        await _db.SaveChangesAsync();

        TempData["Success"] = _L["ImageDeleted"].Value;
        return RedirectToAction(nameof(Detail), new { id = dishId });
    }

    // ------------------------------------------------------------------
    // Helpers
    // ------------------------------------------------------------------
    private async Task LoadCategoriesAsync(int selectedId)
    {
        var categories = await _db.DishCategories.AsNoTracking().OrderBy(c => c.Name).ToListAsync();
        ViewBag.Categories = new SelectList(
            categories,
            nameof(DishCategory_BIT240049.Id),
            nameof(DishCategory_BIT240049.Name),
            selectedId);
        ViewBag.NoCategories = categories.Count == 0;
    }

    // DishCategoryId phai ton tai; ten mon khong trung trong cung mot loai
    private async Task ValidateDishAsync(Dish_BIT240049 dish, int excludeId)
    {
        if (dish.DishCategoryId <= 0)
        {
            return; // loi "chua chon loai" da duoc DataAnnotations bao
        }

        var categoryExists = await _db.DishCategories.AnyAsync(c => c.Id == dish.DishCategoryId);
        if (!categoryExists)
        {
            ModelState.AddModelError(nameof(Dish_BIT240049.DishCategoryId), _L["CategoryNotFound"]);
            return;
        }

        if (!string.IsNullOrWhiteSpace(dish.Name))
        {
            var name = dish.Name;
            var categoryId = dish.DishCategoryId;
            var duplicated = await _db.Dishes.AnyAsync(d =>
                d.DishCategoryId == categoryId && d.Name == name && d.Id != excludeId);
            if (duplicated)
            {
                ModelState.AddModelError(nameof(Dish_BIT240049.Name), _L["DishNameDuplicate"]);
            }
        }
    }
}

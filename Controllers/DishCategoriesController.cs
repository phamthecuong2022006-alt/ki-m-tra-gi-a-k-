using BIT240049_Mid.Data;
using BIT240049_Mid.Models;
using BIT240049_Mid.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace BIT240049_Mid.Controllers;

public class DishCategoriesController : AppController
{
    private readonly AppDbContext _db;
    private readonly IStringLocalizer<SharedResource> _L;

    public DishCategoriesController(AppDbContext db, IStringLocalizer<SharedResource> localizer)
    {
        _db = db;
        _L = localizer;
    }

    public async Task<IActionResult> Index()
    {
        var items = await _db.DishCategories
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .Select(c => new CategoryListItem
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                DishCount = c.Dishes.Count
            })
            .ToListAsync();
        return View(items);
    }

    public IActionResult Create() => View(new DishCategory_BIT240049());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Name,Description")] DishCategory_BIT240049 category)
    {
        Normalize(category);

        if (ModelState.IsValid)
        {
            _db.DishCategories.Add(category);
            try
            {
                await _db.SaveChangesAsync();
                TempData["Success"] = _L["SavedSuccess"].Value;
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError(string.Empty, _L["DbError"]);
            }
        }
        return View(category);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var category = await _db.DishCategories.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
        if (category == null)
        {
            return NotFoundPage("CategoryPageNotFound");
        }
        return View(category);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Name,Description")] DishCategory_BIT240049 category)
    {
        var existing = await _db.DishCategories.FirstOrDefaultAsync(c => c.Id == id);
        if (existing == null)
        {
            return NotFoundPage("CategoryPageNotFound");
        }

        category.Id = id;
        Normalize(category);

        if (ModelState.IsValid)
        {
            existing.Name = category.Name;
            existing.Description = category.Description;
            try
            {
                await _db.SaveChangesAsync();
                TempData["Success"] = _L["SavedSuccess"].Value;
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError(string.Empty, _L["DbError"]);
            }
        }
        return View(category);
    }

    // Chuc nang 5: khong cho xoa loai dang co mon su dung
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var category = await _db.DishCategories.FirstOrDefaultAsync(c => c.Id == id);
        if (category == null)
        {
            return NotFoundPage("CategoryPageNotFound");
        }

        if (await _db.Dishes.AnyAsync(d => d.DishCategoryId == id))
        {
            TempData["Error"] = _L["CategoryInUse"].Value;
            return RedirectToAction(nameof(Index));
        }

        _db.DishCategories.Remove(category);
        try
        {
            await _db.SaveChangesAsync();
            TempData["Success"] = _L["DeletedSuccess"].Value;
        }
        catch (DbUpdateException)
        {
            // Phong truong hop co mon vua duoc them vao loai nay (FK Restrict)
            TempData["Error"] = _L["CategoryInUse"].Value;
        }
        return RedirectToAction(nameof(Index));
    }

    private static void Normalize(DishCategory_BIT240049 category)
    {
        category.Name = category.Name?.Trim() ?? "";
        category.Description = string.IsNullOrWhiteSpace(category.Description) ? null : category.Description.Trim();
    }
}

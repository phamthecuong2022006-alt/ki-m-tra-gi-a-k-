using BIT240049_Mid.Models;
using Microsoft.EntityFrameworkCore;

namespace BIT240049_Mid.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<DishCategory_BIT240049> DishCategories => Set<DishCategory_BIT240049>();
    public DbSet<Dish_BIT240049> Dishes => Set<Dish_BIT240049>();
    public DbSet<DishImage_BIT240049> DishImages => Set<DishImage_BIT240049>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        // DishCategory 1 - n Dish ; khong cho xoa loai dang co mon
        b.Entity<Dish_BIT240049>()
            .HasOne(d => d.DishCategory)
            .WithMany(c => c.Dishes)
            .HasForeignKey(d => d.DishCategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        // Ten mon khong trung trong cung mot loai
        b.Entity<Dish_BIT240049>()
            .HasIndex(d => new { d.DishCategoryId, d.Name })
            .IsUnique();

        // Dish 1 - n DishImage ; xoa mon thi xoa anh
        b.Entity<DishImage_BIT240049>()
            .HasOne(i => i.Dish)
            .WithMany(d => d.Images)
            .HasForeignKey(i => i.DishId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

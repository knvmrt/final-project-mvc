using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebUI.Models;

namespace WebUI.Data
{
	public class AppDbContext : IdentityDbContext<AppUser>
	{
		public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

		public DbSet<Product> Products { get; set; }
		public DbSet<Category> Categories { get; set; }
		public DbSet<WishListItem> WishListItems{ get; set; }
		public DbSet<BasketItem> BasketItems { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId);

            builder.Entity<WishListItem>()
               .HasOne(p => p.AppUser)
               .WithMany(c => c.Wishlist)
               .HasForeignKey(p => p.AppUserId);

            builder.Entity<BasketItem>()
               .HasOne(p => p.AppUser)
               .WithMany(c => c.BasketItems)
               .HasForeignKey(p => p.AppUserId);
            
            // Seed roles
            builder.Entity<IdentityRole>().HasData(
	            new IdentityRole { Id = "1", Name = "Admin", NormalizedName = "ADMIN" },
	            new IdentityRole { Id = "2", Name = "User", NormalizedName = "USER" }
            );

            // Optionally, seed an admin user
            var hasher = new PasswordHasher<AppUser>();
            var adminUser = new AppUser
            {
	            Id = "1",
	            UserName = "admin@example.com",
	            NormalizedUserName = "ADMIN@EXAMPLE.COM",
	            Email = "admin@example.com",
	            NormalizedEmail = "ADMIN@EXAMPLE.COM",
	            EmailConfirmed = true,
	            FirstName = "Admin",
	            LastName = "User",
	            PasswordHash = hasher.HashPassword(null, "Admin@123")
            };

            builder.Entity<AppUser>().HasData(adminUser);

            // Assign admin role to the admin user
            builder.Entity<IdentityUserRole<string>>().HasData(
	            new IdentityUserRole<string> { RoleId = "1", UserId = "1" }
            );
        }
    }
}

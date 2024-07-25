using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebUI.Data;
using WebUI.Models;

namespace PuikWebUI.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public HomeController(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(string? search, string? category, double? minPrice, double? maxPrice, int page = 1, int pageSize = 10)
        {
            var products = _context.Products.AsQueryable();

            if (!String.IsNullOrEmpty(search))
            {
                products = products.Where(p => p.Name.Contains(search));
            }

            if (!String.IsNullOrEmpty(category))
            {
                products = products.Where(p => p.Category.CategoryName == category);
            }

            if (minPrice.HasValue)
            {
                products = products.Where(p => p.Price >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                products = products.Where(p => p.Price <= maxPrice.Value);
            }

            var totalItems = await products.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            var paginatedProducts = await products.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            var categories = await _context.Categories.ToListAsync();

            ViewBag.Categories = categories;
            ViewBag.SelectedCategory = category;
            ViewBag.MinPrice = minPrice;
            ViewBag.MaxPrice = maxPrice;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;

            return View(paginatedProducts);
        }

        public async Task<IActionResult> ShowWishlist()
        {
            var user = await _userManager.Users
                    .Include(u => u.Wishlist)
                    .ThenInclude(w => w.Product)
                    .FirstOrDefaultAsync(u => u.Id == _userManager.GetUserId(User));

            if (user == null) return Unauthorized();

            var wishlist = user?.Wishlist?.Select(p => p.Product)?.ToList() ?? new List<Product>();

            return View(wishlist);
        }

        [HttpPost]
        public async Task<IActionResult> addWishList(int id)
        {
            Product? product = await _context.Products.FindAsync(id);

            if (product == null) return NotFound();
            Category? category = await _context.Categories.FindAsync(product.CategoryId);
            if (category == null) return NotFound();
            product.Category = category;
            var user = await _userManager.Users
               .Include(u => u.BasketItems)
               .ThenInclude(b => b.Product)
               .Include(u => u.Wishlist)
                               .ThenInclude(w => w.Product)
                               .FirstOrDefaultAsync(u => u.Id == _userManager.GetUserId(User));

            if (user == null) return Unauthorized();

            if (user.Wishlist == null)
            {
                user.Wishlist = new List<WishListItem>();
            }

            if (!user.Wishlist.Any(p => p.Product.Id == product.Id))
            {
                user.Wishlist.Add(new WishListItem { Product = product });
            }
            else
            {
                var newWishList = new List<WishListItem>();
                foreach (var item in user.Wishlist)
                {
                    if (item.ProductId != product.Id)
                    {
                        newWishList.Add(item);
                    }
                }
                user.Wishlist = newWishList;
            }
            var result = await _userManager.UpdateAsync(user);

            var refererUrl = Request.Headers["Referer"].ToString();
            if (Uri.TryCreate(refererUrl, UriKind.Absolute, out var uri))
            {
                var action = uri.AbsolutePath.Split('/').Last();
                var controller = uri.AbsolutePath.Split('/').Reverse().Skip(1).FirstOrDefault();

                if (!string.IsNullOrEmpty(action) && !string.IsNullOrEmpty(controller))
                {
                    return RedirectToAction(action, controller);
                }
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId, int count)
        {
            var userId = _userManager.GetUserId(User);
            var user = await _userManager.Users
               .Include(u => u.BasketItems)
               .ThenInclude(b => b.Product)
               .Include(u => u.Wishlist)
                               .ThenInclude(w => w.Product)
                               .FirstOrDefaultAsync(u => u.Id == _userManager.GetUserId(User));

            if (user != null)
            {
                var product = _context.Products.Find(productId);
                if (product != null)
                {
                    if (user.BasketItems.Any(b => b?.Product.Id == productId))
                    {
                        var basketItems = user.BasketItems;
                        foreach (var item in basketItems)
                        {
                            if (item.Product.Id == productId) item.Count += count;
                        }
                        user.BasketItems = basketItems;
                    }
                    else
                    {
                        var basketItem = new BasketItem
                        {
                            Product = product,
                            ProductId = productId,
                            Count = count,
                            AppUser = user,
                            AppUserId = user.Id
                        };
                        user.BasketItems.Add(basketItem);
                    }
                    await _userManager.UpdateAsync(user);
                }
            }

            return Ok(new { message = "Product added to cart successfully" });
        }

        [HttpPost]
        public async Task<IActionResult> removeFromBasket(int basketItemId)
        {
            var userId = _userManager.GetUserId(User);
            var user = await _userManager.Users
               .Include(u => u.BasketItems)
               .ThenInclude(b => b.Product)
               .Include(u => u.Wishlist)
                               .ThenInclude(w => w.Product)
                               .FirstOrDefaultAsync(u => u.Id == _userManager.GetUserId(User));

            if (user != null)
            {
                var basketItem = user.BasketItems.Find(b => b.Id == basketItemId);
                if (basketItem != null)
                {
                    user.BasketItems.Remove(basketItem);
                }
                await _userManager.UpdateAsync(user);
            }

            var refererUrl = Request.Headers["Referer"].ToString();
            if (Uri.TryCreate(refererUrl, UriKind.Absolute, out var uri))
            {
                var action = uri.AbsolutePath.Split('/').Last();
                var controller = uri.AbsolutePath.Split('/').Reverse().Skip(1).FirstOrDefault();

                if (!string.IsNullOrEmpty(action) && !string.IsNullOrEmpty(controller))
                {
                    return RedirectToAction(action, controller);
                }
            }

            return RedirectToAction("Index", "Home");
        }
    }
}

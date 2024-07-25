using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebUI.DTOs;
using WebUI.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace WebUI.Controllers
{
	public class AuthController : Controller
	{
		private readonly UserManager<AppUser> _userManager;
		private readonly SignInManager<AppUser> _signInManager;
		private readonly RoleManager<IdentityRole> _roleManager;


        public AuthController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> CreateDefaultRole()
        {
            IdentityRole defaultrole = new IdentityRole { Name = "Admin" };

            await _roleManager.CreateAsync(defaultrole);
            return RedirectToAction("Index","Home");
        }

        [HttpGet]
		public IActionResult Login()
		{
			if (User.Identity.IsAuthenticated)
			{
				return RedirectToAction("Index", "Home");
			}
			return View();
		}

        [HttpPost]
        public async Task<IActionResult> Login(LoginDTO loginDTO)
        {
            if (!ModelState.IsValid)
            {
                return View(loginDTO);
            }

            var user = await _userManager.FindByEmailAsync(loginDTO.Email);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Email or Password is incorrect!");
                return View(loginDTO);
            }

            var result = await _signInManager.PasswordSignInAsync(user.UserName, loginDTO.Password, loginDTO.RememberMe, true);
            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Email or Password is incorrect!");
                return View(loginDTO);
            }
        }

        [HttpGet]
		public IActionResult Register()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Register(RegisterDTO registerDTO)
		{
			if (!ModelState.IsValid)
			{
				return View();
			}

			var user = await _userManager.FindByEmailAsync(registerDTO.Email);
			if (user != null)
			{
				ModelState.AddModelError("Error", "Email is already in use.");
				return View();
			}

			var newUser = new AppUser
			{
				Email = registerDTO.Email,
				UserName = registerDTO.Email,
				FirstName = registerDTO.FirstName,
				LastName = registerDTO.LastName,
			};

			var result = await _userManager.CreateAsync(newUser, registerDTO.Password);
			if (result.Succeeded)
			{
				await _userManager.AddToRoleAsync(newUser, "User");
				return RedirectToAction(nameof(Login));
			}
			else
			{
				foreach (var error in result.Errors)
				{
					ModelState.AddModelError("Error", error.Description);
				}
				return View();
			}
		
		}

		[HttpGet]
		[Authorize]
		public async Task<IActionResult> Logout()
		{
			await _signInManager.SignOutAsync();

			return RedirectToAction(nameof(Login));
		}
	}
}






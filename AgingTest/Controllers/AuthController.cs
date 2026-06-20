using AgingTest.Data;
using AgingTest.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AgingTest.Controllers
{
    public class AuthController : Controller
    {
        private readonly AppDbContext _context;
        private readonly PasswordHasher<UserModel> _passwordHasher;

        public AuthController(AppDbContext context)
        {
            _context = context;
            _passwordHasher = new PasswordHasher<UserModel>();
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string badge, string password)
        {
            var user = await _context.tb_users
                .FirstOrDefaultAsync(u => u.user_badge == badge && u.user_status == true);

            if (user == null)
            {
                ViewBag.Error = "User tidak ditemukan atau tidak aktif.";
                return View();
            }

            var result = _passwordHasher.VerifyHashedPassword(
                user,
                user.user_password,
                password
            );

            if (result == PasswordVerificationResult.Success)
            {
                user.last_active = DateTime.Now;
                await _context.SaveChangesAsync();

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.id_user.ToString()),
                    new Claim(ClaimTypes.Name, user.username),
                    new Claim("Badge", user.user_badge),
                    new Claim(ClaimTypes.Role, user.user_role)
                };

                var claimsIdentity = new ClaimsIdentity(
                    claims,
                    CookieAuthenticationDefaults.AuthenticationScheme
                );

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity)
                );

                return RedirectToAction("Index", "Main");
            }

            ViewBag.Error = "Password salah!";
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Login");
        }
    }
}
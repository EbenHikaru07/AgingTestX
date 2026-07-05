using AgingTest.Data;
using AgingTest.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authorization;

namespace AgingTest.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;
        private readonly PasswordHasher<UserModel> _passwordHasher;

        public AdminController(AppDbContext context)
        {
            _context = context;
            _passwordHasher = new PasswordHasher<UserModel>();
        }

        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> UserData()
        {
            var users = await _context.tb_users
                .OrderByDescending(u => u.created_at)
                .ToListAsync();

            return View(users);
        }
        public IActionResult RegisterUser()
        {
            return View();
        }

        //private string HashPasswordRegist(string password)
        //{
        //    using (SHA256 sha256 = SHA256.Create())
        //    {
        //        var bytes = Encoding.UTF8.GetBytes(password);
        //        var hash = sha256.ComputeHash(bytes);

        //        return Convert.ToBase64String(hash);
        //    }
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterUser(UserModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Data tidak valid.";
                return View(model);
            }

            try
            {
                var badgeExist = await _context.tb_users
                    .AnyAsync(x => x.user_badge == model.user_badge);

                if (badgeExist)
                {
                    TempData["Error"] = "Badge sudah digunakan.";
                    return View(model);
                }

                var userExist = await _context.tb_users
                    .AnyAsync(x => x.username == model.username);

                if (userExist)
                {
                    TempData["Error"] = "Username sudah digunakan.";
                    return View(model);
                }

                model.user_password =
                    _passwordHasher.HashPassword(
                        model,
                        model.user_password
                    );

                model.created_at = DateTime.Now;
                model.updated_at = DateTime.Now;
                model.user_status = true;

                _context.tb_users.Add(model);

                await _context.SaveChangesAsync();

                TempData["Success"] = "User berhasil didaftarkan.";

                return RedirectToAction("UserData");
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;

                return View(model);
            }
        }
    }
}

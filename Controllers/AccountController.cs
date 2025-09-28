using BeePM.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
 

namespace BeePM.Controllers
{
    public class AccountController : Controller
    {
        private readonly BeePMDbContext _db;

        public AccountController(BeePMDbContext db)
        {
            _db = db;
        }

        // ✅ Profil
        public IActionResult Profile()
        {
            var username = User.Identity?.Name;
            if (string.IsNullOrEmpty(username))
                return RedirectToAction("Login");

            var user = _db.Users.FirstOrDefault(u => u.Username == username);
            if (user == null)
                return RedirectToAction("Login");

            return View(user);
        }

        // ✅ Parola değiştir (GET)
        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        // ✅ Parola değiştir (POST)
        [HttpPost]
        public IActionResult ChangePassword(string oldPassword, string newPassword)
        {
            var username = User.Identity?.Name;
            var user = _db.Users.FirstOrDefault(u => u.Username == username);

            if (user == null)
                return RedirectToAction("Login");

            if (!string.IsNullOrEmpty(user.PasswordHash) && user.PasswordHash != oldPassword)
            {
                ViewBag.Error = "Mevcut parola yanlış!";
                return View();
            }

            user.PasswordHash = newPassword; // ileride hash yapılacak
            _db.SaveChanges();

            ViewBag.Message = "Parola başarıyla değiştirildi.";
            return View();
        }

        // ✅ Çıkış 
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Login");
        }
        // ✅ Giriş (şimdilik basit)
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username)
        {
            var user = _db.Users.FirstOrDefault(u => u.Username == username);
            if (user == null)
            {
                ModelState.AddModelError("", "Kullanıcı bulunamadı.");
                return View();
            }

            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim("FullName", user.FullName),
            new Claim("Role", user.Role)
        };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return RedirectToAction("Index", "Approval");
        }

    }
}
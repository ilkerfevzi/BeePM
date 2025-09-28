using BeePM.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims; 
using System.Security.Cryptography;
using System.Text;
using BCrypt.Net;
using BCryptNet = BCrypt.Net.BCrypt;

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

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            _db.SaveChanges();

            ViewBag.Message = "Parola başarıyla değiştirildi.";
            return View();
        } 
         
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }



        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError("", "Kullanıcı adı ve şifre girilmelidir.");
                return View();
            }

            var user = _db.Users.FirstOrDefault(u => u.Username == username);
            if (user == null)
            {
                ModelState.AddModelError("", "Kullanıcı bulunamadı.");
                return View();
            }
            // var hash = BCrypt.Net.BCrypt.HashPassword("admin123");
            // Console.WriteLine(hash);
            // ✅ Şifre doğrulama (trim ile)
            var passwordClean = password.Trim();
            if (string.IsNullOrEmpty(user.PasswordHash) || !BCrypt.Net.BCrypt.Verify(passwordClean, user.PasswordHash))
            {
                ModelState.AddModelError("", "Geçersiz şifre.");
                return View();
            }

            // ✅ Claims
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, user.Username),
        new Claim("FullName", user.FullName),
        new Claim(ClaimTypes.Role, user.Role)
    };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return RedirectToAction("Index", "Home");
        }


        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }


    }
}
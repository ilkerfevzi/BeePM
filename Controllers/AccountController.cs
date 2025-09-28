using BeePM.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

//namespace BeePM.Controllers
//{
//    public class AccountController : Controller
//    {
//        private readonly BeePMDbContext _db;

//        public AccountController(BeePMDbContext db)
//        {
//            _db = db;
//        }

//        [HttpGet]
//        public IActionResult Login()
//        {
//            return View();
//        }

//        [HttpPost]
//        public async Task<IActionResult> Login(string username, string password)
//        {
//            // Şifreyi SHA256 hash’le
//            using var sha = SHA256.Create();
//            var hash = BitConverter.ToString(
//                sha.ComputeHash(Encoding.UTF8.GetBytes(password))
//            ).Replace("-", "").ToLower();

//            // Kullanıcıyı DB’den kontrol et
//            var user = _db.Users.FirstOrDefault(u =>
//                u.Username == username && u.PasswordHash == hash);

//            if (user == null)
//            {
//                ViewBag.Error = "Kullanıcı adı veya şifre hatalı!";
//                return View();
//            }

//            // Claims oluştur
//            var claims = new List<Claim>
//            {
//                new Claim(ClaimTypes.Name, user.Username),
//                new Claim("FullName", user.FullName),
//                new Claim(ClaimTypes.Role, user.Role) // Örn: Employee, Manager, Board, Admin
//            };

//            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
//            var principal = new ClaimsPrincipal(identity);

//            // Cookie oluştur
//            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

//            return RedirectToAction("Index", "Home");
//        }

//        [HttpGet]
//        public async Task<IActionResult> Logout()
//        {
//            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
//            return RedirectToAction("Login");
//        }
//    }
//}

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
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        // ✅ Giriş (şimdilik basit)
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            var user = _db.Users.FirstOrDefault(u => u.Username == username && (u.PasswordHash == null || u.PasswordHash == password));

            if (user == null)
            {
                ViewBag.Error = "Geçersiz kullanıcı adı veya parola.";
                return View();
            }

            var claims = new List<System.Security.Claims.Claim>
            {
                new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, user.Username),
                new System.Security.Claims.Claim("FullName", user.FullName),
                new System.Security.Claims.Claim("Role", user.Role ?? "Guest")
            };

            var identity = new System.Security.Claims.ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new System.Security.Claims.ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return RedirectToAction("Index", "Home");
        }
    }
}
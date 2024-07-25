using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using WebApplication1.Data;

namespace WebApplication1.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)

        {

        _context = context; 
        
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(string username, string password)
        {
            var passwordHash = ComputeSha256Hash(password);
            await _context.RegisterUserAsync(username, passwordHash);
            return RedirectToAction(nameof(Login));
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            var passwordHash = ComputeSha256Hash(password);
            if (await _context.ValidateUserLoginAsync(username, passwordHash))
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, username)
                };
                var claimsIdentity = new ClaimsIdentity(claims, "MyCookieAuthenticationScheme");
                await HttpContext.SignInAsync("MyCookieAuthenticationScheme", new ClaimsPrincipal(claimsIdentity));

                return RedirectToAction("Index", "Customer");
            }
            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {

            await HttpContext.SignOutAsync("MyCookieAuthenticationScheme");
            return RedirectToAction("Login", "Account");
        }
        private string ComputeSha256Hash(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes= sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(bytes).Replace("-", "").ToLower();
            }
        }
    }
}

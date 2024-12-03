using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using WebApp2.Data;
using WebApp2.Models;

namespace WebApp2.Controllers
{
    [Route("[controller]")]
    public class AccountController : Controller
    {
        private readonly AuthService _authService;

        public AccountController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpGet("register")]
        public IActionResult Register()
        {
            return View(new UserDto());
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(string userName, string password, string phone)
        {
            var userDto = new UserDto
            {
                Username = userName,
                Password = password,
                PhoneNumber = phone
            };
            var result = await _authService.RegisterAsync(userDto);
            return RedirectToAction("Login");
        }

        [HttpGet("login")]
        public IActionResult Login()
        {
            return View(new UserDto());
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(string userName, string password)
        {
            var userDto = new UserDto
            {
                Username = userName,
                Password = password,
                PhoneNumber = ""
            };

            Console.WriteLine($"Username: {userDto.Username}, Password: {userDto.Password}, PhoneNumber: {userDto.PhoneNumber}");

            var token = await _authService.LoginAsync(userDto);

            if (token == "Dont Login")
            {
                // Обработка ошибки: токен не был получен
                ModelState.AddModelError(string.Empty, "Неверное имя пользователя или пароль.");
                return View(userDto); // Вернуть представление с ошибкой
            }

            var encodedToken = Uri.EscapeDataString(token);
            HttpContext.Response.Cookies.Append("Token", encodedToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddDays(7)
            });

            return RedirectToAction("Index", "Home");
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            HttpContext.Response.Cookies.Delete("Token");
            HttpContext.Response.Cookies.Delete("Cart");

            // Обновляем ViewBag.Username после выхода
            var token = HttpContext.Request.Cookies["Token"];
            string username = null;

            if (!string.IsNullOrEmpty(token))
            {
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);
                username = jwtToken.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
            }

            ViewBag.Username = username; // Обновляем ViewBag.Username
            return RedirectToAction("Index", "BrosShopProducts");
        }

    }

}
using Microsoft.AspNetCore.Mvc;
using ProjWebApp.Models;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using WebApp2.Models;

namespace WebApp2.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            var token = HttpContext.Request.Cookies["Token"];
            string username = null;

            if (!string.IsNullOrEmpty(token))
            {
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);
                username = jwtToken.Claims.FirstOrDefault(c => c.Type == "sub")?.Value; // "sub" - это стандартное им€ дл€ идентификатора пользовател€
            }

            ViewBag.Username = username; // ѕередаем им€ пользовател€ в представление
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

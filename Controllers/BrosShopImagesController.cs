using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApp2.Data;
using WebApp2.Models;

namespace WebApp2.Controllers
{
    public class BrosShopImagesController : Controller
    {
        private readonly ApplicationContext _context;
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        public BrosShopImagesController(ApplicationContext context, IConfiguration configuration, HttpClient httpClient)
        {
            _context = context;
            _configuration = configuration;
            _httpClient = httpClient;
        }

        // GET: BrosShopImages
        public async Task<IActionResult> Index()
        {
            var applicationContext = _context.BrosShopImages.Include(b => b.BrosShopProduct);
            return View(await applicationContext.ToListAsync());
        }

        // GET: BrosShopImages/GetImage/{productId}
        [HttpGet]
        // Метод для получения изображения по ID изображения
        public async Task<IActionResult> GetImage(int imageId)
        {
            // Получаем изображение по его идентификатору
            var image = await _context.BrosShopImages
                .FirstOrDefaultAsync(i => i.BrosShopImagesId == imageId); // Используем BrosShopImagesId

            if (image == null)
            {
                return NotFound(); // Если изображение не найдено, возвращаем 404
            }

            // Шаг 2: Получаем BaseUrl из конфигурации
            var baseUrl = _configuration["ApiSettings:BaseUrl"];

            // Шаг 3: Конструируем полный URL API
            var apiUrl = $"{baseUrl}{image.BrosShopImagesId}";

            // Получаем изображение из API
            var response = await _httpClient.GetAsync(apiUrl);
            if (response.IsSuccessStatusCode)
            {
                var imageBytes = await response.Content.ReadAsByteArrayAsync();
                return File(imageBytes, "image/jpeg"); // Возвращаем изображение с правильным MIME-типом
            }

            return NotFound(); // Если запрос к API не успешен, возвращаем 404
        }
    }
}

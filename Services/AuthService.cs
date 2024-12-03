using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using WebApp2.Models;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Buffers; // Замените YourNamespace на фактическое пространство имен вашего проекта

public class AuthService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;


    public AuthService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _configuration = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json")
        .Build();
    }

    public async Task<string> RegisterAsync(UserDto userDto)
    {
        var response = await _httpClient.PostAsJsonAsync("register", userDto);
        if (response.IsSuccessStatusCode)
        {
            var jsonResponse = await response.Content.ReadAsStringAsync();
            return jsonResponse;
        }
        return null;
    }

    public async Task<string> LoginAsync(UserDto userDto)
    {
        userDto.PhoneNumber = "";
        var response = await _httpClient.PostAsJsonAsync("login", userDto);

        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
            return result.Token;
        }

        Console.WriteLine($"Error: {response.StatusCode}, {response.Content}");
        Console.WriteLine($"Username: {userDto.Username}, Password: {userDto.Password}, PhoneNumber: {userDto.PhoneNumber}");

        return "Dont Login";
    }
}

public class LoginResponse
{
    public string Token { get; set; }
}

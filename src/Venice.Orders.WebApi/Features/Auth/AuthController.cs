using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Venice.Orders.Domain.Repositories;
using Venice.Orders.Domain.Entities;
using System.Security.Cryptography;

namespace Venice.Orders.WebApi.Features.Auth;

[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly IUserRepository _userRepository;

    public AuthController(IConfiguration configuration, IUserRepository userRepository)
    {
        _configuration = configuration;
        _userRepository = userRepository;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password) || string.IsNullOrEmpty(request.Email))
        {
            return BadRequest(new { message = "Username, email and password are required" });
        }

        // Verificar se o usuário já existe
        if (await _userRepository.UsernameExistsAsync(request.Username))
        {
            return BadRequest(new { message = "Username already exists" });
        }

        if (await _userRepository.EmailExistsAsync(request.Email))
        {
            return BadRequest(new { message = "Email already exists" });
        }

        // Criar hash da senha
        var passwordHash = HashPassword(request.Password);

        // Criar usuário
        var user = new User(request.Username, request.Email, passwordHash);
        var createdUser = await _userRepository.CreateAsync(user);

        var token = GenerateJwtToken(createdUser.Username);
        return Ok(new { 
            message = "User registered successfully",
            token,
            user = new { username = createdUser.Username, email = createdUser.Email }
        });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
        {
            return BadRequest(new { message = "Username and password are required" });
        }

        // Buscar usuário por username
        var user = await _userRepository.GetByUsernameAsync(request.Username);
        if (user == null || !user.IsActive)
        {
            return Unauthorized(new { message = "Invalid credentials" });
        }

        // Verificar senha
        if (!VerifyPassword(request.Password, user.PasswordHash))
        {
            return Unauthorized(new { message = "Invalid credentials" });
        }

        // Atualizar último login
        user.LastLoginAt = DateTime.UtcNow;
        await _userRepository.UpdateAsync(user);

        var token = GenerateJwtToken(user.Username);
        return Ok(new { token });
    }

    private string GenerateJwtToken(string username)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? "your-secret-key-here"));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.Role, "User")
        };

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"] ?? "VeniceOrders",
            audience: _configuration["Jwt:Audience"] ?? "VeniceOrders",
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
    }

    private bool VerifyPassword(string password, string hash)
    {
        var hashedPassword = HashPassword(password);
        return hashedPassword == hash;
    }

    [HttpPost("init-test-data")]
    public async Task<IActionResult> InitializeTestData()
    {
        try
        {
            // Verificar se já existe um usuário admin
            var existingAdmin = await _userRepository.GetByUsernameAsync("admin");
            if (existingAdmin != null)
            {
                return Ok(new { message = "Test data already exists" });
            }

            // Criar usuário admin padrão
            var adminUser = new User("admin", "admin@venice.com", HashPassword("password"));
            await _userRepository.CreateAsync(adminUser);

            return Ok(new { 
                message = "Test data initialized successfully",
                adminUser = new { username = adminUser.Username, email = adminUser.Email }
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = $"Error initializing test data: {ex.Message}" });
        }
    }
}

public class LoginRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class RegisterRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}


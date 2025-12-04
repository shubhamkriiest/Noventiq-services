using DotNetAssignment.DTOs;
using DotNetAssignment.Models;
using DotNetAssignment.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DotNetAssignment.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        private readonly SimpleLocalizer _simpleLocalizer;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IRefreshTokenRepository _refreshTokenRepository;

        public AuthService(IUserRepository userRepository, IConfiguration configuration, SimpleLocalizer simpleLocalizer, IHttpContextAccessor httpContextAccessor, IRefreshTokenRepository refreshTokenRepository)
        {
            _userRepository = userRepository;
            _configuration = configuration;
            _simpleLocalizer = simpleLocalizer;
            _httpContextAccessor = httpContextAccessor;
            _refreshTokenRepository = refreshTokenRepository;
        }

        public async Task<(bool Success, string Message, LoginResponseDto? Response)> LoginAsync(LoginDto loginDto)
        {
            // Find user by username and include role
            var user = await _userRepository.GetByUsernameAsync(loginDto.Username);
            
            if (user == null)
                return (false, _simpleLocalizer.Get("InvalidCredentials", _httpContextAccessor.HttpContext), null);

            // Verify password using BCrypt
            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash);
            
            if (!isPasswordValid)
                return (false, _simpleLocalizer.Get("InvalidCredentials", _httpContextAccessor.HttpContext), null);

            // Load role if not loaded
            user = await _userRepository.GetUserWithRoleAsync(user.Id);

            // Generate JWT token
            var token = GenerateJwtToken(user!);
            var expiryMinutes = int.Parse(_configuration["JwtSettings:ExpiryMinutes"]!);

            var response = new LoginResponseDto
            {
                Token = token,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role.Name,
                ExpiresAt = DateTime.UtcNow.AddMinutes(expiryMinutes),
                RefreshToken = GenerateRefreshToken(out var refreshExpires)
            };

            // Persist refresh token
            await _refreshTokenRepository.CreateAsync(new RefreshToken
            {
                UserId = user.Id,
                Token = response.RefreshToken,
                ExpiresAt = refreshExpires
            });

            return (true, _simpleLocalizer.Get("LoginSuccessful", _httpContextAccessor.HttpContext), response);
        }

        public async Task<(bool Success, string Message, RefreshResponseDto? Response)> RefreshAsync(string refreshToken)
        {
            var record = await _refreshTokenRepository.GetByTokenAsync(refreshToken);
            if (record == null || record.IsRevoked || record.ExpiresAt <= DateTime.UtcNow)
                return (false, _simpleLocalizer.Get("InvalidCredentials", _httpContextAccessor.HttpContext), null);

            var user = await _userRepository.GetUserWithRoleAsync(record.UserId);
            if (user == null)
                return (false, _simpleLocalizer.Get("InvalidCredentials", _httpContextAccessor.HttpContext), null);

            // revoke old
            record.IsRevoked = true;
            record.RevokedAt = DateTime.UtcNow;
            await _refreshTokenRepository.UpdateAsync(record);

            // issue new
            var token = GenerateJwtToken(user);
            var expiryMinutes = int.Parse(_configuration["JwtSettings:ExpiryMinutes"]!);
            var newRefresh = GenerateRefreshToken(out var refreshExpires);
            await _refreshTokenRepository.CreateAsync(new RefreshToken
            {
                UserId = user.Id,
                Token = newRefresh,
                ExpiresAt = refreshExpires
            });

            var resp = new RefreshResponseDto
            {
                Token = token,
                ExpiresAt = DateTime.UtcNow.AddMinutes(expiryMinutes),
                RefreshToken = newRefresh
            };

            return (true, _simpleLocalizer.Get("LoginSuccessful", _httpContextAccessor.HttpContext), resp);
        }

        public async Task<(bool Success, string Message)> RegisterAsync(CreateUserDto registerDto)
        {
            // Check if username exists
            var existingUser = await _userRepository.GetByUsernameAsync(registerDto.Username);
            if (existingUser != null)
                return (false, _simpleLocalizer.Get("UsernameExists", _httpContextAccessor.HttpContext));

            // Check if email exists
            var existingEmail = await _userRepository.GetByEmailAsync(registerDto.Email);
            if (existingEmail != null)
                return (false, _simpleLocalizer.Get("EmailExists", _httpContextAccessor.HttpContext));

            // Create new user with hashed password
            var user = new User
            {
                Username = registerDto.Username,
                Email = registerDto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password),
                RoleId = registerDto.RoleId
            };

            await _userRepository.CreateAsync(user);

            return (true, _simpleLocalizer.Get("RegistrationSuccessful", _httpContextAccessor.HttpContext));
        }

        private string GenerateJwtToken(User user)
        {
            // Get JWT settings from configuration
            var secretKey = _configuration["JwtSettings:SecretKey"]!;
            var issuer = _configuration["JwtSettings:Issuer"]!;
            var audience = _configuration["JwtSettings:Audience"]!;
            var expiryMinutes = int.Parse(_configuration["JwtSettings:ExpiryMinutes"]!);

            // Create security key from secret
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Define claims (user information stored in token)
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role.Name),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) // Unique token ID
            };

            // Create token
            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
                signingCredentials: credentials
            );

            // Return token as string
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string GenerateRefreshToken(out DateTime refreshExpires)
        {
            var bytes = new byte[64];
            System.Security.Cryptography.RandomNumberGenerator.Fill(bytes);
            refreshExpires = DateTime.UtcNow.AddDays(7);
            return Convert.ToBase64String(bytes)
                .TrimEnd('=')
                .Replace('+', '-')
                .Replace('/', '_');
        }
    }
}
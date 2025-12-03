namespace DotNetAssignment.DTOs
{
    public class CreateUserDto
    {
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;  // Plain password (will be hashed)
        public int RoleId { get; set; }
    }
}
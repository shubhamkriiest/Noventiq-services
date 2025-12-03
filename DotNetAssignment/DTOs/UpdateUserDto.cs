namespace DotNetAssignment.DTOs
{
    public class UpdateUserDto
    {
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Password { get; set; }  // Optional - only if changing password
        public int RoleId { get; set; }
    }
}
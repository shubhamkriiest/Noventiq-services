namespace DotNetAssignment.DTOs
{
    public class RefreshResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public string RefreshToken { get; set; } = string.Empty;
    }
}

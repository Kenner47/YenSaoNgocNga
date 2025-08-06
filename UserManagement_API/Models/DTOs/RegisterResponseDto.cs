namespace UserManagement_API.Models.DTOs
{
    public class RegisterResponseDto
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}

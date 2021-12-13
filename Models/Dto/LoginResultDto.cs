namespace BookStore.Models.Dto
{
    public class LoginResultDto
    {
        public bool Success { get; set; }

        public string? Message { get; set; }

        public string? AccessToken { get; set; }

        public string? RefreshToken { get; set; }

        public long Expires { get; set; }

        public UserDto? User { get; set; }
    }
}

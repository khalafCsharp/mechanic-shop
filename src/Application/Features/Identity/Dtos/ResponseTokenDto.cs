namespace Application.Features.Identity.Dtos
{
    public class ResponseToken
    {
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
        public DateTimeOffset ExpirationDate { get; set; }
    }
}
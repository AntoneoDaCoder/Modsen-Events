namespace Events.Application.Contracts
{
    public record RefreshTokenDTO
    {
        public string AccessToken {  get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
    }
}

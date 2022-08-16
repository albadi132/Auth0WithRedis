namespace Auth0WithRedis.Dto.Tokens
{
    public class RefreshAndAccessToken
    {

        public string? AccessToken { get; set; }

        public string? RefreshToken { get; set; }
    }
}

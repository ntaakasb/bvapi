namespace WebCore.Infrastructure.BusinessObjects
{
    public class TokenOptions
    {
        public string Secret { get; set; }
        public string RefreshSecret { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public int AccessExpiration { get; set; }
        public int RefreshExpiration { get; set; }
        public int ResetPasswordExpirationTime { get; set; }
    }
}
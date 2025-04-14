namespace Data.Models
{
    public class DbSetupModel
    {
        public string ConnectionStrings { get; set; } = string.Empty;
    }

    public class JwtModel
    {
        public string? ValidAudience { get; set; }
        public string? ValidIssuer { get; set; }
        public string? Secret { get; set; }
    }

    public class JWTToken
    {
        public string? TokenString { get; set; }
        public Guid Id { get; set; }
        public string? Avatar { get; set; }
        public string? Name { get; set; }
        public string? Role { get; set; }
        public string? Email { get; set; }
        public long ExpiresInMilliseconds { get; set; }
    }

    public class FirebaseStorageModel
    {
        public string? FirebaseSDKFile { get; set; }
        public string? ProjectId { get; set; }
        public string? Bucket { get; set; }
    }

    public class MailSetupModel
    {
        public string? FromEmail { get; set; }
        public string? DisplayName { get; set; }
        public string? Password { get; set; }
        public string? SmtpServer { get; set; }
        public int SmtpPort { get; set; }
    }

}

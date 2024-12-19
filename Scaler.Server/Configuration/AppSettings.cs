namespace Scaler.Server.Configuration
{
    /// <summary>
    /// 
    /// </summary>
    public class AppSettings
    {
        public SmtpConfig? SmtpConfig { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class SmtpConfig
    {
        public string? Name { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public required string Host { get; set; }
        public int Port { get; set; }
        public bool UseSsl { get; set; }
        public required string EmailAddress { get; set; }
    }
}

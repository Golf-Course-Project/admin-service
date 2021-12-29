namespace AdminService.Misc
{
    public class AppSettings: IAppSettings
    {
        public string IdentityServiceConnectionString { get; set; }      
        public string SaltHash { get; set; }
        public string SecurityKey { get; set; }
        public string SmtpHost { get; set; }
        public string SmtpFrom { get; set; }
        public string SmtpUserName { get; set; }
        public string SmtpPassword { get; set; }
        public string BaseUrl { get; set; }
    }

    public interface IAppSettings
    {
        string IdentityServiceConnectionString { get; set; }  
        string SaltHash { get; set; }
        string SecurityKey { get; set; }
        string SmtpHost { get; set; }
        string SmtpFrom { get; set; }
        string SmtpUserName { get; set; }
        string SmtpPassword { get; set; }  
        string BaseUrl { get; set; }
    }
}

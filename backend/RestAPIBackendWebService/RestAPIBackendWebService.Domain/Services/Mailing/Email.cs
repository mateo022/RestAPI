namespace RestAPIBackendWebService.Domain.Services.Mailing
{
    public class Email
    {
        public string From { get; set; }
        public string FromName { get; set; }
        public string To { get; set; }
        public string SmtpUserName { get; set; }
        public string SmtpPassword { get; set; }
        public string ConfigSet { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public bool EnableSsl { get; set; }
    }
}

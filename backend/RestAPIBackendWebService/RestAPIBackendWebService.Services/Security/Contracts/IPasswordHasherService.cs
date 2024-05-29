namespace RestAPIBackendWebService.Services.Security.Contracts
{
    public interface IPasswordHasherService
    {
        public string HashPassword(string password, out byte[] salt);
        public bool VerifyPassword(string password, string hash, byte[] salt);
        public string HashMessageCode(string code, byte[] salt);
    }
}

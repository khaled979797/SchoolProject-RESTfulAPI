namespace SchoolProject.Service.Abstracts
{
    public interface IEmailService
    {
        public Task<string> SendEmail(string email, string message, string? reason);
    }
}

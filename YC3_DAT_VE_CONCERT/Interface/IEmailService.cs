namespace YC3_DAT_VE_CONCERT.Interface
{
    public interface IEmailService
    {
        Task SendEmail(string name, string email, string title, string text);
    }
}

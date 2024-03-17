namespace G21097711
{
    public interface IEmailService
    {
        public void SendMail(string emailAddress, string subject, string message);
    }
}
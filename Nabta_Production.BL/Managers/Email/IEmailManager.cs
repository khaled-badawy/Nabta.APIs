namespace Nabta_Production.BL
{
    public interface IEmailManager
    {
        public Task<bool> SendEmail(string recieverEmail , string subject , string message);
    }
}

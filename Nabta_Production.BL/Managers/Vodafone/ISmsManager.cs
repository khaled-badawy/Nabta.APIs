namespace Nabta_Production.BL
{
    public interface ISmsManager
    {
        public string SendSMS(string phoneNumber, string textMsg);

    }
}
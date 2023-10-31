using Microsoft.Extensions.Configuration;
using Nabta_Production.DAL;
using Vodafone;

namespace Nabta_Production.BL;

public class SmsManager : ISmsManager
{
    private readonly VodafoneConfiguration _vodaConfig;
    private readonly IConfiguration _config;

    private string urlUpload { get; }

    public SmsManager(VodafoneConfiguration vodaConfig , IConfiguration config)
    {
        _vodaConfig = vodaConfig;
        _config = config;
        urlUpload = $"{_config.GetSection("ServerUploadPath").Value!}";
    }

    public string SendSMS(string phoneNumber , string textMsg)
    {
        Web2SMS.Configure(_vodaConfig.SmsUrl, _vodaConfig.SmsAccountId, _vodaConfig.SmsPassword, _vodaConfig.SmsClientSecret, _vodaConfig.SmsDefaultSenderName);
        try
        {
            //using (StreamWriter writer = System.IO.File.AppendText($"{urlUpload}\\errors.txt"))
            //{
            //    writer.WriteLine($"{_vodaConfig.SmsUrl} \n {_vodaConfig.SmsAccountId} \n {_vodaConfig.SmsPassword} \n {_vodaConfig.SmsClientSecret}");
            //}

            Recepient recepient = new()
            {
                ReceiverMSISDN = phoneNumber,
                SMSText = textMsg,
                SenderName = "IDSC",
            };

            //using (StreamWriter writer = System.IO.File.AppendText($"{urlUpload}\\errors.txt"))
            //{
            //    writer.WriteLine($"{phoneNumber}");
            //}
            var response = Web2SMS.Send(new List<Recepient>() { recepient });
            return response.ResponseText;
        }
        catch (Exception ex)
        {
            using (StreamWriter writer = System.IO.File.AppendText($"{urlUpload}\\errors.txt"))
            {
                writer.WriteLine($"{ex.Message}");
            }
            return ex.Message;
        }
    }


}

using SnacksCalculationAPI.Models;

namespace SnacksCalculationAPI.Services.Mail.Interface
{
    public interface IMailService
    {
        bool SendMail(MailData mailData);
    }

}

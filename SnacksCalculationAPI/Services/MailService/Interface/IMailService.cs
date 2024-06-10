using SnacksCalculationAPI.Models;

namespace SnacksCalculationAPI.Services.MailService.Interface
{
    public interface IMailService
    {
        bool SendMail(MailData mailData);
    }
}

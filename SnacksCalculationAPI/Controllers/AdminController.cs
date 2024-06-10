using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SnacksCalculationAPI.Models;
using SnacksCalculationAPI.Services.MailService.Interface;

namespace SnacksCalculationAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IMailService _mailService;
        //injecting the IMailService into the constructor
        public AdminController(IMailService _MailService)
        {
            _mailService = _MailService;
        }

        [HttpPost]
        [Route("SendMail")]
        public bool SendMail(MailData mailData)
        {
            return _mailService.SendMail(mailData);
        }
    }
}

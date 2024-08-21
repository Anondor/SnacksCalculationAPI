using AutoWrapper.Wrappers;
using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SnacksCalculationAPI.Models;
using Microsoft.EntityFrameworkCore;
using SnacksCalculationAPI.Filters;
using SnacksCalculationAPI.Services.AuthService;

namespace SnacksCalculationAPI.Controllers
{
    [Route("api/[controller]")]
   
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly APIDbContext _context;
        private readonly IAuthService _authService;

        public LoginController  (APIDbContext context,IAuthService authService1)
        {
            _context = context;
            _authService = authService1;
        }

        [HttpPost("LoginUser")]
        public async Task<IActionResult> LoginUser([FromBody] LoginModel model)
        {
            var apiResult = new ApiResponse<IEnumerable<LoginModel>>
            {
                Data = new List<LoginModel>()
            };
            if (ModelState.IsValid)
            {
                try
                {
                    var user = await _context.UserModels.FirstOrDefaultAsync(x => x.Phone == model.Phone && x.Password == model.Password);
                    if (user != null)
                    {
                        var result = await _authService.GetJWTToken(model, user.UserType);
                        return OkResult(result);
                    }
                    else
                    {
                        throw new Exception("Invalid username or password.");
                    }


                }
                catch (Exception ex)
                {
                    
                    apiResult.StatusCode = 500;
                    apiResult.Status = "Fail";
                    apiResult.Msg = ex.Message;
                    return BadRequest(apiResult);
                }

            }
            return BadRequest();


        }
        private IActionResult OkResult(object data)
        {
            var apiResult = new ApiResponse
            {
                StatusCode = 200,
                Message = "Successful",
                Result = data 
            
        };
            return ObjectResult(apiResult);
        }
        protected IActionResult ObjectResult(ApiResponse model)
        {
            var result = new ObjectResult(model)
            {
                StatusCode = model.StatusCode
            };
            return result;
        }

      
    }
}

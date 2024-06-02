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
        [HttpPost("LoginAdmin")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            /*  var response = new ApiResponse();
              try
              {
                  var user =await _context.UserModels.FirstOrDefaultAsync(x=>x.Phone==model.Phone && x.Password==model.Password && x.UserType==0);
                  //var user = await _context.UserModels.FirstOrDefault(x => x.Phone == model.Phone && x.Password == model.Password);
                  if (user != null)
                  {
                      response.Result = user;
                      response.Message = "Admin Login  Successfully";
                      response.StatusCode = (int)HttpStatusCode.OK;
                      return response;
                  }
                  else
                  {
                      response.Message = "Phone number or password is wrong";
                      response.StatusCode = (int)HttpStatusCode.BadRequest;
                      return response;

                  }

              }
              catch (Exception ex)
              {
                  response.Result = null;
                  response.StatusCode = (int)HttpStatusCode.BadRequest;
                  response.ResponseException = ex.Message;
                  response.IsError = true;
                  return response;
              }
            */
            var apiResult = new ApiResponse<IEnumerable<LoginModel>>
            {
                Data = new List<LoginModel>()
            };

            if (ModelState.IsValid)
            {
                try
                {
                    var user = await _context.UserModels.FirstOrDefaultAsync(x => x.Phone == model.Phone && x.Password == model.Password && x.UserType == 0);
                    if (user != null)
                    {
                        var result = await _authService.GetJWTToken(model,0);
                        return OkResult(result);
                    }
                    else
                    {
                        throw new Exception("Invalid username or password.");
                    }


                }
                catch (Exception ex)
                {
                    // ex.ToWriteLog();

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
                    var user = await _context.UserModels.FirstOrDefaultAsync(x => x.Phone == model.Phone && x.Password == model.Password && x.UserType == 1);
                    if (user != null)
                    {
                        var result = await _authService.GetJWTToken(model, 1);
                        return OkResult(result);
                    }
                    else
                    {
                        throw new Exception("Invalid username or password.");
                    }


                }
                catch (Exception ex)
                {
                    // ex.ToWriteLog();

                    apiResult.StatusCode = 500;
                    apiResult.Status = "Fail";
                    apiResult.Msg = ex.Message;
                    return BadRequest(apiResult);
                }

            }
            return BadRequest();


        }
    }
}

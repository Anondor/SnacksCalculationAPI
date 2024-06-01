using AutoWrapper.Wrappers;
using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SnacksCalculationAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace SnacksCalculationAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly APIDbContext _context;
        public LoginController  (APIDbContext context)
        {
            _context = context;
        }
        [HttpPost("LoginAdmin")]
        public async Task<ActionResult<ApiResponse>> Login(LoginModel model)
        {
            var response = new ApiResponse();
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

        }

        [HttpPost("LoginUser")]
        public async Task<ActionResult<ApiResponse>> LoginUser(LoginModel model)
        {
            var response = new ApiResponse();
            try
            {
                var user = await _context.UserModels.FirstOrDefaultAsync(x => x.Phone == model.Phone && x.Password == model.Password && x.UserType == 1);
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

        }
    }
}

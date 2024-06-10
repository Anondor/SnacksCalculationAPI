using AutoWrapper.Wrappers;
using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SnacksCalculationAPI.Models;
using SnacksCalculationAPI.Filters;
using Microsoft.EntityFrameworkCore;

namespace SnacksCalculationAPI.Controllers
{
    [Route("api/[controller]")]
   
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly APIDbContext _context;
        public UserController(APIDbContext context)
        {
            _context = context;
        }
        [HttpPost]
        public async Task<ActionResult<ApiResponse>>Save(UserModel model)
        {
            var response = new ApiResponse();

            try
            {
                var userQuery = _context.UserModels.FirstOrDefault(x=>(x.Phone==model.Phone||x.Email==model.Email));
                if (userQuery != null)
                {
                    response.Message = "Your phone number or email is already exists";
                    return response;
                }

                //  var userList = await userQuery.FirstAsync().where;
                else
                {    await _context.UserModels.AddAsync(model);
                await _context.SaveChangesAsync();

                response.StatusCode = (int)HttpStatusCode.OK;
                response.Message = "User data  save Successfully";
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
        [HttpPost("addCost")]
        public async Task<ActionResult<ApiResponse>> SaveCostInfo(CostModel[] model)
        {
            var response = new ApiResponse();

            try
            {
                for(int i = 0;i< model.Length;i++)
                {
                    var userQuery = _context.CostModels.FirstOrDefault(x => (x.Date == model[i].Date && x.UserId == model[i].UserId));
                    if (userQuery != null)
                    {
                        userQuery.Amount = model[0].Amount;
                        userQuery.Item = model[0].Item;
                        _context.CostModels.Update(userQuery);
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        await _context.CostModels.AddAsync(model[i]);
                        await _context.SaveChangesAsync();

                    }
                }
                response.StatusCode = (int)HttpStatusCode.OK;
                response.Message = "User data  save Successfully";
                return response;



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

        [HttpGet("UserList")]
        public async Task<ActionResult<ApiResponse>> GetAllUser()
        {
            var response = new ApiResponse();

            try
            {
                var userQuery=  _context.UserModels.AsQueryable();

                var userList = await userQuery.ToListAsync();
                response.Result = userList;
                response.StatusCode = (int)HttpStatusCode.OK;
                return response;
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

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
    [JwtAuthorize]
    [ApiController]
    public class UserCostController : ControllerBase
    {
        private readonly APIDbContext _context;
        public UserCostController(APIDbContext context)
        {
            _context = context;
        }
        [HttpPost]

        public async Task<ActionResult<ApiResponse>> SaveAmount(UserInformationModel model)
        {
            var response = new ApiResponse();

            try
            {
                 await _context.UserInformationModels.AddAsync(model);
               await _context.SaveChangesAsync();

                response.StatusCode = (int)HttpStatusCode.OK;
                response.Message = "User Cost Information  save Successfully";
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
        [HttpGet("userAmount")]
        public async Task<ActionResult<ApiResponse>> getAllUserMoney()
        {
            var response = new ApiResponse();   
            try
            {
                var listQuery = _context.UserInformationModels.GroupBy(x => x.UserId).Select(g => new
                {
                    UserId = g.Key,
                    TotalAmount = g.Sum(t => t.Amount)
                });
              
                var list = await listQuery.ToListAsync();

        

                response.Result = list;

            }
            catch (Exception ex)
            {
                response.Result = null;
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                response.ResponseException = ex.Message;
                response.IsError = true;
            }

            return response;
        }
        [HttpGet("userCostAmount")]
        public async Task<ActionResult<ApiResponse>> getAllUserCostMoney()
        {
            var response = new ApiResponse();
            try
            {
                var listQuery = _context.CostModels.GroupBy(x => x.UserId).Select(g => new
                {
                    UserId = g.Key,
                    TotalAmount = g.Sum(t => t.Amount)
                });

                var list = await listQuery.ToListAsync();


                response.Result = list;

            }
            catch (Exception ex)
            {
                response.Result = null;
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                response.ResponseException = ex.Message;
                response.IsError = true;
            }

            return response;
        }

        [HttpPut("update")]
        public async Task<ActionResult<ApiResponse>>Update(CostModel model)
        {
            var response = new ApiResponse();
            try
            {
                var dbModel = await _context.CostModels.FirstOrDefaultAsync(x => x.UserId == model.UserId && x.Date==model.Date);

                if (dbModel == null)
                {
                    response.Message = "Data not found";
                    response.IsError = true;
                    return response;
                }
                dbModel.Amount = model.Amount;
  
                dbModel.Item = model.Item;
                _context.CostModels.Update(dbModel);
                await _context.SaveChangesAsync();
                response.StatusCode = (int)HttpStatusCode.OK;
                response.Message = "Data Updated";
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

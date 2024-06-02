using AutoWrapper.Wrappers;
using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SnacksCalculationAPI.Models;
using SnacksCalculationAPI.Filters;

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

        public async Task<ActionResult<ApiResponse>> Save(UserInformationModel model)
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


    }
}

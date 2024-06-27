using AutoWrapper.Wrappers;
using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SnacksCalculationAPI.Models;
using SnacksCalculationAPI.Filters;
using Microsoft.EntityFrameworkCore;
using SnacksCalculationAPI.Services.Common.Interfaces;
using SnacksCalculationAPI.Services.FileUtility.Implementation;

namespace SnacksCalculationAPI.Controllers
{
    [Route("api/[controller]")]
    [JwtAuthorize]
   
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly APIDbContext _context;
        private readonly ICommonService _commonService;


        public UserController(APIDbContext context, ICommonService commonService)
        {
            _context = context;
            _commonService = commonService;
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
        public async Task<ActionResult<ApiResponse>> SaveCostInfo(UserCostModel[] model)
        {
            var response = new ApiResponse();
            var hasPermission = await _commonService.HasPermission();
            if (!hasPermission)
            {
                throw new Exception("Permission denied.");
            }

            try
            {
                 for(int i=0;i<model.Length;i++)
                {
                    var userQuery = _context.CostModels.FirstOrDefault(x => (x.Date == model[i].Date && x.UserId == model[i].UserId));
                    if (userQuery != null)
                    {
                        userQuery.Amount = model[i].Amount;
                        userQuery.Item = model[i].Item;
                        _context.CostModels.Update(userQuery);
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        CostModel data = new CostModel();
                        data.Item = model[i].Item;
                        data.Amount = model[i].Amount;
                        data.Date = model[i].Date;
                        data.UserId = model[i].UserId;
                        await _context.CostModels.AddAsync(data);
                        await _context.SaveChangesAsync();

                    }

                }
                
                
                response.StatusCode = (int)HttpStatusCode.OK;
                response.Message = "Save Successfully";
              
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
        [HttpGet("exportReport")]

        public async Task<FileContentResult> exportReport(string fromDate, string toDate)
        {
            FileData fileData = await _commonService.GetMonthlyDetailsExcel(fromDate, toDate);

            var cd = new System.Net.Mime.ContentDisposition
            {
                FileName = fileData.Name,
                Inline = true,
            };
            HttpContext.Response.Headers.Add("Content-Disposition", cd.ToString());
            return File(fileData.Data, "application/octet-stream");
        }
        
        [HttpGet("exportGeneratedReport")]

        public async Task<FileContentResult> exportGeneratedReport(string fromDate, string toDate, int userId)
        {
            FileData fileData = await _commonService.GetexportGeneratedReportExcel(fromDate, toDate, userId);

            var cd = new System.Net.Mime.ContentDisposition
            {
                FileName = fileData.Name,
                Inline = true,
            };
            HttpContext.Response.Headers.Add("Content-Disposition", cd.ToString());
            return File(fileData.Data, "application/octet-stream");
        }

        [HttpGet("getMonthlyCost")]
        public async Task<ActionResult<ApiResponse>> getMonthlyCostInfo(string fromDate, string toDate)
        {
            var response = new ApiResponse();

            try
            {
                var listQuery = from cm in _context.CostModels
                                join um in _context.UserModels on cm.UserId equals um.Id
                                where string.Compare(cm.Date, fromDate) >= 0 && string.Compare(cm.Date, toDate) <= 0
                                orderby cm.UserId ascending

                                select new
                                {
                                    Id = cm.Id,
                                    UserId=cm.UserId,
                                    Date=cm.Date,
                                    Amount=cm.Amount,
                                    item=cm.Item,
                                    Name = um.Name,
                                } ;
                var list = await listQuery.ToListAsync();
                response.Result = list;
                response.Message = "Success";
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


        [HttpGet("UserList")]
        public async Task<ActionResult<ApiResponse>> GetAllUser()
        {
            var response = new ApiResponse();

            try
            {
                var userQuery = _context.UserModels.AsQueryable();

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

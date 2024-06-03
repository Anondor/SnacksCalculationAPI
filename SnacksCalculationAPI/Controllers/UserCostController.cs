﻿using AutoWrapper.Wrappers;
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

        [HttpPut("update")]
        public async Task<ActionResult<ApiResponse>>Update(UserInformationModel model)
        {
            var response = new ApiResponse();
            try
            {
                var dbModel = await _context.UserInformationModels.FirstOrDefaultAsync(x => x.UserId == model.UserId && x.Date==model.Date);

                if (dbModel == null)
                {
                    response.Message = "Data not found";
                    response.IsError = true;
                    return response;
                }
                dbModel.Amount = model.Amount;
  
                dbModel.ItemName = model.ItemName;
                _context.UserInformationModels.Update(dbModel);
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
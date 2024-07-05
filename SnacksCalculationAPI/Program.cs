using System.Security.Principal;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OfficeOpenXml;
using SnacksCalculationAPI;
using SnacksCalculationAPI.Helpers;
using SnacksCalculationAPI.Services.AuthService;
using SnacksCalculationAPI.Services.Common.Implementation;
using SnacksCalculationAPI.Services.Common.Interfaces;
using SnacksCalculationAPI.Services.FileUtility.Implementation;
using SnacksCalculationAPI.Services.FileUtility.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder => { builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader(); });
});
builder.Services.AddDbContext<APIDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ICommonService, CommonService>();
builder.Services.AddScoped<IFileService, FileService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<HttpHelper>();



builder.Services.AddSwaggerGen(setup =>
{
    // Include 'SecurityScheme' to use JWT Authentication
    var jwtSecurityScheme = new OpenApiSecurityScheme
    {
        BearerFormat = "JWT",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = JwtBearerDefaults.AuthenticationScheme,
        Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\"",


        Reference = new OpenApiReference
        {
            Id = JwtBearerDefaults.AuthenticationScheme,
            Type = ReferenceType.SecurityScheme
        }
    };

    setup.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);

    setup.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { jwtSecurityScheme, Array.Empty<string>() }
    });

});
builder.Services.AddAuthentication(opt =>
{
    opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}) .AddJwtBearer(cfg =>
                {
                    //cfg.Authority = "https://login.microsoftonline.com/0a495c7a-8a56-41c6-bf09-f14ccaf97dfe";
                    //cfg.Audience = "api://f0f7eef6-41ab-4c71-8b55-79cc2f1871fc"; // Set this to the App ID URL for the web API, which you created when you registered the web API with Azure AD.

                    cfg.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidIssuer = builder.Configuration["Tokens:Issuer"],
                        ValidAudience = builder.Configuration["Tokens:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Tokens:key"])),
                        ValidateIssuerSigningKey = false,
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = false,
                    };


                });
ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseAuthentication();
app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();

app.MapControllers();

app.Run();

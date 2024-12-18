using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using RepairShop.BLL;
using RepairShop.BLL.interfaces;
using RepairShop.DAL;
using RepairShop.DAL.Repositories;
using RepairShop.DAL.Repositories.Interfaces;
using RepairShop.Helpers;
using RepairShop.Helpers.interfaces;

namespace RepairShop;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddDbContext<RepairShopContext>(options =>
        {
            options.UseSqlServer(builder.Configuration.GetConnectionString("DBConnection"));
        });

        builder.Services.AddScoped<IChatLogic, ChatLogic>();
        builder.Services.AddScoped<IUserLogic, UserLogic>();
        builder.Services.AddScoped<ITechnicianLogic, TechnicianLogic>();
        builder.Services.AddScoped<IOrderLogic, OrderLogic>();
        builder.Services.AddScoped<IUserDataLogic, UserDataLogic>();
        
        builder.Services.AddScoped<IChatRepository, ChatRepository>();
        builder.Services.AddScoped<IOrderRepository, OrderRepository>();
        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddScoped<ITechnicianRepository, TechnicianRepository>();
        builder.Services.AddScoped<IUserDataRepository, UserDataRepository>();
        
        builder.Services.AddScoped<IPasswordEncrypter, PasswordEncrypter>();
        builder.Services.AddScoped<IJwtToken, JwtToken>();
        
        builder.Services.AddSingleton<EmailHelper>(sp =>
        {
            var configuration = sp.GetRequiredService<IConfiguration>();
            var smtpServer = configuration["Email:SmtpServer"];
            var smtpPort = int.Parse(configuration["Email:SmtpPort"]);
            var smtpUser = configuration["Email:SmtpUser"];
            var smtpPass = configuration["Email:SmtpPass"];
            return new EmailHelper(smtpServer, smtpPort, smtpUser, smtpPass);
        });
        
        var key = Encoding.ASCII.GetBytes("your_new_32_byte_or_longer_key_here_12345");
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = false;
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true
            };
            
        });
        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy("UserOnly", policy =>
                policy.RequireClaim("EntityType", "User"));
            options.AddPolicy("TechnicianOnly", policy =>
                policy.RequireClaim("EntityType", "Technician"));
        });

        
        //så man kan bruge authorization i swagger
        builder.Services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Insert JWT token only (Swagger will add 'Bearer ' prefix automatically).",
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                BearerFormat = "JWT",
                Scheme = "bearer"
            });
            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });

        
        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        app.UseCors(builder => builder.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseCors();
        app.MapControllers();
        app.Run();
    }
}
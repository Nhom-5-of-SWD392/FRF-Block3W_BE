using Data.EFCore;
using Data.Entities;
using Data.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Service.Core;
using Service.Utilities;
using System.Text;

namespace FRF_Project_Block3W.Extensions;

public static class ServiceExtension
{
    public static void ConfigurePostgreSqlServer(this IServiceCollection services, DbSetupModel model)
    {
        services.AddDbContext<DataContext>(o =>
        {
            o.UseNpgsql(model?.ConnectionStrings);
        });
    }

    public static void ConfigCors(this IServiceCollection services)
    {
        services.AddCors(options => options.AddPolicy("AllowAllOrigins", builder =>
                builder.AllowAnyHeader()
                       .AllowAnyMethod()
                       .AllowAnyOrigin())
        );
    }

    public static void AddBusinessServices(this IServiceCollection services)
    {
        services.AddSingleton<ICloudinaryService, CloudinaryService>();

        services.AddScoped<IJwtUtils, JwtUtils>();

        services.AddScoped<IGoogleAuthService, GoogleAuthService>();

        services.AddScoped<IEmailService, EmailService>();

        //User
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ISortHelpers<User>, SortHelper<User>>();
        services.AddScoped<IFilterHelper<User>, FilterHelper<User>>();

        //ModeratorApplication
        services.AddScoped<ISortHelpers<ModeratorApplication>, SortHelper<ModeratorApplication>>();
        services.AddScoped<IFilterHelper<ModeratorApplication>, FilterHelper<ModeratorApplication>>();

        //Quiz
        services.AddScoped<IQuizService, QuizService>();
        services.AddScoped<ISortHelpers<Quiz>, SortHelper<Quiz>>();

        //Topic
        services.AddScoped<ITopicService, TopicService>();
		services.AddScoped<ISortHelpers<Topic>, SortHelper<Topic>>();

        services.AddScoped<ISortHelpers<QuizResult>, SortHelper<QuizResult>>();

		//Post
		services.AddScoped<IPostService, PostService>();

        //Media
        services.AddScoped<IMediaService, MediaService>();

        //Instruction
        services.AddScoped<IInstructionService, InstructionService>();

    }

    public static void ConfigureJWTToken(this IServiceCollection services, JwtModel? jwtModel, GoogleModel? googleModel)
    {
        services
            .AddAuthentication(op =>
            {
                op.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                op.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                op.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidAudience = jwtModel?.ValidAudience,
                    ValidIssuer = jwtModel?.ValidIssuer,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtModel?.Secret ?? ""))
                };
            })
            .AddCookie()
            .AddGoogle(googleOptions =>
            {
                googleOptions.ClientId = googleModel?.ClientId;
                googleOptions.ClientSecret = googleModel?.ClientSecret;
            });
    }
}

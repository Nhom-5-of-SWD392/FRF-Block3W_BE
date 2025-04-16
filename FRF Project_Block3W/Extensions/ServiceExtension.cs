using Data.EFCore;
using Data.Entities;
using Data.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Service.Core;
using Service.Utilities;
using System.Text;

namespace FRF_Project_Block3W.Extensions
{
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
            services.AddScoped<IJwtUtils, JwtUtils>();

            services.AddScoped<IGoogleAuthService, GoogleAuthService>();

            //User
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ISortHelpers<User>, SortHelper<User>>();
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

        //public static void ConfigureFirebaseCloudServices(this IServiceCollection services, FirebaseStorageModel model)
        //{
        //    var credential = GoogleCredential.FromFile(Environment.CurrentDirectory! + "\\" + model.FirebaseSDKFile);

        //    if (FirebaseApp.DefaultInstance == null)
        //    {
        //        FirebaseApp.Create(new AppOptions
        //        {
        //            Credential = credential,
        //            ProjectId = model.ProjectId
        //        });
        //    }
        //    StorageClient _storageClient = StorageClient.Create(credential);
        //    services.AddSingleton<IFirebaseStorageService>(new FirebaseStorageService(model.Bucket, _storageClient));
        //}
    }
}

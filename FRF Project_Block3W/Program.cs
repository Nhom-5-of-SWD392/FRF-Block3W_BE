using Data.Models;
using FRF_Project_Block3W.Extensions;
using FRF_Project_Block3W.Helpers;
using Microsoft.OpenApi.Models;
using Service.Mapper;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.ConfigurePostgreSqlServer(builder.Configuration.GetSection("DbSetup").Get<DbSetupModel>()!);
builder.Services.AddAutoMapper(typeof(MapperProfiles));
builder.Services.ConfigCors();
builder.Services.ConfigureJWTToken(
    builder.Configuration.GetSection("JWT").Get<JwtModel>()
);
builder.Services.AddBusinessServices();


builder.Services.AddControllers(op =>
{
    op.Filters.Add(new ResultManipulator());
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "Food Related Forum API", Version = "v1" });
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            new string[]{}
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.OAuthClientId("swagger");
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "FRF API v1");
});

app.UseCors("AllowAllOrigins");

app.UseHttpsRedirection();

app.UseRouting();

app.UseMiddleware<ErrorHandlerMiddleware>();

app.UseAuthorization();

app.MapControllers();

app.Run();

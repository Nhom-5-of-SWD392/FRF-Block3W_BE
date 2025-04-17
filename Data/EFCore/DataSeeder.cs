using Data.Entities;
using Data.Enum;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Data.EFCore
{
    public static class DataSeeder
    {
        public static WebApplication SeedData(this WebApplication app)
        {
            using (var scope = app.Services.CreateScope())
            {
                using var context = scope.ServiceProvider.GetRequiredService<DataContext>();
                try
                {
                    context.Database.EnsureCreated();

                    var user = context.User.FirstOrDefault();

                    if (user == null) 
                    {
                        context.User.AddRange(
                            new User { Id = Guid.NewGuid(), FirstName = "Administrator", UserName = "admin", Email = "admin@gmail.com", Password = "$2a$12$TVWmAWWGUbMx0Yi4cl61ZuZffJGWefD.bhWk9sHQjdVmn/m1KDa7u", Role = UserRole.Administrator, Gender = Gender.Male, IsModerator = true }
                            );
                        context.SaveChanges();
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                return app;
            }
        }
    }
}

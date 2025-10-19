using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Touresta.API.Data;
using Touresta.API.Seeders;

namespace Touresta.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Configure Database Context
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
                    new MySqlServerVersion(new Version(8, 0, 36))));

            // Services 
            builder.Services.AddScoped<AuthService>();
            builder.Services.AddScoped<EmailService>();

            //Swagger Configuration
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Touresta API",
                    Version = "v1",
                    Description = "APIs for Touresta Mobile App and Web Dashboard"
                });

                c.TagActionsBy(api =>
                {
                    if (api.RelativePath.Contains("AdminAuth"))
                        return new[] { "Web Dashboard" };
                    else
                        return new[] { "Mobile App" };
                });
            });

            // Add services to the container.
            builder.Services.AddControllers();

            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(); 


            var app = builder.Build();


            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger(); 
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Touresta API v1");
                    options.RoutePrefix = "swagger"; 
                });

            }
            var scope = app.Services.CreateScope();
            var services = scope.ServiceProvider;
            var context = services.GetRequiredService<AppDbContext>();

            AdminSeeder.Seed(context, services);

            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}
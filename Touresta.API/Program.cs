using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Touresta.API.Data;
using Touresta.API.Seeders;
using Touresta.API.Services;

namespace Touresta.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Database
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
                    new MySqlServerVersion(new Version(8, 0, 36))));

            // Controllers & API Explorer
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();

            // Application Services
            builder.Services.AddScoped<AuthService>();
            builder.Services.AddScoped<EmailService>();
            builder.Services.AddHostedService<AutoCleanupService>();

            // Swagger Configuration
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Touresta API",
                    Version = "v1",
                    Description = "Touresta Mobile App & Admin Dashboard APIs",
                    Contact = new OpenApiContact
                    {
                        Name = "Touresta Team"
                    }
                });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (File.Exists(xmlPath))
                {
                    c.IncludeXmlComments(xmlPath);
                }
            });

            var app = builder.Build();

            // CORS
            app.UseCors(policy => policy
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());

            // Swagger UI
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "Touresta API v1");
                options.RoutePrefix = "swagger";
            });

            // Seed admin data
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var context = services.GetRequiredService<AppDbContext>();
                AdminSeeder.Seed(context, services);
            }

            // Middleware pipeline
            app.UseRouting();
            app.UseStaticFiles();
            app.UseAuthorization();
            app.MapControllers();

            // Redirect root to Swagger
            app.MapGet("/", () => Results.Redirect("/swagger"));

            app.Run();
        }
    }
}

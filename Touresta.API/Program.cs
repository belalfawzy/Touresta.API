using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Reflection;
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

            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();

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
                    Description = "Touresta Mobile App & Admin Dashboard APIs"
                });

                try
                {
                    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                    if (File.Exists(xmlPath))
                    {
                        c.IncludeXmlComments(xmlPath);
                    }
                }
                catch
                {
                }
            });


            var app = builder.Build();

            app.UseCors(policy => policy
              .AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader());

            // if (app.Environment.IsDevelopment())
          //    {
            app.UseSwagger(); 
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Touresta API v1");
                    options.RoutePrefix = "swagger"; 
                });

       //   }
            var scope = app.Services.CreateScope();
            var services = scope.ServiceProvider;
            var context = services.GetRequiredService<AppDbContext>();

            AdminSeeder.Seed(context, services);

            app.UseRouting();

            app.UseStaticFiles();


            app.UseAuthorization();


            app.MapControllers();

         

            app.MapGet("/", () => Results.Redirect("/swagger"));

            app.Run();
        }
    }
}
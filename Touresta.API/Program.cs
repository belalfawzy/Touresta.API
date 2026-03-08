using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Mvc;
using Touresta.API.Data;
using Touresta.API.Filters;
using Touresta.API.Middleware;
using Touresta.API.Repositories.Implementations;
using Touresta.API.Repositories.Interfaces;
using Touresta.API.Seeders;
using Touresta.API.Services.Implementations;
using Touresta.API.Services.Interfaces;

namespace Touresta.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseMySql(
                    builder.Configuration.GetConnectionString("DefaultConnection"),
                    new MySqlServerVersion(new Version(8, 0, 36))));

            // Controllers & API Explorer
            builder.Services.AddControllers(options =>
            {
                options.Filters.Add<ModelValidationFilter>();
            });
            builder.Services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });
            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)),
                    RoleClaimType = "role"
                };
            });

            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminOnly", policy =>
                    policy.RequireClaim("type", "admin"));

                options.AddPolicy("SuperAdminOnly", policy =>
                    policy.RequireClaim("type", "admin")
                          .RequireClaim("role", "SuperAdmin"));

                options.AddPolicy("AdminManagement", policy =>
                    policy.RequireClaim("type", "admin")
                          .RequireClaim("role", "SuperAdmin"));
            });
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IAdminRepository, AdminRepository>();
            builder.Services.AddScoped<IHelperRepository, HelperRepository>();
            builder.Services.AddScoped<IHelperLanguageRepository, HelperLanguageRepository>();
            builder.Services.AddScoped<ICertificateRepository, CertificateRepository>();
            builder.Services.AddScoped<ICarRepository, CarRepository>();
            builder.Services.AddScoped<IDrugTestRepository, DrugTestRepository>();
            builder.Services.AddScoped<ILanguageTestRepository, LanguageTestRepository>();
            builder.Services.AddScoped<IAdminAuditLogRepository, AdminAuditLogRepository>();
            builder.Services.AddScoped<IHelperReportRepository, HelperReportRepository>();
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IEmailService, EmailService>();
            builder.Services.AddScoped<ICloudinaryService, CloudinaryService>();
            builder.Services.AddScoped<IHelperService, HelperService>();
            builder.Services.AddScoped<ILanguageEvaluationService, StubLanguageEvaluationService>();
            builder.Services.AddScoped<IAdminNoteRepository, AdminNoteRepository>();
            builder.Services.AddTransient<HelperEligibilityFilter>();
            builder.Services.AddHostedService<AutoCleanupService>();

            builder.Services.AddSwaggerGen(c =>
            {
                // Multiple Swagger documents — one per client group
                c.SwaggerDoc("admin", new OpenApiInfo
                {
                    Title = "Touresta Admin Dashboard API",
                    Version = "v1",
                    Description = "APIs consumed by the Admin Dashboard (authentication, helper management, reports, audit, notes).",
                    Contact = new OpenApiContact { Name = "Touresta Team" }
                });

                c.SwaggerDoc("user", new OpenApiInfo
                {
                    Title = "Touresta User App API",
                    Version = "v1",
                    Description = "APIs consumed by the mobile app for regular users (registration, login, profile management).",
                    Contact = new OpenApiContact { Name = "Touresta Team" }
                });

                c.SwaggerDoc("helper", new OpenApiInfo
                {
                    Title = "Touresta Helper App API",
                    Version = "v1",
                    Description = "APIs consumed by the helper mobile app (onboarding, documents, languages, car, eligibility).",
                    Contact = new OpenApiContact { Name = "Touresta Team" }
                });

                c.SwaggerDoc("system", new OpenApiInfo
                {
                    Title = "Touresta System API",
                    Version = "v1",
                    Description = "Infrastructure and health check endpoints.",
                    Contact = new OpenApiContact { Name = "Touresta Team" }
                });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token.",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
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

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (File.Exists(xmlPath))
                {
                    c.IncludeXmlComments(xmlPath);
                }
            });

            var app = builder.Build();

            app.UseCors(policy => policy
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());

            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/admin/swagger.json", "Admin Dashboard API");
                options.SwaggerEndpoint("/swagger/user/swagger.json", "User App API");
                options.SwaggerEndpoint("/swagger/helper/swagger.json", "Helper App API");
                options.SwaggerEndpoint("/swagger/system/swagger.json", "System API");
                options.RoutePrefix = "swagger";
            });

            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var context = services.GetRequiredService<AppDbContext>();
                AdminSeeder.Seed(context, services);
            }

            app.UseRouting();
            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();
            app.MapGet("/", () => Results.Redirect("/swagger"))
                .ExcludeFromDescription();

            app.Run();
        }
    }
}
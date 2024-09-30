using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SchoolProject.Core;
using SchoolProject.Core.Filters;
using SchoolProject.Core.Middleware;
using SchoolProject.Data.Entities.Identity;
using SchoolProject.Infrastructure;
using SchoolProject.Infrastructure.Data;
using SchoolProject.Infrastructure.Seeder;
using SchoolProject.Service;
using Serilog;
using System.Globalization;

namespace SchoolProject.Api
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("dbcontext"));
            });

            #region Dependency Injections
            builder.Services.AddInfrastructureDependencies().AddServiceDependencies()
                .AddCoreDependencies().AddServiceRegistration(builder.Configuration);
            #endregion

            #region Localization
            builder.Services.AddControllersWithViews();
            builder.Services.AddLocalization(opt =>
            {
                opt.ResourcesPath = "";
            });
            builder.Services.Configure<RequestLocalizationOptions>(opt =>
            {
                List<CultureInfo> supportedCultures = new List<CultureInfo>
                {
                    new CultureInfo("en-US"),
                    new CultureInfo("ar-EG")
                };

                opt.DefaultRequestCulture = new RequestCulture("ar-EG");
                opt.SupportedCultures = supportedCultures;
                opt.SupportedUICultures = supportedCultures;
            });
            #endregion

            #region AllowCors
            var Cors = "_cors";
            builder.Services.AddCors(options =>
            {
                options.AddPolicy(name: Cors,
                                  policy =>
                                  {
                                      policy.AllowAnyHeader();
                                      policy.AllowAnyMethod();
                                      policy.AllowAnyOrigin();
                                  });
            });
            #endregion

            builder.Services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            builder.Services.AddTransient<IUrlHelper>(x =>
            {
                var actionContext = x.GetRequiredService<IActionContextAccessor>().ActionContext;
                var factory = x.GetRequiredService<IUrlHelperFactory>();
                return factory.GetUrlHelper(actionContext);
            });

            builder.Services.AddTransient<AuthFilter>();

            #region Serilog
            Log.Logger = new LoggerConfiguration()
                          .ReadFrom.Configuration(builder.Configuration).CreateLogger();
            builder.Services.AddSerilog();
            #endregion

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();
                await RoleSeeder.SeedAsync(roleManager);
                await UserSeeder.SeedAsync(userManager);
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            #region Localization Middleware
            var options = app.Services.GetService<IOptions<RequestLocalizationOptions>>();
            app.UseRequestLocalization(options.Value);
            #endregion

            app.UseMiddleware<ErrorHandlerMiddleware>();

            app.UseHttpsRedirection();

            app.UseCors(Cors);

            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}

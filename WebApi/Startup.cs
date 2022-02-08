using BO;
using DAO;
using HLP;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SVC;
using WebApi.Middlewares;

namespace WebApi
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<DataContext>();

            services.AddCors();

            services.AddControllers().AddJsonOptions(opts => opts.JsonSerializerOptions.IgnoreNullValues = true);

            services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));

            services.AddScoped<IJwtUtils, JwtUtils>();
            services.AddScoped<IAuthenticateService, AuthenticateService>();
            services.AddScoped<IUserService, UserService>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, DataContext context)
        {
            CreateTestUser(context);

            app.UseRouting();

            app.UseCors(opts => opts
                .SetIsOriginAllowed(origin => true)
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials());

            app.UseMiddleware<ErrorHandlerMiddleware>();

            app.UseMiddleware<JwtMiddleware>();

            app.UseEndpoints(opt => opt.MapControllers());
        }

        private static void CreateTestUser(DataContext context)
        {
            UserEntity testUser = new()
            {
                Username = "nld-audomarog",
                Branch = "BBB",
                DisplayName = "Audomaro Gonzalez",
                Email = "audomaro.gonzalez@email.com"
            };

            context.Users.Add(testUser);
            context.SaveChanges();
        }
    }
}

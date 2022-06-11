using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using Vaan.CMS.API.Authorization;
using Vaan.CMS.API.Data;
using Vaan.CMS.API.Helpers;
using Vaan.CMS.API.IRepository;
using Vaan.CMS.API.Mapping;
using Vaan.CMS.API.Repository;

namespace Vaan.CMS.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
             .AddMicrosoftIdentityWebApi(options =>
             {
                 Configuration.Bind("AzureAdB2C", options);

                 options.TokenValidationParameters.NameClaimType = "name";
             },
            options => { Configuration.Bind("AzureAdB2C", options); });


            services.AddControllers();


            services.AddDbContext<CMSDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("CMSConnectionStrings")));

            services.AddScoped<IJwtUtils, JwtUtils>();
            services.AddTransient<IUserServices, UserServices>();
            services.AddAutoMapper(typeof(UserMapping));
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Vaan.CMS.API", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Vaan.CMS.API v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();
            //  app.UseCookiePolicy(); app.UseCors(builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
            app.UseAuthentication();

            app.UseAuthorization();

            // global error handler
            app.UseMiddleware<ErrorHandlerMiddleware>();

            // custom jwt auth middleware
            // app.UseMiddleware<JwtMiddleware>();


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}

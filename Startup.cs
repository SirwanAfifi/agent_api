using System.Linq;
using agent_api.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace agent_api
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

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "agent_api", Version = "v1" });
            });
            services.AddSingleton<InMemoryDatabase>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, InMemoryDatabase database,
        ILogger<Startup> logger)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "agent_api v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.Use(async (context, next) =>
            {
                var authorization = context.Request.Headers["Authorization"];
                var signinPath = context.Request.Path;


                if (signinPath.ToString() == "/auth/signin") await next();

                if (string.IsNullOrWhiteSpace(authorization))
                {
                    context.Response.StatusCode = 401;
                    return;
                }

                var userId = authorization.ToString().Split("**")[0];
                var foundUser = database.Users.FirstOrDefault(u => u.Id == userId);

                if (foundUser == null)
                {
                    context.Response.StatusCode = 401;
                    return;
                }

                var isTokenValid = authorization.ToString() == foundUser.Token;

                if (!isTokenValid)
                {
                    context.Response.StatusCode = 401;
                    return;
                }

                await next();
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}

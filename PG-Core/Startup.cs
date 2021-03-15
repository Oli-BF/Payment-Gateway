using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PG_DataAccess.Data;
using Prometheus;
using PG_Core.Services.Bank;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace PG_Core
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. This method is used to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            // Database Connection - Located in appsettings.json
            services.AddDbContext<PgDbContext>(options => options
                .UseSqlServer(Configuration.GetConnectionString("PgDbContext")));

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddSingleton<IMockBank, MockBank>();

            // Swagger - API Documentation
            services.AddSwaggerGen();

            // Okta - OAuth 2.0 - See Okta Class for more Details
            var okta = Configuration.GetSection("Okta").Get<Okta>();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(options =>
                    {
                        options.Authority = okta.Authority;
                        options.Audience = okta.Audience;
                    });
        }

        // This method gets called by the runtime. This method is used to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, PgDbContext db)
        {
            // Prometheus - This counts the requests for each endpoint and the method.
            var counter = Metrics.CreateCounter("pg_core_counter", "Counts requests to API endpoints", 
                new CounterConfiguration
                {
                    LabelNames = new[] { "method", "endpoint" }
                });

            app.Use((context, next) =>
            {
                counter.WithLabels(context.Request.Method, context.Request.Path).Inc();
                return next();
            });

            // For Prometheus / Grafana
            app.UseMetricServer();

            // Swagger
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Payment Gateway API V1");
                c.RoutePrefix = string.Empty;
            });

            // Dev Only!
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Testing Only! - Migrations in use for production
            //db.Database.EnsureCreated();
            //var context = serviceProvider.GetService<PgDbContext>();

            // Testing Only! - Moved to Program
            //db.Database.Migrate();

            app.UseHttpsRedirection();

            app.UseRouting();

            // For For Prometheus / Grafana 
            app.UseHttpMetrics();

            // Okta
            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
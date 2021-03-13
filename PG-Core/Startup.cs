
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PG_Core.Controllers;
using PG_DataAccess.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Prometheus;
using PG_Core.Services.Bank;

namespace PG_Core
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
            //services.AddSingleton<IMockBank, MockBank>();
            //services.AddSingleton<ICustomerRepository, CustomerRepository>();

            //services.AddScoped<PgDbContext, PaymentProcessController>();
            //services.AddScoped<IMockBank, MockBank>();


            services.AddControllers();

            services.AddDbContext<PgDbContext>(options => options
                .UseSqlServer(Configuration.GetConnectionString("PgDbContext")));

            //?
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddSingleton<IMockBank, MockBank>();

            services.AddSwaggerGen();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, PgDbContext db)//, IServiceProvider serviceProvider)
        {
            // Prometheus - This counts the requests for each endpoint and the method.
            var counter = Metrics.CreateCounter("PgPathCounter", "Counts requests to endpoints", 
                new CounterConfiguration
                {
                    LabelNames = new[] { "method", "endpoint" }
                });

            app.Use((context, next) =>
            {
                counter.WithLabels(context.Request.Method, context.Request.Path).Inc();
                return next();
            });

            app.UseMetricServer();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Payment Gateway API V1");
                    c.RoutePrefix = string.Empty;
                });
            }

            // Testing only - Use Migrations
            //db.Database.EnsureCreated();
            //var context = serviceProvider.GetService<PgDbContext>();
            
            // Migrations - Implemented in Program.cs?
            //db.Database.Migrate();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}

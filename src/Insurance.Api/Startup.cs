using Insurance.Api.Configs;
using Insurance.Api.Extensions;
using Insurance.Api.Helpers;
using Insurance.Api.Middlewares;
using Insurance.Business.Service.Services;
using Insurance.Business.Services;
using Insurance.BusinessLayer.Services.Contracts;
using Insurance.Data.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;

namespace Insurance.Api
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
            services.Configure<AppConfig>(Configuration.GetSection("Configurations"));

            services.AddHttpContextAccessor();
            services.AddSingleton<IBaseAddressProvider, BaseAddressProvider>();

            services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddControllers();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Insurance API",
                    Version = "v1",
                    Description = "An API for managing insurance services",
                    Contact = new OpenApiContact
                    {
                        Name = "Vishal Singh",
                        Email = "vishal.singh@gmail.com",
                        Url = new Uri("https://www.linkedin.com/in/vishal-singh-694502112/")
                    }
                });

                c.EnableAnnotations();
            });
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
            services.AddAutoMapper(typeof(Program));

            services.AddScoped<ISurchargeRepository, SurchargeRepository>();

            services.AddScoped<IInsuranceService, InsuranceService>();
            services.AddScoped<ISurchargeService, SurchargeService>();

            services.AddHttpClientService();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Insurance API V1");
                    c.RoutePrefix = string.Empty; // Set Swagger UI at the root
                });
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseMiddleware<GlobalExceptionMiddleware>();
            }

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

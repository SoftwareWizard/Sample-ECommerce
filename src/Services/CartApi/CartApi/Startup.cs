using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CartApi.Infrastructure.Filters;
using CartApi.Model;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CartApi
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
            services.AddMvc();
            //services.AddMvc(options =>
            //{
            //    options.Filters.Add(typeof(HttpGlobalExceptionFilter));
            //}).AddControllersAsServices();

            services.Configure<CartSettings>(Configuration);
            services.AddSingleton<ConnectionMultiplexer>(provider =>
            {
                var settings = provider.GetRequiredService<IOptions<CartSettings>>().Value;
                var configuration = ConfigurationOptions.Parse(settings.ConnectionString, true);
                configuration.ResolveDns = true;
                configuration.AbortOnConnectFail = false;
                return ConnectionMultiplexer.Connect(configuration);
            });

            services.AddSwaggerGen(SwaggerOptions);

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddTransient<ICartRepository, RedisCartRepository>();
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //var pathBase = Configuration["PATH_BASE"];
            //if (!string.IsNullOrEmpty(pathBase))
            //{
            //    app.UsePathBase(pathBase);
            //}

            app.UseMvc();

            app.UseSwagger()
                .UseSwaggerUI(options =>
                {
                    //var path = !string.IsNullOrEmpty(pathBase) 
                    //    ? pathBase 
                    //    : string.Empty;
                    options.SwaggerEndpoint($"/swagger/v1/swagger.json", "Basket.API V1");
                    //options.ConfigureOAuth2("basketswaggerui", "", "", "Basket Swagger UI");
                });
        }

        private void SwaggerOptions(SwaggerGenOptions options)
        {
            options.DescribeAllEnumsAsStrings();
            options.SwaggerDoc("v1", new Info()
            {
                Title = "Basket HTTP API",
                Version = "v1",
                Description = "The Basket Service HTTP API",
                TermsOfService = "Terms of Service"
            });
        }
    }
}

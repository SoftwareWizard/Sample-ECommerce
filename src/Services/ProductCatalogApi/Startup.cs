using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ProductCatalogApi.Data;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ProductCatalogApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private string ConnectionString
        {
            get
            {
                var server = Configuration["DatabaseServer"];
                var database = Configuration["DatabaseName"];
                var user = Configuration["DatabaseUser"];
                var password = Configuration["DatabaseUserPassword"];
                var connectionString = $"Server={server};" +
                                       $"Database={database};" +
                                       $"User Id={user};" +
                                       $"Password={password};" +
                                       "MultipleActiveResultSets=true";
                return connectionString;
            }
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CatalogSettings>(Configuration);
            services.AddDbContext<CatalogContext>(builder => builder.UseSqlServer(ConnectionString));
            services.AddMvc();
            services.AddSwaggerGen(SwaggerConfiguration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app
                .UseSwagger()
                .UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint($"/swagger/v1/swagger.json", "ProductCatalogAPI V1");
                });

            app.UseMvc();
        }

        private void SwaggerConfiguration(SwaggerGenOptions options)
        {
            options.DescribeAllEnumsAsStrings();
            options.SwaggerDoc("v1", new Swashbuckle.AspNetCore.Swagger.Info
            {
                Title = "ShoesOnContainers - Product Catalog HTTP API",
                Version = "v1",
                Description = "The Product Catalog Microservice HTTP API. This is a Data-Driven/CRUD microservice sample",
                TermsOfService = "Terms Of Service"
            });
        }

    }
}

using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using CartApi.Infrastructure.Filters;
using CartApi.Model;
using Microsoft.AspNetCore.Authentication.JwtBearer;
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

            ConfigureAuthService(services);
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
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                loggerFactory.AddConsole(LogLevel.Trace);
            }

            //var pathBase = Configuration["PATH_BASE"];
            //if (!string.IsNullOrEmpty(pathBase))
            //{
            //    app.UsePathBase(pathBase);
            //}
            app.UseAuthentication();
            app.UseMvc();

            app.UseSwagger()
                .UseSwaggerUI(options =>
                {
                    //var path = !string.IsNullOrEmpty(pathBase) 
                    //    ? pathBase 
                    //    : string.Empty;
                    options.SwaggerEndpoint($"/swagger/v1/swagger.json", "Basket.API V1");
                    options.ConfigureOAuth2("basketswaggerui", "", "", "Basket Swagger UI");
                });
        }

        private void ConfigureAuthService(IServiceCollection services)
        {
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            var identityUrl = Configuration.GetValue<string>("IdentityUrl");
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.Authority = identityUrl;
                options.RequireHttpsMetadata = false;
                options.Audience = "basket";
            });
        }

        private void SwaggerOptions(SwaggerGenOptions options)
        {
            options.DescribeAllEnumsAsStrings();
            options.SwaggerDoc("v1", new Info
            {
                Title = "Basket HTTP API",
                Version = "v1",
                Description = "The Basket Service HTTP API",
                TermsOfService = "Terms of Service"
            });

            var identityUrl = Configuration.GetValue<string>("IdentityUrl");
            options.AddSecurityDefinition("oauth2", new OAuth2Scheme
            {
                Type = "oauth2",
                Flow = "implicit",
                AuthorizationUrl = $"{identityUrl}/connect/authorize",
                TokenUrl = $"{identityUrl}/connect/token",
                Scopes = new Dictionary<string, string>
                {
                    {"basket", "Basket Api"}
                }
            });
            options.OperationFilter<AuthorizeCheckOperationFilter>();
        }
    }
}

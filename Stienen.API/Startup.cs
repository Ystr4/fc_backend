using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data.Postgres;
using Data.Postgres.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Stienen.Backend.DataAccess;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Stienen.Backend.DataAccess.Models;
using Stienen.Backend.Filters;
using Stienen.Backend.Infrastructure;
using Stienen.Backend.Services;

namespace Stienen.Backend {
    public class Startup {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add DI support for IOptions
            services.AddOptions();

            // Add DI mappings
            services.AddTransient<IDeviceDataService, DefaultDeviceDataService>();
            services.AddTransient<IDeviceDataRepository, DeviceDataRepository>();

            // Add EF core database
            AddDbContextServices(services);

            // Add configuration used by Data.Posgres
            services.Configure<DatabaseSettings>(Configuration.GetSection("DeviceDataDB"));

            // Add bearer tokens auth
            AddAuthenticationServices(services, Configuration);

            // Add Cors support
            AddCorsServices(services);

            // Add ASP.NET Core Identity
            AddIdentityCoreServices(services);

            // Add logging
            AddLoggingServices(services, Configuration);

            // Add Mvc
            AddMvcServices(services);
            services.AddRouting(options => options.LowercaseUrls = true);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
                app.UseCors("AllowAny");
            }
            else {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
                app.UseCors("AllowAppSpecific");
            }
            app.UseHttpsRedirection();
            app.UseAuthentication();

            app.UseMvc();
        }

        private void AddDbContextServices(IServiceCollection services)
        {
            services.AddDbContext<AppDataContext>(
                                                  options => {
                                                      // Use in-memory database for quick dev and testing
                                                      // TODO: Swap out for a real database in production
                                                      options.UseInMemoryDatabase("stienenDB");
                                                      //                                                         options.UseNpgsql(Configuration.GetSection("").Value);
                                                      //                                                         options.UseOpenIddict<Guid>();
                                                  });


            //            services.AddEntityFrameworkNpgsql()
            //                           .AddDbContext<AppDataContext>()
            //                           .BuildServiceProvider();
        }

        private static void AddIdentityCoreServices(IServiceCollection services)
        {
            var builder = services.AddIdentityCore<UserEntity>();
            builder = new IdentityBuilder(
                                          builder.UserType,
                                          typeof(UserRoleEntity),
                                          builder.Services);

            builder.AddRoles<UserRoleEntity>()
                   .AddEntityFrameworkStores<AppDataContext>()
                   .AddDefaultTokenProviders()
                   .AddSignInManager<SignInManager<UserEntity>>();
        }

        private static void AddAuthenticationServices(IServiceCollection services, IConfiguration config)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(options => {
                        var symmetricKey = Convert.FromBase64String(config.GetSection("RabbitMq")["TokenKey"]);
                        SecurityKey signingKey = new SymmetricSecurityKey(symmetricKey);
                        //                        SecurityAlgorithms.HmacSha256Signature
                        options.TokenValidationParameters = new TokenValidationParameters {
                                // Clock skew compensates for server time drift.
                                // We recommend 5 minutes or less:
                                ClockSkew = TimeSpan.FromMinutes(5),
                                // Specify the key used to sign the token:
                                IssuerSigningKey = signingKey,
                                RequireSignedTokens = true,
                                // Ensure the token hasn't expired:
                                RequireExpirationTime = false,
                                ValidateLifetime = false,
                                // Ensure the token audience matches our audience value (default true):
                                ValidateAudience = true,
                                ValidAudience = "SBE_audience",
                                // Ensure the token was issued by a trusted authorization server (default true):
                                ValidateIssuer = true,
                                ValidIssuer = "SBE_issuer",
                        };
                    });
        }

        private void AddLoggingServices(IServiceCollection services, IConfiguration config)
        {
            services.AddLogging(logging => {
                logging.AddConfiguration(config.GetSection("Logging"));
                logging.AddConsole();
                logging.AddDebug();
            });
        }

        private void AddCorsServices(IServiceCollection services)
        {
            services.AddCors(options => {
                options.AddPolicy("AllowAny",
                                  builder => {
                                      builder.AllowAnyOrigin()
                                             .AllowAnyHeader()
                                             .AllowAnyMethod();
                                  });
                options.AddPolicy("AllowAppSpecific",
                                  builder => {
                                      builder.WithOrigins("https://devcloud.farmconnect.eu")
                                             .AllowAnyHeader()
                                             .AllowAnyMethod();
                                  });
            });
        }

        private void AddMvcServices(IServiceCollection services)
        {
            services.AddMvc(options => {
                        //                        options.CacheProfiles.Add("Static", new CacheProfile {Duration = 86400});
                        //                        options.CacheProfiles.Add("Collection", new CacheProfile {Duration = 60});
                        //                        options.CacheProfiles.Add("Resource", new CacheProfile {Duration = 180});

                        options.Filters.Add<JsonExceptionFilter>();
                        options.Filters.Add<RequireHttpsOrCloseAttribute>();
                        //                        options.Filters.Add<LinkRewritingFilter>();
                    })
                    .SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                    .AddJsonOptions(options => {
                        // These should be the defaults, but we can be explicit:
                        options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
                        options.SerializerSettings.DateFormatHandling = DateFormatHandling.IsoDateFormat;
                        options.SerializerSettings.DateParseHandling = DateParseHandling.DateTimeOffset;
                    });
        }
    }
}
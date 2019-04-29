using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fc_backend.DataAccess;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;


namespace fc_backend
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
            services.AddDbContext<AppDataContext>(
                                                     options =>
                                                     {
                                                         // Use in-memory database for quick dev and testing
                                                         // TODO: Swap out for a real database in production
                                                         options.UseInMemoryDatabase("stienenDB");
//                                                         options.UseNpgsql(Configuration.GetSection("DB_ConncetionString").Value);
//                                                         options.UseOpenIddict<Guid>();
                                                     });


            //            services.AddEntityFrameworkNpgsql()
            //                           .AddDbContext<AppDataContext>()
            //                           .BuildServiceProvider();


            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using datingApp.api.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
// using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace datingApp.api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // public Startup(IConfiguration configuration) 
        // {
        //     this.Configuration = configuration;
               
        // }
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<DataContext>(x => x.UseSqlite(Configuration.GetConnectionString("DefaultConnection")));
            //services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
            services.AddControllers();            
            services.AddCors( o => o.AddPolicy("CorePolicy",builder => {
                builder.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin();
            }));
            
            services.AddScoped<IAuthRepository,AuthRepository>();
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(
                options => {
                     options.RequireHttpsMetadata = false;
                     options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters{
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey
                            (Encoding.ASCII.GetBytes(Configuration.GetSection("AppSettings:Token").Value)),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                }
            );                  
            

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                //throw new Exception("Test Exception");
                app.UseDeveloperExceptionPage();
            }
            else
            {

            }

            // app.UseHttpsRedirection();
            
            //app.UseMvc();

            
            

            app.UseAuthentication();//to be moved down

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                //app.UseMvc();
            });

            app.UseCors("CorePolicy");

            // app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            
            
            //app.UseMvc();


        }
    }
}

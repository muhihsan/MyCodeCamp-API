using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyCodeCamp.Data;
using Newtonsoft.Json;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using MyCodeCamp.Data.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using MyCodeCamp.Services.AppSettings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace MyCodeCamp
{
    public class Startup
    {
        public IConfiguration _configuration { get; }

        private IHostingEnvironment _env;

        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            _configuration = configuration;
            _env = env;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.Configure<JWTTokenOptions>(options => _configuration.GetSection("Token").Bind(options));

            services.AddSingleton(_configuration);
            
            services.AddDbContext<CampContext>(ServiceLifetime.Scoped);
            services.AddScoped<ICampRepository, CampRepository>();
            services.AddTransient<CampDbInitializer>();
            services.AddTransient<CampIdentityInitializer>();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddAutoMapper();

            services.AddIdentity<CampUser, IdentityRole>()
                .AddEntityFrameworkStores<CampContext>();

            //services.AddAuthorization(cfg => 
            //{
            //    cfg.AddPolicy("SuperUser", p => p.RequireClaim("SuperUser", "True"));
            //});

            //services.AddJwtBearerAuthentication(options => 
            //{
            //    options.TokenValidationParameters = new TokenValidationParameters
            //    {
            //        ValidIssuer = _configuration["Token:Issuer"],
            //        ValidAudience = _configuration["Token:Audience"],
            //        ValidateIssuerSigningKey = true,
            //        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Token:Key"])),
            //        ValidateLifetime = true
            //    };
            //});

            //services.AddAuthentication(o =>
            //{
            //    o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            //    o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            //});

            //services.Configure<IdentityOptions>(config =>
            //{
            //    config.Cookies.ApplicationCookie.Events =
            //        new CookieAuthenticationEvents
            //        {
            //            OnRedirectToLogin = (ctx) =>
            //            {
            //                if (ctx.Request.Path.StartsWithSegments("/api") && ctx.Response.StatusCode == 200)
            //                    ctx.Response.StatusCode = 401;

            //                return Task.CompletedTask;
            //            },
            //            OnRedirectToAccessDenied = (ctx) =>
            //            {
            //                if (ctx.Request.Path.StartsWithSegments("/api") && ctx.Response.StatusCode == 200)
            //                    ctx.Response.StatusCode = 403;

            //                return Task.CompletedTask;
            //            },
            //        };
            //});

            services.AddCors(cfg => 
            {
                cfg.AddPolicy("MIhsanOnly", policy =>
                {
                    policy
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .WithOrigins("http://github.com/muhihsan");
                });

                cfg.AddPolicy("AnyGET", policy =>
                {
                    policy
                        .AllowAnyHeader()
                        .WithMethods("GET")
                        .AllowAnyOrigin();
                });
            });

            services.AddMvc(opt => 
            {
                if (!_env.IsProduction())
                    opt.SslPort = 44359;

                opt.Filters.Add(new RequireHttpsAttribute());
            })
            .AddJsonOptions(opt =>
            {
                opt.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            });

            // Build the intermediate service provider then return it
            return services.BuildServiceProvider();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IServiceProvider serviceProvider)
        {
            //app.UseIdentity();

            //app.UseJwtBearerAuthentication();

            app.UseMvc();

            var seeder = serviceProvider.GetService<CampDbInitializer>();
            seeder.Seed().Wait();

            //var identitySeeder = serviceProvider.GetService<CampIdentityInitializer>();
            //identitySeeder.Seed().Wait();
        }
    }
}

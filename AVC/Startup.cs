using AVC.GenericRepository;
using AVC.GenericRepository.Implement;
using AVC.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Text;
using AVC.Extension;
using Microsoft.Extensions.Logging;
using Serilog;
using System.Reflection;
using System.IO;
using Tagent.EmailService;
using Tagent.EmailService.Define;
using Tagent.EmailService.Implement;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using Morcatko.AspNetCore.JsonMergePatch;
using AVC.Repositories.Interface;
using AVC.Repositories.Implement;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using AVC.Services.Implements;
using AVC.Services.Interfaces;
using AVC.Hubs;

namespace AVC
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
            services.AddCors();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
               .AddJwtBearer(option =>
               {
                   option.TokenValidationParameters = new TokenValidationParameters
                   {
                       ValidateIssuer = true,
                       ValidateAudience = true,
                       ValidateLifetime = true,
                       ValidateIssuerSigningKey = true,
                       ValidIssuer = Configuration["Jwt:Issuer"],
                       ValidAudience = Configuration["Jwt:Issuer"],
                       IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"])),
                       RequireExpirationTime = false
                   };
               }
               );

            services.AddSignalR(o =>
            {
                o.EnableDetailedErrors = true;
                o.MaximumReceiveMessageSize = 50000000;
            }).AddAzureSignalR();

            int apiVersion = Configuration.GetValue<int>("Version");

            services.AddMvcCore().AddNewtonsoftJsonMergePatch();

            services.AddControllers().AddNewtonsoftJson(
                s =>
                {
                    s.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                    s.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                }).AddNewtonsoftJsonMergePatch();
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
            services.AddDbContext<AVCContext>(
                option => option.UseSqlServer(Configuration.GetConnectionString("Default"))
                );

            //Add AutoMapper
            services.AddHttpContextAccessor();

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.AddSingleton<IEmailSender, EmailSender>();

            //service 
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IAuthenticateService, AuthenticateService>();
            services.AddScoped<ICarService, CarService>();
            services.AddScoped<IProfileService, ProfileService>();
            services.AddScoped<IAssignCarService, AssignCarService>();
            services.AddScoped<IIssueService, IssueService>();
            services.AddScoped<IModelVersionService, ModelVersionService>();
            services.AddScoped<IRoleService, RoleService>();

            //repository
            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddScoped<IAssignedCarRepository, AssignedCarRepository>();
            services.AddScoped<ICarRepository, CarRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IIssueRepository, IssueRepository>();
            services.AddScoped<IModelVersionRepository, ModelVersionRepository>();
            services.AddScoped<IIssueTypeRepository, IssueTypeRepository>();
            services.AddScoped<IDefaultConfigurationRepository, DefaultConfigurationRepository>();

            //unit of work
            services.AddTransient<IUnitOfWork, UnitOfWork>();

            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddScoped<IUrlHelper>(x =>
            {
                var actionContext = x.GetRequiredService<IActionContextAccessor>().ActionContext;
                var factory = x.GetRequiredService<IUrlHelperFactory>();
                if (actionContext != null)
                    return factory.GetUrlHelper(actionContext);
                else
                {
                    return null;
                }
            });

            //email sender
            var emailConfig = Configuration.GetSection("EmailConfiguration")
                .Get<EmailConfiguration>();
            services.AddSingleton(emailConfig);

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v" + apiVersion, new OpenApiInfo { Title = "AVC System", Version = "v" + apiVersion });
                c.OperationFilter<JsonMergePatchDocumentOperationFilter>();

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please insert JWT with Bearer into field",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement {
                   {
                     new OpenApiSecurityScheme
                     {
                       Reference = new OpenApiReference
                       {
                         Type = ReferenceType.SecurityScheme,
                         Id = "Bearer"
                       }
                      },
                      new string[] { }
                    }
                  });

                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });




            services.AddRouting(option => option.LowercaseUrls = true);


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, Microsoft.Extensions.Logging.ILogger<Startup> logger)
        {
            int apiVersion = Configuration.GetValue<int>("Version");

            app.UseCors(
               options => options.SetIsOriginAllowed(x => _ = true).AllowAnyMethod().AllowAnyHeader().AllowCredentials()
           );

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v" + apiVersion + "/swagger.json", "AVC System");
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();

            app.UseHttpsRedirection();

            app.UseSerilogRequestLogging();

            app.ConfigureExceptionHandler(logger, env);

            app.UseRouting();

            //authen before author
            app.UseAuthentication();

            app.UseAuthorization();


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<AVCHub>("/hub");
                endpoints.MapControllers();
            });


        }
    }
}

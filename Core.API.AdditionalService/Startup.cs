using Autofac;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using System;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.IO;
using Core.Library.ArivuTharavuThalam.Repository;
using Core.Library.ArivuTharavuThalam.DataCaching;
using Core.API.BusinessLogics.Common;

namespace Core.API.AdditionalService
{
    public class Startup
    {
        private readonly string allowAllOrigins = "_AllowAllOrigins";
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            ConfigureServices_DBContext(services);

            ConfigureServices_AddAuthentication(services);

            ConfigureServices_Cache(services);

            services.AddHttpClient();

            services.AddGrpc();

            services.AddControllers();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Core.API.AdditionalService", Version = "v1" });

                var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
            });

            ConfigureServices_ConfigureAddCors(services);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseDeveloperExceptionPage();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Core.API.AdditionalService v1");
                c.RoutePrefix = string.Empty;
            });

            app.UseCors(allowAllOrigins);

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        public void ConfigureServices_Cache(IServiceCollection services)
        {
            switch (Configuration.CurrenCachingType())
            {
                case "Empty":
                default:
                    break;

                case "RedisCache":
                    var redisCacheSettingOption = Configuration.GetSection("RedisCacheSetting");
                    var redisCacheSetting = redisCacheSettingOption.Get<RedisCacheSetting>();

                    services.Configure<RedisCacheSetting>(redisCacheSettingOption);

                    services.AddStackExchangeRedisCache(options =>
                    {
                        options.Configuration = redisCacheSetting.ConnectionStrings;
                        options.InstanceName = redisCacheSetting.ApplicationInstanceName;
                    });

                    break;
            }
        }
        public void ConfigureServices_AddAuthentication(IServiceCollection services)
        {
            if (Configuration.CurrenIdentityServerType() == "IdentityServerSettings")
            {
                var identityServerSettingsOption = Configuration.GetSection("IdentityServerSettings");
                var identityServerSettings = identityServerSettingsOption.Get<IdentityServerSettings>();
                services.Configure<IdentityServerSettings>(identityServerSettingsOption);

                var principleServiceIdentityServerClientSettings = Configuration.GetSection("PrincipleServiceIdentityServerClientSettings");
                services.Configure<PrincipleServiceIdentityServerClientSettings>(principleServiceIdentityServerClientSettings);

                //AUTHENTICATION CONFIGURATION 
                //AddJwtBearer-> is designed to work with open id connect

                services.AddAuthentication(identityServerSettings.AuthenticationSchema)
                                      .AddJwtBearer(identityServerSettings.AuthenticationSchema, config =>
                                      {
                                          config.Authority = identityServerSettings.Authority;
                                          config.Audience = identityServerSettings.Audience; //I AM ADDITIONAL SERVICE API
                                          config.RequireHttpsMetadata = false;
                                      });
            }

            services.AddMvc(
                options =>
                {
                    if (Configuration.CurrenIdentityServerType() == "IdentityServerSettings")
                    {
                        var identityServerSettingsOption = Configuration.GetSection("IdentityServerSettings");
                        var identityServerSettings = identityServerSettingsOption.Get<IdentityServerSettings>();

                        var policy = new AuthorizationPolicyBuilder(identityServerSettings.AuthenticationSchema)
                        .RequireAuthenticatedUser()
                        .Build();

                        //Global Authorization Filter Configuration
                        options.Filters.Add(new AuthorizeFilter(policy));
                    }
                });
        }

        public void ConfigureServices_DBContext(IServiceCollection services)
        {
            switch (Configuration.CurrentDBContext())
            {
                case "DapperContext":
                    services.AddDbContext<DapperAppContext>(options =>
                          options.UseSqlServer(
                              Configuration.GetConnectionString("SQLDBContext")));
                    break;

                case "PostgresDBContext":
                    services.AddDbContext<SqlDataBaseDataContext>(opt =>
                    opt.UseNpgsql(Configuration.GetConnectionString("PostgresDBContext")));
                    break;

                case "SqliteDBContext":
                    services.AddDbContext<SqlDataBaseDataContext>(opt =>
                    opt.UseSqlite(Configuration.GetConnectionString("SqliteDBContext")));
                    break;

                case "SQLDBContext":
                    services.AddDbContext<SqlDataBaseDataContext>(opt =>
                    opt.UseSqlServer(Configuration.GetConnectionString("SQLDBContext")));
                    break;

                case "MongoDBContext":
                    //not required here since, for now not doing dependency injection for it
                    //services.AddTransient<MongoClient>();
                    break;

                case "InMemoryContext":
                default:
                    services.AddDbContext<InMemorySqlDatabaseDataContext>(opt =>
                    opt.UseInMemoryDatabase(Configuration.GetConnectionString("InMemoryContext")));
                    break;
            }
        }

        //AUTO FAC
        public void ConfigureContainer(ContainerBuilder builder)
        {
            //var redisQueuesMessageSettingsOption = Configuration.GetSection("RedisQueuesMessageSettings");
            //var redisQueuesMessageSettings = redisQueuesMessageSettingsOption.Get<RedisQueuesMessageSettings>();

            //builder.RegisterInstance(redisQueuesMessageSettings);

            /*ALl this coding done only to keep "MongoDBContext" as part of "ConnectionStrings" - CUSTOM LOGIC
                 Whenever create a seperate section for "MongoDBContext" lets do a proper IOption
                 Configuration.Bind creates a seperate section at runtime*/

            if (Configuration.CurrentDBContext() == "MongoDBContext")
            {
                var mongoDBSettings = Configuration.GetConnectionString("MongoDBContext");

                builder.RegisterInstance(new MongoDBSettings() { ConnectionString = mongoDBSettings });
            }

            builder.RegisterModule(new AutofacModule(Configuration));
        }

        /// <summary>
        /// MUST USE BUILDER TO ONLY ALLOW REQUIRED APPLICATION 
        /// THIS LINE OF CODE IS ONLY FOR DEVELOPMENT
        /// REFER AZURE PROJECT FOR REAL IMPLEMENTATION
        /// </summary>
        /// <param name="services"></param>
        private void ConfigureServices_ConfigureAddCors(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy(allowAllOrigins, builder => builder
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
            });
        }
    }
}


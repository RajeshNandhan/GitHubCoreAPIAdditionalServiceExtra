using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using MongoDB.Driver;
using System.Text;
using Core.Library.ArivuTharavuThalam.Repository;
using Core.API.BusinessLogics.Common;

namespace Core.API.AdditionalService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = CreateHostBuilder(args);
            //var builder = CreateHostBuilderDev(args);

            try
            {
                var host = builder.Build();

                CreateDbIfNotExists(host);

                host.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal("Failed to start Program");
                Log.Fatal(ex, "Failed to start Program");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            if (string.IsNullOrEmpty(environment))
                environment = "Development";

            IConfiguration config = new ConfigurationBuilder()
            .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
            .Build();

            string kestralEndpointUrl = config.GetSection("Kestrel:Endpoints:Http:Url").Value;

            return Host.CreateDefaultBuilder(args)
                 .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                 .ConfigureWebHostDefaults(webBuilder =>
                 {
                     webBuilder.ConfigureLogging((hostingContext, logging) =>
                     {
                         logging.AddEventLog();
                     })
                     .ConfigureAppConfiguration((context, config) =>
                     {
                         Log.Logger = new LoggerConfiguration()
                        .ReadFrom.Configuration(config.Build())
                        .CreateLogger();
                     })
                     .UseSerilog()
                    .UseKestrel(option =>
                    {
                        StringBuilder sb = new StringBuilder();
                        foreach (var arg in args)
                        {
                            sb.Append(arg);
                            sb.Append(Environment.NewLine);
                        }
                        if (!string.IsNullOrEmpty(sb.ToString()))
                        {
                            Log.Information($"--> appliation argments - {sb.ToString()}");
                        }
                        Log.Information($"--> ASPNETCORE_ENVIRONMENT - {environment}, Kestrel Url - {config.GetSection("Kestrel:Endpoints:Http:Url").Value}");
                    })
                    .UseUrls(kestralEndpointUrl)
                    //.UseIISIntegration()
                    .UseStartup<Startup>();
                 });
            //.UseWindowsService();
        }

        private static void CreateDbIfNotExists(IHost host)
        {
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                var sqldbContext = services.GetService<SqlDataBaseDataContext>();

                if (sqldbContext != null)
                {
                    //CREATE A NEW SQL DATA BASE, IF NOT FOUND
                    sqldbContext.Database.EnsureCreated();
                    //INSERT RECORDS
                    //RetrySQLDB.Retry(sqldbContext);
                    //DatabaseInitializerPerson.InitializeSQLDB(sqldbContext);
                    sqldbContext.Dispose();
                }

                var inmemoryContext = services.GetService<InMemorySqlDatabaseDataContext>();

                if (inmemoryContext != null)
                {
                    //CREATE A NEW DATA BASE IN MEMORY
                    inmemoryContext.Database.EnsureCreated();
                    //INSERT RECORDS
                    //DatabaseInitializerPerson.InitializeSQLDBInMemory(inmemoryContext);
                }

                var mongoDBSettings = services.GetService<MongoDBSettings>();

                if (mongoDBSettings != null)
                {
                    //CREATE A MONGO DATABASE
                    var _databaseName = MongoUrl.Create(mongoDBSettings.ConnectionString).DatabaseName;

                    //var database = mongoClient.GetDatabase(_databaseName);

                    //var documents = database.GetCollection<Person>(typeof(Person).Name);

                    //DatabaseInitializerPerson.InitializeMongoCollection(documents);
                }
            }
        }

        public static IHostBuilder CreateHostBuilderDev(string[] args) =>
           Host.CreateDefaultBuilder(args)
               .UseServiceProviderFactory(new AutofacServiceProviderFactory())
               .ConfigureWebHostDefaults(webBuilder =>
               {
                   webBuilder.UseStartup<Startup>();
               });
    }
}

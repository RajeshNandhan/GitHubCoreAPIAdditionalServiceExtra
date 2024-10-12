using Autofac;
using Autofac.Extras.DynamicProxy;
using Core.API.BusinessLogics.Common;
using Core.API.BusinessLogics.Common.Data.UnitOfWork;
using Core.Library.ArivuTharavuThalam;
using Core.Library.ArivuTharavuThalam.DataCaching;
using Core.Library.ArivuTharavuThalam.Repository;
using Microsoft.Extensions.Configuration;

namespace Core.API.AdditionalService
{
    public class AutofacModule : Module
    {
        private IConfiguration configuration { get; }

        public AutofacModule(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        protected override void Load(ContainerBuilder builder)
        {
            //base.Load(builder);

            builder.RegisterInstance(configuration).As<IConfiguration>();

            //builder.RegisterType<ServiceActionLogger>()
            //    .As<ServiceActionLogger>()
            //    .SingleInstance();

            builder.RegisterType<BookDirector>()
                .As<IBookDirector>()
                .EnableClassInterceptors()
                .InterceptedBy(typeof(ServiceActionLogger));

            builder.RegisterType<PersonDirector>()
               .As<IPersonDirector>()
                .EnableClassInterceptors()
                .InterceptedBy(typeof(ServiceActionLogger));

            ConfigureServices_DataCaching(builder);

            ConfigureServices_DataContext(builder);

            builder.Register(c => new ServiceActionLogger());
        }

        public void ConfigureServices_DataContext(ContainerBuilder builder)
        {
            switch (configuration.CurrentDBContext())
            {
                case "DapperContext":

                    //sqlDbContext.Database.SetCommandTimeout(TimeSpan.FromMinutes(3));
                    builder.RegisterType<DapperUnitOfWork>()
                           .As<IUnitOfWork>()
                           .WithParameter((parameter, context) => parameter.ParameterType == typeof(IGenericRepository<Person>),
                                            (parameter, context) => new DapperGenericRepository<Person>(context.Resolve<IConfiguration>()));

                    break;

                case "SQLDBContext":
                case "SqliteDBContext":
                case "PostgresDBContext":
                    //sqlDbContext.Database.SetCommandTimeout(TimeSpan.FromMinutes(3));
                    builder.RegisterType<SqlDatabaseUnitOfWork>()
                           .As<IUnitOfWork>()
                           .WithParameter((parameter, context) => parameter.ParameterType == typeof(IGenericRepository<Book>),
                                            (parameter, context) => new EntityFrameworkGenericRepository<Book>(context.Resolve<SqlDataBaseDataContext>()))
                           .WithParameter((parameter, context) => parameter.ParameterType == typeof(IGenericRepository<Person>),
                                            (parameter, context) => new EntityFrameworkGenericRepository<Person>(context.Resolve<SqlDataBaseDataContext>()))
                            .EnableClassInterceptors()
                            .InterceptedBy(typeof(ServiceActionLogger))
                           .InstancePerLifetimeScope();

                    break;

                case "MongoDBContext":

                    builder.RegisterType<MongoDatabaseUnitOfWork>()
                        .As<IUnitOfWork>()
                        .WithParameter((parameter, context) => parameter.ParameterType == typeof(IGenericRepository<Book>),
                                         (parameter, context) =>
                                         {
                                             return new MongoDBGenericRepository<Book>(context.Resolve<MongoDBSettings>());
                                         }
                                         )
                        .WithParameter((parameter, context) => parameter.ParameterType == typeof(IGenericRepository<Person>),
                                         (parameter, context) =>
                                         {
                                             return new MongoDBGenericRepository<Person>(context.Resolve<MongoDBSettings>());
                                         }
                                         )
                         .EnableClassInterceptors()
                         .InterceptedBy(typeof(ServiceActionLogger))
                        .InstancePerLifetimeScope();

                    break;

                case "InMemoryContext":
                default:

                    builder.RegisterType<InMemorySqlDatabaseUnitOfWork>()
                       .As<IUnitOfWork>()
                       .WithParameter((parameter, context) => parameter.ParameterType == typeof(IGenericRepository<Book>),
                                        (parameter, context) => new EntityFrameworkGenericRepository<Book>(context.Resolve<InMemorySqlDatabaseDataContext>()))
                       .WithParameter((parameter, context) => parameter.ParameterType == typeof(IGenericRepository<Person>),
                                        (parameter, context) => new EntityFrameworkGenericRepository<Person>(context.Resolve<InMemorySqlDatabaseDataContext>()))
                        .EnableClassInterceptors()
                        .InterceptedBy(typeof(ServiceActionLogger))
                       .InstancePerLifetimeScope();

                    break;
            }
        }

        public void ConfigureServices_DataCaching(ContainerBuilder builder)
        {
            switch (configuration.CurrenCachingType())
            {
                /*Check ConnectionStringExtensions.IsCacheRequired for "Empty" implementation*/
                case "Empty":
                case "DistributedMemory":
                    builder.RegisterType<NetCoreMemoryDataCache>()
                        .As<IDataCache>();

                    break;
                case "RedisCache":
                    var redisCacheSettingOption = configuration.GetSection("RedisCacheSetting");
                    var redisCacheSetting = redisCacheSettingOption.Get<RedisCacheSetting>();

                    builder.RegisterType<RedisDataCache>()
                    .As<IDataCache>()
                    .WithParameter((parameter, context) => parameter.ParameterType == typeof(RedisCacheSetting),
                                     (parameter, context) =>
                                     {
                                         return new RedisCacheSetting()
                                         {
                                             ApplicationInstanceName = redisCacheSetting.ApplicationInstanceName,
                                             ConnectionStrings = redisCacheSetting.ConnectionStrings
                                         };
                                     });
                    break;
            }
        }
    }
}

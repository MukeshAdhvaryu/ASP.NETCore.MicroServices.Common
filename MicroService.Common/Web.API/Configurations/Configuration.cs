/* Licensed under the MIT/X11 license.
* This notice may not be removed from any source distribution.
 Author: Mukesh Adhvaryu.
*/
//-:cnd:noEmit
#if !TDD 
//+:cnd:noEmit
using System.Reflection;
using System.Security.Cryptography;
using System.Text.Json.Serialization;

using MicroService.Common.Attributes;
using MicroService.Common.Interfaces;
using MicroService.Common.Models;
using MicroService.Common.Services;
using MicroService.Common.Web.API.Middlewares;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MicroService.Common.Web.API
{
    #region CONFIGURATION
    /// <summary>
    /// Configures dynamic controller - model relationship.
    /// </summary>
    public static class Configuration
    {
        #region VARIABLES
        //-:cnd:noEmit
#if !MODEL_USEMYOWNCONTROLLER
        static List<Tuple<Type, Type, Type, Type>> ControllerTypes = new List<Tuple<Type, Type, Type, Type>>(3);
        static volatile HashSet<string> ControllerNames = new HashSet<string>();

#if !MODEL_NONQUERYABLE
        static List<Tuple<Type, Type>> QueryControllerTypes = new List<Tuple<Type, Type>>(3);
        static List<Tuple<Type, Type, Type>> QueryKeyedControllerTypes = new List<Tuple<Type, Type, Type>>(3);
#endif
#endif
        //+:cnd:noEmit
        #endregion

        #region PROPERTIES
        public static bool IsProductionEnvironment { get; private set; }
        #endregion

        #region CONFIGURE MVC
        /// <summary>
        /// Creates MVCBuilder from service collection instance using the given action.
        /// </summary>
        /// <param name="services">Service collection to be used to create MvcBuilder.</param>
        /// <param name="isProductionEnvironment">True indicates if the application in production environment, otherwise, in development environement.</param>
        /// <param name="SwaggerDocTitle">Optional title to be used for swagger doc.
        /// Only relevant if the conditional constant MODEL_USESWAGGER is defined.</param>
        /// <param name="SwaggerDocDescription">Optional description to be used for swagger doc.
        /// Only relevant if the conditional constant MODEL_USESWAGGER is defined.</param>
        /// <returns>IMvcBuilder instance.</returns>
        public static IMvcBuilder AddMVC(this IServiceCollection services, bool isProductionEnvironment, string? SwaggerDocTitle = null, string? SwaggerDocDescription = null)
        {
            IsProductionEnvironment = isProductionEnvironment;
            var mvcBuilder = MvcServiceCollectionExtensions.AddMvc(services);
            //-:cnd:noEmit
#if !MODEL_USEMYOWNCONTROLLER
            /*
                 * This will register a controller dynamically based on TOutDTO type.
                 * So, now we do not need to create controller class in an actual microservice project.
                 * Inspired from the article: https://www.strathweb.com/2018/04/generic-and-dynamically-generated-controllers-in-asp-net-core-mvc/
                */
            Action<MvcOptions> nativeAction = (o) =>
            {
                o.Conventions.Add(new ControllerRouteConvention());
            };
            OptionsServiceCollectionExtensions.Configure(mvcBuilder.Services, nativeAction);
            mvcBuilder.ConfigureApplicationPartManager(m => m.FeatureProviders.Add(new ControllerFeatureProvider()));
#endif
            //+:cnd:noEmit

            mvcBuilder = services.AddControllers(option =>
            {
                option.Filters.Add<HttpExceptionFilter>();
            });

            mvcBuilder.AddJsonOptions(option =>
            {
                option.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });



            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddEndpointsApiExplorer();

            //-:cnd:noEmit
#if MODEL_USESWAGGER
            services.AddSwaggerGen(opt => {
                opt.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = SwaggerDocTitle,
                    Description = SwaggerDocDescription
                });


                opt.SchemaFilter<EnumSchemaFilter>();
            });
#endif
            //+:cnd:noEmit

            return mvcBuilder;
        }

        /// <summary>
        /// Creates MVCBuilder from service collection instance using the given action.
        /// </summary>
        /// <param name="services">Service collection to be used to create MvcBuilder.</param>
        /// <param name="action">Action with MvcOptions to be used while creating MvcBuilder.</param>
        /// <param name="isProductionEnvironment">True indicates if the application in production environment, otherwise, in development environement.</param>
        /// <param name="SwaggerDocTitle">Optional title to be used for swagger doc.
        /// Only relevant if the conditional constant MODEL_USESWAGGER is defined.</param>
        /// <param name="SwaggerDocDescription">Optional description to be used for swagger doc.
        /// Only relevant if the conditional constant MODEL_USESWAGGER is defined.</param>
        /// <returns>IMvcBuilder instance.</returns>
        public static IMvcBuilder AddMVC(this IServiceCollection services, Action<MvcOptions> action, bool isProductionEnvironment, string? SwaggerDocTitle = null, string? SwaggerDocDescription = null)
        {
            IsProductionEnvironment = isProductionEnvironment;
            var nativeAction = action;
            //-:cnd:noEmit
#if !MODEL_USEMYOWNCONTROLLER
            if (nativeAction == null)
            {
                nativeAction = (o) => o.Conventions.Add(new ControllerRouteConvention());
                goto CONFIGURE;
            }

            nativeAction = (o) =>
            {
                action(o);
                o.Conventions.Add(new ControllerRouteConvention());
            };
#endif
            //+:cnd:noEmit
            CONFIGURE:
            var mvcBuilder = MvcServiceCollectionExtensions.AddMvc(services);

            //-:cnd:noEmit
#if !MODEL_USEMYOWNCONTROLLER
            /*
             * This will register a controller dynamically based on TOutDTO type.
             * So, now we do not need to create controller class in an actual microservice project.
             * Inspired from the article: https://www.strathweb.com/2018/04/generic-and-dynamically-generated-controllers-in-asp-net-core-mvc/
            */
            OptionsServiceCollectionExtensions.Configure(mvcBuilder.Services, nativeAction);
            mvcBuilder.ConfigureApplicationPartManager(m => m.FeatureProviders.Add(new ControllerFeatureProvider()));
#endif
            //+:cnd:noEmit

            mvcBuilder = services.AddControllers(option =>
            {
                option.Filters.Add<HttpExceptionFilter>();
            });

            mvcBuilder.AddJsonOptions(option =>
            {
                option.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

            //-:cnd:noEmit
#if MODEL_USESWAGGER
            services.AddSwaggerGen(opt => {
                opt.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = SwaggerDocTitle,
                    Description = SwaggerDocDescription
                });


                opt.SchemaFilter<EnumSchemaFilter>();
            });
#endif
            //+:cnd:noEmit

            return mvcBuilder;
        }
        #endregion

        #region ADD KEYED MODEL
        /// <summary>
        /// Adds a new model to model processing layer.
        /// Use this mehod if you are to provide your own implementation of service class, otherwise 
        /// use another override of 'AddModel' method.
        /// </summary>
        /// <typeparam name="TOutDTO">DTO interface of your choice as a return type of GET calls - must derived from IModel interface.</typeparam>
        /// <typeparam name="TModel">Model implementation of your choice - must derived from Model class.</typeparam>
        /// <typeparam name="TID">Type of primary key such as type of int or Guid etc. </typeparam>
        /// <typeparam name="TInDTO">DTO interface of your choice  as an input type of PUT, POST calls- must derived from IModel interface.</typeparam>
        /// <typeparam name="TService">Service implementation of your choice - must be inherited from Service class.</typeparam>
        /// <typeparam name="TDBContext">DBContext<typeparamref name="TModel"/> of your choice.</typeparam>
        /// <param name="services">Service collection instance which to add services to.</param>
        /// <param name="configuration">Instance of application configuration class.</param>
        /// <param name="dbContextOptions">DbContextOptions to use in creating DbContextOptionsBuilder.</param>
        public static void AddModel<TOutDTO, TModel, TID, TInDTO, TService, TDBContext>(this IServiceCollection services, IConfiguration configuration, DbContextOptions? dbContextOptions = null)
            #region TYPE CONSTRAINTS
            where TModel : Model<TID, TModel>,
            //-:cnd:noEmit
#if (!MODEL_USEDTO)
            TOutDTO,
#endif
            //+:cnd:noEmit
            new()
            where TOutDTO : IModel
            where TInDTO: IModel
            where TService : Service<TOutDTO, TModel, TID, TDBContext>
            where TID : struct
            where TDBContext : DBContext
            #endregion
        {
            var type = typeof(TOutDTO);
            var modelType = typeof(TModel);

            modelType.GetOptions(configuration, out ServiceScope scope, out bool addController, out Action<DbContextOptionsBuilder> action, dbContextOptions);

            //-:cnd:noEmit
#if !MODEL_USEMYOWNCONTROLLER
            if (addController)
                ControllerTypes.Add(Tuple.Create(type, modelType, typeof(TID), typeof(TInDTO)));
#endif
            //+:cnd:noEmit

            services.AddDbContext<TDBContext>(action);
            switch (scope)
            {
                case ServiceScope.Scoped:
                default:
                    services.AddScoped<IContract<TOutDTO, TModel, TID>, TService>();
                    break;
                case ServiceScope.Transient:
                    services.AddTransient<IContract<TOutDTO, TModel, TID>, TService>();
                    break;
                case ServiceScope.Singleton:
                    services.AddSingleton<IContract<TOutDTO, TModel, TID>, TService>();
                    break;
            }
        }

        /// <summary>
        /// Adds a new model to model processing layer.
        /// Use this mehod if you are to provide your own implementation of service class, otherwise 
        /// use another override of 'AddModel' method.
        /// </summary>
        /// <typeparam name="TOutDTO">DTO interface of your choice as a return type of GET calls - must derived from IModel interface.</typeparam>
        /// <typeparam name="TModel">Model implementation of your choice - must derived from Model class.</typeparam>
        /// <typeparam name="TID">Type of primary key such as type of int or Guid etc. </typeparam>
        /// <typeparam name="TService">Service implementation of your choice - must be inherited from Service class.</typeparam>
        /// <typeparam name="TDBContext">DBContext<typeparamref name="TModel"/> of your choice.</typeparam>
        /// <param name="services">Service collection instance which to add services to.</param>
        /// <param name="configuration">Instance of application configuration class.</param>
        /// <param name="dbContextOptions">DbContextOptions to use in creating DbContextOptionsBuilder.</param>
        public static void AddModel<TOutDTO, TModel, TID, TService, TDBContext>(this IServiceCollection services, IConfiguration configuration, DbContextOptions? dbContextOptions = null)
            #region TYPE CONSTRAINTS
            where TModel : Model<TID, TModel>,
            //-:cnd:noEmit
#if (!MODEL_USEDTO)
            TOutDTO,
#endif
            //+:cnd:noEmit
            new()
            where TOutDTO : IModel
            where TService : Service<TOutDTO, TModel, TID, TDBContext>
            where TID : struct
            where TDBContext : DBContext
            #endregion
            => AddModel<TOutDTO, TModel, TID, TOutDTO, TService, TDBContext>(services, configuration, dbContextOptions);

        /// <summary>
        /// Adds a new model to model processing layer.
        /// Use this mehod if you are to provide your own implementation of service class, otherwise 
        /// use another override of 'AddModel' method.
        /// </summary>
        /// <typeparam name="TOutDTO">DTO interface of your choice as a return type of GET calls - must derived from IModel interface.</typeparam>
        /// <typeparam name="TModel">Model implementation of your choice - must derived from Model class.</typeparam>
        /// <typeparam name="TID">Type of primary key such as type of int or Guid etc. </typeparam>
        /// <typeparam name="TInDTO">DTO interface of your choice  as an input type of PUT, POST calls- must derived from IModel interface.</typeparam>
        /// <param name="services">Service collection instance which to add services to.</param>
        /// <param name="configuration">Instance of application configuration class.</param>
        /// <param name="dbContextOptions">DbContextOptions to use in creating DbContextOptionsBuilder.</param>
        public static void AddModel<TOutDTO, TModel, TID, TInDTO>(this IServiceCollection services, IConfiguration configuration, DbContextOptions? dbContextOptions = null)
            #region TYPE CONSTRAINTS
            where TModel : Model<TID, TModel>,
            //-:cnd:noEmit
#if (!MODEL_USEDTO)
            TOutDTO,
#endif
            //+:cnd:noEmit
            new()
            where TOutDTO : IModel
            where TInDTO : IModel
            where TID : struct
            #endregion
            => AddModel<TOutDTO, TModel, TID, TInDTO, Service<TOutDTO, TModel, TID, DBContext>, DBContext>(services, configuration, dbContextOptions);

        /// <summary>
        /// Adds a new model to model processing layer.
        /// Use this mehod if you are to provide your own implementation of service class, otherwise 
        /// use another override of 'AddModel' method.
        /// </summary>
        /// <typeparam name="TOutDTO">DTO interface of your choice as a return type of GET calls - must derived from IModel interface.</typeparam>
        /// <typeparam name="TModel">Model implementation of your choice - must derived from Model class.</typeparam>
        /// <typeparam name="TInDTO">DTO interface of your choice  as an input type of PUT, POST calls- must derived from IModel interface.</typeparam>
        /// <param name="services">Service collection instance which to add services to.</param>
        /// <param name="configuration">Instance of application configuration class.</param>
        /// <param name="dbContextOptions">DbContextOptions to use in creating DbContextOptionsBuilder.</param>
        public static void AddModel<TOutDTO, TModel, TInDTO>(this IServiceCollection services, IConfiguration configuration, DbContextOptions? dbContextOptions = null)
            #region TYPE CONSTRAINTS
            where TModel : Model<int, TModel>,
            //-:cnd:noEmit
#if (!MODEL_USEDTO)
            TOutDTO,
#endif
            //+:cnd:noEmit
            new()
            where TOutDTO : IModel
            where TInDTO : IModel
            #endregion
            => AddModel<TOutDTO, TModel, int, TInDTO, Service<TOutDTO, TModel, int, DBContext>, DBContext>(services, configuration, dbContextOptions);

        /// <summary>
        /// Adds a new model to model processing layer.
        /// Uses default Service repository class.
        /// Uses default DBContext of TModel class.
        /// </summary>
        /// <typeparam name="TOutDTO">DTO interface of your choice as a return type of GET calls - must derived from IModel interface.</typeparam>
        /// <typeparam name="TModel">Model implementation of your choice - must derived from Model with int id type.</typeparam>
        /// <param name="services">Service collection instance which to add services to.</param>
        /// <param name="configuration">Instance of application configuration class.</param>
        /// <param name="dbContextOptions">DbContextOptions to use in creating DbContextOptionsBuilder.</param>
        public static void AddModel<TOutDTO, TModel>(this IServiceCollection services, IConfiguration configuration, DbContextOptions? dbContextOptions = null)
            #region TYPE CONSTRAINTS
            where TModel : Model<int, TModel>,
            //-:cnd:noEmit
#if (!MODEL_USEDTO)
            TOutDTO,
#endif
            //+:cnd:noEmit
            new()
            where TOutDTO : IModel
            #endregion
            => AddModel<TOutDTO, TModel, int, Service<TOutDTO, TModel, int, DBContext>, DBContext>(services, configuration, dbContextOptions);

        /// <summary>
        /// Adds a new model to model processing layer.
        /// Uses default Service repository class.
        /// Uses default DBContext of TModel class.
        /// Uses TModal as TOutDTO type argument.
        /// </summary>
        /// <typeparam name="TModel">Model implementation of your choice - must derived from Model with int id type.</typeparam>
        /// <param name="services">Service collection instance which to add services to.</param>
        /// <param name="configuration">Instance of application configuration class.</param>
        /// <param name="dbContextOptions">DbContextOptions to use in creating DbContextOptionsBuilder.</param>
        public static void AddModel<TModel>(this IServiceCollection services, IConfiguration configuration, DbContextOptions? dbContextOptions = null)
            #region TYPE CONSTRAINTS
            where TModel : Model<int, TModel>,
            new()
            #endregion
            =>
            AddModel<TModel, TModel, int, Service<TModel, TModel, int, DBContext>, DBContext>(services, configuration, dbContextOptions);
        #endregion

        #region ADD KEY LESS QUERY MODEL
        //-:cnd:noEmit
#if !MODEL_NONQUERYABLE
        /// <summary>
        /// Adds a new model to model query processing layer.
        /// Use this mehod if you are to provide your own implementation of query service class, otherwise 
        /// use another override of 'AddQueryModel' method.
        /// </summary>
        /// <typeparam name="TOutDTO">DTO interface of your choice as a return type of GET calls - must derived from IModel interface.</typeparam>
        /// <typeparam name="TModel">Model implementation of your choice - must derived from Model class.</typeparam>
        /// <typeparam name="TService">Query Service implementation of your choice - must be inherited from Service class.</typeparam>
        /// <typeparam name="TDBContext">DBContext mplementation of your choice.</typeparam>
        /// <param name="services">Service collection instance which to add services to.</param>
        /// <param name="configuration">Instance of application configuration class.</param>
        /// <param name="dbContextOptions">DbContextOptions to use in creating DbContextOptionsBuilder.</param>
        public static void AddQueryModel<TOutDTO, TModel, TService, TDBContext>(this IServiceCollection services, IConfiguration configuration, DbContextOptions? dbContextOptions = null)
            #region TYPE CONSTRAINTS
            where TModel : Model<TModel>,
            //-:cnd:noEmit
#if (!MODEL_USEDTO)
            TOutDTO,
#endif
            //+:cnd:noEmit
            new()
            where TOutDTO : IModel
            where TService : QueryService<TOutDTO, TModel, TDBContext>
            where TDBContext : DBContext
            #endregion
        {
            var type = typeof(TOutDTO);
            var modelType = typeof(TModel);

            modelType.GetOptions(configuration, out ServiceScope scope, out bool addController, out Action<DbContextOptionsBuilder> action, dbContextOptions);

            //-:cnd:noEmit
#if !MODEL_USEMYOWNCONTROLLER
            if (addController)
                QueryControllerTypes.Add(Tuple.Create(type, modelType));
#endif
            //+:cnd:noEmit

            services.AddDbContext<TDBContext>(action);
            switch (scope)
            {
                case ServiceScope.Scoped:
                default:
                    services.AddScoped<IQueryContract<TOutDTO, TModel>, TService>();
                    break;
                case ServiceScope.Transient:
                    services.AddTransient<IQueryContract<TOutDTO, TModel>, TService>();
                    break;
                case ServiceScope.Singleton:
                    services.AddSingleton<IQueryContract<TOutDTO, TModel>, TService>();
                    break;
            }
        }

        /// <summary>
        /// Adds a new model to model query processing layer.
        /// Use this mehod if you are to provide your own implementation of query service class, otherwise 
        /// use another override of 'AddQueryModel' method.
        /// </summary>
        /// <typeparam name="TOutDTO">DTO interface of your choice as a return type of GET calls - must derived from IModel interface.</typeparam>
        /// <typeparam name="TModel">Model implementation of your choice - must derived from Model class.</typeparam>
        /// <param name="services">Service collection instance which to add services to.</param>
        /// <param name="configuration">Instance of application configuration class.</param>
        /// <param name="dbContextOptions">DbContextOptions to use in creating DbContextOptionsBuilder.</param>
        public static void AddQueryModel<TOutDTO, TModel>(this IServiceCollection services, IConfiguration configuration, DbContextOptions? dbContextOptions = null)
            #region TYPE CONSTRAINTS
            where TModel : Model<TModel>,
            //-:cnd:noEmit
#if (!MODEL_USEDTO)
            TOutDTO,
#endif
            //+:cnd:noEmit
            new()
            where TOutDTO : IModel
            #endregion
            => AddQueryModel<TOutDTO, TModel, QueryService<TOutDTO, TModel, DBContext>, DBContext>(services, configuration, dbContextOptions);

        /// <summary>
        /// Adds a new model to model query processing layer.
        /// Use this mehod if you are to provide your own implementation of query service class, otherwise 
        /// use another override of 'AddQueryModel' method.
        /// </summary>
        /// <typeparam name="TModel">Model implementation of your choice - must derived from Model class.</typeparam>
        /// <param name="services">Service collection instance which to add services to.</param>
        /// <param name="configuration">Instance of application configuration class.</param>
        /// <param name="dbContextOptions">DbContextOptions to use in creating DbContextOptionsBuilder.</param>
        public static void AddQueryModel<TModel>(this IServiceCollection services, IConfiguration configuration, DbContextOptions? dbContextOptions = null)
            #region TYPE CONSTRAINTS
            where TModel : Model<TModel>,
            new()
            #endregion
            =>
            AddQueryModel<TModel, TModel, QueryService<TModel, TModel, DBContext>, DBContext>(services, configuration, dbContextOptions);
#endif
        //+:cnd:noEmit
        #endregion

        #region ADD KEYED QUERY MODEL
        //-:cnd:noEmit
#if !MODEL_NONQUERYABLE
        /// <summary>
        /// Adds a new model to model query processing layer.
        /// Use this mehod if you are to provide your own implementation of query service class, otherwise 
        /// use another override of 'AddQueryModel' method.
        /// </summary>
        /// <typeparam name="TOutDTO">DTO interface of your choice as a return type of GET calls - must derived from IModel interface.</typeparam>
        /// <typeparam name="TModel">Model implementation of your choice - must derived from Model class.</typeparam>
        /// <typeparam name="TID">Type of primary key such as type of int or Guid etc. </typeparam>
        /// <typeparam name="TService">Query Service implementation of your choice - must be inherited from Service class.</typeparam>
        /// <typeparam name="TDBContext">DBContext mplementation of your choice.</typeparam>
        /// <param name="services">Service collection instance which to add services to.</param>
        /// <param name="configuration">Instance of application configuration class.</param>
        /// <param name="dbContextOptions">DbContextOptions to use in creating DbContextOptionsBuilder.</param>
        public static void AddKeyedQueryModel<TOutDTO, TModel, TID, TService, TDBContext>(this IServiceCollection services, IConfiguration configuration, DbContextOptions? dbContextOptions = null)
            #region TYPE CONSTRAINTS
            where TModel : Model<TID, TModel>,
            //-:cnd:noEmit
#if (!MODEL_USEDTO)
            TOutDTO,
#endif
            //+:cnd:noEmit
            new()
            where TOutDTO : IModel
            where TService : QueryService<TOutDTO, TModel, TID, TDBContext>
            where TDBContext : DBContext
            where TID: struct
            #endregion
        {
            var type = typeof(TOutDTO);
            var modelType = typeof(TModel);
           
            modelType.GetOptions(configuration, out ServiceScope scope, out bool addController, out Action<DbContextOptionsBuilder> action, dbContextOptions);

            //-:cnd:noEmit
#if !MODEL_USEMYOWNCONTROLLER
            if (addController)
                QueryKeyedControllerTypes.Add(Tuple.Create(type, modelType, typeof(TID)));
#endif
            //+:cnd:noEmit

            services.AddDbContext<TDBContext>(action);
            switch (scope)
            {
                case ServiceScope.Scoped:
                default:
                    services.AddScoped<IQueryContract<TOutDTO, TModel, TID>, TService>();
                    break;
                case ServiceScope.Transient:
                    services.AddTransient<IQueryContract<TOutDTO, TModel, TID>, TService>();
                    break;
                case ServiceScope.Singleton:
                    services.AddSingleton<IQueryContract<TOutDTO, TModel, TID>, TService>();
                    break;
            }
        }

        /// <summary>
        /// Adds a new model to model query processing layer.
        /// Use this mehod if you are to provide your own implementation of query service class, otherwise 
        /// use another override of 'AddQueryModel' method.
        /// </summary>
        /// <typeparam name="TOutDTO">DTO interface of your choice as a return type of GET calls - must derived from IModel interface.</typeparam>
        /// <typeparam name="TModel">Model implementation of your choice - must derived from Model class.</typeparam>
        /// <param name="services">Service collection instance which to add services to.</param>
        /// <param name="configuration">Instance of application configuration class.</param>
        /// <param name="dbContextOptions">DbContextOptions to use in creating DbContextOptionsBuilder.</param>
        public static void AddKeyedQueryModel<TOutDTO, TModel, TID>(this IServiceCollection services, IConfiguration configuration, DbContextOptions? dbContextOptions = null)
            #region TYPE CONSTRAINTS
            where TModel : Model<TID, TModel>,
            //-:cnd:noEmit
#if (!MODEL_USEDTO)
            TOutDTO,
#endif
            //+:cnd:noEmit
            new()
            where TOutDTO : IModel
            where TID : struct
            #endregion
            => AddKeyedQueryModel<TOutDTO, TModel, TID, QueryService<TOutDTO, TModel, TID, DBContext>, DBContext>(services, configuration, dbContextOptions);

        /// <summary>
        /// Adds a new model to model query processing layer.
        /// Use this mehod if you are to provide your own implementation of query service class, otherwise 
        /// use another override of 'AddQueryModel' method.
        /// </summary>
        /// <typeparam name="TOutDTO">DTO interface of your choice as a return type of GET calls - must derived from IModel interface.</typeparam>
        /// <typeparam name="TModel">Model implementation of your choice - must derived from Model class.</typeparam>
        /// <param name="services">Service collection instance which to add services to.</param>
        /// <param name="configuration">Instance of application configuration class.</param>
        /// <param name="dbContextOptions">DbContextOptions to use in creating DbContextOptionsBuilder.</param>
        public static void AddKeyedQueryModel<TOutDTO, TModel>(this IServiceCollection services, IConfiguration configuration, DbContextOptions? dbContextOptions = null)
            #region TYPE CONSTRAINTS
            where TModel : Model<int, TModel>,
            //-:cnd:noEmit
#if (!MODEL_USEDTO)
            TOutDTO,
#endif
            //+:cnd:noEmit
            new()
            where TOutDTO : IModel
            #endregion
            => AddKeyedQueryModel<TOutDTO, TModel, int, QueryService<TOutDTO, TModel, int, DBContext>, DBContext>(services, configuration, dbContextOptions);

        /// <summary>
        /// Adds a new model to model query processing layer.
        /// Use this mehod if you are to provide your own implementation of query service class, otherwise 
        /// use another override of 'AddQueryModel' method.
        /// </summary>
        /// <typeparam name="TModel">Model implementation of your choice - must derived from Model class.</typeparam>
        /// <param name="services">Service collection instance which to add services to.</param>
        /// <param name="configuration">Instance of application configuration class.</param>
        /// <param name="dbContextOptions">DbContextOptions to use in creating DbContextOptionsBuilder.</param>
        public static void AddKeyedQueryModel<TModel>(this IServiceCollection services, IConfiguration configuration, DbContextOptions? dbContextOptions = null)
            #region TYPE CONSTRAINTS
            where TModel : Model<int, TModel>,
            new()
            #endregion
            =>
            AddKeyedQueryModel<TModel, TModel, int, QueryService<TModel, TModel, int, DBContext>, DBContext>(services, configuration, dbContextOptions);
#endif
        //+:cnd:noEmit
        #endregion

        #region GET MODEL NAME
        //-:cnd:noEmit
#if !MODEL_USEMYOWNCONTROLLER
        static string GetModelName(this Type type)
        {
            //Let's check if a specific name for the associated controller is provided as an attribute of TModel.
            //If provided, no do further, assign name and exit.
            var nameAttribute = type.GetCustomAttribute<ModelAttribute>();
            if (!string.IsNullOrEmpty(nameAttribute?.Name))
                return nameAttribute.Name;

            var name = type.Name;

            //If TModel is an interface remove 'I' suffix from the name.
            //if (type.IsInterface && name.Length > 1 && char.IsUpper(name[1]) && (name[0] == 'I' || name[0] == 'i'))
            //    name = name.Substring(1);

            if (name.Length == 1 || !type.IsGenericType)
            {
                return name;
            }
            //If TModel is genereic type, remove part representing generic name for example: name of Any<T> resolves in Any `1 as name of the type.
            var idx = name.IndexOf('`');
            if (idx != -1)
                name = name.Substring(0, idx);
            return name;
        }
#endif
        //+:cnd:noEmit
        #endregion

        #region GET OPTIONS
        static void GetOptions(this Type modelType, IConfiguration configuration, out ServiceScope scope, out bool addController, 
            out Action<DbContextOptionsBuilder> action, DbContextOptions? dbContextOptions = null)
        {
            scope = ServiceScope.Scoped;
            addController = true;
            var modelAttribute = modelType.GetCustomAttribute<ModelAttribute>();
            ConnectionKey connectionKey = ConnectionKey.InMemory;
            string? dbName = null;

            if (modelAttribute != null)
            {
                scope = modelAttribute.Scope;
                addController = modelAttribute.AutoController;
            }
            var connectAttribute = modelType.GetCustomAttribute<DBConnectAttribute>();
            if (connectAttribute != null)
            {
                connectionKey = connectAttribute.ConnectionKey;
                dbName = connectAttribute.Database;
            }

            DbContextOptionsBuilder dbOptionBuilder;
            if (dbContextOptions == null)
                dbOptionBuilder = new DbContextOptionsBuilder();
            else
                dbOptionBuilder = new DbContextOptionsBuilder(dbContextOptions);

            string connectionString = configuration?.GetSection("ConnectionStrings")[connectionKey.ToString()] ?? "";
            action = (opt) =>
            {
                if (string.IsNullOrEmpty(connectionString) || string.IsNullOrEmpty(dbName))
                {
                    connectionString = configuration?.GetSection("ConnectionStrings")["InMemory"] ?? "Data Source=.\\Data\\SQlLiteDatabase.db";
                    _ = opt.UseInMemoryDatabase(connectionString);
                    return;
                }

                connectionString = string.Format(connectionString, dbName);
                switch (connectionKey)
                {
                    case ConnectionKey.InMemory:
                    default:
                        _ = opt.UseInMemoryDatabase(connectionString);
                        break;
                        //-:cnd:noEmit
#if MODEL_CONNECTSQLSERVER
                    case ConnectionKey.SQLServer:
                        _= opt.UseSqlServer(connectionString);
                        break;
#elif MODEL_CONNECTPOSTGRESQL
                    case ConnectionKey.PostgreSQL:
                       _= opt.UseNpgsql(connectionString);
                        break;
#elif MODEL_CONNECTMYSQL
                        case ConnectionKey.MySQL:
                       _= opt.UseMySQL(connectionString);
                        break;
#endif
                        //+:cnd:noEmit
                }
            };
        }
        #endregion

        #region DYNAMIC CONTROLLER CONFIGURATION CLASSES
        //-:cnd:noEmit
#if !MODEL_USEMYOWNCONTROLLER
        class ControllerRouteConvention : IControllerModelConvention
        {
            void IControllerModelConvention.Apply(ControllerModel controller)
            {
                if (controller.ControllerType.IsGenericType)
                {
                    string finalName;
                    var controllerType = controller.ControllerType;
                    var type = controllerType.GenericTypeArguments[1];
                    var modelAttr = type.GetCustomAttribute<ModelAttribute>();
                    finalName = type.GetModelName();
                    if (!string.IsNullOrEmpty(modelAttr?.Name) && !ControllerNames.Contains(modelAttr.Name))
                        finalName = modelAttr.Name;

                    //bool IsQueryController = controllerType.GetGenericTypeDefinition() == (typeof(QueryController<,>));
                    int i = -1;
                    int count = controllerType.GenericTypeArguments.Length;

                    while (ControllerNames.Contains(finalName))
                    {
                        ++i;
                        if (i == 2)
                            continue;
                        if (i >= count)
                            break;
                        type = controllerType.GenericTypeArguments[i];
                        modelAttr = type.GetCustomAttribute<ModelAttribute>();
                        finalName = type.GetModelName();
                        if (!string.IsNullOrEmpty(modelAttr?.Name) && !ControllerNames.Contains(modelAttr.Name))
                            finalName = modelAttr.Name;
                    }
                    controller.ControllerName = finalName;
                    ControllerNames.Add(finalName);
                }
            }
        }
        class ControllerFeatureProvider : IApplicationFeatureProvider<ControllerFeature>
        {
            void IApplicationFeatureProvider<ControllerFeature>.PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature)
            {
                Type controllerType;

                if (ControllerTypes.Count == 0)
                    goto HANDLE_QUERYCONTROLLER;

                controllerType = typeof(Controller<,,,>);
                foreach (var typeParam in ControllerTypes)
                {
                    feature.Controllers.Add(
                        controllerType.MakeGenericType(typeParam.Item1, typeParam.Item2, typeParam.Item3, typeParam.Item4).GetTypeInfo()
                    );
                }
                ControllerTypes.Clear();

                HANDLE_QUERYCONTROLLER:
                //-:cnd:noEmit
#if !MODEL_NONQUERYABLE
                if (QueryControllerTypes.Count == 0)
                    goto HANDLE_QUERYKEYEDCONTROLLER;

                controllerType = typeof(QueryController<,>);
                foreach (var typeParam in QueryControllerTypes)
                {
                    feature.Controllers.Add(
                        controllerType.MakeGenericType(typeParam.Item1, typeParam.Item2).GetTypeInfo()
                    );
                }
                QueryControllerTypes.Clear();
                HANDLE_QUERYKEYEDCONTROLLER:

                if (QueryKeyedControllerTypes.Count == 0)
                    goto EXIT;

                controllerType = typeof(QueryController<,,>);
                foreach (var typeParam in QueryKeyedControllerTypes)
                {
                    feature.Controllers.Add(
                        controllerType.MakeGenericType(typeParam.Item1, typeParam.Item2, typeParam.Item3).GetTypeInfo()
                    );
                }
                QueryKeyedControllerTypes.Clear ();
#endif
                //+:cnd:noEmit
                EXIT:
                return;
            }
        }
#endif
        //+:cnd:noEmit
        #endregion
    }
    #endregion
}
//-:cnd:noEmit
#endif
//+:cnd:noEmit

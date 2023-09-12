﻿/* Licensed under the MIT/X11 license.
* This notice may not be removed from any source distribution.
 Author: Mukesh Adhvaryu.
*/
//-:cnd:noEmit
#if !TDD
//+:cnd:noEmit
using System.Reflection;

using MicroService.API.Controllers;
using MicroService.API.Data;
using MicroService.Common.Attributes;
using MicroService.Common.Models;
using MicroService.Common.Services;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MicroService.API.Configurations
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
        static List<Tuple<Type, Type, Type>> ControllerTypes = new List<Tuple<Type, Type, Type>>(3);
#endif
        //+:cnd:noEmit
        #endregion

        #region CONFIGURE MVC
        /// <summary>
        /// Creates MVCBuilder from the given WebApplication builder.
        /// </summary>
        /// <param name="builder">Instance of web application builder which to build mvc builder for.</param>
        /// <returns>IMvcBuilder instance.</returns>
        public static IMvcBuilder AddMVC(this IServiceCollection services)
        {
            var mvcBuilder = MvcServiceCollectionExtensions.AddMvc(services);
            //-:cnd:noEmit
#if !MODEL_USEMYOWNCONTROLLER
            /*
                 * This will register a controller dynamically based on TModelInterface type.
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
            return mvcBuilder;
        }

        /// <summary>
        /// Creates MVCBuilder from the given WebApplication builder.
        /// </summary>
        /// <param name="builder">Instance of web application builder which to build mvc builder for.</param>
        /// <param name="action">Mvc Options action to be executed on mvc builder.</param>
        /// <returns>IMvcBuilder instance.</returns>
        public static IMvcBuilder AddMVC(this IServiceCollection services, Action<MvcOptions> action)
        {
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
             * This will register a controller dynamically based on TModelInterface type.
             * So, now we do not need to create controller class in an actual microservice project.
             * Inspired from the article: https://www.strathweb.com/2018/04/generic-and-dynamically-generated-controllers-in-asp-net-core-mvc/
            */
            OptionsServiceCollectionExtensions.Configure(mvcBuilder.Services, nativeAction);
            mvcBuilder.ConfigureApplicationPartManager(m => m.FeatureProviders.Add(new ControllerFeatureProvider()));
#endif
            //+:cnd:noEmit

            return mvcBuilder;
        }
        #endregion

        #region ADD MODEL
        /// <summary>
        /// Adds a new model to model processing layer.
        /// Use this mehod if you are to provide your own implementation of service class, otherwise 
        /// use another override of 'AddModel' method.
        /// </summary>
        /// <typeparam name="TModelInterface">Model interface of your choice - must derived from IModel interface.</typeparam>
        /// <typeparam name="TModel">Model implementation of your choice - must derived from Model class.</typeparam>
        /// <typeparam name="TIDType">Type of primary key such as type of int or Guid etc. </typeparam>
        /// <typeparam name="TService">Service implementation of your choice - must be inherited from Service class.</typeparam>
        /// <typeparam name="TDBContext">DBContext<typeparamref name="TModel"/> of your choice.</typeparam>
        /// <param name="services">Service collection instance which to add services to.</param>
        /// <param name="configuration">Instance of application configuration class.</param>
        /// <param name="dbOptionBuilder">Acion providing DbContextOptionsBuilder.</param>
        public static void AddModel<TModelInterface, TModel, TIDType, TService, TDBContext>(this IServiceCollection services, IConfiguration configuration, Action<DbContextOptionsBuilder>? dbOptionBuilder = null)
            #region TYPE CONSTRAINTS
            where TModel : Model<TIDType>,
            //-:cnd:noEmit
#if (!MODEL_USEDTO)
            TModelInterface,
#endif
            //+:cnd:noEmit
            new()
            where TModelInterface : IModel
            where TService : class, IService<TModelInterface, TModel, TIDType>
            where TIDType : struct
            where TDBContext : DBContext<TModel, TIDType>
            #endregion
        {
            var type = typeof(TModelInterface);
            ServiceScope scope = ServiceScope.Scoped;
            bool addController = true;
            var modelAttribute = type.GetCustomAttribute<ModelAttribute>();

            if (modelAttribute != null)
            {
                scope = modelAttribute.Scope;
                addController = modelAttribute.AutoController;
            }

            //-:cnd:noEmit
#if !MODEL_USEMYOWNCONTROLLER
            if (addController)
                ControllerTypes.Add(Tuple.Create(type, typeof(TModel), typeof(TIDType)));
#endif
            //+:cnd:noEmit
            var dummyModel = (IExModel)new TModel();

            if (dbOptionBuilder == null)
            {
                string connectionString = configuration?.GetSection("ConnectionStrings")["InMemory"] ?? "";
                dbOptionBuilder = (opt) =>
                {
                    _ = opt.UseInMemoryDatabase(connectionString);
                };
            }


            services.AddDbContext<TDBContext>(dbOptionBuilder);
            switch (scope)
            {
                case ServiceScope.Scoped:
                default:
                    services.AddScoped<IService<TModelInterface, TModel, TIDType>, TService>();
                    break;
                case ServiceScope.Transient:
                    services.AddTransient<IService<TModelInterface, TModel, TIDType>, TService>();
                    break;
                case ServiceScope.Singleton:
                    services.AddSingleton<IService<TModelInterface, TModel, TIDType>, TService>();
                    break;
            }
        }

        /// <summary>
        /// Adds a new model to model processing layer.
        /// Uses default Service repository class.
        /// </summary>
        /// <typeparam name="TModelInterface">Model interface of your choice - must derived from IModel interface.</typeparam>
        /// <typeparam name="TModel">Model implementation of your choice - must derived from Model class.</typeparam>
        /// <typeparam name="TIDType">Type of primary key such as type of int or Guid etc. </typeparam>
        /// <param name="services">Service collection instance which to add services to.</param>
        /// <param name="configuration">Instance of application configuration class.</param>
        /// <param name="dbOptionBuilder">Acion providing DbContextOptionsBuilder.</param>
        public static void AddModel<TModelInterface, TModel, TIDType>(this IServiceCollection services, IConfiguration configuration, Action<DbContextOptionsBuilder>? dbOptionBuilder = null)
            #region TYPE CONSTRAINTS
            where TModel : Model<TIDType>,
            //-:cnd:noEmit
#if (!MODEL_USEDTO)
            TModelInterface,
#endif
            //+:cnd:noEmit
            new()
            where TModelInterface : IModel
            where TIDType : struct
            #endregion
            => AddModel<TModelInterface, TModel, TIDType, Service<TModelInterface, TModel, TIDType, DBContext<TModel, TIDType>>, DBContext<TModel, TIDType>>(services, configuration, dbOptionBuilder);

        /// <summary>
        /// Adds a new model to model processing layer.
        /// Uses default Service repository class.
        /// Uses default DBContext of TModel class.
        /// </summary>
        /// <typeparam name="TModelInterface">Model interface of your choice - must derived from IModel interface.</typeparam>
        /// <typeparam name="TModel">Model implementation of your choice - must derived from Model with int id type.</typeparam>
        /// <param name="services">Service collection instance which to add services to.</param>
        /// <param name="configuration">Instance of application configuration class.</param>
        /// <param name="dbOptionBuilder">Acion providing DbContextOptionsBuilder.</param>
        public static void AddModel<TModelInterface, TModel>(this IServiceCollection services, IConfiguration configuration, Action<DbContextOptionsBuilder>? dbOptionBuilder = null)
            #region TYPE CONSTRAINTS
            where TModel : Model<int>,
            //-:cnd:noEmit
#if (!MODEL_USEDTO)
            TModelInterface,
#endif
            //+:cnd:noEmit
            new()
            where TModelInterface : IModel
            #endregion
            => AddModel<TModelInterface, TModel, int, Service<TModelInterface, TModel, int, DBContext<TModel, int>>, DBContext<TModel, int>>(services, configuration, dbOptionBuilder);

        /// <summary>
        /// Adds a new model to model processing layer.
        /// Uses default Service repository class.
        /// Uses default DBContext of TModel class.
        /// Uses TModal as TModelInterface type argument.
        /// </summary>
        /// <typeparam name="TModelInterface">Model interface of your choice - must derived from IModel interface.</typeparam>
        /// <typeparam name="TModel">Model implementation of your choice - must derived from Model with int id type.</typeparam>
        /// <param name="services">Service collection instance which to add services to.</param>
        /// <param name="configuration">Instance of application configuration class.</param>
        /// <param name="dbOptionBuilder">Acion providing DbContextOptionsBuilder.</param>
        public static void AddModel<TModel>(this IServiceCollection services, IConfiguration configuration, Action<DbContextOptionsBuilder>? dbOptionBuilder = null)
            #region TYPE CONSTRAINTS
            where TModel : Model<int>,
            new()
            #endregion
            =>
            AddModel<TModel, TModel, int, Service<TModel, TModel, int, DBContext<TModel, int>>, DBContext<TModel, int>>(services, configuration, dbOptionBuilder);
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

        #region DYNAMIC CONTROLLER CONFIGURATION CLASSES
        //-:cnd:noEmit
#if !MODEL_USEMYOWNCONTROLLER
        class ControllerRouteConvention : IControllerModelConvention
        {
            public void Apply(ControllerModel controller)
            {
                if (controller.ControllerType.IsGenericType)
                {
                    var name = controller.ControllerType.GenericTypeArguments[0].GetModelName();
                    controller.ControllerName = name;
                }
            }
        }
        class ControllerFeatureProvider : IApplicationFeatureProvider<ControllerFeature>
        {
            public void PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature)
            {
                if (ControllerTypes.Count == 0)
                    return;

                var controllerType = typeof(Controller<,,>);
                foreach (var typeParam in ControllerTypes)
                {

                    feature.Controllers.Add(
                        controllerType.MakeGenericType(typeParam.Item1, typeParam.Item2, typeParam.Item3).GetTypeInfo()
                    );
                }
                ControllerTypes.Clear();
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
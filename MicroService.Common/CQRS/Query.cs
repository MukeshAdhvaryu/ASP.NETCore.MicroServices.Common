﻿//-:cnd:noEmit
#if !MODEL_NONREADABLE || !MODEL_NONQUERYABLE
//+:cnd:noEmit
using System;
using System.Reflection;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;

using MicroService.Common.Attributes;
using MicroService.Common.Exceptions;
using MicroService.Common.Interfaces;
using MicroService.Common.Models;
//-:cnd:noEmit
#if MODEL_SEARCHABLE
using MicroService.Common.Parameters;
#endif
//+:cnd:noEmit

namespace MicroService.Common.CQRS
{
    #region Query<TOutDTO, TModel>
    /// <summary>
    /// This class represents an object that allows reading a single model or multiple models.
    /// </summary>
    /// <typeparam name="TOutDTO">Interface representing the model.</typeparam>
    /// <typeparam name="TModel">Model of your choice.</typeparam>
    /// <typeparam name="TID">Primary key type of the model.</typeparam>
    public abstract class Query<TOutDTO, TModel> : IQuery<TOutDTO, TModel>
        #region TYPE CONSTRAINTS
        where TModel : class, ISelfModel<TModel>, new()
        where TOutDTO : IModel, new()
        #endregion
    {
        #region VARIABLES
        readonly static IExModel DummyModel = (IExModel)new TModel();
        //-:cnd:noEmit
#if MODEL_USEDTO
        static readonly Type DTOType = typeof(TOutDTO);
        static readonly bool NeedToUseDTO = !DTOType.IsAssignableFrom(typeof(TModel));
#endif
        //+:cnd:noEmit
        #endregion

        #region GET INITIAL DATA
        protected IEnumerable<TModel>? GetInitialData()
        {
            IEnumerable<TModel>? items = null;
            bool provideSeedData = false;
            var attribute = typeof(TModel).GetCustomAttribute<DBConnectAttribute>();
            if (attribute != null)
                provideSeedData = attribute.ProvideSeedData;
            if (provideSeedData && GetModelCount() == 0)
            {
                var model = (IExModel)new TModel();
                items = model.GetInitialData()?.OfType<TModel>();
            }
            return items;
        }
        #endregion

        #region GET ITEMS
        protected abstract IEnumerable<TModel> GetItems();
        #endregion

        #region GET ALL (Optional: count)
        //-:cnd:noEmit
#if !MODEL_NONREADABLE || !MODEL_NONQUERYABLE
        /// <summary>
        /// Gets all models contained in this object.
        /// The count of models returned can be limited by the count parameter.
        /// If the parameter value is zero, then all models are returned.
        /// </summary>
        /// <param name="count">Number to limit the number of models returned.</param>
        /// <returns>IEnumerable of Instance of TModelImplementations represented through TModelInterfaces.</returns>
        /// <exception cref="Exception"></exception>
        public virtual Task<IEnumerable<TOutDTO>?> GetAll(int count = 0)
        {
            if (GetModelCount() == 0)
                throw DummyModel.GetModelException(ExceptionType.NoModelsFound);

            if (count < 0)
                throw DummyModel.GetModelException(ExceptionType.NegativeFetchCount, count.ToString());
            if (count == 0)
                return Task.FromResult(ToDTO(GetItems()));
            return Task.FromResult(ToDTO(GetItems().Take(count)));
        }
#endif
        //+:cnd:noEmit
        #endregion

        #region GET ALL (start, count)
        //-:cnd:noEmit
#if !MODEL_NONREADABLE || !MODEL_NONQUERYABLE
        /// <summary>
        /// Gets all models contained in this object picking from the index specified up to a count determined by count.
        /// The count of models returned can be limited by the count parameter.
        /// If the parameter value is zero, then all models are returned.
        /// </summary>
        /// <param name="startIndex">Start index which to start picking records from.</param>
        /// <param name="count">Number to limit the number of models returned.</param>
        /// <returns>IEnumerable of models.</returns>
        public virtual Task<IEnumerable<TOutDTO>?> GetAll(int startIndex, int count)
        {
            if (GetModelCount() == 0)
                throw DummyModel.GetModelException(ExceptionType.NoModelsFound);

            if (count < 0)
                throw DummyModel.GetModelException(ExceptionType.NegativeFetchCount, count.ToString());

            --startIndex;

            if (startIndex < 0)
                startIndex = 0;

            if (startIndex == 0 && count == 0)
                return Task.FromResult(ToDTO(GetItems()));

            var items = GetItems();
            return Task.FromResult(ToDTO(items.Skip(startIndex).Take(count)));
        }
#endif
        //+:cnd:noEmit
        #endregion

        #region FIND (parameters, conditionJoin = 0)
        //-:cnd:noEmit
#if (!MODEL_NONREADABLE || !MODEL_NONQUERYABLE) && MODEL_SEARCHABLE
        public virtual Task<TOutDTO?> Find<T>(AndOr conditionJoin, params T?[]? parameters)
            where T : ISearchParameter
        {
            if (parameters == null || parameters.Length == 0)
                throw DummyModel.GetModelException(ExceptionType.NoParametersSupplied);

            if (GetModelCount() == 0)
                throw DummyModel.GetModelException(ExceptionType.NoModelsFound);


            var items = GetItems();

            Predicate<TModel> predicate;
            
            if(parameters.Length == 1)
            {
                var parameter = parameters[0];

                if (parameter == null)
                    throw DummyModel.GetModelException(ExceptionType.NoParameterSupplied);

                if(string.IsNullOrEmpty(parameter.Name))
                    throw DummyModel.GetModelException(ExceptionType.NoParameterSupplied, "Missing Name!");

                predicate = (m) =>
                {
                    if (!DummyModel.Parse(parameter.Name, parameter.Value, out object? value, false, parameter.Criteria))
                        return false;

                    var currentValue = m[parameter.Name];
                    if (!Operations.Compare(currentValue, parameter.Criteria, value))
                        return false;

                    return true;
                };

                goto EXIT;
            }

            switch (conditionJoin)
            {
                case AndOr.AND:
                default:
                    predicate = (m) =>
                    {
                        foreach (var parameter in parameters)
                        {
                            if (parameter == null || string.IsNullOrEmpty(parameter.Name))
                                continue;
                            if (!DummyModel.Parse(parameter.Name, parameter.Value, out object? value, false, parameter.Criteria))
                                return false;
                            
                            var currentValue = m[parameter.Name];
                            if (!Operations.Compare(currentValue, parameter.Criteria, value))
                                return false;
                        }
                        return true;
                    };
                    break;
                case AndOr.OR:
                    predicate = (m) =>
                    {
                        bool result = false;
                        foreach (var parameter in parameters)
                        {
                            if (parameter == null || string.IsNullOrEmpty(parameter.Name))
                                continue;
                            if (!DummyModel.Parse(parameter.Name, parameter.Value, out object? value, false, parameter.Criteria))
                                return false;

                            var currentValue = m[parameter.Name];
                            if (Operations.Compare(currentValue, parameter.Criteria, value))
                                return true; 
                        }
                        return result;
                    };
                    break;
            }

            EXIT:
            return Task.FromResult(ToDTO(items.FirstOrDefault((m) => predicate(m))));
        }
#endif
        //+:cnd:noEmit
        #endregion

        #region FIND ALL (parameters)
        //-:cnd:noEmit
#if (!MODEL_NONREADABLE || !MODEL_NONQUERYABLE) && MODEL_SEARCHABLE
        public virtual Task<IEnumerable<TOutDTO>?> FindAll<T>(AndOr conditionJoin, params T?[]? parameters)
            where T : ISearchParameter
        {
            if (parameters == null)
                throw DummyModel.GetModelException(ExceptionType.NoParameterSupplied);
           
            if (GetModelCount() == 0)
                throw DummyModel.GetModelException(ExceptionType.NoModelsFound);

            Predicate<TModel> predicate;

            if (parameters.Length == 1)
            {
                var parameter = parameters[0];

                if (parameter == null)
                    throw DummyModel.GetModelException(ExceptionType.NoParameterSupplied);

                if (string.IsNullOrEmpty(parameter.Name))
                    throw DummyModel.GetModelException(ExceptionType.NoParameterSupplied, "Missing Name!");

                predicate = (m) =>
                {
                    if (!DummyModel.Parse(parameter.Name, parameter.Value, out object? value, false, parameter.Criteria))
                        return false;

                    var currentValue = m[parameter.Name];
                    if (!Operations.Compare(currentValue, parameter.Criteria, value))
                        return false;

                    return true;
                };

                goto EXIT;
            }

            switch (conditionJoin)
            {
                case AndOr.AND:
                default:
                    predicate = (m) =>
                    {
                        foreach (var parameter in parameters)
                        {
                            if (parameter == null || string.IsNullOrEmpty(parameter.Name))
                                continue;
                            if (!DummyModel.Parse(parameter.Name, parameter.Value, out object? value, false, parameter.Criteria))
                                return false;

                            var currentValue = m[parameter.Name];
                            if (!Operations.Compare(currentValue, parameter.Criteria, value))
                                return false;
                        }
                        return true;
                    };
                    break;
                case AndOr.OR:
                    predicate = (m) =>
                    {
                        bool result = false;
                        foreach (var parameter in parameters)
                        {
                            if (parameter == null || string.IsNullOrEmpty(parameter.Name))
                                continue;
                            if (!DummyModel.Parse(parameter.Name, parameter.Value, out object? value, false, parameter.Criteria))
                                return false;

                            var currentValue = m[parameter.Name];
                            if (Operations.Compare(currentValue, parameter.Criteria, value))
                                return true;
                        }
                        return result;
                    };
                    break;
            }

            EXIT:
            return Task.FromResult(ToDTO(GetItems().Where((m) => predicate(m))));
        }
#endif
        //+:cnd:noEmit
        #endregion

        #region GET FIRST MODEL
        IModel? IFirstModel.GetFirstModel()
        {
            if(GetModelCount() == 0)
                return null;
          return  GetItems().FirstOrDefault();
        }
        TModel? IFirstModel<TModel>.GetFirstModel()
        {
            if (GetModelCount() == 0)
                return null;
            return GetItems().FirstOrDefault();
        }
        #endregion

        #region GET MODEL COUNT
        public abstract int GetModelCount();
        #endregion

        #region GET MODEL EXCEPTION
        /// <summary>
        /// Supplies an appropriate exception for a failure in a specified method.
        /// </summary>
        /// <param name="exceptionType">Type of exception to get.</param>
        /// <param name="additionalInfo">Additional information to aid the task of exception supply.</param>
        /// <param name="innerException">Inner exception which is already thrown.</param>
        /// <returns>Instance of SpecialException class.</returns>
        protected ModelException GetModelException(ExceptionType exceptionType, string? additionalInfo = null, Exception? innerException = null) =>
            DummyModel.GetModelException(exceptionType, additionalInfo, innerException);
        #endregion

        #region TO DTO
        /// <summary>
        /// Converts model to an apporiate object of TOutDTO type.
        /// </summary>
        /// <param name="model">Model to convert.</param>
        /// <returns>Converted model ito an apporiate object of TOutDTO type.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected virtual TOutDTO? ToDTO(TModel? model)
        {
            if (model == null)
                return default(TOutDTO);
            //-:cnd:noEmit
#if (MODEL_USEDTO)
            if (NeedToUseDTO)
            {
                var result = ((IExModel)model).ToDTO(DTOType);
                if (result == null)
                    return default(TOutDTO);
                return (TOutDTO?)result;
            }
#endif
            //+:cnd:noEmit
            return (TOutDTO?)(object)model;
        }

        /// <summary>
        /// Converts models to an apporiate objects of TOutDTO type.
        /// </summary>
        /// <param name="model">Models to convert.</param>
        /// <returns>Converted models to an apporiate objects of TOutDTO type</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected virtual IEnumerable<TOutDTO>? ToDTO(IEnumerable<TModel>? models)
        {
            if (models == null)
                return Enumerable.Empty<TOutDTO>();
            //-:cnd:noEmit
#if (MODEL_USEDTO)
            if (NeedToUseDTO)
                return (IEnumerable<TOutDTO>?)models.Select(m => ToDTO(m));
#endif
            //+:cnd:noEmit

            return (IEnumerable<TOutDTO>?)models;
        }
        #endregion
    }
    #endregion
}
//-:cnd:noEmit
#endif
//+:cnd:noEmit

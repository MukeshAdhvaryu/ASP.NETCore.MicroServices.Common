/* Licensed under the MIT/X11 license.
* This notice may not be removed from any source distribution.
 Author: Mukesh Adhvaryu.
*/
//-:cnd:noEmit
#if (!MODEL_NONREADABLE || !MODEL_NONQUERYABLE)
//+:cnd:noEmit

using System.Collections;
using System.Reflection;

using MicroService.Common.Attributes;
using MicroService.Common.CQRS;
using MicroService.Common.Interfaces;
using MicroService.Common.Models;
using MicroService.Common.Parameters;

namespace MicroService.Common.Sets
{
    #region Query<TModel, TItems>
    class QuerySet<TModel, TItems> : IExQuery<TModel>, IEnumerable<TModel>
        #region TYPE CONSTRAINTS
        where TModel : class, ISelfModel<TModel>, new()
        where TItems : IEnumerable<TModel>
        #endregion
    {
        #region VARIABLES
        protected readonly TItems Items;
        #endregion

        #region CONSTRUCTORS
        public QuerySet(TItems models, Action<IEnumerable<TModel>>? addRange = null) :
            this(models, true, addRange)
        {
        }
        public QuerySet(TItems models, bool initializeData = true, Action<IEnumerable<TModel>>? addRange = null)
        {
            Items = models;
            if (!initializeData)
                return;

            IEnumerable<TModel>? items = null;
            bool provideSeedData = false;
            var attribute = typeof(TModel).GetCustomAttribute<DBConnectAttribute>();
            if (attribute != null)
                provideSeedData = attribute.ProvideSeedData;
            if (provideSeedData && !Items.Any())
            {
                var model = (IExModel)new TModel();
                items = model.GetInitialData()?.OfType<TModel>();
            }
            if (items == null || addRange == null)
                return;
            try
            {
                addRange(items);
            }
            catch { }
        }
        #endregion

        #region GET ALL (count)
        /// <summary>
        /// Gets all models contained in this object.
        /// The count of models returned can be limited by the limitOfResult parameter.
        /// If the parameter value is zero, then all models are returned.
        /// </summary>
        /// <param name="count">Number to limit the number of models returned.</param>
        /// <returns>IEnumerable of models.</returns>
        public virtual Task<IEnumerable<TModel>?> GetAll(int count = 0)
        {
            if (count == 0)
                return Task.FromResult((IEnumerable<TModel>?)Items);

            if (count < 0)
                return Task.FromResult((IEnumerable<TModel>?)Enumerable.Empty<TModel>());

            return Task.FromResult(Items.Take(count) ?? (IEnumerable<TModel>?)Enumerable.Empty<TModel>());
        }
        #endregion

        #region GET ALL (startIndex, count)
        /// <summary>
        /// Gets all models contained in this object picking from the index specified up to a count determined by limitOfResult.
        /// The count of models returned can be limited by the limitOfResult parameter.
        /// If the parameter value is zero, then all models are returned.
        /// </summary>
        /// <param name="startIndex">Start index which to start picking records from.</param>
        /// <param name="count">Number to limit the number of models returned.</param>
        /// <returns>IEnumerable of models.</returns>
        public virtual Task<IEnumerable<TModel>?> GetAll(int startIndex, int count)
        {
            --startIndex;
            if (startIndex < 0)
                startIndex = 0;

            if (count == 0 && startIndex == 0)
                return Task.FromResult((IEnumerable<TModel>?)Items);

            if (count < 0)
                return Task.FromResult((IEnumerable<TModel>?)Enumerable.Empty<TModel>());

            return Task.FromResult((IEnumerable<TModel>?)Items.Skip(startIndex).Take(count));
        }
        #endregion

        #region FIND (parameters, conditionJoin)
        /// <summary>
        /// Finds a model based on given paramters.
        /// </summary>
        /// <param name="paramters">Parameters to be used to find the model.</param>
        /// <param name="conditionJoin">Option from AndOr enum to join search conditions.</param>
        /// <returns>Task with result of collection of type TModel.</returns>
        public virtual Task<TModel?> Find(IEnumerable<ISearchParameter>? parameters, AndOr conditionJoin)
        {
            if (parameters == null)
                return Task.FromResult(default(TModel?));

            Predicate<TModel> predicate;

            switch (conditionJoin)
            {
                case AndOr.AND:
                default:
                    predicate = (m) =>
                    {
                        IMatch match = m;

                        foreach (var key in parameters)
                        {
                            if (key == null)
                                continue;
                            if (!match.IsMatch(key))
                                return false;
                        }
                        return true;
                    };
                    break;
                case AndOr.OR:
                    predicate = (m) =>
                    {
                        IMatch match = m;
                        bool result = false;
                        foreach (var key in parameters)
                        {
                            if (key == null)
                                continue;
                            if (match.IsMatch(key))
                            {
                                result = true;
                            }
                        }
                        return result;
                    };
                    break;
            }
            return Task.FromResult(Items.FirstOrDefault((m) => predicate(m)));
        }
        #endregion

        #region FIND ALL (parameters, conditionJoin)
        /// <summary>
        /// Finds all models matched based on given parameters.
        /// </summary>
        /// <param name="parameters">Parameters to be used to find the model.</param>
        /// <returns>Task with result of collection of type TModel.</returns>
        /// <param name="conditionJoin">Option from AndOr enum to join search conditions.</param>
        /// <returns>Task with result of collection of type TModel.</returns>
        public virtual Task<IEnumerable<TModel>?> FindAll(IEnumerable<ISearchParameter>? parameters, AndOr conditionJoin)
        {
            if (parameters == null)
                return Task.FromResult((IEnumerable<TModel>?)Enumerable.Empty<TModel>());

            Predicate<TModel> predicate;

            switch (conditionJoin)
            {
                case AndOr.AND:
                default:
                    predicate = (m) =>
                    {
                        IMatch match = m;

                        foreach (var key in parameters)
                        {
                            if (key == null)
                                continue;
                            if (!match.IsMatch(key))
                                return false;
                        }
                        return true;
                    };
                    break;
                case AndOr.OR:
                    predicate = (m) =>
                    {
                        IMatch match = m;
                        bool result = false;
                        foreach (var key in parameters)
                        {
                            if (key == null)
                                continue;
                            if (match.IsMatch(key))
                            {
                                result = true;
                            }
                        }
                        return result;
                    };
                    break;
            }

            return Task.FromResult((IEnumerable<TModel>?)Items.Where((m) => predicate(m)));
        }
        #endregion

        #region FIND ALL (parameters, conditionJoin)
        /// <summary>
        /// Finds all models matched based on given parameter.
        /// </summary>
        /// <param name="parameter">Parameter to be used to find the model.</param>
        /// <returns>Task with result of type TModel.</returns>
        public virtual Task<IEnumerable<TModel>?> FindAll(ISearchParameter? parameter)
        {
            if (parameter == null)
                return Task.FromResult((IEnumerable<TModel>?)Enumerable.Empty<TModel>());
            Func<TModel, bool> func = (m) =>
            {
                return ((IMatch)m).IsMatch(parameter);
            };
            return Task.FromResult((IEnumerable<TModel>?)Items.Where(m => func(m)));
        }
        #endregion

        #region FIND (parameter)
        public Task<TModel?> Find(ISearchParameter? parameter)
        {
            if (parameter == null)
                return Task.FromResult(default(TModel?));

            return Task.FromResult((Items.FirstOrDefault(m => ((IMatch)m).IsMatch(parameter))));
        }
        #endregion

        #region GET FIRST MODEL
        TModel? IFirstModel<TModel>.GetFirstModel() =>
            Items.FirstOrDefault();
        IModel? IFirstModel.GetFirstModel() =>
            Items.FirstOrDefault();
        #endregion

        #region GET MODEL COUNT
        int IModelCount.GetModelCount() =>
            Items.Count();
        #endregion

        #region ENUMERATORS
        public IEnumerator<TModel> GetEnumerator()
        {
            foreach (var item in Items)
                yield return item;
        }
        IEnumerator IEnumerable.GetEnumerator() =>
            GetEnumerator();
        #endregion
    }
    #endregion
}
//-:cnd:noEmit
#endif
//+:cnd:noEmit

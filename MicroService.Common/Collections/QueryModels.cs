/* Licensed under the MIT/X11 license.
* This notice may not be removed from any source distribution.
 Author: Mukesh Adhvaryu.
*/

//-:cnd:noEmit
#if !MODEL_NONQUERYABLE
//+:cnd:noEmit
using System.Collections;

using MicroService.Common.CQRS;
using MicroService.Common.Interfaces;
using MicroService.Common.Models;
using MicroService.Common.Parameters;
using MicroService.Common.Sets;

namespace MicroService.Common.Collections
{
    #region ModelQuery<TModel>
    /// <summary>
    /// Represents an object which holds a collection of keyless models.
    /// </summary>
    /// <typeparam name="TModel">Type of keyless Model/></typeparam>
    public abstract class QueryModels<TModel, TItems> : ModelSet<TModel, TItems>, IExModelQuery<TModel>, IEnumerable<TModel>
        #region TYPE CONSTRAINTS
        where TModel : ISelfModel<TModel>, new()
        where TItems : IEnumerable<TModel>
        #endregion
    {
        #region CONSTRUCTORS
        public QueryModels(TItems models) :
            base(models)
        { }
        public QueryModels(TItems models, bool initializeData = true) :
            base(models, initializeData)
        { }
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
            if (startIndex > 0)
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

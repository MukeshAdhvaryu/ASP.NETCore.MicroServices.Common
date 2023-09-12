/* Licensed under the MIT/X11 license.
* This notice may not be removed from any source distribution.
 Author: Mukesh Adhvaryu.
*/

using MicroService.Common.Interfaces;

namespace MicroService.Common.Models
{
    /// <summary>
    /// Provides Update method to update a model class.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Binder<T> 
    {
        #region BIND MODEL
        protected Task<Tuple<bool, BindingResultMessage[]>>  Update<U>(IUpdatable<T> model, IReadOnlyList<U> valueList)
            where U: IValueStore<T>
        {
            var bindingResults = new BindingResultMessage[valueList.Count];
            int i = -1;
            string message;
            BindingResultStatus notification;
            bool failed = false;

            if (valueList.Count > 0)
            {
                foreach (var result in valueList)
                {
                    model.Update(result, out notification, out message);
                    bindingResults[++i] = new BindingResultMessage(message, notification);

                    switch (bindingResults[i].Notification)
                    {
                        case BindingResultStatus.Failure:
                        case BindingResultStatus.MissingRequiredValue:
                            failed = true;
                            break;
                        default:
                            break;
                    }
                }
            }
            return Task.FromResult(Tuple.Create(!failed, bindingResults));
        }
        #endregion
    }
}

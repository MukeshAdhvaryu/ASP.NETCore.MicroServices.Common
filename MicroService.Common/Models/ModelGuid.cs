﻿/* Licensed under the MIT/X11 license.
* This notice may not be removed from any source distribution.
Author: Mukesh Adhvaryu.
*/

namespace MicroService.Common.Models
{
    #region ModelGuid
    /// <summary>
    /// Represents a model with Guid type primary key.
    /// </summary>
    /// <typeparam name="TModel">Type of user-defined model</typeparam>
    public abstract class ModelGuid<TModel> : Model<Guid, TModel>
        where TModel : ModelGuid<TModel>
    {
        #region CONSTRUCTORS
        protected ModelGuid() :
            base(false)
        { }
        protected ModelGuid(bool generateNewID) :
            base(generateNewID)
        { }
        #endregion

        #region GET NEW ID
        protected override Guid GetNewID()
        {
            return Guid.NewGuid();
        }
        #endregion
    }
    #endregion
}

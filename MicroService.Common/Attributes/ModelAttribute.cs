/* Licensed under the MIT/X11 license.
* This notice may not be removed from any source distribution.
 Author: Mukesh Adhvaryu.
*/
using MicroService.Common.Services;

namespace MicroService.Common.Attributes
{
    #region MODEL ATTRIBUTE
    /// <summary>
    /// Provides various options for a model to represent itself through Web API. 
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = false)]
    public class ModelAttribute : Attribute
    {
        #region CONSTRUCTORS
        public ModelAttribute()
        {
            AutoController = true;
            ProvideSeedData = false;
        }
        public ModelAttribute(string name, bool autoController = true, bool provideSeedData = false)
        {
            Name = name;
            AutoController = autoController;
            ProvideSeedData = provideSeedData;
        }
        public ModelAttribute(string name, ServiceScope scope, bool autoController = true, bool provideSeedData = false) :
            this(name, autoController, provideSeedData)
        {
            Scope = scope;
        }
        public ModelAttribute(ServiceScope scope, bool autoController = true, bool provideSeedData = false) :
            this(null, scope, autoController, provideSeedData)
        { }
        #endregion

        #region PROPERTIES
        /// <summary>
        /// Gets or sets Name of a dynamic controller to be used for a model which uses this attribute.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Gets or sets a scope of service to be used for a model which uses this attribute.
        /// </summary>
        public ServiceScope Scope { get; set; }

        /// <summary>
        /// Gets or sets a flag to choose whether to use a dynamic controller or not for a model which uses this attribute.
        /// </summary>
        public bool AutoController { get; set; }

        /// <summary>
        /// Gets or sets a flag to indicate whether or not to provide an initial collection of models when model collection is empty for a model which uses this attribute.
        /// </summary>
        public bool ProvideSeedData { get; set; }
        #endregion
    }
    #endregion
}

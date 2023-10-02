/* Licensed under the MIT/X11 license.
* This notice may not be removed from any source distribution.
Author: Mukesh Adhvaryu.
*/

//-:cnd:noEmit
#if MODEL_ADDTEST && MODEL_USEDTO
//+:cnd:noEmit

using MicroService.Common.Models;

namespace MicroService.Common.Tests
{
    #region ITestModelDTO
    /// <summary>
    /// Represents a DTO for test model.
    /// </summary>
    public interface ITestModelDTO: IModel
    {
        /// <summary>
        /// Gets name of the model.
        /// </summary>
        string Name { get; }
    }
    #endregion

    #region TestModelDTO
    /// <summary>
    /// Represents a DTO for test model.
    /// </summary>
    public class TestModelDTO : ITestModelDTO
    {
        /// <summary>
        /// Gets name of the model.
        /// </summary>
        public string Name { get; }

        #region CONSTRUCTOR
        public TestModelDTO(TestModel model)
        {
            Name = model.Name;
        }
        #endregion
    }
    #endregion
}
//-:cnd:noEmit
#endif
//+:cnd:noEmit

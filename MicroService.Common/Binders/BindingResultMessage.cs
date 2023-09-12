/* Licensed under the MIT/X11 license.
* This notice may not be removed from any source distribution.
 Author: Mukesh Adhvaryu.
*/

namespace MicroService.Common.Models
{
    #region BINDING RESULT MESSAGE STRUCT
    public struct BindingResultMessage
    {
        public readonly string Message;

        //[JsonConverter(typeof(StringEnumConverter))]
        public readonly BindingResultStatus Notification;

        public BindingResultMessage(string message, BindingResultStatus notification = 0)
        {
            Message = message;
            Notification = notification;
        }

        public override string ToString()
        {
            return Notification + " " + Message;
        }
    }
    #endregion

}

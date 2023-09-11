/* Licensed under the MIT/X11 license.
* This notice may not be removed from any source distribution.
 Author: Mukesh Adhvaryu.
*/
//-:cnd:noEmit
#if MODEL_ADDTEST
//+:cnd:noEmit

using System.Reflection;


//-:cnd:noEmit
#if MODEL_USEXUNIT
using Xunit;
using Xunit.Sdk;
#elif MODEL_USENUNIT
using NUnit.Framework;
using NUnit.Framework.Internal;
#endif
//+:cnd:noEmit

namespace MicroService.Common.Tests.Attributes
{
    //-:cnd:noEmit
#if MODEL_USEXUNIT
    public class TestableAttribute : Attribute { }
    public class NoArgsAttribute : FactAttribute { }
    public class WithArgsAttribute : TheoryAttribute { }

    [DataDiscoverer("Xunit.Sdk.InlineDataDiscoverer", "xunit.core")]
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class ArgsAttribute : DataAttribute
    {
        readonly object[] data;

        public ArgsAttribute(params object[] _data)
        {
            data = _data;
        }
        public override IEnumerable<object[]> GetData(MethodInfo testMethod)
        {
            return new[] { data };
        }
    }


#elif MODEL_USENUNIT
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class TestableAttribute : TestFixtureAttribute { }

    public class NoArgsAttribute : TestAttribute { }
    public class WithArgsAttribute : TestAttribute { }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public class ArgsAttribute : TestCaseAttribute
    {
        public ArgsAttribute(params object?[]? arguments) :
            base(arguments)
        { }

        public ArgsAttribute(object? arg) :
            base(arg)
        { }

        public ArgsAttribute(object? arg1, object? arg2) :
            base(arg1, arg2)
        { }

        public ArgsAttribute(object? arg1, object? arg2, object? arg3) :
            base(arg1, arg2, arg3)
        { }
    }

#else
    public class TestableAttribute : TestClassAttribute { }
    public class NoArgsAttribute : TestMethodAttribute { }
    public class WithArgsAttribute : TestMethodAttribute { }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class ArgsAttribute : DataRowAttribute
    {
        public ArgsAttribute() :
            base()
        { }

        public ArgsAttribute(object data1) :
            base(data1)
        { }

        public ArgsAttribute(object data1, params object[] moreData) :
            base(data1, moreData)
        { }
    }
#endif
    //+:cnd:noEmit
}
//-:cnd:noEmit
#endif
//+:cnd:noEmit


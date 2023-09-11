/* Licensed under the MIT/X11 license.
* This notice may not be removed from any source distribution.
 Author: Mukesh Adhvaryu.
*/
//-:cnd:noEmit
#if MODEL_ADDTEST
//+:cnd:noEmit

//-:cnd:noEmit
#if MODEL_USEXUNIT
    using Assert = Xunit.Assert;
#elif MODEL_USENUNIT
    using Assert = NUnit.Framework.Assert;
#else
    using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;
#endif
//+:cnd:noEmit


using System.Collections;


namespace MicroService.Common.Tests
{
    public static class Verifier
    {
        public static void Equal<T>(T expected, T actual)
        {
            //-:cnd:noEmit
#if MODEL_USEXUNIT
            Equal(expected, actual);
            return;
#elif MODEL_USENUNIT
            Assert.AreEqual(expected, actual);
            return;
#else
            Assert.AreEqual(expected, actual);
            return;
#endif
            //+:cnd:noEmit
        }

        public static void Empty(IEnumerable expected)
        {
            //-:cnd:noEmit
#if MODEL_USEXUNIT
            if(expected == null)
            {
                Assert.True(expected == null);
                return;
            }
            Assert.Empty(expected);
            return;
#elif MODEL_USENUNIT
            Assert.IsEmpty(expected);
            return;
#else
            if (expected is ICollection)
            {
                Assert.IsTrue(((ICollection)expected).Count == 0);
                return;
            }
            Assert.IsNull(expected);
            return;
#endif
            //+:cnd:noEmit
        }

        public static void Empty(string expected)
        {
            //-:cnd:noEmit
#if MODEL_USEXUNIT
            if (expected == null)
            {
                Assert.True(expected == null);
                return;
            }
            Assert.Empty(expected);
            return;
#elif MODEL_USENUNIT
            Assert.IsEmpty(expected);
            return;
#else
            Assert.IsTrue(string.IsNullOrEmpty(expected));
            return;
#endif
            //+:cnd:noEmit
        }

        public static void IsNull<T>(T expected)
        {
            //-:cnd:noEmit
#if MODEL_USEXUNIT
            if (expected == null)
            {
                Assert.True(true == true);
                return;
            }
                Assert.True(false == true);
            return;
#elif MODEL_USENUNIT
            Assert.IsNull(expected);
            return;
#else
            Assert.IsTrue(Equals(expected, default(T)));
            return;
#endif
            //+:cnd:noEmit
        }
        public static void NotNull<T>(T expected)
        {
            //-:cnd:noEmit
#if MODEL_USEXUNIT
            if (expected == null)
            {
                Assert.True(true == true);
                return;
            }
            Assert.NotNull(expected);
            return;
#elif MODEL_USENUNIT
            Assert.NotNull(expected);
            return;
#else
            //Assert.NotNull(expected);
            return;
#endif
            //+:cnd:noEmit
        }

    }
}
//-:cnd:noEmit
#endif
//+:cnd:noEmit

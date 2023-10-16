//-:cnd:noEmit
#if MODEL_ADDTEST 
//+:cnd:noEmit

using MicroService.Common.Interfaces;
using MicroService.Common.Services;
using MicroService.Common.Tests;
using MicroService.Common.Tests.Attributes;
using MicroService.Common.Contexts;
/*
* Yo can choose your own model to test by changing the using statements given beolow:
* For example:
* using TOutDTO = UserDefined.Models.ISubject;
* using TID = System.Int32;
* using TModel = UserDefined.Models.Subject;
* OR
* using TOutDTO = UserDefined.DTOs.ISubjectDTO;
* using TID = System.Int32;
* using TModel = UserDefined.Models.Subject;
* 
* Please note that TModel must be a concrete class deriving from the base Model class.
*/
//-:cnd:noEmit
#if !MODEL_USEDTO
using TOutDTO = MicroService.Common.Tests.TestModel;
using TID = System.Int32;
using TModel = MicroService.Common.Tests.TestModel;
#else
using TOutDTO = MicroService.Common.Tests.ITestModelDTO;
using TID = System.Int32;
using TModel = MicroService.Common.Tests.TestModel;
#endif
//+:cnd:noEmit

namespace UserDefined.Tests
{
    [Testable]
    public class ServiceTest : ServiceTest<TOutDTO, TModel, TID>
    {
        protected override IContract<TOutDTO, TModel, TID> CreateService()
        {
            return new Service<TOutDTO, TModel, TID, ModelContext>(new ModelContext());
        }
    }
}
//-:cnd:noEmit
#endif
//+:cnd:noEmit

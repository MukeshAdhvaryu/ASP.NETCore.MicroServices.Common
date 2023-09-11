//-:cnd:noEmit
#if MODEL_ADDTEST && !TDD
//+:cnd:noEmit

using MicroService.Common.Tests;
using MicroService.Common.Tests.Attributes;
using MicroService.Common.Interfaces;
using MicroService.Common.Services;
using MicroService.Common.Web.API;

/*
 * Yo can choose your own model to test by changing the using statements given beolow:
 * For example:
 * using TModelDTO = UserDefined.Models.ISubject;
 * using TID = System.Int32;
 * using TModel = UserDefined.Models.Subject;
 * OR
 * using TModelDTO = UserDefined.DTOs.ISubjectDTO;
 * using TID = System.Int32;
 * using TModel = UserDefined.Models.Subject;
 * 
 * Please note that TModel must be a concrete class deriving from the base Model class.
*/

//-:cnd:noEmit
#if !MODEL_USEDTO
using TModelDTO = MicroService.Common.Tests.TestModel;
using TID = System.Int32;
using TModel = MicroService.Common.Tests.TestModel;
#else
using TModelDTO = MicroService.Common.Tests.ITestModelDTO;
using TID = System.Int32;
using TModel = MicroService.Common.Tests.TestModel;
#endif
//+:cnd:noEmit


namespace UserDefined.Tests
{
    [Testable]
    public class StandardTest: TestStandard<TModelDTO, TModel, TID>
    {
        protected override IContract<TModelDTO, TModel, TID> CreateContract(IService<TModelDTO, TModel, TID> service)
        {
            return new Controller<TModelDTO, TModel, TID>(service);
        }
    }
}
//-:cnd:noEmit
#endif
//+:cnd:noEmit

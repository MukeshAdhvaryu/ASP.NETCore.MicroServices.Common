//-:cnd:noEmit
#if MODEL_ADDTEST && !TDD
//+:cnd:noEmit

using MicroService.Common.Tests;
using MicroService.Common.Tests.Attributes;
using MicroService.Common.Interfaces;
using MicroService.Common.Services;
using MicroService.Common.Web.API;

//-:cnd:noEmit
#if !(MODEL_ADDTEST && (!MODEL_USEACTION || TDD))
using MicroService.Common.Web.API.Interfaces;
#endif
//+:cnd:noEmit

/*
    * Yo can choose your own model to test by changing the using statements given beolow:
    * For example:
    * using TOutDTO = UserDefined.Models.ISubject;
    * using TInDTO = UserDefined.Models.ISubject;
    * using TID = System.Int32;
    * using TModel = UserDefined.Models.Subject;
    * OR
    * using TOutDTO = UserDefined.DTOs.ISubjectOutDTO;
    * using TInDTO = UserDefined.Models.ISubjectInDTO;
    * using TID = System.Int32;
    * using TModel = UserDefined.Models.Subject;
    * 
    * Please note that TModel must be a concrete class deriving from the base Model class.
*/

//-:cnd:noEmit
#if !MODEL_USEDTO
using TOutDTO = MicroService.Common.Tests.TestModel;
using TInDTO = MicroService.Common.Tests.TestModel;
using TID = System.Int32;
using TModel = MicroService.Common.Tests.TestModel;
#else
using TOutDTO = MicroService.Common.Tests.ITestModelDTO;
using TInDTO = MicroService.Common.Tests.ITestModelDTO;
using TID = System.Int32;
using TModel = MicroService.Common.Tests.TestModel;
#endif
//+:cnd:noEmit

 
namespace UserDefined.Tests
{
    [Testable]
    public class StandardTest: TestStandard<TOutDTO, TModel, TID, TInDTO>
    {
        //-:cnd:noEmit
#if MODEL_ADDTEST && (!MODEL_USEACTION || TDD)

        protected override IContract<TOutDTO, TModel, TID> CreateContract(IService<TOutDTO, TModel, TID> service)
        {
            return new Controller<TOutDTO, TModel, TID, TInDTO>(service);
        }
#else
        protected override IActionContract<TModel, int> CreateContract(IService<TInDTO, TModel, int> service)
        {
            return new Controller<TOutDTO, TModel, TID, TInDTO>(service);
        }
#endif
        //+:cnd:noEmit
    }
}
//-:cnd:noEmit
#endif
//+:cnd:noEmit

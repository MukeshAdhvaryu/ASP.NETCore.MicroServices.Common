//-:cnd:noEmit
#if MODEL_ADDTEST && !TDD
//+:cnd:noEmit

using MicroService.Common.Tests;
using MicroService.Common.Tests.Attributes;
using MicroService.Common.Interfaces;
using MicroService.Common.Services;
using MicroService.API.Controllers;

//-:cnd:noEmit
#if !MODEL_USEDTO
using TModelInterface = MicroService.Common.Tests.TestModel;
using TIDType = System.Int32;
using TModel = MicroService.Common.Tests.TestModel;
#else
using TModelInterface = MicroService.Common.Tests.ITestModelDTO;
using TIDType = System.Int32;
using TModel = MicroService.Common.Tests.TestModel;
#endif
//+:cnd:noEmit


namespace UserDefined.Tests
{
    [Testable]
    public class StandardTest: TestStandard<TModelInterface, TModel, TIDType>
    {
        protected override IContract<TModelInterface, TModel, TIDType> CreateContract(IService<TModelInterface, TModel, TIDType> service)
        {
            return new Controller<TModelInterface, TModel, TIDType>(service);
        }
    }
}
//-:cnd:noEmit
#endif
//+:cnd:noEmit

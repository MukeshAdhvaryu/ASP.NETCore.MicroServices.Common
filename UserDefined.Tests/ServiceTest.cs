//-:cnd:noEmit
#if MODEL_ADDTEST 
//+:cnd:noEmit

using MicroService.Common.Interfaces;
using MicroService.Common.Services;
using MicroService.Common.Tests;
using MicroService.Common.Tests.Attributes;
using MicroService.Common.Collections;

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
    public class ServiceTest : ServiceTest<TModelInterface, TModel, TIDType>
    {
        protected override IService<TModelInterface, TModel, TIDType> CreateService()
        {
            return new Service<TModelInterface, TModel, TIDType, ModelCollection<TModel, TIDType>>(new ModelCollection<TModel, TIDType>());
        }
    }
}
//-:cnd:noEmit
#endif
//+:cnd:noEmit

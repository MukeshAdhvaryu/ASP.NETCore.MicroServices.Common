﻿//-:cnd:noEmit
#if MODEL_ADDTEST 
//+:cnd:noEmit

using MicroService.Common.Interfaces;
using MicroService.Common.Services;
using MicroService.Common.Tests;
using MicroService.Common.Tests.Attributes;
using MicroService.Common.Collections;
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
    public class ServiceTest : ServiceTest<TModelDTO, TModel, TID>
    {
        protected override IService<TModelDTO, TModel, TID> CreateService()
        {
            return new Service<TModelDTO, TModel, TID, ModelCollection<TModel, TID>>(new ModelCollection<TModel, TID>());
        }
    }
}
//-:cnd:noEmit
#endif
//+:cnd:noEmit

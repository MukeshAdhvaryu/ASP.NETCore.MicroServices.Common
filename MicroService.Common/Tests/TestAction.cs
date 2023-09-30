/* Licensed under the MIT/X11 license.
* This notice may not be removed from any source distribution.
Author: Mukesh Adhvaryu.
*/

//-:cnd:noEmit
#if MODEL_ADDTEST && MODEL_USEACTION && !TDD
//+:cnd:noEmit

using System.Linq.Expressions;

using AutoFixture;
using AutoFixture.AutoMoq;

using MicroService.Common.Exceptions;
using MicroService.Common.Interfaces;
using MicroService.Common.Models;
using MicroService.Common.Services;
using MicroService.Common.Tests.Attributes;
using MicroService.Common.Web.API.Interfaces;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using Moq;

namespace MicroService.Common.Tests
{
    public abstract class TestStandard<TModelDTO, TModel, TID>
        #region TYPE CONSTRINTS
        where TModelDTO : IModel
        where TModel : Model<TID>, IModel<TID>,
        //-:cnd:noEmit
#if (!MODEL_USEDTO)
        TModelDTO,
#endif
        //+:cnd:noEmit
        new()
        where TID : struct
        #endregion
    {
        #region VARIABLES
        protected readonly Mock<IService<TModelDTO, TModel, TID>> MockService;
        readonly IActionContract<TModel, TID> Contract;
        protected readonly List<TModel> Models;
        protected readonly IFixture Fixture;

        static readonly IExModelExceptionSupplier DummyModel = new TModel();
        //-:cnd:noEmit
#if MODEL_USEDTO
        static readonly Type DTOType = typeof(TModelDTO);
        static readonly bool NeedToUseDTO = !DTOType.IsAssignableFrom(typeof(TModel));
#endif
        //+:cnd:noEmit
        #endregion

        #region CONSTRUCTOR
        public TestStandard()
        {
            Fixture = new Fixture().Customize(new AutoMoqCustomization { ConfigureMembers = true });
            MockService = Fixture.Freeze<Mock<IService<TModelDTO, TModel, TID>>>();
            Contract = CreateContract(MockService.Object);
            var count = DummyModelCount;
            if (count < 5)
                count = 5;
            Models = Fixture.CreateMany<TModel>(count).ToList();
        }
        #endregion

        #region PROPERTIES
        //-:cnd:noEmit
#if (MODEL_USEDTO)
        protected IEnumerable<TModelDTO> Items => Models.Select(x => ToDTO(x));
#else
        protected IEnumerable<TModelDTO> Items => (IEnumerable<TModelDTO>)Models;

#endif
        //+:cnd:noEmit

        protected virtual int DummyModelCount => 5;
        #endregion

        #region CREATE CONTROLLER
        protected abstract IActionContract<TModel, TID> CreateContract(IService<TModelDTO, TModel, TID> service);
        #endregion

        #region SETUP FUNCTION
        protected void Setup<TResult>(Expression<Func<IService<TModelDTO, TModel, TID>, Task<TResult>>> expression, TResult returnedValue)
        {
            MockService.Setup(expression).ReturnsAsync(returnedValue);
        }
        protected void Setup<TResult>(Expression<Func<IService<TModelDTO, TModel, TID>, Task<TResult>>> expression, Exception exception)
        {
            MockService.Setup(expression).Throws(exception);
        }
        #endregion

        #region GET MODEL/S
        //-:cnd:noEmit
#if !MODEL_NONREADABLE
        [NoArgs]
        public async Task Get_ByIDSuccess()
        {
            var id = Models[0].ID;
            var model = ToDTO(Models[0]);
            Setup((m) => m.Get(id), model);
            var result = await Contract.Get(id) as ObjectResult;
            var expected = result?.Value;
            Verifier.Equal(model, expected);
            Verifier.Equal(StatusCodes.Status200OK, result?.StatusCode);
        }

        [NoArgs]
        public async Task Get_ByIDFail()
        {
            var id = default(TID);
            var e = DummyModel.GetModelException(ExceptionType.NoModelFoundForIDException, id.ToString());
            Setup((m) => m.Get(id), e);
            var result = await Contract.Get(id) as ObjectResult;
            var expected = result?.Value as IModelException;
            Verifier.Equal(e.Message, expected?.Message);
            Verifier.Equal(StatusCodes.Status400BadRequest, result?.StatusCode);
        }

        [WithArgs]
        [Args(0)]
        [Args(3)]
        public async Task GetAll_Success(int limitOfResult = 0)
        {
            int count = limitOfResult;

            if (limitOfResult == 0)
            {
                limitOfResult = Models.Count;
                Setup((m) => m.GetAll(limitOfResult), Items);
                var result = await Contract.GetAll(limitOfResult) as ObjectResult;
                var expected = result?.Value as IEnumerable<TModelDTO>;
                Verifier.Equal(limitOfResult, expected?.Count());
            }
            else
            {
                Setup((m) => m.GetAll(limitOfResult), Items.Take(limitOfResult));
                var result = await Contract.GetAll(limitOfResult) as ObjectResult;
                var expected = result?.Value as IEnumerable<TModelDTO>;
                Verifier.Equal(limitOfResult, expected?.Count());
                Verifier.Equal(StatusCodes.Status200OK, result?.StatusCode);
            }
        }

        [WithArgs]
        [Args(-1)]
        public async Task GetAll_Fail(int limitOfResult = 0)
        {
            var e = DummyModel.GetModelException(ExceptionType.NegativeFetchCountException, limitOfResult.ToString());
            Setup((m) => m.GetAll(limitOfResult), e);
            var result = await Contract.GetAll(limitOfResult) as ObjectResult;
            var expected = (result )?.Value as IModelException;
            Verifier.Equal(e.Message, expected?.Message);
            Verifier.Equal(StatusCodes.Status400BadRequest, result?.StatusCode);
        }
#endif
        //+:cnd:noEmit
        #endregion

        #region ADD MODEL
        //-:cnd:noEmit
#if MODEL_APPENDABLE
        [NoArgs]
        public async Task Add_Sucess()
        {
            var model = Fixture.Create<TModel>();
            var returnModel = ToDTO(model);
            Setup((m) => m.Add(model), returnModel);
            var result = await Contract.Add(model) as ObjectResult;
            var expected = (result)?.Value;
            Verifier.Equal(expected, returnModel);
            Verifier.Equal(StatusCodes.Status200OK, result?.StatusCode);
        }
        [NoArgs]
        public async Task Add_Fail()
        {
            var ID = Models[0].ID;

            var e = DummyModel.GetModelException(ExceptionType.AddOperationFailedException, ID.ToString());

            var model = Fixture.Create<TModel>();
            model.ID = ID;
            Setup((m) => m.Add(model), e);
            var result = await Contract.Add(model) as ObjectResult;
            var expected = (result)?.Value as IModelException;
            Verifier.Equal(e.Message, expected?.Message);
            Verifier.Equal(StatusCodes.Status400BadRequest, result?.StatusCode);
        }
#endif
        //+:cnd:noEmit
        #endregion

        #region DELETE MODEL
        //-:cnd:noEmit
#if MODEL_DELETABLE
        [NoArgs]
        public async Task Delete_Success()
        {
            TModel? model = Fixture.Create<TModel>();
            var returnModel = ToDTO(model);
            Setup((m) => m.Delete(model.ID), returnModel);
            var result = await Contract.Delete(model.ID) as ObjectResult;
            var expected = result?.Value;
            Verifier.Equal(returnModel, expected);
            Verifier.Equal(StatusCodes.Status200OK, result?.StatusCode);
        }

        [NoArgs]
        public async Task Delete_Fail()
        {
            var ID = Fixture.Create<TID>();
            var e = DummyModel.GetModelException(ExceptionType.DeleteOperationFailedException, ID.ToString());

            Setup((m) => m.Delete(ID), e);
            var result = await Contract.Delete(ID) as ObjectResult;
            var expected = result?.Value as IModelException;
            Verifier.Equal(e.Message, expected?.Message);
            Verifier.Equal(StatusCodes.Status400BadRequest, result?.StatusCode);
        }
#endif
        //+:cnd:noEmit
        #endregion

        #region UPDATE MODEL
        //-:cnd:noEmit
#if MODEL_UPDATABLE
        [NoArgs]
        public async Task Update_Success()
        {
            TModel? model = Fixture.Create<TModel>();
            var returnModel = ToDTO(model);
            Setup((m) => m.Update(model.ID, model), returnModel);
            var result = await Contract.Update(model.ID, model) as ObjectResult;
            var expected = result?.Value;
            Verifier.Equal(returnModel, expected);
            Verifier.Equal(StatusCodes.Status200OK, result?.StatusCode);
        }

        [NoArgs]
        public async Task Update_Fail()
        {
            var ID = Fixture.Create<TID>();
            var e = DummyModel.GetModelException(ExceptionType.UpdateOperationFailedException, ID.ToString());
            var model = Fixture.Create<TModel>();
            Setup((m) => m.Update(ID, model), e);
            var result = await Contract.Update(ID, model) as ObjectResult;
            var expected = result?.Value as IModelException;
            Verifier.Equal(e.Message, expected?.Message);
            Verifier.Equal(StatusCodes.Status400BadRequest, result?.StatusCode);
        }
#endif
        //+:cnd:noEmit
        #endregion

        #region TO DTO
        //-:cnd:noEmit
#if (MODEL_USEDTO)
        protected TModelDTO? ToDTO(TModel? model)
        {
            if (model == null)
                return default(TModelDTO);
            if (NeedToUseDTO)
                return (TModelDTO)((IExModel)model).ToDTO(DTOType);
            return (TModelDTO)(object)model;
        }
#else
        protected TModelDTO? ToDTO(TModel? model)
        {
            if(model == null)
                return default(TModelDTO);
            return (TModelDTO)(object)model;
        }
#endif
        //+:cnd:noEmit
        #endregion

        #region CLASS DATA & MEMBER DATA EXAMPLE 
        /*      
        //This is an example on how to use source member data.
        //To use member data, you must define a static method or property returning IEnumerable<object[]>.
        [WithArgs]
        [ArgSource(typeof(MemberDataExample), "GetData")]
        public Task GetAll_ReturnAllUseMemberData(int limitOfResult = 0)
        {
            //
        }

        //This is an example on how to use source class data.
        //To use class data, ArgSource<source> will suffice.
        [WithArgs]
        [ArgSource<ClassDataExample>]
        public Task GetAll_ReturnAllUseClassData(int limitOfResult = 0)
        {
            //
        }

        class MemberDataExample 
        {
            public static IEnumerable<object[]> GetData
            {
                get
                {
                    yield return new object[] { 0 };
                    yield return new object[] { 3 };
                    yield return new object[] { -1 };
                }
            }
        }

        class ClassDataExample: ArgSource 
        {
            public override IEnumerable<object[]> Data
            {
                get
                {
                    yield return new object[] { 0 };
                    yield return new object[] { 3 };
                    yield return new object[] { -1 };
                }
            }
        }
        */
        #endregion
    }
}
//-:cnd:noEmit
#endif
//+:cnd:noEmit

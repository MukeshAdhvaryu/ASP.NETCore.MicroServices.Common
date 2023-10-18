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
using MicroService.Common.Tests.Attributes;
using MicroService.Common.Web.API;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using Moq;

namespace MicroService.Common.Tests
{
    [Testable]
    public abstract class TestAction<TOutDTO, TModel, TID, TInDTO>
        #region TYPE CONSTRINTS
        where TOutDTO : IModel
        where TInDTO : IModel
        where TModel : Model<TID, TModel>,
        //-:cnd:noEmit
#if (!MODEL_USEDTO)
        TOutDTO,
#endif
        //+:cnd:noEmit
        new()
        where TID : struct
        #endregion
    {
        #region VARIABLES
        protected readonly Controller<TOutDTO, TModel, TID, TInDTO> Contract;

        //-:cnd:noEmit
#if MODEL_USEDTO
        static readonly Type DTOType = typeof(TOutDTO);
        static readonly bool NeedToUseDTO = !DTOType.IsAssignableFrom(typeof(TModel));
#endif
        //+:cnd:noEmit

        readonly Mock<IContract<TOutDTO, TModel, TID>> MockService;
        protected readonly List<TModel> Models;
        protected readonly IFixture Fixture;

        static readonly IExModelExceptionSupplier DummyModel = new TModel();
        #endregion

        #region CONSTRUCTOR
        public TestAction()
        {
            Fixture = new Fixture().Customize(new AutoMoqCustomization { ConfigureMembers = true });
            MockService = Fixture.Freeze<Mock<IContract<TOutDTO, TModel, TID>>>();

            Contract = CreateController(MockService.Object);
            var Service = MockService.Object;
            var count = DummyModelCount;
            if (count < 5)
                count = 5;
            Models = Fixture.CreateMany<TModel>(count).ToList();
        }
        #endregion

        #region PROPERTIES
        //protected readonly Mock<IService<TOutDTO, TModel, TID>> MockService;

        //-:cnd:noEmit
#if (MODEL_USEDTO)
        protected IEnumerable<TOutDTO> Items => Models.Select(x => ToDTO(x));
#else
        protected IEnumerable<TOutDTO> Items => (IEnumerable<TOutDTO>)Models;

#endif
        //+:cnd:noEmit

        protected virtual int DummyModelCount => 5;
        #endregion

        #region CREATE CONTROLLER
        protected abstract Controller<TOutDTO, TModel, TID, TInDTO> CreateController(IContract<TOutDTO, TModel, TID> service);
        #endregion

        #region SETUP FUNCTION
        protected void Setup<TResult>(Expression<Func<IContract<TOutDTO, TModel, TID>, Task<TResult>>> expression, TResult returnedValue)
        {
            MockService.Setup(expression).ReturnsAsync(returnedValue);
        }
        protected void Setup<TResult>(Expression<Func<IContract<TOutDTO, TModel, TID>, Task<TResult>>> expression, Exception exception)
        {
            MockService.Setup(expression).Throws(exception);
        }
        #endregion

        #region GET MODEL/S
        //-:cnd:noEmit
#if !MODEL_NONREADABLE && !MODEL_NONQUERYABLE
        [NoArgs]
        public async Task Get_ByIDSuccess()
        {
            var id = Models[0].ID;
            var model = ToDTO(Models[0]);
            Setup((m) => m.Query.Get(id), model);
            var result = await Contract.Get(id) as ObjectResult;
            var expected = result?.Value;
            Verifier.Equal(model, expected);
            Verifier.Equal(StatusCodes.Status200OK, result?.StatusCode);
        }

        [NoArgs]
        public async Task Get_ByIDFail()
        {
            var id = default(TID);
            var e = DummyModel.GetModelException(ExceptionType.NoModelFoundForID, id.ToString());
            Setup((m) => m.Query.Get(id), e);
            try
            {
                await Contract.Get(id);
                Verifier.Equal(true, false);
            }
            catch (Exception ex)
            {
                var expected = ex as IModelException;
                Verifier.Equal(e.Message, ex?.Message);
            }
        }
#endif
        //+:cnd:noEmit
        #endregion

        #region GET ALL
        //-:cnd:noEmit
#if !MODEL_NONREADABLE && !MODEL_NONQUERYABLE
        [WithArgs]
        [Args(0)]
        [Args(3)]
        public async Task GetAll_Success(int count = 0)
        {
            if (count == 0)
            {
                count = Models.Count;
                Setup((m) => m.Query.GetAll(0), Items);
                var result = await Contract.GetAll(0) as ObjectResult;
                var expected = result?.Value as IEnumerable<TOutDTO>;
                Verifier.Equal(count, expected?.Count());
            }
            else
            {
                Setup((m) => m.Query.GetAll(count), Items.Take(count));
                var result = await Contract.GetAll(count) as ObjectResult;
                var expected = result?.Value as IEnumerable<TOutDTO>;
                Verifier.Equal(count, expected?.Count());
                Verifier.Equal(StatusCodes.Status200OK, result?.StatusCode);
            }
        }

        [WithArgs]
        [Args(-1)]
        public async Task GetAll_Fail(int count = 0)
        {
            var e = DummyModel.GetModelException(ExceptionType.NegativeFetchCount, count.ToString());
            Setup((m) => m.Query.GetAll(count), e);
            try
            {
                await Contract.GetAll(count);
                Verifier.Equal(true, false);
            }
            catch (Exception ex)
            {
                var expected = ex as IModelException;
                Verifier.Equal(e.Message, ex?.Message);
            }
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
            var inModel = Fixture.Create<TInDTO>();

            var returnModel = ToDTO(model);
            Setup((m) => m.Command.Add(inModel), returnModel);
            var result = await Contract.Add(inModel) as ObjectResult;
            var expected = (result)?.Value;
            Verifier.Equal(expected, returnModel);
            Verifier.Equal(StatusCodes.Status200OK, result?.StatusCode);
        }
        [NoArgs]
        public async Task Add_Fail()
        {
            var ID = Models[0].ID;

            var e = DummyModel.GetModelException(ExceptionType.AddOperationFailed, ID.ToString());

            var inModel = Fixture.Create<TInDTO>();
            Setup((m) => m.Command.Add(inModel), e);
            try
            {
                await Contract.Add(inModel);
                Verifier.Equal(true, false);
            }
            catch (Exception ex)
            {
                var expected = ex as IModelException;
                Verifier.Equal(e.Message, ex?.Message);
            }
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
            Setup((m) => m.Command.Delete(model.ID), returnModel);
            var result = await Contract.Delete(model.ID) as ObjectResult;
            var expected = result?.Value;
            Verifier.Equal(returnModel, expected);
            Verifier.Equal(StatusCodes.Status200OK, result?.StatusCode);
        }

        [NoArgs]
        public async Task Delete_Fail()
        {
            var ID = Fixture.Create<TID>();
            var e = DummyModel.GetModelException(ExceptionType.DeleteOperationFailed, ID.ToString());

            Setup((m) => m.Command.Delete(ID), e);
            try
            {
                await Contract.Delete(ID);
                Verifier.Equal(true, false);
            }
            catch (Exception ex)
            {
                var expected = ex as IModelException;
                Verifier.Equal(e.Message, ex?.Message);
            }
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
            var model = Fixture.Create<TModel>();
            var inModel = Fixture.Create<TInDTO>();
            var returnModel = ToDTO(model);
            Setup((m) => m.Command.Update(model.ID, inModel), returnModel);
            var result = await Contract.Update(model.ID, inModel) as ObjectResult;
            var expected = result?.Value;
            Verifier.Equal(returnModel, expected);
            Verifier.Equal(StatusCodes.Status200OK, result?.StatusCode);
        }

        [NoArgs]
        public async Task Update_Fail()
        {
            var ID = Fixture.Create<TID>();
            var e = DummyModel.GetModelException(ExceptionType.UpdateOperationFailed, ID.ToString());
            var inModel = Fixture.Create<TInDTO>();
            Setup((m) => m.Command.Update(ID, inModel), e);
            try
            {
                await Contract.Update(ID, inModel);
                Verifier.Equal(true, false);
            }
            catch (Exception ex)
            {
                var expected = ex as IModelException;
                Verifier.Equal(e.Message, ex?.Message);
            }
        }
#endif
        //+:cnd:noEmit
        #endregion

        #region TO DTO
        //-:cnd:noEmit
#if (MODEL_USEDTO)
        protected TOutDTO? ToDTO(TModel? model)
        {
            if (model == null)
                return default(TOutDTO);
            if (NeedToUseDTO)
                return (TOutDTO)((IExModel)model).ToDTO(DTOType);
            return (TOutDTO)(object)model;
        }
#else
        protected TOutDTO? ToDTO(TModel? model)
        {
            if(model == null)
                return default(TOutDTO);
            return (TOutDTO)(object)model;
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
        public Task GetAll_ReturnAllUseMemberData(int count = 0)
        {
            //
        }

        //This is an example on how to use source class data.
        //To use class data, ArgSource<source> will suffice.
        [WithArgs]
        [ArgSource<ClassDataExample>]
        public Task GetAll_ReturnAllUseClassData(int count = 0)
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

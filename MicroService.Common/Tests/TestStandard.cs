/* Licensed under the MIT/X11 license.
* This notice may not be removed from any source distribution.
Author: Mukesh Adhvaryu.
*/

//-:cnd:noEmit
#if MODEL_ADDTEST  
//+:cnd:noEmit

using System.Linq.Expressions;

using AutoFixture;
using AutoFixture.AutoMoq;

using MicroService.Common.CQRS;
using MicroService.Common.Interfaces;
using MicroService.Common.Models;
using MicroService.Common.Tests.Attributes;

using Moq;

namespace MicroService.Common.Tests
{
    public abstract class TestStandard<TOutDTO, TModel, TID, TInDTO>
        #region TYPE CONSTRINTS
        where TOutDTO : IModel, new()
        where TInDTO: IModel, new()
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
        protected readonly IContract<TOutDTO, TModel, TID> Contract;
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
        public TestStandard()
        {
            Fixture = new Fixture().Customize(new AutoMoqCustomization { ConfigureMembers = true });
            MockService = Fixture.Freeze<Mock<IContract<TOutDTO, TModel, TID>>>();
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
        protected IEnumerable<TOutDTO> Items => Models.Select(x => ToDTO(x));
#else
        protected IEnumerable<TOutDTO> Items => (IEnumerable<TOutDTO>)Models;

#endif
        //+:cnd:noEmit

        protected virtual int DummyModelCount => 5;
        #endregion

        #region CREATE CONTROLLER
        protected abstract IContract<TOutDTO, TModel, TID> CreateContract(IContract<TOutDTO, TModel, TID> service);
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
            var result = await Contract.Query.Get(id);
            Verifier.Equal(model, result);
        }

        [NoArgs]
        public async Task Get_ByIDFail()
        {
            var id = Fixture.Create<TID>();
            var e = DummyModel.GetModelException(ExceptionType.NoModelFoundForID, id.ToString());
            Setup((m) => m.Query.Get(id), e);
            try
            {
                await Contract.Query.Get(id);
                Verifier.Equal(true, true);
            }
            catch (Exception ex)
            {
                Verifier.Equal(e.Message, ex.Message);
            }
        }

        [WithArgs]
        [Args(0)]
        [Args(3)]
        public async Task GetAll_Success(int count = 0)
        {
            if (count == 0)
            {
                count = Models.Count;
                Setup((m) => m.Query.GetAll(0), Items);
                var expected = await Contract.Query.GetAll(0);
                Verifier.Equal(count, expected?.Count());
            }
            else
            {
                Setup((m) => m.Query.GetAll(count), Items.Take(count));
                var expected = await Contract.Query.GetAll(count);
                Verifier.Equal(count, expected?.Count());
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
                await Contract.Query.GetAll(count);
                Verifier.Equal(true, true);
            }
            catch (Exception ex)
            {
                Verifier.Equal(e.Message, ex.Message);
            }
        }
#endif
        //+:cnd:noEmit
        #endregion

        #region ADD MODEL
        //-:cnd:noEmit
#if MODEL_APPENDABLE
        [NoArgs]
        public async Task Add_Success()
        {
            var model = Fixture.Create<TModel>();
            var inModel = Fixture.Create<TInDTO>();

            var returnModel = ToDTO(model);
            Setup((m) => m.Command.Add(inModel), returnModel);
            var expected = await Contract.Command.Add(inModel);
            Verifier.Equal(expected, returnModel);
        }
        [NoArgs]
        public async Task Add_Fail()
        {
            var e = DummyModel.GetModelException(ExceptionType.AddOperationFailed);
            var inModel = Fixture.Create<TInDTO>();
            Setup((m) => m.Command.Add(inModel), e);
            try
            {
                await Contract.Command.Add(inModel);
                Verifier.Equal(true, true);
            }
            catch (Exception ex)
            {
                Verifier.Equal(e.Message, ex.Message);
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
            var expected = await Contract.Command.Delete(model.ID);
            Verifier.Equal(returnModel, expected);
        }

        [NoArgs]
        public async Task Delete_Fail()
        {
            var ID = default(TID);
            var e = DummyModel.GetModelException(ExceptionType.DeleteOperationFailed, ID.ToString());
            Setup((m) => m.Command.Delete(ID), e);
            try
            {
                await Contract.Command.Delete(ID);
                Verifier.Equal(true, true);
            }
            catch (Exception ex)
            {
                Verifier.Equal(e.Message, ex.Message);
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
            TModel? model = Fixture.Create<TModel>();
            var inModel = Fixture.Create<TInDTO>();
            var returnModel = ToDTO(model);
            Setup((m) => m.Command.Update(model.ID, inModel), returnModel);
            var expected = await Contract.Command.Update(model.ID, inModel);
            Verifier.Equal(returnModel, expected);
        }

        [NoArgs]
        public async Task Update_Fail()
        {
            var ID = default(TID);
            var e = DummyModel.GetModelException(ExceptionType.UpdateOperationFailed, ID.ToString());
            var model = Fixture.Create<TModel>();
            var inModel = Fixture.Create<TInDTO>();
            Setup((m) => m.Command.Update(ID, inModel), e);
            try
            {
                await Contract.Command.Update(ID, inModel);
                Verifier.Equal(true, true);
            }
            catch (Exception ex)
            {
                Verifier.Equal(e.Message, ex.Message);
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
            {
                var result = ((IExModel)model).ToDTO(DTOType);
                if(result == null)
                    return default(TOutDTO);

                return (TOutDTO)result;
            }
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

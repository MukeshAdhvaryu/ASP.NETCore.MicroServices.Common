/* Licensed under the MIT/X11 license.
* This notice may not be removed from any source distribution.
Author: Mukesh Adhvaryu.
*/

//-:cnd:noEmit
#if MODEL_ADDTEST
//+:cnd:noEmit

using System;
using System.Linq.Expressions;

using AutoFixture;
using AutoFixture.AutoMoq;

using MicroService.Common.Collections;
using MicroService.Common.Exceptions;
using MicroService.Common.Interfaces;
using MicroService.Common.Models;
using MicroService.Common.Services;
using MicroService.Common.Tests.Attributes;

using Moq;

namespace MicroService.Common.Tests
{
    public abstract class ServiceTest<TOutDTO, TModel, TID>  
        #region TYPE CONSTRINTS
        where TOutDTO : IModel
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
        readonly IService<TOutDTO, TModel, TID> Contract;
        protected readonly IFixture Fixture;

        static readonly IExModelExceptionSupplier DummyModel = new TModel();
        //-:cnd:noEmit
#if MODEL_USEDTO
        static readonly Type DTOType = typeof(TOutDTO);
        static readonly bool NeedToUseDTO = !DTOType.IsAssignableFrom(typeof(TModel));
#endif
        //+:cnd:noEmit
        #endregion

        #region CONSTRUCTOR
        public ServiceTest()
        {
            Fixture = new Fixture().Customize(new AutoMoqCustomization { ConfigureMembers = true });
            Contract = CreateService();
        }
        #endregion

        #region CREATE SERVICE
        protected abstract IService<TOutDTO, TModel, TID> CreateService();
        #endregion

        #region GET MODEL/S
        //-:cnd:noEmit
#if !MODEL_NONREADABLE
        [NoArgs]
        public async Task Get_ByIDSuccess()
        {
            var model = Contract.GetFirstModel();
            var result = await Contract.Get(model.ID);
            Verifier.NotNull(result);
        }

        [NoArgs]
        public async Task Get_ByIDFail()
        {
            var id = default(TID);
            try
            {
                await Contract.Get(id);
                Verifier.Equal(true, true);
            }
            catch (Exception ex)
            {
                var exceptionType = (ex as IModelException)?.Type ?? 0;
                switch (exceptionType)
                {
                    case ExceptionType.NoModelFoundException:
                    case ExceptionType.NoModelFoundForIDException:
                        Verifier.Equal(true, true);
                        return;
                    default:
                        break;
                }
                Verifier.Equal(false, false);
            }
        }

        [WithArgs]
        [Args(0)]
        [Args(3)]
        public async Task GetAll_Success(int limitOfResult = 0)
        {
            if (limitOfResult == 0)
            {
                limitOfResult = Contract.GetModelCount();
                var expected = await Contract.GetAll(limitOfResult);
                Verifier.Equal(limitOfResult, expected?.Count());
            }
            else
            {
                var expected = await Contract.GetAll(limitOfResult);
                Verifier.Equal(limitOfResult, expected?.Count());
            }
        }

        [WithArgs]
        [Args(-1)]
        public async Task GetAll_Fail(int limitOfResult = 0)
        {
            try
            {
                await Contract.GetAll(limitOfResult);
                Verifier.Equal(true, true);
            }
            catch (Exception ex)
            {
                var exceptionType = (ex as IModelException)?.Type ?? 0;
                switch (exceptionType)
                {
                    case ExceptionType.NegativeFetchCountException:
                        Verifier.Equal(true, true);
                        return;
                    default:
                        break;
                }
                Verifier.Equal(false, false);
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
            var expected = await Contract.Add(model);
            Verifier.NotNull(expected);
        }
        [NoArgs]
        public async Task Add_Fail()
        {
            var model = Fixture.Create<TModel>();
            model.ID = Contract.GetFirstModel().ID;

            try
            { 
                await Contract.Add(model);
                Verifier.Equal(true, true);
            }
            catch (Exception ex)
            {
                var exceptionType = (ex as IModelException)?.Type ?? 0;
                switch (exceptionType)
                {
                    case ExceptionType.AddOperationFailedException:
                    case ExceptionType.NoModelSuppliedException:
                        Verifier.Equal(true, true);
                        return;
                    default:
                        break;
                }
                Verifier.Equal(false, false);
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
            TModel? model = Contract.GetFirstModel();
            var expected = await Contract.Delete(model.ID);
            Verifier.NotNull(expected);
        }

        [NoArgs]
        public async Task Delete_Fail()
        {
            var ID = default(TID);
            try
            {
                await Contract.Delete(ID);
                Verifier.Equal(true, true);
            }
            catch (Exception ex)
            {
                var exceptionType = (ex as IModelException)?.Type ?? 0;
                switch (exceptionType)
                {
                    case ExceptionType.NoModelFoundForIDException:
                    case ExceptionType.DeleteOperationFailedException:
                        Verifier.Equal(true, true);
                        return;
                    default:
                        break;
                }
                Verifier.Equal(false, false);
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
            var ID = Contract.GetFirstModel().ID;
            var expected = await Contract.Update(ID, model);
            Verifier.NotNull(expected);
        }

        [NoArgs]
        public async Task Update_Fail()
        {
            var ID = default(TID);
            var model = Contract.GetFirstModel();
            try
            {
                await Contract.Update(ID, model);
                Verifier.Equal(true, true);
            }
            catch (Exception ex)
            {
                var exceptionType = (ex as IModelException)?.Type ?? 0;
                switch (exceptionType)
                {
                    case ExceptionType.UpdateOperationFailedException:
                    case ExceptionType.NoModelSuppliedException:
                    case ExceptionType.ModelCopyOperationFailed:
                        Verifier.Equal(true, true);
                        return;
                    default:
                        break;
                }
                Verifier.Equal(false, false);
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

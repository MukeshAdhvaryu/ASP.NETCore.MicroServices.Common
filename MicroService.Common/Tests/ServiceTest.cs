/* Licensed under the MIT/X11 license.
* This notice may not be removed from any source distribution.
Author: Mukesh Adhvaryu.
*/

//-:cnd:noEmit
#if MODEL_ADDTEST
//+:cnd:noEmit

using System;

using AutoFixture;
using AutoFixture.AutoMoq;

using MicroService.Common.Collections;
using MicroService.Common.Interfaces;
using MicroService.Common.Models;
using MicroService.Common.Services;
using MicroService.Common.Tests.Attributes;

namespace MicroService.Common.Tests
{
    public abstract class ServiceTest<TModelDTO, TModel, TID>  
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
        readonly IService<TModelDTO, TModel, TID> Service;
        protected readonly IFixture Fixture;
        //-:cnd:noEmit
#if MODEL_USEDTO
        static readonly Type DTOType = typeof(TModelDTO);
        static readonly bool NeedToUseDTO = !DTOType.IsAssignableFrom(typeof(TModel));
#endif
        //+:cnd:noEmit
        #endregion

        #region CONSTRUCTOR
        public ServiceTest()
        {
            Fixture = new Fixture().Customize(new AutoMoqCustomization { ConfigureMembers = true });
            Service = CreateService();
        }
        #endregion

        #region GET MODEL/S
        //-:cnd:noEmit
#if !MODEL_NONREADABLE
        [NoArgs]
        public async Task Get_ReturnSingle()
        {
            TModelDTO expected;
            var first = Service.GetFirstModel();
            expected = await Service.Get(first?.ID ?? default(TID));
            Verifier.NotNull(expected);
        }

        [NoArgs]
        public async Task Get_ReturnSingleFail()
        {
            TModelDTO expected;
            var id = default(TID);
            try
            {
                expected = await Service.Get(id);
                Verifier.NotNull(expected);
            }
            catch (Exception exception)
            {
                Verifier.Equal(string.Format("No such {0} found with ID: " + id, typeof(TModel).Name), exception.Message);
            }
        }

        [WithArgs]
        [Args(0)]
        [Args(3)]
        public async Task GetAll_ReturnAll(int limitOfResult = 0)
        {
            var expected = await Service.GetAll(limitOfResult);

            if (limitOfResult == 0)
                limitOfResult = Service.GetModelCount();
            Verifier.Equal(limitOfResult, expected.Count());
        }

        [WithArgs]
        [Args(-1)]
        public async Task GetAll_ReturnNull(int limitOfResult = 0)
        {
            var expected = await Service.GetAll(limitOfResult);
            Verifier.Empty(expected);
        }
#endif
        //+:cnd:noEmit
        #endregion

        #region ADD MODEL
        //-:cnd:noEmit
#if MODEL_APPENDABLE
        [NoArgs]
        public async Task Add_ReturnAdded()
        {
            TModelDTO expected;
            var model = Fixture.Create<TModel>();
            expected = await Service.Add(model);
            Verifier.NotNull(expected);
        }
#endif
        //+:cnd:noEmit
        #endregion

        #region DELETE MODEL
        //-:cnd:noEmit
#if MODEL_DELETABLE
        [NoArgs]
        public async Task Delete_ReturnDeleted()
        {
            TModelDTO expected;
            var first = Service.GetFirstModel();
            expected = await Service.Delete(first?.ID ?? default(TID));
            Verifier.NotNull(expected);
        }
#endif
        //+:cnd:noEmit
        #endregion

        #region UPDATE MODEL
        //-:cnd:noEmit
#if MODEL_UPDATABLE
        [NoArgs]
        public async Task Update_ReturnUpdated()
        {
            TModelDTO expected;
            var id = Service.GetFirstModel()?.ID ?? default(TID);
            var model = Fixture.Create<TModel>();
            model.ID = id;
            expected = await Service.Update(id, model);
            Verifier.NotNull(expected);
        }
#endif
        //+:cnd:noEmit
        #endregion

        #region CREATE CONTROLLER
        protected virtual IService<TModelDTO, TModel, TID> CreateService()
        {
            return new Service<TModelDTO, TModel, TID, ModelCollection<TModel, TID>>(new ModelCollection<TModel, TID>());
        }
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
    }
}
//-:cnd:noEmit
#endif
//+:cnd:noEmit

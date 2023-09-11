/* Licensed under the MIT/X11 license.
* This notice may not be removed from any source distribution.
Author: Mukesh Adhvaryu.
*/

//-:cnd:noEmit
#if MODEL_ADDTEST
//+:cnd:noEmit

using System;

using AutoFixture;

using MicroService.Common.Collections;
using MicroService.Common.Interfaces;
using MicroService.Common.Models;
using MicroService.Common.Services;
using MicroService.Common.Tests.Attributes;

namespace MicroService.Common.Tests
{
    public abstract class ServiceTest<TModelDTO, TModel, TID> : Test<TModelDTO, TModel, TID>
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
        #endregion

        #region CONSTRUCTOR
        public ServiceTest()
        {
            Service = CreateService();
        }
        #endregion

        #region GET MODEL/S
        //-:cnd:noEmit
#if !MODEL_nonreadable
        [NoArgs]
        public override async Task Get_ReturnSingle()
        {
            TModelDTO expected;
            var first = Service.GetFirstModel();
            expected = await Service.Get(first?.ID ?? default(TID));
            Verifier.NotNull(expected);
        }

        [NoArgs]
        public override async Task Get_ReturnSingleFail()
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
        public override async Task GetAll_ReturnAll(int limitOfResult = 0)
        {
            var expected = await Service.GetAll(limitOfResult);

            if (limitOfResult == 0)
                limitOfResult = Service.GetModelCount();
            Verifier.Equal(limitOfResult, expected.Count());
        }

        [WithArgs]
        [Args(-1)]
        public override async Task GetAll_ReturnNull(int limitOfResult = 0)
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
        public override async Task Add_ReturnAdded()
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
        public override async Task Delete_ReturnDeleted()
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
        public override async Task Update_ReturnUpdated()
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
    }
}
//-:cnd:noEmit
#endif
//+:cnd:noEmit

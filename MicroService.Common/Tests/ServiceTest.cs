/* Licensed under the MIT/X11 license.
* This notice may not be removed from any source distribution.
Author: Mukesh Adhvaryu.
*/

//-:cnd:noEmit
#if MODEL_ADDTEST
//+:cnd:noEmit

using System;

using AutoFixture;

using MicroService.Common.Models;
using MicroService.Common.Services;
using MicroService.Common.Tests.Attributes;

namespace MicroService.Common.Tests
{
    public abstract class ServiceTest<TModelInterface, TModel, TIDType> : Test<TModelInterface, TModel, TIDType>
        #region TYPE CONSTRINTS
        where TModelInterface : IModel
        where TModel : Model<TIDType>, IModel<TIDType>,
        //-:cnd:noEmit
#if (!MODEL_USEDTO)
        TModelInterface,
#endif
        //+:cnd:noEmit
        new()
        where TIDType : struct
        #endregion
    {
        #region VARIABLES
        readonly IService<TModelInterface, TModel, TIDType> Service;
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
            TModelInterface expected;
            var first = Service.GetFirstModel();
            expected = await Service.Get(first?.ID ?? default(TIDType));
            Verifier.NotNull(expected);
        }

        [NoArgs]
        public override async Task Get_ReturnSingleFail()
        {
            TModelInterface expected;
            var id = default(TIDType);
            try
            {
                expected = await Service.Get(id);

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
            TModelInterface expected;
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
            TModelInterface expected;
            var first = Service.GetFirstModel();
            expected = await Service.Delete(first?.ID ?? default(TIDType));
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
            TModelInterface expected;
            var id = Service.GetFirstModel?.ID ?? default(TIDType);
            var model = Fixture.Create<TModel>();
            model.ID = id;
            expected = await Service.Update(id, model);
            Verifier.NotNull(expected);
        }
#endif
        //+:cnd:noEmit
        #endregion

        #region CREATE CONTROLLER
        protected abstract IService<TModelInterface, TModel, TIDType> CreateService();
        #endregion
    }
}
//-:cnd:noEmit
#endif
//+:cnd:noEmit

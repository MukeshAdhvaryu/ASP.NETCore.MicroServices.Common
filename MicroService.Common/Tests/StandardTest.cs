﻿/* Licensed under the MIT/X11 license.
* This notice may not be removed from any source distribution.
Author: Mukesh Adhvaryu.
*/

//-:cnd:noEmit
#if MODEL_ADDTEST
//+:cnd:noEmit

using System.Linq.Expressions;

using AutoFixture;

using MicroService.Common.Interfaces;
using MicroService.Common.Models;
using MicroService.Common.Services;
using MicroService.Common.Tests.Attributes;

using Moq;

namespace MicroService.Common.Tests
{
    public abstract class TestStandard<TModelInterface, TModel, TIDType>: Test<TModelInterface, TModel, TIDType>
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
        protected readonly Mock<IService<TModelInterface, TModel, TIDType>> MockService;
        readonly IContract<TModelInterface, TModel, TIDType> Contract;
        protected readonly List<TModel> Models;
        #endregion

        #region CONSTRUCTOR
        public TestStandard()
        {
            MockService = Fixture.Freeze<Mock<IService<TModelInterface, TModel, TIDType>>>();
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
        protected IEnumerable<TModelInterface> Items => Models.Select(x => ToDTO(x));
#else
        protected IEnumerable<TModelInterface> Items => (IEnumerable<TModelInterface>)Models;

#endif
        //+:cnd:noEmit

        protected virtual int DummyModelCount => 5;
        #endregion

        #region CREATE CONTROLLER
        protected abstract IContract<TModelInterface, TModel, TIDType> CreateContract(IService<TModelInterface, TModel, TIDType> service);
        #endregion

        #region SETUP FUNCTION
        protected void Setup<TResult>(Expression<Func<IService<TModelInterface, TModel, TIDType>, Task<TResult>>> expression, TResult returnedValue)
        {
            MockService.Setup(expression).ReturnsAsync(returnedValue);
        }
        #endregion

        #region GET MODEL/S
        //-:cnd:noEmit
#if !MODEL_nonreadable
        [NoArgs]
        public override async Task Get_ReturnSingle()
        {
            TModelInterface expected;
            var id = Models[0].ID;
            var model = ToDTO(Models[0]);
            Setup((m) => m.Get(id), model);
            expected = await Contract.Get(id);
            Verifier.Equal(model, expected);
        }

        [NoArgs]
        public override async Task Get_ReturnSingleFail()
        {
            var id = Fixture.Create<TIDType>();
            TModelInterface model = default(TModelInterface);
            Setup((m) => m.Get(id), model);
            var expected = await Contract.Get(id);
            Verifier.Equal(model, expected);
        }

        [WithArgs]
        [Args(0)]
        [Args(3)]
        public override async Task GetAll_ReturnAll(int limitOfResult = 0)
        {
            Setup((m) => m.GetAll(limitOfResult), Items);
            var expected = await Contract.GetAll(limitOfResult);

            if (limitOfResult == 0)
                limitOfResult = Models.Count;
            Verifier.Equal(limitOfResult, expected.Count());
        }

        [WithArgs]
        [Args(-1)]
        public override async Task GetAll_ReturnNull(int limitOfResult = 0)
        {
            Setup((m) => m.GetAll(limitOfResult), new TModelInterface[] { });
            var expected = await Contract.GetAll(limitOfResult);
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
            var returnModel = ToDTO(model);
            Setup((m) => m.Add(model), returnModel);
            expected = await Contract.Add(model);
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
            var model = Fixture.Create<TModel>();
            var returnModel = ToDTO(model);
            Setup((m) => m.Delete(model.ID), returnModel);
            expected = await Contract.Delete(model.ID);
            Verifier.Equal(returnModel, expected);
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
            var model = Fixture.Create<TModel>();
            var returnModel = ToDTO(model);
            Setup((m) => m.Update(model.ID, model), returnModel);
            expected = await Contract.Update(model.ID, model);
            Verifier.Equal(returnModel, expected);
        }
#endif
        //+:cnd:noEmit
        #endregion
    }
}
//-:cnd:noEmit
#endif
//+:cnd:noEmit

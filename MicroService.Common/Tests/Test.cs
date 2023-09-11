/* Licensed under the MIT/X11 license.
* This notice may not be removed from any source distribution.
Author: Mukesh Adhvaryu.
*/

//-:cnd:noEmit
#if MODEL_ADDTEST
//+:cnd:noEmit

using AutoFixture;
using AutoFixture.AutoMoq;

using MicroService.Common.Models;
using MicroService.Common.Tests.Attributes;

namespace MicroService.Common.Tests
{
    [Testable]
    public abstract class Test<TModelInterface, TModel, TIDType> 
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
        static Type modelInterFaceType = typeof(TModelInterface);

        protected readonly IFixture Fixture;
        //-:cnd:noEmit
#if MODEL_USEDTO
        static readonly Type DTOType = typeof(TModelInterface);
        static readonly bool NeedToUseDTO = !DTOType.IsAssignableFrom(typeof(TModel));
#endif
        //+:cnd:noEmit
        #endregion

        #region CONSTRUCTOR
        public Test()
        {

            Fixture = new Fixture().Customize(new AutoMoqCustomization { ConfigureMembers = true });
        }
        #endregion

        #region GET MODEL/S
        //-:cnd:noEmit
#if !MODEL_nonreadable
        [NoArgs]
        public abstract Task Get_ReturnSingle();

        [NoArgs]
        public abstract Task Get_ReturnSingleFail();

        [WithArgs]
        [Args(0)]
        [Args(3)]
        public abstract Task GetAll_ReturnAll(int limitOfResult = 0);

        [WithArgs]
        [Args(-1)]
        public abstract Task GetAll_ReturnNull(int limitOfResult = 0);
#endif
        //+:cnd:noEmit
        #endregion

        #region ADD MODEL
        //-:cnd:noEmit
#if MODEL_APPENDABLE
        [NoArgs]
        public abstract Task Add_ReturnAdded();
#endif
        //+:cnd:noEmit
        #endregion

        #region DELETE MODEL
        //-:cnd:noEmit
#if MODEL_DELETABLE
        [NoArgs]
        public abstract Task Delete_ReturnDeleted();
#endif
        //+:cnd:noEmit
        #endregion

        #region UPDATE MODEL
        //-:cnd:noEmit
#if MODEL_UPDATABLE
        [NoArgs]
        public abstract Task Update_ReturnUpdated();
#endif
        //+:cnd:noEmit
        #endregion

        #region TO DTO
        //-:cnd:noEmit
#if (MODEL_USEDTO)
        protected TModelInterface? ToDTO(TModel? model)
        {
            if(model == null)
                return default(TModelInterface);
            if (NeedToUseDTO)
                return (TModelInterface)((IExModel)model).ToDTO(DTOType);
            return (TModelInterface)(object)model;
        }
#else
        protected TModelInterface? ToDTO(TModel? model)
        {
            if(model == null)
                return default(TModelInterface);
            return (TModelInterface)(object)model;
        }
#endif
        //+:cnd:noEmit
        #endregion
    }
}
//-:cnd:noEmit
#endif
//+:cnd:noEmit

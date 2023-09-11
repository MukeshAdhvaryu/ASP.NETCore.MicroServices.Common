/* Licensed under the MIT/X11 license.
* This notice may not be removed from any source distribution.
Author: Mukesh Adhvaryu.
*/

//-:cnd:noEmit
#if MODEL_ADDTEST
//+:cnd:noEmit

using System.ComponentModel.DataAnnotations;

using MicroService.Common.Attributes;
using MicroService.Common.Interfaces;
using MicroService.Common.Models;
using MicroService.Common.Parameters;

namespace MicroService.Common.Tests
{
    //-:cnd:noEmit
#if MODEL_USEDTO
    public interface ITestModelDTO: IModel
    {
        string Name { get; }
    }
    public class TestModelDTO : ITestModelDTO
    {
        public string Name { get; }

        public TestModelDTO(TestModel model)
        {
            Name = model.Name;
        }
    }
#endif
    //+:cnd:noEmit

    [Model(ProvideSeedData = true)]
    public class TestModel: Model<int>
    {
        static int iid;
        public TestModel(string name) :
            base(true)
        {
            Name = name;
        }
        public TestModel() :
            base(false)
        { 

        }

        [Required]  
        public string? Name { get; set; }

        protected override void Update(IValueStore<string> value, out BindingResultStatus notification, out string message)
        {
            notification = BindingResultStatus.Sucess;
            message = string.Empty;
        }

        protected override Task<bool> CopyFrom(IModel model)
        {
            if(!(model is TestModel))
                return Task.FromResult(false);
            Name = ((TestModel)model).Name;
            return Task.FromResult(true);
        }

        protected override IEnumerable<IModel> GetInitialData()
        {
            return new TestModel[]
            {
               new TestModel("John"),
               new TestModel("Timothy"),
               new TestModel("Charlie"),
               new TestModel("Felicity"),
               new TestModel("Margot"),
            };
        }

        protected override int GetNewID()
        {
            return (++iid);
        }
        protected override bool Match(string propertyName, object value)
        {
            return (propertyName == "Name" && Equals(value , Name));
        }

        //-:cnd:noEmit
#if MODEL_USEDTO
        #region IModelToDTO
        protected override IModel ToDTO(Type type)
        {
            if (type == typeof(ITestModelDTO))
                return new TestModelDTO(this);
            return base.ToDTO(type);
        }
        #endregion
#endif
        //+:cnd:noEmit
    }
}
//-:cnd:noEmit
#endif
//+:cnd:noEmit

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

namespace MicroService.Common.Tests
{
    #region TestModel
    [DBConnect(ProvideSeedData = true)]
    public sealed class TestModel: ModelInt32<TestModel>
    {
        #region CONSTRUCTORS
        public TestModel(string name) :
            base(true)
        {
            Name = name;
        }
        public TestModel() :
            base(false)
        { 

        }
        #endregion

        #region PROPERTIES
        [Required]  
        public string? Name { get; set; }
        #endregion

        #region GET PROEPRTY NAMES
        protected override IReadOnlyList<string> GetPropertyNames(bool forSearch = false) =>
            new string[] { nameof(ID) };
        #endregion

        #region COPY FROM
        protected override Task<bool> CopyFrom(IModel model)
        {
            if (!(model is TestModel))
                return Task.FromResult(false);
            Name = ((TestModel)model).Name;
            return Task.FromResult(true);
        }
        #endregion

        #region GET INITIAL DATA
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
        #endregion

        #region PARSE
        protected override Message Parse(IParameter parameter, out object? currentValue, out object? parsedValue, bool updateValueIfParsed = false)
        {
            var value = parameter is IModelParameter ? ((IModelParameter)parameter).FirstValue : parameter.Value;
            currentValue = parsedValue = null;
            var name = parameter.Name;

            switch (parameter.Name)
            {
                case nameof(Name):
                    currentValue = Name;
                    if (value is string)
                    {
                        parsedValue = (string)value;
                        return Message.Sucess(name);
                    }
                    if (value == null)
                        return Message.MissingRequiredValue(name);
                    break;
                default:
                    break;
            }
            return Message.Failure(name);
        }
        #endregion

        //-:cnd:noEmit
#if MODEL_USEDTO
        #region IModelToDTO
        protected override IModel? ToDTO(Type type)
        {
            if (type == typeof(ITestModelDTO))
                return new TestModelDTO(this);
            return base.ToDTO(type);
        }
        #endregion
#endif
        //+:cnd:noEmit
    }
    #endregion
}
//-:cnd:noEmit
#endif
//+:cnd:noEmit

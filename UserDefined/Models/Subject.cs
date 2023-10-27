using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using MicroService.Common;
using MicroService.Common.Attributes;
using MicroService.Common.Interfaces;
using MicroService.Common.Models;
using MicroService.Common.Parameters;
using MicroService.Common.Services;

//-:cnd:noEmit
#if MODEL_USEDTO
using UserDefined.DTOs;
#endif
//+:cnd:noEmit

namespace UserDefined.Models
{
    public enum Faculty : byte
    {
        Arts,
        Commerce,
        Science

    }

    #region ISubject
    public interface ISubject : IModel<int>
    {
        [Required]
        string? Name { get; }

        [Required]
        Faculty Faculty { get; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        string Description { get; }
    }
    #endregion

    #region Subject
    [Model(Scope = ServiceScope.Scoped, Name = "SubjectCmd")]
    //[DBConnect(Database = "SubjectDB", ConnectionKey = ConnectionKey.SQLServer)]
    [DBConnect(ProvideSeedData = true)]
    public class Subject : ModelInt32<Subject>, ISubject
    {
        #region VARIABLES
        Faculty faculty;
        #endregion

        #region CONSTRUCTORS
        public Subject() : 
            base(false)
        { }
        public Subject(string name, Faculty stream):
            base(true)
        {
            Name = name;
            faculty = stream;
        }
        public Subject(int id, string name, Faculty stream) :
            base(false)
        {
            this.ID = id;
            Name = name;
            faculty = stream;
        }
        #endregion

        #region PROPERTIES
        [Required]
        public string? Name { get; internal set; }

        [Required]
        public Faculty Faculty { get => faculty; internal set => faculty = value; }


        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public string Description => Faculty + ": " + Name;
        #endregion

        #region PARSE
        protected override Message Parse(IParameter parameter, out object? currentValue, out object? parsedValue, bool updateValueIfParsed = false)
        {
            var valueFromParameter = parameter is IModelParameter? ((IModelParameter)parameter).FirstValue: parameter.Value;            
            currentValue =  null;
            parsedValue = null;
            var name = parameter.Name.ToLower();

            switch (name)
            {
                case "name":
                    currentValue = Name;
                    if (valueFromParameter is string)
                    {
                        var result = (string)valueFromParameter;
                        parsedValue = result;
                        if (updateValueIfParsed)
                            Name = result;
                        return Message.Sucess(name);
                    }
                    if (valueFromParameter == null)
                        return Message.MissingRequiredValue(name);

                    break;
                case "faculty":
                    currentValue = faculty;
                    Faculty f;
                    if (valueFromParameter is Faculty)
                    {
                        var result = (Faculty)valueFromParameter;
                        parsedValue = result;
                        if (updateValueIfParsed)
                            faculty = result;
                        return Message.Sucess(name);
                    }
                    if (valueFromParameter is string && (Enum.TryParse((string)valueFromParameter, out f)) ||
                        valueFromParameter != null && (Enum.TryParse(valueFromParameter.ToString(), out f)))
                    {
                        parsedValue = f;
                        if (updateValueIfParsed)
                            faculty = f;
                        return Message.Sucess(name);
                    }
                    if (valueFromParameter == null)
                        return Message.MissingValue(name);
                    break;
                default:
                    break;
            }
            return Message.Failure(name);
        }
        #endregion

        #region COPY FROM
        protected override Task<bool> CopyFrom(IModel model)
        {
            if (model is ISubject)
            {
                var subject = (ISubject)model;
                faculty = subject.Faculty;
                Name = subject.Name;
                return Task.FromResult(true);
            }

            //-:cnd:noEmit
#if MODEL_USEDTO
            if (model is ISubjectOutDTO)
            {
                var createSubjectDTO = (ISubjectOutDTO)model;
                Name = createSubjectDTO.Name;
                faculty = createSubjectDTO.Faculty;
                return Task.FromResult(true);
            }
            if (model is ISubjectInDTO)
            {
                var createSubjectDTO = (ISubjectInDTO)model;
                Name = createSubjectDTO.Name;
                faculty = createSubjectDTO.Faculty;
                return Task.FromResult(true);
            }
#endif
            //+:cnd:noEmit
            return Task.FromResult(false);
        }
        #endregion

        #region GET INITIAL DATA
        protected override IEnumerable<IModel> GetInitialData()
        {
            return new Subject[]
                {
                    new Subject("Psycology", Faculty.Science),
                    new Subject("Chemistry", Faculty.Science),
                    new Subject("Physics", Faculty.Science),
                    new Subject("Maths", Faculty.Science),
                    new Subject("Software Engineering", Faculty.Science),

                    new Subject("Geography", Faculty.Science),
                    new Subject("Economics", Faculty.Commerce),
                    new Subject("Accountancy", Faculty.Commerce),
                    new Subject("English", Faculty.Arts),
                    new Subject("Hindi", Faculty.Arts),
                    new Subject("French", Faculty.Arts),
                    new Subject("Historty", Faculty.Arts),
                };
        }
        #endregion

        #region Model To DTO
        //-:cnd:noEmit
#if MODEL_USEDTO
        protected override IModel? ToDTO(Type type)
        {
           if(type == typeof(ISubjectOutDTO) || type == typeof(SubjectOutDTO))
                return new SubjectOutDTO(this);
            if (type == typeof(ISubjectInDTO) || type == typeof(SubjectInDTO))
                return new SubjectInDTO(this);
            return base.ToDTO(type);
        }

#endif
        //+:cnd:noEmit
        #endregion
    }
    #endregion
}
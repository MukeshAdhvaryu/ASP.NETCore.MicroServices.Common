using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using MicroService.Common.Attributes;
using MicroService.Common.Interfaces;
using MicroService.Common.Models;
using MicroService.Common.Parameters;
using MicroService.Common.Services;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

//-:cnd:noEmit
#if MODEL_USEDTO
using UserDefined.DTOs;
#endif
//+:cnd:noEmit

using JsonConverter = System.Text.Json.Serialization.JsonConverter;

namespace UserDefined.Models
{
    public enum Faculty : byte
    {
        Arts,
        Commerce,
        Science

    }

    #region ISubject
    [Model(Scope = ServiceScope.Scoped)]
    public interface ISubject : IModel<int>
    {
        [Required]
        string? Name { get; }

        [Required]
        [JsonConverter(typeof(StringEnumConverter))]
        Faculty Faculty { get; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        string Description { get; }
    }
    #endregion

    #region Subject
    [Model(Scope = ServiceScope.Scoped, ProvideSeedData = true)]
    public class Subject : Model<int>, ISubject
    {
        #region VARIABLES
        static int iid;
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
            this.id = id;
            Name = name;
            faculty = stream;
        }
        #endregion

        #region PROPERTIES
        [Required]
        public string? Name { get; internal set; }

        [Required]
        [JsonConverter(typeof(StringEnumConverter))]
        public Faculty Faculty { get => faculty; internal set => faculty = value; }


        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public string Description => Faculty + ": " + Name;

        protected sealed override IReadOnlyList<string> Properties => 
            new string[] 
            { 
                nameof(Name),
                nameof(Faculty),
                nameof(ID),
            };
        #endregion

        #region UPDATE
        protected override void Update(IValueStore<string> value, out BindingResultStatus notification, out string message)
        {
            var name = value.Name;
            string messageStr = "{0} of {1}";
            message = string.Format(messageStr, "Success of value assignment", name);
            ;

            notification = 0;
            if(value.IsEmpty)
            {
                if(name == nameof(Name) || name == nameof(Faculty))
                {
                    message = string.Format(messageStr, "Missing required value " , name);
                    notification = BindingResultStatus.MissingRequiredValue;
                }
                else
                {
                    message = string.Format(messageStr, "Missing value", name);
                    notification = BindingResultStatus.MissingValue;
                }
                goto EXIT;
            }
            switch (name)
            {
                case nameof(Faculty):
                    if (Enum.TryParse(value.FirstValue, out faculty))
                        goto EXIT;
                    message = string.Format(messageStr, "Failure of value assignment", name);
                    notification = BindingResultStatus.Failure;
                    break;
                case nameof(Name):
                    Name = value.FirstValue;
                    goto EXIT;
                case nameof(ID):
                    if(int.TryParse(value.FirstValue, out id))
                        goto EXIT;
                    message = string.Format(messageStr, "Failure of required value assignment", name);
                    notification = BindingResultStatus.Failure;
                    break;
                default:
                    message = string.Format(messageStr, "Ignored value assignment", name);
                    notification = BindingResultStatus.Ignored;
                    break;
            }
            EXIT:
            return;
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
            if (model is ISubjectDTO)
            {
                var createSubjectDTO = (ISubjectDTO)model;
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

        #region GET NEW ID
        protected override int GetNewID()
        {
            return ++iid;
        }
        #endregion

        #region MATCH
        protected override bool Match(string propertyName, object value)
        {
            switch (propertyName)
            {
                case nameof(Name):
                    if (value is string)
                        return (string)value == Name;
                    break;
                case nameof(Faculty):
                    if (value is Faculty)
                        return faculty == (Faculty)value;
                    if (value is string && (Enum.TryParse((string)value, out faculty)))
                        return true;
                    if (value != null && (Enum.TryParse(value.ToString(), out faculty)))
                        return true;
                    break;
                default:
                    return false;
            }
            return false;
        }
        #endregion

        #region Model To DTO
        //-:cnd:noEmit
#if MODEL_USEDTO
        protected override IModel ToDTO(Type type)
        {
           if(type == typeof(ISubjectDTO) || type == typeof(SubjectDTO))
                return new SubjectDTO(this);
            return base.ToDTO(type);
        }
#endif
        //+:cnd:noEmit
        #endregion
    }
    #endregion
}
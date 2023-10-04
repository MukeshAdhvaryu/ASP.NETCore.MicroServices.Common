using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

using MicroService.Common;
using MicroService.Common.Attributes;
using MicroService.Common.Interfaces;
using MicroService.Common.Models;
using MicroService.Common.Services;

using Microsoft.AspNetCore.Mvc;

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
    [Model(Scope = ServiceScope.Scoped, Name = "Subject")]
    //[DBConnect(Database = "SubjectDB", ConnectionKey = ConnectionKey.SQLServer)]
    [DBConnect(ProvideSeedData = true)]
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
        public Faculty Faculty { get => faculty; internal set => faculty = value; }


        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public string Description => Faculty + ": " + Name;

        protected sealed override IReadOnlyList<string> GetPropertyNames(bool forSerch = false) => 
            new string[] 
            { 
                nameof(Name),
                nameof(Faculty),
                nameof(ID),
            };
        #endregion

        #region PARSE
        protected override Message Parse(IParameter parameter, out object currentValue, out object parsedValue, bool updateValueIfParsed = false)
        {
            var value = parameter is IModelParameter? ((IModelParameter)parameter).FirstValue: parameter.Value;            
            currentValue =  null;
            parsedValue = null;
            var name = parameter.Name;

            switch (name)
            {
                case nameof(Name):
                    currentValue = Name;
                    if (value is string)
                    {
                        var result = (string)value;
                        parsedValue = result;
                        if (updateValueIfParsed)
                            Name = result;
                        return Message.Sucess(name);
                    }
                    if (value == null)
                        return Message.MissingRequiredValue(name);

                    break;
                case nameof(Faculty):
                    currentValue = faculty;
                    Faculty f;
                    if (value is Faculty)
                    {
                        var result = (Faculty)value;
                        parsedValue = result;
                        if (updateValueIfParsed)
                            faculty = result;
                        return Message.Sucess(name);
                    }
                    if (value is string && (Enum.TryParse((string)value, out f)) ||
                        value != null && (Enum.TryParse(value.ToString(), out f)))
                    {
                        parsedValue = f;
                        if (updateValueIfParsed)
                            faculty = f;
                        return Message.Sucess(name);
                    }
                    if (value == null)
                        return Message.MissingValue(name);
                    break;
                case nameof(ID):
                    currentValue = ID;
                    
                    if (value is int)
                    {
                        parsedValue = parameter.Value;
                        if (updateValueIfParsed && id == 0)
                            id = (int)parsedValue;
                        return Message.Sucess(name);
                    }
                    else if (int.TryParse(value.ToString(), out int i))
                    {
                        currentValue = id;
                        parsedValue = i;
                        if (updateValueIfParsed && id == 0)
                            id = i;
                        return Message.Sucess(name);
                    }
                    return Message.Ignored(name);
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

        #region GET NEW ID
        protected override int GetNewID()
        {
            return ++iid;
        }
        #endregion

        #region TRY PARSE ID
        protected override bool TryParseID(object value, out int newID) =>
            int.TryParse(value.ToString(), out newID);
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
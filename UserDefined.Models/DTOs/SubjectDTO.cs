//-:cnd:noEmit
#if MODEL_USEDTO
//+:cnd:noEmit
using System.ComponentModel.DataAnnotations;

using MicroService.Common.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

using UserDefined.Models;

using JsonConverter = System.Text.Json.Serialization.JsonConverter;

namespace UserDefined.DTOs
{
    //[Model(Scope = MicroService.Common.Services.ServiceScope.Singleton)]
    public interface ISubjectDTO : IModel
    {
        [Required]
        string? Name { get; }

        [Required]
        [JsonConverter(typeof(StringEnumConverter))]
        Faculty Faculty { get; }
    }

    //[Model(Scope = MicroService.Common.Services.ServiceScope.Singleton)]
    public class SubjectDTO : ISubjectDTO
    {
        public SubjectDTO(ISubject subject)
        {
            Name = subject.Name;
            Faculty = subject.Faculty;
        }
        public string? Name { get; }
        public Faculty Faculty { get; }
    }
}

//-:cnd:noEmit
#endif
//+:cnd:noEmit

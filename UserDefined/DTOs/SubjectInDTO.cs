//-:cnd:noEmit
#if MODEL_USEDTO
//+:cnd:noEmit
using System.ComponentModel.DataAnnotations;

using MicroService.Common.Models;

using UserDefined.Models;
using MicroService.Common.Attributes;

namespace UserDefined.DTOs
{
    //[Model(Scope = MicroService.Common.Services.ServiceScope.Singleton)]
    [Model(Name = "SubjectIn")]
    public interface ISubjectInDTO : IModel
    {
        [Required]
        string? Name { get; }

        [Required]
        Faculty Faculty { get; }
    }

    //[Model(Scope = MicroService.Common.Services.ServiceScope.Singleton)]
    [Model(Name = "SubjectIn")]
    public class SubjectInDTO : ISubjectInDTO
    {
        public SubjectInDTO(ISubject subject)
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

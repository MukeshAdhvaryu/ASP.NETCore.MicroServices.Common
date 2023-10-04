//-:cnd:noEmit
#if MODEL_USEDTO
//+:cnd:noEmit
using System.ComponentModel.DataAnnotations;

using MicroService.Common.Models;

using UserDefined.Models;

namespace UserDefined.DTOs
{
    public interface ISubjectInDTO : IModel
    {
        [Required]
        string? Name { get; }

        [Required]
        Faculty Faculty { get; }
    }

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

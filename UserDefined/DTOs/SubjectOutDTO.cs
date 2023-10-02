//-:cnd:noEmit
#if MODEL_USEDTO
//+:cnd:noEmit
using MicroService.Common.Attributes;
using MicroService.Common.Models;

using UserDefined.Models;

namespace UserDefined.DTOs
{
    //[Model(Scope = MicroService.Common.Services.ServiceScope.Singleton)]
    [Model(Name = "SubjectOut")]
    public interface ISubjectOutDTO : IModel
    {
        int ID { get; }
        string? Name { get; }
        Faculty Faculty { get; }
    }

    //[Model(Scope = MicroService.Common.Services.ServiceScope.Singleton)]
    [Model(Name = "SubjectOut")]
    public class SubjectOutDTO : ISubjectOutDTO
    {
        public SubjectOutDTO(ISubject subject)
        {
            Name = subject.Name;
            Faculty = subject.Faculty;
            ID = subject.ID;
        }
        public string? Name { get; }
        public Faculty Faculty { get; }
        public int ID { get; }
    }
}

//-:cnd:noEmit
#endif
//+:cnd:noEmit

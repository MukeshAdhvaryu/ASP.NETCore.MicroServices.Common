//-:cnd:noEmit
#if MODEL_USEDTO
//+:cnd:noEmit
using MicroService.Common.Attributes;
using MicroService.Common.Models;

using UserDefined.Models;

namespace UserDefined.DTOs
{
    #region ISubjectOutDTO
    [Model(Name = "SubjectQry")]
    public interface ISubjectOutDTO : IModel
    {
        int ID { get; }
        string? Name { get; }
        Faculty Faculty { get; }
    }
    #endregion

    #region SubjectOutDTO
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
    #endregion
}

//-:cnd:noEmit
#endif
//+:cnd:noEmit

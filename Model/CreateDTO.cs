using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

using Model.Interface;

namespace Model
{
    public class CreateDTO : ICreateDTO
    {
        [DataMember(Name = "toDoId")]
        [Range(1, int.MaxValue)]
        public int ToDoId { get ; set ; }
        [DataMember(Name = "name")]
        [StringLength(maximumLength: 255)]
        [Required(AllowEmptyStrings = false)]
        public string Name { get ; set ; }
    }
}

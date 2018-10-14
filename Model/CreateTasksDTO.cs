using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

using Model.Interface;

namespace Model
{
    public class CreateTasksDTO : ICreateTasksDTO
    {
        [DataMember(Name = "name")]
        [StringLength(maximumLength: 255)]
        [Required(AllowEmptyStrings = false)]
        public string Name { get; set; }
    }
}

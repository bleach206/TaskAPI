using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

using Model.Interface;

namespace Model
{
    public class TaskDTO : ITaskDTO
    {
        [DataMember(Name = "id")]
        [Range(1, int.MaxValue)]
        public int Id { get; set; }
        [DataMember(Name = "name")]
        [StringLength(maximumLength: 255)]
        [Required(AllowEmptyStrings = false)]
        public string Name { get; set; }        
        [DataMember(Name = "isCompleted")]
        public bool IsCompleted { get; set; }
        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
}

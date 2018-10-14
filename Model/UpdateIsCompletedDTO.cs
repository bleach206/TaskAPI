using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

using Model.Interface;

namespace Model
{
    public class UpdateIsCompletedDTO : IUpdateIsCompletedDTO
    {
        [DataMember(Name = "userId")]
        [Range(1, int.MaxValue)]
        [Required]
        public int UserId { get; set; }
        public bool IsCompleted { get; set; }
    }
}

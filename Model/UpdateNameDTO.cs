using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

using Model.Interface;

namespace Model
{
    public class UpdateNameDTO : IUpdateNameDTO
    {
        [DataMember(Name = "userId")]
        [Range(1, int.MaxValue)]
        [Required]
        public int UserId { get; set; }
        [DataMember(Name = "name")]
        [StringLength(maximumLength: 255)]
        [Required(AllowEmptyStrings = false)]
        public string Name { get; set; }
    }
}

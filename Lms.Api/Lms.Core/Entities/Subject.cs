using System.ComponentModel.DataAnnotations;

namespace Lms.API.Core.Entities
{
    public class Subject
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(30)]
        public string Title { get; set; }
    }
}
using System.ComponentModel.DataAnnotations;

namespace Lms.API.Core.Entities
{
    public class Level
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(30)]
        public string Name { get; set; }
    }
}
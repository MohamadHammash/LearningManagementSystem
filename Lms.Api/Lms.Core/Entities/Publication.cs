using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Lms.API.Core.Entities
{
    public class Publication
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        public string Description { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime ReleaseDate { get; set; }

        public string Url { get; set; }

        public Subject Subject { get; set; }

        public Level Level { get; set; }

        [Required]
        public ICollection<Author> Authors { get; set; }
    }
}
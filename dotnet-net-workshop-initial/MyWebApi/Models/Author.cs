using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Policy;

namespace BookService.Models
{
    public class Author
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }

        public bool RestrictedAuthor { get; set; }

        [NotMapped]
        public double AverageBookScore { get; set; }

    }
}

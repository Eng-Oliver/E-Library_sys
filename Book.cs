using System.ComponentModel.DataAnnotations;

namespace LibrarySystem.Models
{
    public class Book
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Author { get; set; } = string.Empty;

        public int Year { get; set; }

        public string? CoverUrl { get; set; }
        public string? Description { get; set; }
        public string? FileUrl { get; set; }

       
        public int? PageCount { get; set; }   
        public double? Rating { get; set; }
        public int? GenreId { get; set; }
        public Genre? Genre { get; set; }
    }
}

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LibrarySystem.Models
{
    public class UserRead
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int BookId { get; set; }
        public DateTime ReadDate { get; set; } = DateTime.Now;

        public User User { get; set; }
        public Book Book { get; set; }
    }
}
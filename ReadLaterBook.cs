using System;

namespace LibrarySystem.Models
{
    public class ReadLaterBook
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int BookId { get; set; }
        public DateTime AddedAt { get; set; } = DateTime.Now;

        public User User { get; set; }
        public Book Book { get; set; }
    }
}

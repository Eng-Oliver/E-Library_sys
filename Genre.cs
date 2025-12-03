using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LibrarySystem.Models
{
	public class Genre
	{
		public int Id { get; set; }

		[Required]
		[StringLength(100)]
		public string Name { get; set; } = string.Empty; // اسم التصنيف

		// علاقة التصنيف مع الكتب
		public ICollection<Book>? Books { get; set; }
	}
}

using LibrarySystem.Data;
using LibrarySystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibrarySystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReadLaterApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ReadLaterApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ✅ عرض كل الكتب في القراءة لاحقًا
        [HttpGet("{userId}")]
        public async Task<ActionResult<IEnumerable<Book>>> GetReadLater(int userId)
        {
            var readLaterBooks = await _context.ReadLaterBooks
                .Include(r => r.Book)
                .ThenInclude(b => b.Genre)
                .Where(r => r.UserId == userId)
                .Select(r => r.Book)
                .ToListAsync();

            if (!readLaterBooks.Any())
                return Ok(new { message = "لا توجد كتب في قائمة القراءة لاحقًا.", readLater = new List<Book>() });

            return Ok(new { message = "تم جلب قائمة القراءة لاحقًا ✅", readLater = readLaterBooks });
        }

        // ✅ إضافة كتاب لقائمة القراءة لاحقًا
        [HttpPost]
        public async Task<IActionResult> AddReadLater([FromBody] ReadLaterBook readLater)
        {
            if (readLater == null || readLater.UserId <= 0 || readLater.BookId <= 0)
                return BadRequest(new { message = "بيانات غير صحيحة." });

            bool exists = await _context.ReadLaterBooks
                .AnyAsync(r => r.UserId == readLater.UserId && r.BookId == readLater.BookId);

            if (exists)
                return BadRequest(new { message = "هذا الكتاب موجود بالفعل في القراءة لاحقًا." });

            _context.ReadLaterBooks.Add(readLater);
            await _context.SaveChangesAsync();

            return Ok(new { message = "تمت إضافة الكتاب إلى القراءة لاحقًا بنجاح ⏳" });
        }

        // ✅ حذف كتاب من القراءة لاحقًا
        [HttpDelete("{userId}/{bookId}")]
        public async Task<IActionResult> RemoveReadLater(int userId, int bookId)
        {
            var readLater = await _context.ReadLaterBooks
                .FirstOrDefaultAsync(r => r.UserId == userId && r.BookId == bookId);

            if (readLater == null)
                return NotFound(new { message = "الكتاب غير موجود في قائمة القراءة لاحقًا." });

            _context.ReadLaterBooks.Remove(readLater);
            await _context.SaveChangesAsync();

            return Ok(new { message = "تم حذف الكتاب من قائمة القراءة لاحقًا 🗑️" });
        }
    }
}

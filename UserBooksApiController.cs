using LibrarySystem.Data;
using LibrarySystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace LibrarySystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserBooksApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UserBooksApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // =============================
        // ✅ عرض الكتب المفضلة للمستخدم
        // =============================
        [HttpGet("favorites/{userId}")]
        public async Task<ActionResult<IEnumerable<Book>>> GetFavoriteBooks(int userId)
        {
            var favs = await _context.FavoriteBooks
                .Include(f => f.Book)
                .ThenInclude(b => b.Genre)
                .Where(f => f.UserId == userId)
                .Select(f => f.Book)
                .ToListAsync();

            if (!favs.Any())
                return Ok(new { message = "لا توجد كتب مفضلة بعد.", favorites = new List<Book>() });

            return Ok(new { message = "تم جلب المفضلات بنجاح ✅", favorites = favs });
        }

        // =============================
        // ✅ إضافة كتاب إلى المفضلة
        // =============================
        [HttpPost("favorites")]
        public async Task<IActionResult> AddFavorite([FromBody] FavoriteBook favorite)
        {
            if (favorite == null || favorite.UserId <= 0 || favorite.BookId <= 0)
                return BadRequest(new { message = "بيانات غير صحيحة." });

            bool exists = await _context.FavoriteBooks
                .AnyAsync(f => f.UserId == favorite.UserId && f.BookId == favorite.BookId);

            if (exists)
                return BadRequest(new { message = "هذا الكتاب موجود بالفعل في المفضلة." });

            _context.FavoriteBooks.Add(favorite);
            await _context.SaveChangesAsync();

            return Ok(new { message = "تمت إضافة الكتاب إلى المفضلة بنجاح ❤️" });
        }

        // =============================
        // ✅ حذف كتاب من المفضلة
        // =============================
        [HttpDelete("favorites/{userId}/{bookId}")]
        public async Task<IActionResult> RemoveFavorite(int userId, int bookId)
        {
            var fav = await _context.FavoriteBooks
                .FirstOrDefaultAsync(f => f.UserId == userId && f.BookId == bookId);

            if (fav == null)
                return NotFound(new { message = "الكتاب غير موجود في المفضلة." });

            _context.FavoriteBooks.Remove(fav);
            await _context.SaveChangesAsync();

            return Ok(new { message = "تم حذف الكتاب من المفضلة بنجاح 🗑️" });
        }

        // =============================
        // ✅ عرض الكتب المقروءة للمستخدم
        // =============================
        [HttpGet("reads/{userId}")]
        public async Task<ActionResult<IEnumerable<Book>>> GetReadBooks(int userId)
        {
            var reads = await _context.UserReads
                .Include(r => r.Book)
                .ThenInclude(b => b.Genre)
                .Where(r => r.UserId == userId)
                .Select(r => r.Book)
                .ToListAsync();

            if (!reads.Any())
                return Ok(new { message = "لا توجد كتب مقروءة بعد.", reads = new List<Book>() });

            return Ok(new { message = "تم جلب سجل القراءة بنجاح 📖", reads });
        }

        // =============================
        // ✅ حفظ كتاب تمت قراءته
        // =============================
        [HttpPost("reads")]
        public async Task<IActionResult> AddReadBook([FromBody] UserRead read)
        {
            if (read == null || read.UserId <= 0 || read.BookId <= 0)
                return BadRequest(new { message = "بيانات غير صحيحة." });

            bool alreadyRead = await _context.UserReads
                .AnyAsync(r => r.UserId == read.UserId && r.BookId == read.BookId);

            if (alreadyRead)
                return BadRequest(new { message = "تم تسجيل قراءة هذا الكتاب من قبل." });

            _context.UserReads.Add(read);
            await _context.SaveChangesAsync();

            return Ok(new { message = "تم حفظ الكتاب في سجل القراءة ✅" });
        }
    }
}

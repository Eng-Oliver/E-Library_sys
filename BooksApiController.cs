using System.Collections.Generic;
using System.Threading.Tasks;
using LibrarySystem.Data;
using LibrarySystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace LibrarySystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public BooksApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // =====================================
        // ✅ عرض كل الكتب (مع تضمين التصنيف)
        // =====================================
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Book>>> GetBooks()
        {
            // نستخدم Include لجلب بيانات التصنيف المرتبطة بكل كتاب
            return await _context.Books.Include(b => b.Genre).ToListAsync();
        }

        // =====================================
        // ✅ عرض كتاب معين (مع تضمين التصنيف)
        // =====================================
        [HttpGet("{id}")]
        public async Task<ActionResult<Book>> GetBook(int id)
        {
            // نستخدم Include لجلب بيانات التصنيف عند عرض التفاصيل
            var book = await _context.Books
                .Include(b => b.Genre)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (book == null)
                return NotFound();

            return book;
        }

        // =====================================
        // ✅ إضافة كتاب جديد (POST)
        // =====================================
        [HttpPost]
        public async Task<ActionResult<Book>> PostBook(Book book)
        {
            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            // نستخدم GetBook لجلب الكتاب المضاف مرة أخرى مع بيانات التصنيف (Include)
            return CreatedAtAction(nameof(GetBook), new { id = book.Id }, book);
        }

        // =====================================
        // ✅ تعديل كتاب موجود (PUT)
        // (تم التعديل لضمان حفظ كل الحقول مثل Pages و Rating)
        // =====================================
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBook(int id, Book book)
        {
            if (id != book.Id)
                return BadRequest();

            // 💡 ملاحظة: طريقة التحديث الأكثر أمانًا هي جلب الكائن الأصلي وتحديث خصائصه، 
            // لكن لتبسيط الكود، سنستخدم الطريقة التالية.

            // نحتاج فقط لتعيين الحالة إلى Modified
            _context.Entry(book).State = EntityState.Modified;

            // وهذا السطر يحمي علاقة الكائن (Genre) من محاولة إضافتها أو تحديثها ككيان مستقل
            _context.Entry(book).Reference(b => b.Genre).IsModified = false;

            await _context.SaveChangesAsync();
            return NoContent(); // 204 Success
        }

        // =====================================
        // ✅ حذف كتاب (DELETE)
        // =====================================
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null)
                return NotFound();

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
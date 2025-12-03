using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibrarySystem.Data;
using LibrarySystem.Models;

[Route("api/[controller]")]
[ApiController]
public class GenresApiController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public GenresApiController(ApplicationDbContext context)
    {
        _context = context;
    }

    // 1. POST: api/GenresApi (إضافة تصنيف جديد)
    [HttpPost]
    public async Task<ActionResult<Genre>> PostGenre(Genre genre)
    {
        // التحقق من اسم التصنيف (للتأكد من عدم تكراره أو أنه فارغ)
        if (string.IsNullOrWhiteSpace(genre.Name))
        {
            return BadRequest("اسم التصنيف مطلوب.");
        }

        // ASP.NET Core سيقوم تلقائيًا بملء حقل Id بـ 0 إذا لم يتم إرساله
        // ونحن نعتمد على قاعدة البيانات لتوليد رقم جديد.

        _context.Genres.Add(genre);
        await _context.SaveChangesAsync();

        // إرجاع النتيجة بنجاح
        return CreatedAtAction(nameof(GetGenre), new { id = genre.Id }, genre);
    }

    // 2. GET: api/GenresApi (جلب جميع التصنيفات)
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Genre>>> GetGenres()
    {
        return await _context.Genres.ToListAsync();
    }

    // 3. GET: api/GenresApi/5 (جلب تصنيف واحد)
    [HttpGet("{id}")]
    public async Task<ActionResult<Genre>> GetGenre(int id)
    {
        var genre = await _context.Genres.FindAsync(id);

        if (genre == null)
        {
            return NotFound();
        }

        return genre;
    }

    // 4. PUT: api/GenresApi/5 (تعديل تصنيف)
    [HttpPut("{id}")]
    public async Task<IActionResult> PutGenre(int id, Genre genre)
    {
        if (id != genre.Id)
        {
            return BadRequest("معرف التصنيف غير متطابق.");
        }

        _context.Entry(genre).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.Genres.Any(e => e.Id == id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent(); // 204 Success
    }

    // 5. DELETE: api/GenresApi/5 (حذف تصنيف)
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteGenre(int id)
    {
        var genre = await _context.Genres.FindAsync(id);
        if (genre == null)
        {
            return NotFound();
        }

        _context.Genres.Remove(genre);
        await _context.SaveChangesAsync();

        return NoContent(); // 204 Success
    }
}
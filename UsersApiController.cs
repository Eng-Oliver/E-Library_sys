using Microsoft.AspNetCore.Mvc;
using LibrarySystem.Data;
using LibrarySystem.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace LibrarySystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UsersApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ============================
        // ✅ عرض كل المستخدمين
        // ============================
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return await _context.Users.ToListAsync();
        }

        // ============================
        // ✅ البحث عن مستخدم
        // ============================
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<User>>> SearchUsers([FromQuery] string q)
        {
            if (string.IsNullOrWhiteSpace(q))
                return await _context.Users.ToListAsync();

            var results = await _context.Users
                .Where(u => u.Name.Contains(q) || u.Email.Contains(q))
                .ToListAsync();

            return Ok(results);
        }

        // ============================
        // ✅ تسجيل مستخدم جديد
        // ============================
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] User user)
        {
            if (await _context.Users.AnyAsync(u => u.Email.ToLower() == user.Email.ToLower()))
                return BadRequest(new { message = "البريد الإلكتروني مستخدم بالفعل ❌" });

            user.Email = user.Email.Trim().ToLower();
            user.Password = user.Password.Trim();

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "تم تسجيل المستخدم بنجاح ✅",
                id = user.Id,
                name = user.Name,
                email = user.Email
            });
        }

        // ============================
        // ✅ تسجيل الدخول
        // ============================
        public class LoginDto
        {
            public string Email { get; set; }
            public string Password { get; set; }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto login)
        {
            if (login == null)
                return BadRequest(new { message = "لم يتم إرسال بيانات الدخول" });

            var email = login.Email?.Trim().ToLower();
            var password = login.Password?.Trim();

            var user = await _context.Users
                .FirstOrDefaultAsync(u =>
                    u.Email.ToLower() == email &&
                    u.Password == password
                );

            if (user == null)
                return Unauthorized(new { message = "بيانات الدخول غير صحيحة ❌" });

            // ✅ نرجع البيانات بشكل منظم وواضح
            return Ok(new
            {
                id = user.Id,
                name = user.Name,
                email = user.Email
            });
        }

        // ============================
        // ✅ تعديل مستخدم
        // ============================
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] User user)
        {
            if (id != user.Id)
                return BadRequest(new { message = "❌ معرف المستخدم غير مطابق" });

            var existingUser = await _context.Users.FindAsync(id);
            if (existingUser == null)
                return NotFound(new { message = "❌ المستخدم غير موجود" });

            existingUser.Name = user.Name;
            existingUser.Email = user.Email.Trim().ToLower();
            existingUser.Password = user.Password;

            await _context.SaveChangesAsync();
            return Ok(new { message = "✅ تم تعديل المستخدم بنجاح" });
        }

        // ============================
        // ✅ حذف مستخدم
        // ============================
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound(new { message = "❌ المستخدم غير موجود" });

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return Ok(new { message = "✅ تم حذف المستخدم بنجاح" });
        }
    }
}

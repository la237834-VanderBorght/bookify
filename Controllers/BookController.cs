using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Bookify.Data;
using Bookify.Services;
using System.Text.Json;
using Bookify.Models;

namespace Bookify.Controllers
{
    /// <summary>
    /// Controller pour gérer les opérations liées aux livres
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private readonly ApplicationDb _context;
        public BookController(ApplicationDb context)
        {
            _context = context;
        }

        /// <summary>
        /// Récupérer tous les livres (avec le nom du genre)
        /// </summary>
        [HttpGet] // GET: api/book
        public async Task<IActionResult> GetAllBooks()
        {
            // JOIN explicite -> pas d'Include sur la FK
            var books = await (
                from b in _context.Books
                join g in _context.Genders on b.GenderId equals g.Id
                select new
                {
                    b.Id,
                    b.Title,
                    b.Author,
                    b.ISBN,
                    b.Price,
                    b.Description,
                    b.Publisher,
                    b.GenderId,
                    GenderName = g.Name
                }
            ).ToListAsync();

            return Ok(books);
        }

        /// <summary>
        /// Récupérer un livre par ID (avec le nom du genre)
        /// </summary>
        [HttpGet("{id:int}")] // GET: api/book/5
        public async Task<IActionResult> GetBookById(int id)
        {
            var book = await (
                from b in _context.Books
                join g in _context.Genders on b.GenderId equals g.Id
                where b.Id == id
                select new
                {
                    b.Id,
                    b.Title,
                    b.Author,
                    b.ISBN,
                    b.Price,
                    b.Description,
                    b.Publisher,
                    b.GenderId,
                    GenderName = g.Name
                }
            ).FirstOrDefaultAsync();

            if (book == null) return NotFound(new { Message = "Book not found" });
            return Ok(book);
        }

        /// <summary>
        /// Créer un nouveau livre
        /// </summary>
        [HttpPost] // POST: api/book
        public async Task<IActionResult> Create([FromBody] Book book)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            if(book.Gender != null)
            {
                var genderExists = await _context.Genders
                    .FirstOrDefaultAsync(g => g.Id == book.Gender.Id || g.Name == book.Gender.Name);

                if (genderExists != null)
                {
                    book.GenderId = genderExists.Id;
                    book.Gender = null;
                }
                else
                {
                    _context.Genders.Add(book.Gender);
                    await _context.SaveChangesAsync();

                    book.GenderId = book.Gender.Id;
                    book.Gender = null;
                }
            }
            else
            {
                var genderExists = await _context.Genders.AnyAsync(g => g.Id == book.GenderId);
                if (!genderExists) return BadRequest(new { Message = "Invalid GenderId" });
            }

            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBookById), new { id = book.Id }, book);
        }

        /// <summary>
        /// Mettre à jour un livre
        /// </summary>
        [HttpPut("{id:int}")] // PUT: api/book/5
        public async Task<IActionResult> Update(int id, [FromBody] Book book)
        {
            if (id != book.Id) return BadRequest(new { Message = "ID mismatch" });
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var exists = await _context.Books.AnyAsync(b => b.Id == id);
            if (!exists) return NotFound(new { Message = "Book not found" });

            var genderExists = await _context.Genders.AnyAsync(g => g.Id == book.GenderId);
            if (!genderExists) return BadRequest(new { Message = "Invalid GenderId" });

            _context.Entry(book).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Supprimer un livre
        /// </summary>
        [HttpDelete("{id:int}")] // DELETE: api/book/5
        public async Task<IActionResult> Delete(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null) return NotFound(new { Message = "Book not found" });

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>
        /// Rediriger vers Amazon.com pour le livre (via ISBN en DB -> Open Library -> ASIN)
        /// </summary>
        [HttpGet("{id:int}/amazon")]
        public async Task<IActionResult> RedirectBookToAmazon(
            int id,
            [FromServices] OpenLibraryService ol,
            [FromServices] AmazonLinkBuilder amazon,
            CancellationToken ct = default)
        {
            var book = await _context.Books.AsNoTracking().FirstOrDefaultAsync(b => b.Id == id, ct);
            if (book == null) return NotFound(new { Message = "Livre introuvable" });

            var isbn = (book.ISBN ?? "").Trim();
            if (string.IsNullOrWhiteSpace(isbn)) return BadRequest(new { Message = "Ce livre n’a pas d’ISBN" });

            string? asin = null;
            try { asin = await ol.GetAmazonAsinAsync(isbn, ct); } catch { /* fallback */ }

            var url = amazon.BuildProductOrSearchUrl(isbn, asin); // configuré en .com
            return Redirect(url.ToString());
        }
    }
}

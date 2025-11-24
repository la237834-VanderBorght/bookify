using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Bookify.Models;
using Bookify.Data;

namespace Bookify.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LivreLuController : ControllerBase
    {
        private readonly ApplicationDb _context;

        public LivreLuController(ApplicationDb context)
        {
            _context = context;
        }

        // Récupérer tous les livres lus par un utilisateur
        [HttpGet("{userId:int}")]
        public async Task<IActionResult> GetLivreLus(int userId)
        {
            var livresLus = await _context.LivreLu
                .Include(r => r.Book)
                .Include(r => r.User)
                .Where(r => r.UserId == userId)
                .Select(r => new
                {
                    r.Id,
                    r.BookId,
                    r.Book.Title,
                    r.Book.Author,
                    r.Book.ISBN,
                    r.Book.Price,
                    r.Book.Publisher,
                    r.DateLu,
                    UserName = r.User.Username
                })
                .ToListAsync();

            return Ok(livresLus);
        }

        // Ajouter un livre à la liste des livres lus pour un utilisateur
        [HttpPost("{bookId:int}/{userId:int}")]
        public async Task<IActionResult> AddLivreLu(int bookId, int userId)
        {
            var bookExists = await _context.Books.AnyAsync(b => b.Id == bookId);
            if (!bookExists)
                return NotFound(new { Message = "Livre introuvable." });

            var userExists = await _context.Users.AnyAsync(u => u.Id == userId);
            if (!userExists)
                return NotFound(new { Message = "Utilisateur introuvable." });

            var alreadyInList = await _context.LivreLu.AnyAsync(r => r.BookId == bookId && r.UserId == userId);
            if (alreadyInList)
                return BadRequest(new { Message = "Ce livre est déjà marqué comme lu par cet utilisateur." });

            var livreLu = new LivreLu { BookId = bookId, UserId = userId, DateLu = DateTime.Now };
            _context.LivreLu.Add(livreLu);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Livre ajouté avec succès à la liste des livres lus." });
        }

        // Supprimer un livre de la liste des livres lus pour un utilisateur
        [HttpDelete("{bookId:int}/{userId:int}")]
        public async Task<IActionResult> RemoveLivreLu(int bookId, int userId)
        {
            var item = await _context.LivreLu.FirstOrDefaultAsync(r => r.BookId == bookId && r.UserId == userId);
            if (item == null)
                return NotFound(new { Message = "Livre non présent dans la liste des livres lus pour cet utilisateur." });

            _context.LivreLu.Remove(item);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Livre retiré avec succès de la liste des livres lus." });
        }
    }
}
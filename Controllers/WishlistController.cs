using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Bookify.Models;
using Bookify.Data;

namespace Bookify.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WishlistController : ControllerBase
    {
        private readonly ApplicationDb _context;

        public WishlistController(ApplicationDb context)
        {
            _context = context;
        }

        // Récupérer les livres de la wishlist pour un user donné
        // GET api/wishlist/{userId}
        [HttpGet("{userId:int}")]
        public async Task<IActionResult> GetWishlist(int userId)
        {
            var userExists = await _context.Users.AnyAsync(u => u.Id == userId);
            if (!userExists)
                return NotFound(new { Message = "Utilisateur introuvable." });

            var wishlist = await _context.Wishlist
                .Where(w => w.UserId == userId)
                .Include(w => w.Book)
                .Select(w => new
                {
                    w.Id,
                    w.UserId,
                    w.BookId,
                    Title = w.Book.Title,
                    Author = w.Book.Author,
                    ISBN = w.Book.ISBN,
                    Description = w.Book.Description,
                    Genre = w.Book.Gender,
                    Price = w.Book.Price,
                    Publisher = w.Book.Publisher,
                    w.DateAdded
                })
                .ToListAsync();

            return Ok(wishlist);
        }

        // Ajouter un livre à la wishlist d'un user
        // POST api/wishlist/{userId}/{bookId}
        [HttpPost("{userId:int}/{bookId:int}")]
        public async Task<IActionResult> AddToWishlist(int userId, int bookId)
        {
            var userExists = await _context.Users.AnyAsync(u => u.Id == userId);
            if (!userExists)
                return NotFound(new { Message = "Utilisateur introuvable." });

            var bookExists = await _context.Books.AnyAsync(b => b.Id == bookId);
            if (!bookExists)
                return NotFound(new { Message = "Livre introuvable." });

            var alreadyInList = await _context.Wishlist.AnyAsync(w => w.BookId == bookId && w.UserId == userId);
            if (alreadyInList)
                return BadRequest(new { Message = "Ce livre est déjà dans la wishlist de cet utilisateur." });

            var wishlist = new Wishlist
            {
                BookId = bookId,
                UserId = userId,
                DateAdded = DateTime.UtcNow
            };

            _context.Wishlist.Add(wishlist);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Livre ajouté avec succès à la wishlist." });
        }

        // Supprimer un livre de la wishlist d'un user
        // DELETE api/wishlist/{userId}/{bookId}
        [HttpDelete("{userId:int}/{bookId:int}")]
        public async Task<IActionResult> RemoveFromWishlist(int userId, int bookId)
        {
            var item = await _context.Wishlist.FirstOrDefaultAsync(w => w.BookId == bookId && w.UserId == userId);
            if (item == null)
                return NotFound(new { Message = "Livre non présent dans la wishlist de cet utilisateur." });

            _context.Wishlist.Remove(item);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Livre retiré avec succès de la wishlist." });
        }
    }
}

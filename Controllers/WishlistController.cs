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


        //Récupérer les livres de la wishlist
        [HttpGet]
        public async Task<IActionResult> GetWishlist()
        {
            var wishlist = await _context.Wishlist
                .Include(w => w.Book)
                .Select(w => new
                {
                    w.Id,
                    w.BookId,
                    w.Book.Title,
                    w.Book.Author,
                    w.Book.ISBN,
                    w.Book.Price,
                    w.Book.Publisher,
                    w.DateAdded
                })
                .ToListAsync();

            return Ok(wishlist);
        }

        //Ajouter un livre à la wishlist
        [HttpPost("{bookId:int}")]
        public async Task<IActionResult> AddToWishlist(int bookId)
        {
            var bookExists = await _context.Books.AnyAsync(b => b.Id == bookId);
            if (!bookExists)
                return NotFound(new { Message = "Livre introuvable." });

            var alreadyInList = await _context.Wishlist.AnyAsync(w => w.BookId == bookId);
            if (alreadyInList)
                return BadRequest(new { Message = "Ce livre est déjà dans la wishlist." });

            var wishlist = new Wishlist { BookId = bookId };
            _context.Wishlist.Add(wishlist);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Livre ajouté avec succès à la wishlist." });
        }

        //Supprimer un livre de la wishlist
        [HttpDelete("{bookId:int}")]
        public async Task<IActionResult> RemoveFromWishlist(int bookId)
        {
            var item = await _context.Wishlist.FirstOrDefaultAsync(w => w.BookId == bookId);
            if (item == null)
                return NotFound(new { Message = "Livre non présent dans la wishlist." });

            _context.Wishlist.Remove(item);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Livre retiré avec succès de la wishlist." });
        }
    }
}

using Bookify.Models;
using BCrypt.Net; // 👈 ajoute cette ligne

namespace Bookify.Data
{
    public static class DbInitializer
    {
        public static void Seed(ApplicationDb context)
        {
            // --- GENRES ---
            if (!context.Genders.Any())
            {
                var genres = new List<Gender>
                {
                    new Gender { Name = "Roman" },
                    new Gender { Name = "Science-Fiction" },
                    new Gender { Name = "Policier" }
                };
                context.Genders.AddRange(genres);
                context.SaveChanges();
            }

            // --- LIVRES ---
            var booksToAdd = new List<Book>
            {
                new Book { Title = "1984", Author = "George Orwell", ISBN = "9780451524935", Price = 15, Description = "Dystopie célèbre", Publisher = "Secker & Warburg", GenderId = context.Genders.First(g => g.Name == "Science-Fiction").Id },
                new Book { Title = "Le Petit Prince", Author = "Antoine de Saint-Exupéry", ISBN = "9780156013987", Price = 10, Description = "Conte poétique", Publisher = "Gallimard", GenderId = context.Genders.First(g => g.Name == "Roman").Id },
                new Book { Title = "Sherlock Holmes", Author = "Arthur Conan Doyle", ISBN = "9780241952894", Price  = 12, Description = "Enquêtes policières", Publisher = "Penguin", GenderId = context.Genders.First(g => g.Name == "Policier").Id }
            };

            foreach (var book in booksToAdd)
            {
                if (!context.Books.Any(b => b.ISBN == book.ISBN))
                {
                    context.Books.Add(book);
                }
            }
            context.SaveChanges();

            // --- UTILISATEURS ---
            var usersToAdd = new List<User>
            {
                new User
                {
                    Name = "Alice Dupont",
                    Email = "alice@test.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("123"), // hashé
                    Username = "alice"
                },
                new User
                {
                    Name = "Bob Martin",
                    Email = "bob@test.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("456"), // hashé
                    Username = "bob"
                }
            };

            foreach (var user in usersToAdd)
            {
                if (!context.Users.Any(u => u.Email == user.Email))
                {
                    context.Users.Add(user);
                }
            }
            context.SaveChanges();

            // --- AVIS ---
            var book1984 = context.Books.First(b => b.Title == "1984");
            var petitPrince = context.Books.First(b => b.Title == "Le Petit Prince");
            var alice = context.Users.First(u => u.Email == "alice@test.com");
            var bob = context.Users.First(u => u.Email == "bob@test.com");

            var reviewsToAdd = new List<Review>
            {
                new Review { BookId = book1984.Id, UserId = alice.Id, Note = 5, Titre = "Chef d'œuvre", Avis = "Un classique absolu." },
                new Review { BookId = petitPrince.Id, UserId = bob.Id, Note = 4, Titre = "Très beau", Avis = "Poétique et touchant." }
            };

            foreach (var review in reviewsToAdd)
            {
                if (!context.Reviews.Any(r => r.BookId == review.BookId && r.UserId == review.UserId))
                {
                    context.Reviews.Add(review);
                }
            }
            context.SaveChanges();

            // --- WISHLIST ---
            var wishlistsToAdd = new List<Wishlist>
            {
                new Wishlist { BookId = book1984.Id, DateAdded = DateTime.Now.AddDays(-2) },
                new Wishlist { BookId = petitPrince.Id, DateAdded = DateTime.Now.AddDays(-5) }
            };

            foreach (var wish in wishlistsToAdd)
            {
                if (!context.Wishlist.Any(w => w.BookId == wish.BookId))
                {
                    context.Wishlist.Add(wish);
                }
            }
            context.SaveChanges();

            // --- LIVRES LUS ---
            var livreLusToAdd = new List<LivreLu>
            {
                new LivreLu { BookId = petitPrince.Id, DateLu = DateTime.Now.AddDays(-10) },
                new LivreLu { BookId = book1984.Id, DateLu = DateTime.Now.AddDays(-20) }
            };

            foreach (var lu in livreLusToAdd)
            {
                if (!context.LivreLu.Any(l => l.BookId == lu.BookId))
                {
                    context.LivreLu.Add(lu);
                }
            }
            context.SaveChanges();
        }
    }
}

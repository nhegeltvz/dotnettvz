// ============================================================
// EF Core — Library Database
// Entities: Author → Book → Review
// SQLite file: library.db  (created automatically on first run)
// ============================================================

using Microsoft.EntityFrameworkCore;

// ── Entities ─────────────────────────────────────────────────

public class Author
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Country { get; set; } = "";
    public int BirthYear { get; set; }
    public List<Book> Books { get; set; } = new();
}

public class Book
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string Genre { get; set; } = "";
    public decimal Price { get; set; }
    public int PublishedYear { get; set; }
    public bool InStock { get; set; }

    public int AuthorId { get; set; }
    public Author Author { get; set; } = null!;
    public List<Review> Reviews { get; set; } = new();
}

public class Review
{
    public int Id { get; set; }
    public int Rating { get; set; }   // 1–5
    public string Comment { get; set; } = "";

    public int BookId { get; set; }
    public Book Book { get; set; } = null!;
}

// ── DbContext ─────────────────────────────────────────────────

public class LibraryContext : DbContext
{
    public DbSet<Author> Authors => Set<Author>();
    public DbSet<Book>   Books   => Set<Book>();
    public DbSet<Review> Reviews => Set<Review>();

    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite("Data Source=library.db");
}

// ── Seeder ────────────────────────────────────────────────────

public static class DbSeeder
{
    public static void Seed(LibraryContext db)
    {
        db.Database.EnsureCreated();
        if (db.Authors.Any()) return;   // already seeded

        var orwell   = new Author { Name = "George Orwell",    Country = "UK",  BirthYear = 1903 };
        var rowling  = new Author { Name = "J.K. Rowling",     Country = "UK",  BirthYear = 1965 };
        var king     = new Author { Name = "Stephen King",      Country = "USA", BirthYear = 1947 };
        var christie = new Author { Name = "Agatha Christie",   Country = "UK",  BirthYear = 1890 };
        var huxley   = new Author { Name = "Aldous Huxley",     Country = "UK",  BirthYear = 1894 };

        db.Authors.AddRange(orwell, rowling, king, christie, huxley);

        var b1  = new Book { Title = "1984",                         Genre = "Dystopia",         Price = 12.99m, PublishedYear = 1949, InStock = true,  Author = orwell };
        var b2  = new Book { Title = "Animal Farm",                  Genre = "Satire",            Price =  9.99m, PublishedYear = 1945, InStock = true,  Author = orwell };
        var b3  = new Book { Title = "Harry Potter and the Philosopher's Stone", Genre = "Fantasy", Price = 14.99m, PublishedYear = 1997, InStock = true,  Author = rowling };
        var b4  = new Book { Title = "Harry Potter and the Chamber of Secrets",  Genre = "Fantasy", Price = 14.99m, PublishedYear = 1998, InStock = false, Author = rowling };
        var b5  = new Book { Title = "The Shining",                  Genre = "Horror",            Price = 11.99m, PublishedYear = 1977, InStock = true,  Author = king };
        var b6  = new Book { Title = "It",                           Genre = "Horror",            Price = 15.99m, PublishedYear = 1986, InStock = true,  Author = king };
        var b7  = new Book { Title = "Carrie",                       Genre = "Horror",            Price =  8.99m, PublishedYear = 1974, InStock = false, Author = king };
        var b8  = new Book { Title = "Murder on the Orient Express", Genre = "Mystery",           Price = 10.99m, PublishedYear = 1934, InStock = true,  Author = christie };
        var b9  = new Book { Title = "And Then There Were None",     Genre = "Mystery",           Price = 10.99m, PublishedYear = 1939, InStock = true,  Author = christie };
        var b10 = new Book { Title = "Brave New World",              Genre = "Dystopia",          Price = 11.99m, PublishedYear = 1932, InStock = true,  Author = huxley };

        db.Books.AddRange(b1, b2, b3, b4, b5, b6, b7, b8, b9, b10);

        db.Reviews.AddRange(
            new Review { Book = b1,  Rating = 5, Comment = "A masterpiece of dystopian fiction" },
            new Review { Book = b1,  Rating = 4, Comment = "Chilling and still relevant today" },
            new Review { Book = b1,  Rating = 5, Comment = "Should be mandatory reading" },
            new Review { Book = b2,  Rating = 5, Comment = "Perfect political allegory" },
            new Review { Book = b2,  Rating = 3, Comment = "Short but powerful" },
            new Review { Book = b3,  Rating = 5, Comment = "Magical from start to finish" },
            new Review { Book = b3,  Rating = 4, Comment = "Great start to an iconic series" },
            new Review { Book = b4,  Rating = 4, Comment = "Even better than the first" },
            new Review { Book = b5,  Rating = 4, Comment = "Genuinely terrifying" },
            new Review { Book = b6,  Rating = 5, Comment = "King's magnum opus" },
            new Review { Book = b6,  Rating = 3, Comment = "Too long but worth it" },
            new Review { Book = b7,  Rating = 3, Comment = "Good but not his best" },
            new Review { Book = b8,  Rating = 5, Comment = "The perfect mystery novel" },
            new Review { Book = b8,  Rating = 4, Comment = "Twist I did not see coming" },
            new Review { Book = b9,  Rating = 5, Comment = "Her best work" },
            new Review { Book = b10, Rating = 4, Comment = "Prescient and disturbing" },
            new Review { Book = b10, Rating = 5, Comment = "Orwell and Huxley complement each other" }
        );

        db.SaveChanges();
    }
}

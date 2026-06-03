// ============================================================
// E09 — EF Core  (10 exercises)
// Database: SQLite library.db  (auto-created + seeded on first run)
// Entities: Author → Book → Review
// ============================================================
// IMPORTANT CONCEPTS:
//   Include()              = eager loading (loads related data in same query)
//   ThenInclude()          = nested eager loading
//   AsNoTracking()         = read-only query, faster (no change tracking)
//   Select() projection    = avoid loading full entity when you only need some fields
//   ToListAsync()          = materialize query to list (hits the DB)
//   FirstOrDefaultAsync()  = single item or null
// ============================================================

using Microsoft.EntityFrameworkCore;

public static class E09_EFCore
{
    public static async Task RunAsync()
    {
        Console.WriteLine("\n========== E09: EF CORE ==========");

        using var db = new LibraryContext();
        DbSeeder.Seed(db);   // creates DB + seeds data if not already done

        // ── EXERCISE 1: Basic Select (AsNoTracking) ───────────
        // Get all books from the DB and print Title + Price.
        // Use AsNoTracking() since this is a read-only query.
        //
        // TODO: query db.Books, use AsNoTracking, ToListAsync

        // var books = await db.Books.AsNoTracking()...ToListAsync();
        // foreach (var b in books) Console.WriteLine($"{b.Title} — {b.Price:C}");

        // ── EXERCISE 2: Where + OrderBy ───────────────────────
        // Get all Horror books that are InStock,
        // sorted by Price ascending.
        // Print Title and Price.
        //
        // TODO: use Where + OrderBy + ToListAsync

        // var horror = await db.Books
        //     .Where(...)
        //     .OrderBy(...)
        //     ...

        // ── EXERCISE 3: Include (eager loading) ───────────────
        // Get all books WITH their Author loaded (single SQL JOIN).
        // Print "Title by AuthorName (Country)".
        // Without Include, Author would be null.
        //
        // TODO: use Include(b => b.Author)

        // var booksWithAuthor = await db.Books
        //     .Include(b => b.Author)
        //     .AsNoTracking()
        //     .ToListAsync();
        // foreach (var b in booksWithAuthor)
        //     Console.WriteLine($"{b.Title} by {b.Author.Name} ({b.Author.Country})");

        // ── EXERCISE 4: ThenInclude (nested eager loading) ────
        // Get all Authors, with their Books, with each Book's Reviews.
        // For each author print: Name → book count → avg review rating.
        // Use Include(...).ThenInclude(...)
        //
        // TODO: use Include + ThenInclude

        // var authors = await db.Authors
        //     .Include(a => a.Books)
        //         .ThenInclude(b => b.Reviews)
        //     .AsNoTracking()
        //     .ToListAsync();
        // foreach (var a in authors)
        // {
        //     var avgRating = a.Books.SelectMany(b => b.Reviews)
        //                            .Select(r => r.Rating)
        //                            .DefaultIfEmpty(0)
        //                            .Average();
        //     Console.WriteLine($"{a.Name}: {a.Books.Count} books, avg rating {avgRating:F1}");
        // }

        // ── EXERCISE 5: Select projection ────────────────────
        // Instead of loading full Book entities, project into a lightweight DTO:
        //   (Title, AuthorName, Genre, Price)
        // This avoids loading fields you don't need.
        //
        // TODO: use Select(b => new { b.Title, AuthorName = b.Author.Name, ... })
        //       Include is NOT needed when you Select into a projection —
        //       EF Core generates the JOIN automatically

        // var projections = await db.Books
        //     .Select(b => new { b.Title, AuthorName = b.Author.Name, b.Genre, b.Price })
        //     .AsNoTracking()
        //     .ToListAsync();

        // ── EXERCISE 6: GroupBy in EF Core ────────────────────
        // Count books per Genre and print "Genre | Count | AvgPrice".
        // Note: GroupBy in EF Core translates to SQL GROUP BY.
        //
        // TODO: use GroupBy(b => b.Genre) + Select aggregate

        // var byGenre = await db.Books
        //     .GroupBy(b => b.Genre)
        //     .Select(g => new { Genre = g.Key, Count = g.Count(), AvgPrice = g.Average(b => b.Price) })
        //     .OrderByDescending(g => g.Count)
        //     .ToListAsync();

        // ── EXERCISE 7: FirstOrDefaultAsync + null check ──────
        // Find the book titled "1984". If found, print its details + review count.
        // Then try finding "The Hobbit" and handle the null case gracefully.
        //
        // TODO: use FirstOrDefaultAsync

        // var found = await db.Books
        //     .Include(b => b.Reviews)
        //     .FirstOrDefaultAsync(b => b.Title == "1984");
        // if (found is not null)
        //     Console.WriteLine($"Found: {found.Title}, Reviews: {found.Reviews.Count}");

        // ── EXERCISE 8: AnyAsync + CountAsync ────────────────
        // a) Check (AnyAsync) if there are out-of-stock books. Print true/false.
        // b) Count how many books each author has using CountAsync per author.
        // c) Get the total review count for Horror books using SumAsync.
        //
        // TODO: use AnyAsync, CountAsync, SumAsync

        // bool anyOutOfStock = await db.Books.AnyAsync(b => !b.InStock);
        // Console.WriteLine($"Any out of stock: {anyOutOfStock}");

        // ── EXERCISE 9: Complex query — top rated books ───────
        // Find the 3 books with the highest average review rating.
        // Only include books that have AT LEAST 2 reviews.
        // Print: Rank | Title | AuthorName | AvgRating | ReviewCount
        //
        // TODO: use Include + Select projection + Where + OrderByDescending + Take

        // var topRated = await db.Books
        //     .Include(b => b.Reviews)
        //     .Include(b => b.Author)
        //     .AsNoTracking()
        //     .Where(b => b.Reviews.Count >= 2)
        //     .Select(b => new
        //     {
        //         b.Title,
        //         AuthorName = b.Author.Name,
        //         AvgRating  = b.Reviews.Average(r => r.Rating),
        //         ReviewCount = b.Reviews.Count
        //     })
        //     .OrderByDescending(b => b.AvgRating)
        //     .Take(3)
        //     .ToListAsync();

        // ── EXERCISE 10: Add + SaveChanges + Delete ───────────
        // a) Add a new Author "Fyodor Dostoevsky", Country "Russia", BirthYear 1821.
        //    Add one book for him: "Crime and Punishment", Genre "Classic",
        //    Price 10.99, PublishedYear 1866, InStock true.
        //    SaveChanges and print the new book's generated Id.
        //
        // b) Delete that book and author again (clean up).
        //    Use Remove() + SaveChanges.
        //
        // TODO: use Add, SaveChanges, Remove

        // var dostoevsky = new Author { Name = "Fyodor Dostoevsky", Country = "Russia", BirthYear = 1821 };
        // var crime = new Book { Title = "Crime and Punishment", Genre = "Classic",
        //                        Price = 10.99m, PublishedYear = 1866, InStock = true, Author = dostoevsky };
        // db.Authors.Add(dostoevsky);
        // db.Books.Add(crime);
        // await db.SaveChangesAsync();
        // Console.WriteLine($"New book Id: {crime.Id}");
        //
        // db.Books.Remove(crime);
        // db.Authors.Remove(dostoevsky);
        // await db.SaveChangesAsync();
        // Console.WriteLine("Cleaned up.");
    }
}

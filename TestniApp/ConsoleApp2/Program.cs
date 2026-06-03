// C# Interview Prep — All exercises
// Comment out sections you don't want to run.

// ── C# Basics ────────────────────────────────
//E01_ValueTypes.Run();
//E02_OOP.Run();
//E03_Nullability.Run();
//E04_Generics.Run();
//E05_Delegates.Run();
//E06_Exceptions.Run();
//E07_ModernCSharp.Run();

// ── Interview topics ─────────────────────────
//E08_LINQ.Run();
//await E09_EFCore.RunAsync();   // EF Core — creates library.db on first run
//await E10_AsyncAwait.RunAsync();
//E11_Collections.Run();
public class Program
{
    public async static Task Main()
    {
        try
        {
            var result = await RetryAsync(async () => await TryDivide(7,0), 3);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    public static async Task<T> RetryAsync<T>(Func<Task<T>> operation, int maxAttempts)
    {
        var numberOfAttempts = 0;
        var lastException = new Exception();
        do
        {
            numberOfAttempts++;
            try
            {
                Console.WriteLine("Trying to perform operation on the " + numberOfAttempts + " try...");
                return await operation();
            }catch (Exception ex) {
                Console.WriteLine(numberOfAttempts + ". try failed...");
                lastException = ex;
                await Task.Delay(50 * numberOfAttempts);
            }
        }while (numberOfAttempts < maxAttempts);

        throw lastException;
    }

    public static async Task<int> TryDivide(int a, int b)
    {
        await Task.Delay(100);
        if (b == 0) throw new DivideByZeroException("Trying to divide by zero...");
        return a / b;
    }

    public static IEnumerable<int> GetDivider()
    {
        for (int i = 0; i < 4; i++)
            yield return i;
    }
}

#region LINQ
//var result = E08_LINQ.Products
//            .Where(p => p.Stock > 0 && p.Category == "Electronics")
//            .Select(p => (p.Name, p.Price))
//            .OrderByDescending(productInfo => productInfo.Price);

//foreach (var p in result) Console.WriteLine($"{p.Name} {p.Price}");

//Console.WriteLine();

//var sorted = E08_LINQ.Employees
//    .OrderBy(e => e.Department)
//    .ThenByDescending(e => e.Salary);
//foreach (var e in sorted) Console.WriteLine($"{e.Department,-15} {e.Name,-10} {e.Salary}");

//Console.WriteLine();

//var groups = E08_LINQ.Products
//    .GroupBy(p => p.Category)
//    .Select(group => new
//    {
//        Category = group.Key,
//        Count = group.Count(),
//        Average = Math.Round(group.Average(g => g.Price), 2),
//        TotalStock = group.Sum(g => g.Stock)
//    });
//foreach (var g in groups) Console.WriteLine($"Category: {g.Category} — Count: {g.Count} — AvgPrice: {g.Average} — TotalStock: {g.TotalStock}");

//Console.WriteLine();

//var joined = E08_LINQ.Orders
//    .Join(E08_LINQ.Products, o => o.Product, p => p.Name, (order, product) => new { order.Customer, TotalOrderValue = order.UnitPrice * order.Quantity, order.Product })
//    .OrderByDescending(data => data.TotalOrderValue);

//foreach (var row in joined) Console.WriteLine($"{row.Customer} | {row.Product} | {row.TotalOrderValue}");

//Console.WriteLine();

//var summary = E08_LINQ.Orders.GroupBy(o => o.Customer)
//    .Select(g => new { g.Key, OrderCount = g.Count(), TotalSpent = g.Sum(o => o.UnitPrice * o.Quantity) })
//    .OrderByDescending(data => data.TotalSpent);


//Console.WriteLine($"{"Customer",-12} | {"OrderCount",-12} | {"TotalSpent",-12}");
//foreach (var row in summary) Console.WriteLine($"{row.Key,8} | {row.OrderCount,-12} | {row.TotalSpent,-12}");

//Console.WriteLine();

//var top3Score = E08_LINQ.Students.OrderByDescending(s => s.Score).Take(3);

//Console.WriteLine($"{"Student",-12} | {"Subject",-12} | {"Score",-12}");
//foreach (var student in top3Score) Console.WriteLine($"{student.Name,-12} | {student.Subject,-12} | {student.Score,-12}");

//Console.WriteLine();

//var averageMathResult = E08_LINQ.Students
//    .Where(s => s.Subject == "Math")
//    .Select(s => s.Score)
//    .Aggregate(1, (s1, s2) => s1 * s2);

//Console.WriteLine(averageMathResult);

//var concatedEngineers = E08_LINQ.Employees
//    .Where(e => e.Department == "Engineering")
//    .Select(s => s.Name)
//    .Aggregate((s1, s2) => s1 + ", " + s2);

//Console.WriteLine(concatedEngineers);

//Console.WriteLine("Are there any products with Stock == 0? Answer: " + E08_LINQ.Products.Any(product => product.Stock == 0));
//Console.WriteLine("Do ALL Engineering employees earn more than 80000? Answer: " + E08_LINQ.Employees.Where(e => e.Department == "Engineering").All(e => e.Salary > 80000));
//Console.WriteLine("How many distinct customers placed orders? Answer: " + E08_LINQ.Orders.DistinctBy(o => o.Customer).Count());
//Console.WriteLine("What is the most common Department in Employees? Answer: " + E08_LINQ.Employees.GroupBy(e => e.Department, (department, group) => new { Count = group.Count(), department }).OrderByDescending(data => data.Count).First().department);

//Console.WriteLine();

//Console.WriteLine("Get the first Electronics product with Price < 100. Answer: " + E08_LINQ.Products.First(p => p.Category == "Electronics" && p.Price < 100).Name);
//Console.WriteLine("Get the product named \"Tablet\" — use FirstOrDefault. Answer: " + (E08_LINQ.Products.FirstOrDefault(p => p.Name.Contains("Tablet"))?.Name ?? "No such product"));
////Console.WriteLine("Get the ONLY product in Furniture with Stock == 0. Answer: " + E08_LINQ.Products.Single(p => p.Stock == 0).Name);

//for (int page = 1; page <= 3; page++)
//{
//    var pageData = E08_LINQ.Products.Skip((page - 1) * (E08_LINQ.Products.Count / 3)).Take(3);
//    Console.WriteLine($"--- Page {page} ---");
//    foreach (var p in pageData) Console.WriteLine(p.Name);
//}

//var topEmployees = E08_LINQ.Employees.OrderByDescending(e => e.Salary).Take(5);
//var topStudents = E08_LINQ.Students.Where(s => s.Subject == "Math").OrderByDescending(s => s.Score).Take(5);
//var zipped = topEmployees.Zip(topStudents, (e, s) => $"{e.Name} ↔ {s.Name}");
//foreach (var pair in zipped) Console.WriteLine(pair);

#endregion

#region EFCore
//using var db = new LibraryContext();
//DbSeeder.Seed(db);

//var allBooks = db.Books.AsNoTracking().ToListAsync();
//foreach (var b in allBooks.Result) Console.WriteLine($"{b.Title} — {b.Price:C}");

//Console.WriteLine();

//var horrorBooks = await db.Books.Where(book => book.InStock && book.Genre=="Horror").OrderBy(b => (double)b.Price).AsNoTracking().ToListAsync();
//foreach (var b in horrorBooks) Console.WriteLine($"{b.Title} — {b.Price:C}");

//Console.WriteLine();

//var authorGrades = await db.Authors.Include(a => a.Books).ThenInclude(b => b.Reviews).AsNoTracking().ToListAsync();
//foreach (var a in authorGrades) Console.WriteLine($"{a.Name} by {a.Books.Count} ({a.Books.SelectMany(b => b.Reviews.Select(r => r.Rating)).Average()})");

//Console.WriteLine();

//var bookDataDto = await db.Books.Select(b => new { b.Title, b.Author.Name, b.Genre, b.Price }).AsNoTracking().ToListAsync();
//foreach (var a in bookDataDto) Console.WriteLine($"({a.Title}, {a.Name}, {a.Genre}, {a.Price})");

//Console.WriteLine();

//Console.WriteLine($"{"Genre",-12} | {"Count",-12} | {"AvgPrice",-12}");

//var booksPerGenre = await db.Books.GroupBy(b => b.Genre).Select(group => new { group.Key, Count = group.Count(), AvgPrice = Math.Round(group.Average(g => (double)g.Price),2) }).AsNoTracking().ToListAsync();
//foreach (var a in booksPerGenre) Console.WriteLine($"{a.Key,-12} | {a.Count,-12} | {a.AvgPrice,-12} ");


//Console.WriteLine();

////Consider that book has a toString...
//Console.WriteLine((await db.Books.FirstOrDefaultAsync(b => b.Title == "1984"))?.ToString() ?? "1984: No such book Found"); 
//Console.WriteLine((await db.Books.FirstOrDefaultAsync(b => b.Title == "Hobbit"))?.ToString() ?? "Hobbit: No such book Found");


//Console.WriteLine();

//Console.WriteLine("Check (AnyAsync) if there are out-of-stock books. Answer: " + await db.Books.AnyAsync(b => !b.InStock)); 
//Console.WriteLine("Count how many books each author has using CountAsync per author. Answer: ");
//var authorBooks = await db.Authors.Select(a => new { a.Name, a.Books }).ToListAsync();
//foreach (var author in authorBooks) Console.WriteLine("Author " + author.Name + " Has" + author.Books.Count);
//Console.WriteLine("Total review count for horror books. Answer: " + await db.Books.Where(b => b.Genre=="Horror").Select(b => b.Reviews).CountAsync());

//Console.WriteLine();
//Console.WriteLine("Rank | Title | AuthorName | AvgRating | ReviewCount");

//var topRatedBooks = await db.Books.Include(b => b.Reviews).Where(b => b.Reviews.Count>1).Select(b => new { b.Title, AuhtorName = b.Author.Name, AvgRating = b.Reviews.Average(b1 => b1.Rating), ReviewCount = b.Reviews.Count}).OrderByDescending(bookData => bookData.AvgRating).Take(3).AsNoTracking().ToArrayAsync();
//for (int i = 0; i < topRatedBooks.Length; i++)
//    Console.WriteLine($"{i + 1} | {topRatedBooks[i].Title} | {topRatedBooks[i].AuhtorName} | {topRatedBooks[i].AvgRating} | {topRatedBooks[i].ReviewCount}");


//Console.WriteLine();
//Console.WriteLine("Adding A book...");
#endregion
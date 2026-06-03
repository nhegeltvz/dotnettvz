// ============================================================
// E08 — LINQ  (10 exercises)
// Topics: Where/Select, OrderBy, GroupBy, Join, SelectMany,
//         Aggregate, Any/All, First/Single, set ops, Take/Skip/Zip
// All exercises use in-memory data — no database required.
// ============================================================

public static class E08_LINQ
{
    // ── Seed data ─────────────────────────────────────────────
    public record Product(int Id, string Name, string Category, decimal Price, int Stock);
    public record Employee(string Name, string Department, decimal Salary, int Age);
    public record Order(int Id, string Customer, string Product, int Quantity, decimal UnitPrice);
    public record Student(string Name, string Subject, int Score);

    public static readonly List<Product> Products = new()
    {
        new(1,  "Laptop",      "Electronics",  1200m, 15),
        new(2,  "Phone",       "Electronics",   800m,  0),
        new(3,  "Desk",        "Furniture",     350m,  8),
        new(4,  "Chair",       "Furniture",     150m, 20),
        new(5,  "Monitor",     "Electronics",   400m,  5),
        new(6,  "Keyboard",    "Electronics",    80m, 30),
        new(7,  "Bookshelf",   "Furniture",     200m,  0),
        new(8,  "Headphones",  "Electronics",   120m, 12),
        new(9,  "Lamp",        "Furniture",      60m,  3),
        new(10, "Webcam",      "Electronics",    90m,  7),
    };

    public static readonly List<Employee> Employees = new()
    {
        new("Alice",   "Engineering", 95000m, 32),
        new("Bob",     "Marketing",   60000m, 28),
        new("Carol",   "Engineering", 88000m, 41),
        new("Dave",    "HR",          55000m, 35),
        new("Eve",     "Engineering", 102000m,29),
        new("Frank",   "Marketing",   72000m, 44),
        new("Grace",   "HR",          58000m, 31),
        new("Heidi",   "Engineering", 91000m, 38),
        new("Ivan",    "Marketing",   65000m, 26),
        new("Judy",    "HR",          61000m, 50),
    };

    public static readonly List<Order> Orders = new()
    {
        new(1, "Alice",   "Laptop",     2, 1200m),
        new(2, "Bob",     "Phone",      1,  800m),
        new(3, "Alice",   "Monitor",    3,  400m),
        new(4, "Carol",   "Keyboard",   5,   80m),
        new(5, "Bob",     "Laptop",     1, 1200m),
        new(6, "Dave",    "Chair",      4,  150m),
        new(7, "Alice",   "Headphones", 2,  120m),
        new(8, "Carol",   "Desk",       1,  350m),
        new(9, "Eve",     "Webcam",     3,   90m),
        new(10,"Dave",    "Lamp",       2,   60m),
    };

    public static readonly List<Student> Students = new()
    {
        new("Alice",  "Math",    92), new("Alice",  "Science", 88),
        new("Bob",    "Math",    45), new("Bob",    "Science", 72),
        new("Carol",  "Math",    78), new("Carol",  "Science", 95),
        new("Dave",   "Math",    61), new("Dave",   "Science", 55),
        new("Eve",    "Math",    99), new("Eve",    "Science", 91),
    };

    // ─────────────────────────────────────────────────────────
    public static void Run()
    {
        Console.WriteLine("\n========== E08: LINQ ==========");

        // ── EXERCISE 1: Where + Select ────────────────────────
        // Query: get the Name and Price of all Electronics products
        // that are in stock (Stock > 0), sorted by Price descending.
        // Project to an anonymous type or tuple (Name, Price).
        //
        // Expected: Monitor 400 / Headphones 120 / Webcam 90 / Keyboard 80 / Laptop 1200 (sorted desc)
        //
        // TODO: write the LINQ query (method syntax preferred)

        // var result = Products...
        // foreach (var p in result) Console.WriteLine($"{p.Name} {p.Price}");

        // ── EXERCISE 2: OrderBy + ThenBy ─────────────────────
        // Sort employees: primary by Department (A→Z),
        // secondary by Salary descending within each department.
        // Print "Department | Name | Salary".
        //
        // TODO: write the LINQ query

        // var sorted = Employees...
        // foreach (var e in sorted) Console.WriteLine($"{e.Department,-15} {e.Name,-10} {e.Salary}");

        // ── EXERCISE 3: GroupBy ────────────────────────────────
        // Group products by Category. For each group print:
        //   "Category: X — Count: Y — AvgPrice: Z — TotalStock: W"
        //
        // TODO: write the GroupBy query

        // var groups = Products.GroupBy(...)
        // foreach (var g in groups) Console.WriteLine(...);

        // ── EXERCISE 4: Join ──────────────────────────────────
        // Join Orders with Products on Product name.
        // Calculate the total value (Quantity * UnitPrice) per order.
        // Print "Customer | Product | Total" sorted by Total descending.
        //
        // TODO: write the Join query

        // var joined = Orders.Join(Products, ...)
        // foreach (var row in joined) Console.WriteLine(...);

        // ── EXERCISE 5: GroupBy + aggregate on orders ─────────
        // Group orders by Customer.
        // For each customer print: "Customer | OrderCount | TotalSpent"
        // where TotalSpent = sum of (Quantity * UnitPrice) across all their orders.
        // Sort by TotalSpent descending.
        //
        // TODO: write the GroupBy + Sum query

        // var summary = Orders.GroupBy(...)
        //     .Select(g => new { ... })
        //     .OrderByDescending(...);

        // ── EXERCISE 6: SelectMany ────────────────────────────
        // Each student has TWO subjects (Math and Science).
        // Using SelectMany, flatten all students into a single list of
        // (Name, Subject, Score) tuples and find the top 3 scores overall.
        //
        // Hint: group Students by Name first (or just use Students list directly)
        //
        // TODO: use SelectMany or just query Students directly, find top 3

        // var top3 = Students.OrderByDescending(...).Take(3)...

        // ── EXERCISE 7: Aggregate ─────────────────────────────
        // a) Use Aggregate to compute the product (multiplication) of
        //    all scores in Math: 92 * 45 * 78 * 61 * 99.
        //
        // b) Use Aggregate to build a comma-separated string of all
        //    Engineering employees' names.
        //
        // TODO: use Aggregate (not Sum/string.Join)

        // var product = Students.Where(s => s.Subject == "Math")
        //                       .Select(s => s.Score)
        //                       .Aggregate((acc, n) => acc * n);
        // Console.WriteLine(product);

        // var names = Employees.Where(...)
        //                      .Aggregate("", (acc, e) => ...);
        // Console.WriteLine(names);

        // ── EXERCISE 8: Any / All / Count ────────────────────
        // Answer these using LINQ (one query each, print true/false/number):
        // a) Are there any products with Stock == 0?
        // b) Do ALL Engineering employees earn more than 80000?
        // c) How many distinct customers placed orders?
        // d) What is the most common Department in Employees?
        //
        // TODO: write four separate queries

        // Console.WriteLine(Products.Any(...));
        // Console.WriteLine(Employees.Where(...).All(...));
        // Console.WriteLine(Orders.Select(...).Distinct().Count());
        // var topDept = Employees.GroupBy(...).OrderByDescending(...).First().Key;

        // ── EXERCISE 9: First / FirstOrDefault / Single ───────
        // a) Get the first Electronics product with Price < 100 (use First)
        // b) Get the product named "Tablet" — use FirstOrDefault, handle null
        // c) Get the ONLY product in Furniture with Stock == 0 — use Single
        //    (Single throws if 0 or >1 results — verify there's exactly one first)
        //
        // KEY DIFFERENCE to know:
        //   First → returns first match, throws if NO match
        //   FirstOrDefault → returns first match or default (null for ref types)
        //   Single → throws if NOT exactly 1 match
        //   SingleOrDefault → throws if MORE than 1 match, returns default if 0
        //
        // TODO: write three queries

        // var cheap = Products.First(...);
        // var tablet = Products.FirstOrDefault(...);
        // var outOfStock = Products.Where(p => p.Category == "Furniture").Single(...);

        // ── EXERCISE 10: Take / Skip / Zip ────────────────────
        // a) Implement "paging": page size = 3. Print page 1, page 2, page 3 of Products.
        //    (Skip((page-1)*size).Take(size))
        //
        // b) Zip the top-5 highest-paid employees with the top-5 highest-scoring
        //    Math students, pairing them by rank.
        //    Print "Employee: X  ↔  Student: Y"
        //
        // TODO: implement paging and Zip

        // for (int page = 1; page <= 3; page++)
        // {
        //     var pageData = Products.Skip(...).Take(3);
        //     Console.WriteLine($"--- Page {page} ---");
        //     foreach (var p in pageData) Console.WriteLine(p.Name);
        // }

        // var topEmployees = Employees.OrderByDescending(e => e.Salary).Take(5);
        // var topStudents  = Students.Where(s => s.Subject == "Math").OrderByDescending(s => s.Score).Take(5);
        // var zipped = topEmployees.Zip(topStudents, (e, s) => $"{e.Name} ↔ {s.Name}");
        // foreach (var pair in zipped) Console.WriteLine(pair);
    }
}

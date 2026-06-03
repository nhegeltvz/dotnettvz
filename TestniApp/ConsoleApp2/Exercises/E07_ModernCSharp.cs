// ============================================================
// E07 — Modern C#  (10 exercises)
// Topics: records, pattern matching, switch expressions,
//         tuples, extension methods, StringBuilder
// ============================================================

public static class E07_ModernCSharp
{
    public static void Run()
    {
        Console.WriteLine("\n========== E07: MODERN C# ==========");

        // ── EXERCISE 1: Positional record ─────────────────────────
        // Create a positional record Money(decimal Amount, string Currency).
        // Add a computed property Display → "100.00 USD".
        // Add a method Money Add(Money other) that returns a new Money
        //   (throw InvalidOperationException if currencies differ).
        // Demonstrate `with` expression, value equality, and ToString.
        //
        // TODO: implement Money record

        // var a = new Money(100m, "USD");
        // var b = new Money(50m,  "USD");
        // Console.WriteLine(a.Add(b));           // 150.00 USD
        // Console.WriteLine(a == new Money(100m, "USD")); // True
        // var c = a with { Amount = 999m };
        // Console.WriteLine(c.Display);          // 999.00 USD
        // Console.WriteLine(a.Display);          // 100.00 USD  ← unchanged

        // ── EXERCISE 2: switch expression for categorization ──────
        // Implement static string ClassifyBMI(double bmi) using a switch expression:
        //   < 18.5  → "Underweight"
        //   < 25    → "Normal"
        //   < 30    → "Overweight"
        //   >= 30   → "Obese"
        //
        // Implement static string GradeToLetter(int score):
        //   90-100 → "A", 80-89 → "B", 70-79 → "C", 60-69 → "D", else → "F"
        //
        // TODO: implement ClassifyBMI and GradeToLetter

        // Console.WriteLine(ClassifyBMI(17));  // Underweight
        // Console.WriteLine(ClassifyBMI(22));  // Normal
        // Console.WriteLine(ClassifyBMI(27));  // Overweight
        // Console.WriteLine(ClassifyBMI(35));  // Obese
        // Console.WriteLine(GradeToLetter(95)); // A
        // Console.WriteLine(GradeToLetter(73)); // C
        // Console.WriteLine(GradeToLetter(50)); // F

        // ── EXERCISE 3: Type and property patterns ────────────────
        // Given an abstract record Shape and derived records
        //   Circle(double Radius), Rectangle(double Width, double Height), Triangle(double Base, double Height)
        // Implement static double Area(Shape s) using a switch expression
        // with type patterns only (no method on Shape).
        // Also implement static string Describe(Shape s) that returns different
        // descriptions using property patterns (e.g. "unit circle" when Radius == 1).
        //
        // TODO: implement Shape records and Area / Describe

        // Shape[] shapes = { new Circle(1), new Circle(5), new Rectangle(4, 6), new Triangle(3, 8) };
        // foreach (var s in shapes)
        //     Console.WriteLine($"{s} → area={Area(s):F2}, desc={Describe(s)}");

        // ── EXERCISE 4: Deconstruction ────────────────────────────
        // Create class Point3D(double X, double Y, double Z).
        // Implement Deconstruct(out double x, out double y, out double z).
        // Implement static double Distance(Point3D a, Point3D b)
        //   using deconstruction in the method body (no .X/.Y/.Z access).
        // Demonstrate tuple deconstruction in a foreach loop.
        //
        // TODO: implement Point3D and Distance

        // var p1 = new Point3D(1, 2, 3);
        // var p2 = new Point3D(4, 6, 3);
        // Console.WriteLine(Distance(p1, p2)); // 5
        // var (x, y, z) = p1;
        // Console.WriteLine($"{x},{y},{z}");   // 1,2,3

        // ── EXERCISE 5: Named tuple returns ──────────────────────
        // Implement static (int Min, int Max, double Mean, double Median)
        //     Summarize(IEnumerable<int> values)
        // Return all four statistics in one call.
        // Use it with a foreach to print a report.
        //
        // TODO: implement Summarize

        // var (min, max, mean, median) = Summarize(new[] { 3, 1, 4, 1, 5, 9, 2, 6 });
        // Console.WriteLine($"Min={min} Max={max} Mean={mean:F2} Median={median:F1}");
        // // Min=1 Max=9 Mean=3.88 Median=3.5

        // ── EXERCISE 6: Extension methods on string ───────────────
        // In a top-level static class StringEx, implement:
        //   bool   IsEmail(this string s)        → basic check: contains @ and .
        //   string ToPascalCase(this string s)   → "hello world" → "HelloWorld"
        //   string Repeat(this string s, int n)  → "ab".Repeat(3) → "ababab"
        //   bool   IsPalindrome(this string s)   → "racecar" → true
        //
        // TODO: implement StringEx (must be a top-level static class)

        // Console.WriteLine("user@example.com".IsEmail()); // True
        // Console.WriteLine("notanemail".IsEmail());        // False
        // Console.WriteLine("hello world".ToPascalCase());  // HelloWorld
        // Console.WriteLine("ab".Repeat(3));                // ababab
        // Console.WriteLine("racecar".IsPalindrome());      // True
        // Console.WriteLine("hello".IsPalindrome());        // False

        // ── EXERCISE 7: Extension methods on IEnumerable<T> ──────
        // In a top-level static class EnumerableEx, implement:
        //   IEnumerable<T>  Shuffle<T>(this IEnumerable<T> src)  → random order
        //   IEnumerable<IEnumerable<T>> Batch<T>(this IEnumerable<T> src, int size)
        //     → splits into chunks of `size`
        //   T Second<T>(this IEnumerable<T> src) → returns second element or throws
        //
        // TODO: implement EnumerableEx (top-level static class)

        // var nums = Enumerable.Range(1, 9);
        // foreach (var batch in nums.Batch(3))
        //     Console.WriteLine(string.Join(",", batch));
        // // 1,2,3 / 4,5,6 / 7,8,9
        // Console.WriteLine(nums.Second()); // 2

        // ── EXERCISE 8: StringBuilder pipeline ───────────────────
        // Implement static string BuildHtmlTable(IEnumerable<string[]> rows)
        // using StringBuilder (no string concatenation).
        // Output a simple HTML table: <table><tr><td>...</td></tr></table>.
        //
        // TODO: implement BuildHtmlTable

        // var rows = new[]
        // {
        //     new[] { "Alice", "30", "Zagreb" },
        //     new[] { "Bob",   "25", "Split"  },
        // };
        // Console.WriteLine(BuildHtmlTable(rows));
        // // <table>
        // //   <tr><td>Alice</td><td>30</td><td>Zagreb</td></tr>
        // //   <tr><td>Bob</td><td>25</td><td>Split</td></tr>
        // // </table>

        // ── EXERCISE 9: Record inheritance ────────────────────────
        // Create an abstract record Vehicle(string Make, string Model, int Year).
        // Create records Car : Vehicle with int Doors
        //             and Truck : Vehicle with double PayloadTons.
        // Show that record equality includes all properties,
        // and that `with` works on derived records too.
        //
        // TODO: implement Vehicle, Car, Truck records

        // var c1 = new Car("Toyota", "Corolla", 2020, 4);
        // var c2 = new Car("Toyota", "Corolla", 2020, 4);
        // Console.WriteLine(c1 == c2);               // True
        // var c3 = c1 with { Year = 2021 };
        // Console.WriteLine(c3);                     // Car { Make = Toyota, ... Year = 2021, Doors = 4 }

        // ── EXERCISE 10: is pattern in a method ──────────────────
        // Implement static string FormatValue(object? value) using only
        // `is` type patterns and relational patterns (no if/else chains, no casting):
        //   null         → "(null)"
        //   int > 1_000_000 → "big number: X"
        //   int          → "int: X"
        //   double d     → $"double: {d:F2}"
        //   string s when s.Length == 0 → "(empty string)"
        //   string s     → $"string: '{s}'"
        //   bool b       → $"bool: {b}"
        //   _            → $"other: {value}"
        //
        // TODO: implement FormatValue using switch expression with patterns

        // Console.WriteLine(FormatValue(null));        // (null)
        // Console.WriteLine(FormatValue(2_000_000));   // big number: 2000000
        // Console.WriteLine(FormatValue(42));          // int: 42
        // Console.WriteLine(FormatValue(3.14));        // double: 3.14
        // Console.WriteLine(FormatValue(""));          // (empty string)
        // Console.WriteLine(FormatValue("hi"));        // string: 'hi'
        // Console.WriteLine(FormatValue(true));        // bool: True
    }
}

// Remember: extension methods must be in a TOP-LEVEL static class (not nested).
// TODO: implement StringEx here
// public static class StringEx { ... }

// TODO: implement EnumerableEx here
// public static class EnumerableEx { ... }

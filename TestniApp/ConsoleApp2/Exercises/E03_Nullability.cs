// ============================================================
// E03 — Nullability & Properties  (10 exercises)
// Topics: int?, ??, ?., ??=, init, readonly, computed props
// ============================================================

public static class E03_Nullability
{
    public static void Run()
    {
        Console.WriteLine("\n========== E03: NULLABILITY & PROPERTIES ==========");

        // ── EXERCISE 1: Nullable value type operations ────────────
        // Implement static string Describe(int? value) that returns:
        //   "empty"     when value is null
        //   "zero"      when value is 0
        //   "positive"  when value > 0
        //   "negative"  when value < 0
        // Use HasValue / Value (do NOT use ==null shorthand).
        //
        // TODO: implement Describe(int? value)

        // Console.WriteLine(Describe(null));  // empty
        // Console.WriteLine(Describe(0));     // zero
        // Console.WriteLine(Describe(5));     // positive
        // Console.WriteLine(Describe(-3));    // negative

        // ── EXERCISE 2: Null coalescing chain ─────────────────────
        // Given three string? variables (some null, some not),
        // implement static string FirstNonNull(string? a, string? b, string? c)
        // that returns the first non-null value, or "none" if all are null.
        // Use only the ?? operator (no if statements).
        //
        // TODO: implement FirstNonNull

        // Console.WriteLine(FirstNonNull(null, null, "c")); // c
        // Console.WriteLine(FirstNonNull(null, "b", "c"));  // b
        // Console.WriteLine(FirstNonNull("a", "b", "c"));   // a
        // Console.WriteLine(FirstNonNull(null, null, null)); // none

        // ── EXERCISE 3: Safe navigation chain (?.) ────────────────
        // Create classes: Order → Customer → Address → string City.
        // Each reference in the chain can be null.
        // Implement static string GetCity(Order? order)
        // that returns the city or "Unknown" — using ?. and ?? only.
        //
        // TODO: implement Order, Customer, Address, GetCity

        // var full  = new Order { Customer = new Customer { Address = new Address { City = "Zagreb" } } };
        // var noAddr = new Order { Customer = new Customer { Address = null } };
        // var noCust = new Order { Customer = null };
        // Console.WriteLine(GetCity(full));    // Zagreb
        // Console.WriteLine(GetCity(noAddr));  // Unknown
        // Console.WriteLine(GetCity(noCust));  // Unknown
        // Console.WriteLine(GetCity(null));    // Unknown

        // ── EXERCISE 4: ??= lazy initialization ───────────────────
        // Create class ReportService with a private List<string>? _log field.
        // Implement void LogMessage(string msg) that initializes _log with ??=
        // on first call, then adds the message.
        // Implement IReadOnlyList<string> GetLog() that returns the log (never null).
        //
        // TODO: implement ReportService

        // var svc = new ReportService();
        // svc.LogMessage("started");
        // svc.LogMessage("processing");
        // svc.LogMessage("done");
        // foreach (var msg in svc.GetLog()) Console.WriteLine(msg);
        // // started / processing / done

        // ── EXERCISE 5: Property with validation ──────────────────
        // Create class BankAccount with:
        //   - string Owner (cannot be null or empty — throw ArgumentException)
        //   - decimal Balance (cannot go below 0 — throw InvalidOperationException)
        //   - void Deposit(decimal amount) and void Withdraw(decimal amount)
        //
        // TODO: implement BankAccount

        // var acc = new BankAccount("Alice");
        // acc.Deposit(500);
        // acc.Withdraw(200);
        // Console.WriteLine(acc.Balance); // 300
        // try { acc.Withdraw(1000); } catch (InvalidOperationException e) { Console.WriteLine(e.Message); }

        // ── EXERCISE 6: init-only + required ──────────────────────
        // Create a class UserProfile with:
        //   - required string Username  (init-only — set once at creation)
        //   - required string Email     (init-only)
        //   - DateTime CreatedAt        (init-only, defaults to DateTime.Now)
        //   - string? Bio               (settable after creation)
        // After creation Username and Email cannot be changed.
        //
        // TODO: implement UserProfile

        // var user = new UserProfile { Username = "nikol", Email = "n@test.com" };
        // user.Bio = "Developer";
        // Console.WriteLine(user.Username);  // nikol
        // Console.WriteLine(user.Bio);       // Developer
        // // user.Username = "x";  // should not compile

        // ── EXERCISE 7: Computed properties ──────────────────────
        // Create class Rectangle with double Width and Height.
        // Add computed (get-only) properties:
        //   Area       → Width * Height
        //   Perimeter  → 2 * (Width + Height)
        //   IsSquare   → true when Width == Height
        //   Diagonal   → Math.Sqrt(Width² + Height²)
        //
        // TODO: implement Rectangle

        // var r = new Rectangle { Width = 3, Height = 4 };
        // Console.WriteLine(r.Area);       // 12
        // Console.WriteLine(r.Perimeter);  // 14
        // Console.WriteLine(r.IsSquare);   // False
        // Console.WriteLine(r.Diagonal);   // 5

        // ── EXERCISE 8: Nullable reference type awareness ─────────
        // Implement static int SafeLength(string? s)
        // that returns s.Length when not null, or -1 when null.
        // Implement static string SafeUpper(string? s)
        // that returns s.ToUpper() when not null, or "(null)" when null.
        // Do NOT use ?. for SafeLength (use explicit null check).
        // DO use ?. and ?? for SafeUpper.
        //
        // TODO: implement SafeLength and SafeUpper

        // Console.WriteLine(SafeLength("hello")); // 5
        // Console.WriteLine(SafeLength(null));    // -1
        // Console.WriteLine(SafeUpper("hello"));  // HELLO
        // Console.WriteLine(SafeUpper(null));     // (null)

        // ── EXERCISE 9: Config with defaults ──────────────────────
        // Create class AppSettings where every property has a sensible default
        // but can be overridden. Use init-only properties and demonstrate
        // that the "with" approach (non-destructive copy) is NOT available
        // on regular classes — then switch it to a record to make it work.
        //
        // TODO: implement AppSettings as a record

        // var defaults = new AppSettings();
        // var custom   = defaults with { MaxConnections = 20, Timeout = 60 };
        // Console.WriteLine(defaults.MaxConnections); // 10
        // Console.WriteLine(custom.MaxConnections);   // 20
        // Console.WriteLine(defaults.Timeout);        // 30

        // ── EXERCISE 10: Null object pattern ──────────────────────
        // Define interface ILogger with void Log(string message).
        // Create ConsoleLogger that prints to console.
        // Create NullLogger that does nothing (swallows calls silently).
        // Implement a Service class that accepts ILogger? in its constructor
        // and uses ??= to fall back to NullLogger if null was passed.
        //
        // TODO: implement ILogger, ConsoleLogger, NullLogger, Service

        // var s1 = new Service(new ConsoleLogger());
        // var s2 = new Service(null);
        // s1.DoWork(); // prints: [LOG] Working...
        // s2.DoWork(); // prints nothing (NullLogger swallows it)
    }
}

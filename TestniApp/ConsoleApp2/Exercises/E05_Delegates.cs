// ============================================================
// E05 — Delegates & Events  (10 exercises)
// Topics: delegate, Func/Action/Predicate, lambda,
//         multicast, events, closures, higher-order functions
// ============================================================

public static class E05_Delegates
{
    public static void Run()
    {
        Console.WriteLine("\n========== E05: DELEGATES & EVENTS ==========");

        // ── EXERCISE 1: Basic Func / Action / Predicate ───────────
        // Using ONLY lambda expressions (no named methods), create:
        //   Func<int, int, int>  add        → sums two ints
        //   Func<string, string> shout      → converts string to uppercase + "!"
        //   Action<string, int>  repeat     → prints the string N times
        //   Predicate<string>    isLong     → true when string length > 5
        //
        // TODO: implement as lambda expressions only

        // Console.WriteLine(add(3, 4));      // 7
        // Console.WriteLine(shout("hello")); // HELLO!
        // repeat("hi", 3);                   // hi / hi / hi
        // Console.WriteLine(isLong("hello")); // False
        // Console.WriteLine(isLong("foobar")); // True

        // ── EXERCISE 2: Multicast delegate ────────────────────────
        // Create an Action<string> pipeline that:
        //   1. Prints the original message
        //   2. Prints the message in uppercase
        //   3. Prints the character count
        // Combine all three into one multicast delegate and invoke it.
        // Then remove step 2 and show it no longer runs.
        //
        // TODO: create the three actions and combine them

        // pipeline("hello");
        // // hello
        // // HELLO
        // // Length: 5
        // pipeline -= uppercase;
        // pipeline("world");
        // // world
        // // Length: 5

        // ── EXERCISE 3: Event + EventHandler ─────────────────────
        // Create class StockTicker with:
        //   string Symbol
        //   decimal Price (private set)
        //   event EventHandler<decimal> PriceChanged
        //   void UpdatePrice(decimal newPrice) → raises event only when price CHANGES
        //
        // TODO: implement StockTicker

        // var ticker = new StockTicker("MSFT", 300m);
        // ticker.PriceChanged += (s, price) => Console.WriteLine($"MSFT new price: {price}");
        // ticker.UpdatePrice(310m);  // MSFT new price: 310
        // ticker.UpdatePrice(310m);  // (no event — price didn't change)
        // ticker.UpdatePrice(295m);  // MSFT new price: 295

        // ── EXERCISE 4: Higher-order functions ────────────────────
        // Implement (without LINQ):
        //   static IEnumerable<T> Where<T>(IEnumerable<T> src, Func<T,bool> pred)
        //   static IEnumerable<R> Select<T,R>(IEnumerable<T> src, Func<T,R> map)
        //   static void ForEach<T>(IEnumerable<T> src, Action<T> action)
        //
        // TODO: implement Where, Select, ForEach

        // var numbers = new[] { 1, 2, 3, 4, 5, 6 };
        // var result = Select(Where(numbers, n => n % 2 == 0), n => n * n);
        // ForEach(result, n => Console.Write(n + " ")); // 4 16 36
        // Console.WriteLine();

        // ── EXERCISE 5: Function composition ─────────────────────
        // Implement static Func<T, TResult> Compose<T, TMiddle, TResult>(
        //     Func<T, TMiddle> first, Func<TMiddle, TResult> second)
        // Then implement static Func<T, TResult> Pipe<T, TMiddle, TResult>(...)
        // (same as Compose but arguments in reversed order — first is applied first).
        //
        // TODO: implement Compose and Pipe

        // Func<int, string>  intToStr   = n => n.ToString();
        // Func<string, int>  strLen     = s => s.Length;
        // Func<int, bool>    isEven     = n => n % 2 == 0;
        // var lengthIsEven = Compose(intToStr, Compose(strLen, isEven));
        // Console.WriteLine(lengthIsEven(12345)); // True  (length 5 is odd? False)
        // Console.WriteLine(lengthIsEven(1234));  // True  (length 4 is even)

        // ── EXERCISE 6: Closure ───────────────────────────────────
        // Implement static Func<int> MakeCounter(int start, int step)
        // that returns a function which returns the next value each time it is called.
        // The counter state must live inside the closure (no class fields).
        //
        // TODO: implement MakeCounter

        // var counter = MakeCounter(0, 1);
        // Console.WriteLine(counter()); // 0
        // Console.WriteLine(counter()); // 1
        // Console.WriteLine(counter()); // 2
        // var byFive = MakeCounter(10, 5);
        // Console.WriteLine(byFive()); // 10
        // Console.WriteLine(byFive()); // 15

        // ── EXERCISE 7: Memoization ───────────────────────────────
        // Implement static Func<T, TResult> Memoize<T, TResult>(Func<T, TResult> fn)
        //   where T : notnull
        // that wraps a function with a cache: the first call with a given argument
        // executes fn; subsequent calls with the same argument return the cached result.
        //
        // TODO: implement Memoize

        // int callCount = 0;
        // Func<int, int> expensive = n => { callCount++; return n * n; };
        // var memoized = Memoize(expensive);
        // Console.WriteLine(memoized(5));   // 25
        // Console.WriteLine(memoized(5));   // 25  (from cache)
        // Console.WriteLine(memoized(3));   // 9
        // Console.WriteLine(callCount);     // 2  (fn only called for distinct inputs)

        // ── EXERCISE 8: Custom event args ─────────────────────────
        // Create class FileWatcher that raises two events:
        //   event EventHandler<FileChangedArgs> FileChanged
        //   event EventHandler<FileDeletedArgs> FileDeleted
        // FileChangedArgs : EventArgs has string Path and long NewSize.
        // FileDeletedArgs : EventArgs has string Path.
        // Implement SimulateChange(string path, long size) and SimulateDelete(string path).
        //
        // TODO: implement FileChangedArgs, FileDeletedArgs, FileWatcher

        // var watcher = new FileWatcher();
        // watcher.FileChanged += (s, e) => Console.WriteLine($"Changed: {e.Path} ({e.NewSize} bytes)");
        // watcher.FileDeleted += (s, e) => Console.WriteLine($"Deleted: {e.Path}");
        // watcher.SimulateChange("report.txt", 1024); // Changed: report.txt (1024 bytes)
        // watcher.SimulateDelete("old.log");          // Deleted: old.log

        // ── EXERCISE 9: Delegate as strategy ─────────────────────
        // Create class Sorter<T> with:
        //   Func<T, T, int> CompareStrategy (publicly settable)
        //   List<T> Sort(IEnumerable<T> input) → sorts using CompareStrategy
        // Demonstrate switching strategies at runtime (ascending / descending / by length).
        //
        // TODO: implement Sorter<T>

        // var sorter = new Sorter<string>();
        // sorter.CompareStrategy = (a, b) => string.Compare(a, b, StringComparison.Ordinal);
        // var asc = sorter.Sort(new[] { "banana", "apple", "cherry" });
        // Console.WriteLine(string.Join(", ", asc));  // apple, banana, cherry
        // sorter.CompareStrategy = (a, b) => b.Length - a.Length;
        // var byLen = sorter.Sort(new[] { "banana", "apple", "cherry" });
        // Console.WriteLine(string.Join(", ", byLen)); // banana, cherry, apple

        // ── EXERCISE 10: Pipeline builder ────────────────────────
        // Implement class Pipeline<T> with:
        //   Pipeline<T> AddStep(Func<T, T> step)  → adds a transformation step
        //   T Execute(T input)                     → runs all steps in order
        // Each step transforms the value and passes it to the next.
        //
        // TODO: implement Pipeline<T>

        // var pipeline = new Pipeline<string>()
        //     .AddStep(s => s.Trim())
        //     .AddStep(s => s.ToLower())
        //     .AddStep(s => s.Replace(" ", "_"));
        // Console.WriteLine(pipeline.Execute("  Hello World  ")); // hello_world
    }
}

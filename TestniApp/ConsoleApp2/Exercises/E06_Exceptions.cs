// ============================================================
// E06 — Exception Handling & IDisposable  (10 exercises)
// Topics: try/catch/finally, catch order, when filter,
//         throw vs throw ex, custom exceptions,
//         IDisposable, using, finalizers
// ============================================================

public static class E06_Exceptions
{
    public static void Run()
    {
        Console.WriteLine("\n========== E06: EXCEPTIONS & IDISPOSABLE ==========");

        // ── EXERCISE 1: Custom exception hierarchy ────────────────
        // Create an exception hierarchy:
        //   AppException : Exception                (base, takes message)
        //   ValidationException : AppException      (adds string Field)
        //   NotFoundException : AppException        (adds string ResourceName)
        //   ConflictException  : AppException       (adds string ConflictReason)
        // Each must chain the message to base and expose its extra property.
        //
        // TODO: implement the exception hierarchy

        // try { throw new ValidationException("Email", "Invalid format"); }
        // catch (ValidationException e) { Console.WriteLine($"{e.Field}: {e.Message}"); }
        // // Email: Invalid format
        // try { throw new NotFoundException("User"); }
        // catch (NotFoundException e) { Console.WriteLine($"Not found: {e.ResourceName}"); }
        // // Not found: User

        // ── EXERCISE 2: Correct catch ordering ────────────────────
        // Write a method static void HandleException(Exception ex)
        // that catches in this order and prints a label:
        //   ArgumentNullException  → "null argument"
        //   ArgumentException      → "bad argument"
        //   InvalidOperationException → "invalid op"
        //   Exception              → "general error"
        // Demonstrate with all four types that ordering matters.
        //
        // TODO: implement HandleException and call it with each type

        // HandleException(new ArgumentNullException("x")); // null argument
        // HandleException(new ArgumentException("bad"));   // bad argument
        // HandleException(new InvalidOperationException()); // invalid op
        // HandleException(new Exception("generic"));       // general error

        // ── EXERCISE 3: Exception filter (when) ──────────────────
        // Implement static void ProcessHttpCall(int statusCode)
        // that throws an HttpCallException(int code) and catches it with
        // three separate `catch ... when` clauses:
        //   4xx → "Client error: {code}"
        //   5xx → "Server error: {code}"
        //   other → "Unexpected code: {code}"
        //
        // TODO: implement HttpCallException and ProcessHttpCall

        // ProcessHttpCall(404); // Client error: 404
        // ProcessHttpCall(500); // Server error: 500
        // ProcessHttpCall(301); // Unexpected code: 301

        // ── EXERCISE 4: throw vs throw ex ─────────────────────────
        // Implement two methods that re-throw a caught exception:
        //   static void BadRethrow()    → uses `throw ex`  (loses stack trace)
        //   static void GoodRethrow()   → uses `throw`     (preserves stack trace)
        // Call both inside a try/catch that prints ex.StackTrace.
        // Add a comment explaining the difference.
        //
        // TODO: implement BadRethrow and GoodRethrow, demonstrate difference

        // ── EXERCISE 5: IDisposable implementation ────────────────
        // Implement class DatabaseConnection : IDisposable with:
        //   - Constructor that prints "Connection opened"
        //   - void ExecuteQuery(string sql) that prints "Executing: {sql}"
        //   - Dispose() that prints "Connection closed" (idempotent — safe to call twice)
        //   - A private bool _disposed guard
        //
        // TODO: implement DatabaseConnection

        // using var conn = new DatabaseConnection();
        // conn.ExecuteQuery("SELECT 1");
        // conn.ExecuteQuery("SELECT 2");
        // // After using block: "Connection closed" printed exactly once

        // ── EXERCISE 6: Multiple resources disposal order ─────────
        // Using the DatabaseConnection from Ex 5 (or a similar Disposable stub),
        // open two connections inside a using block.
        // Verify the REVERSE disposal order: last opened is first closed.
        // (This matches how C# disposes `using` declarations in a scope.)
        //
        // TODO: demonstrate with two using-declarations in the same scope

        // // Expected:
        // // Connection A opened
        // // Connection B opened
        // // Connection B closed   ← reverse order
        // // Connection A closed

        // ── EXERCISE 7: Retry with exponential backoff ───────────
        // Implement static T Retry<T>(Func<T> operation, int maxAttempts, int initialDelayMs = 0)
        // Each attempt prints "Attempt N/maxAttempts".
        // On failure, doubles the delay before next attempt.
        // On final failure, re-throws the last exception (preserving stack trace).
        // (Set initialDelayMs = 0 in tests so it runs instantly.)
        //
        // TODO: implement Retry<T>

        // int attempt = 0;
        // var result = Retry(() => {
        //     attempt++;
        //     if (attempt < 4) throw new TimeoutException($"Timeout #{attempt}");
        //     return "done";
        // }, maxAttempts: 5);
        // Console.WriteLine(result); // done  (after 3 failures)

        // ── EXERCISE 8: Safe division with AggregateException ─────
        // Implement static List<int> DivideAll(int[] numerators, int denominator)
        // that divides each numerator by denominator.
        // If denominator is 0, throw AggregateException that wraps a
        // DivideByZeroException for each numerator that failed.
        //
        // TODO: implement DivideAll

        // var results = DivideAll(new[] { 10, 20, 30 }, 5);
        // Console.WriteLine(string.Join(", ", results)); // 2, 4, 6
        // try { DivideAll(new[] { 1, 2, 3 }, 0); }
        // catch (AggregateException ae)
        // {
        //     Console.WriteLine($"Errors: {ae.InnerExceptions.Count}"); // Errors: 3
        // }

        // ── EXERCISE 9: finally guarantee ────────────────────────
        // Implement static string ReadFileSafely(string path) that:
        //   - Opens a (simulated) file resource
        //   - Attempts to read it (may throw)
        //   - Guarantees the resource is always "closed" via finally
        //   - Returns the content or "[error: {message}]" on failure
        // Simulate the resource with a class ResourceHandle : IDisposable.
        //
        // TODO: implement ResourceHandle and ReadFileSafely

        // Console.WriteLine(ReadFileSafely("good.txt"));  // file content (simulated)
        // Console.WriteLine(ReadFileSafely("bad.txt"));   // [error: file not found]
        // // "resource closed" must print in both cases

        // ── EXERCISE 10: Wrap third-party exceptions ─────────────
        // Implement static T Execute<T>(Func<T> operation, string contextMessage)
        // that wraps any exception thrown by operation in an AppException
        // (from Exercise 1) with contextMessage + original exception as InnerException.
        // If operation succeeds, return its result.
        //
        // TODO: implement Execute<T>

        // var result = Execute(() => int.Parse("42"), "parsing config");
        // Console.WriteLine(result);  // 42
        // try
        // {
        //     Execute(() => int.Parse("bad"), "parsing config");
        // }
        // catch (AppException e)
        // {
        //     Console.WriteLine(e.Message);           // parsing config
        //     Console.WriteLine(e.InnerException!.GetType().Name); // FormatException
        // }
    }
}

// ============================================================
// E10 — Async / Await  (10 exercises)
// Topics: Task/Task<T>, WhenAll, WhenAny, CancellationToken,
//         async void danger, ConfigureAwait, exception handling,
//         IAsyncEnumerable, SemaphoreSlim
// ============================================================
// KEY RULES to remember:
//   - Always return Task/Task<T>, never async void (except event handlers)
//   - await does NOT block the thread — it suspends the method
//   - Use Task.WhenAll for parallel independent work
//   - Use CancellationToken to support cancellation
//   - ConfigureAwait(false) in library code — avoids deadlocks in sync contexts
// ============================================================

public static class E10_AsyncAwait
{
    // Simulates a slow I/O operation (network/disk)
    static async Task<string> FetchDataAsync(string source, int delayMs)
    {
        await Task.Delay(delayMs);
        return $"[Data from {source}]";
    }

    static async Task<int> ComputeAsync(int input, int delayMs)
    {
        await Task.Delay(delayMs);
        return input * input;
    }

    public static async Task RunAsync()
    {
        Console.WriteLine("\n========== E10: ASYNC / AWAIT ==========");

        // ── EXERCISE 1: Basic async method ────────────────────
        // Implement async Task<string> GetGreetingAsync(string name)
        // that waits 100ms (simulating I/O) then returns "Hello, {name}!".
        // Call it with await and print the result.
        //
        // TODO: implement GetGreetingAsync

        // var greeting = await GetGreetingAsync("Alice");
        // Console.WriteLine(greeting); // Hello, Alice!

        // ── EXERCISE 2: Sequential vs parallel awaiting ────────
        // You have two "fetch" operations, each taking 300ms.
        //
        // a) Await them SEQUENTIALLY and measure total time (~600ms).
        // b) Await them IN PARALLEL with Task.WhenAll and measure time (~300ms).
        //
        // KEY: sequential = await A, then await B (total = A+B)
        //      parallel   = start A and B together, await both (total = max(A,B))
        //
        // TODO: implement both versions and print elapsed time

        // // Sequential:
        // var sw = System.Diagnostics.Stopwatch.StartNew();
        // var r1 = await FetchDataAsync("API-1", 300);
        // var r2 = await FetchDataAsync("API-2", 300);
        // Console.WriteLine($"Sequential: {sw.ElapsedMilliseconds}ms"); // ~600ms
        //
        // // Parallel:
        // sw.Restart();
        // var (p1, p2) = await ... // Task.WhenAll
        // Console.WriteLine($"Parallel: {sw.ElapsedMilliseconds}ms");   // ~300ms

        // ── EXERCISE 3: Task.WhenAll with results ─────────────
        // Start 5 compute tasks in parallel (inputs 1–5, each 100ms delay).
        // Collect all results with Task.WhenAll.
        // Print each result and the sum.
        //
        // TODO: use Task.WhenAll on a list of tasks

        // var tasks = Enumerable.Range(1, 5).Select(n => ComputeAsync(n, 100));
        // var results = await Task.WhenAll(tasks);
        // foreach (var r in results) Console.Write(r + " "); // 1 4 9 16 25
        // Console.WriteLine($"\nSum: {results.Sum()}");      // 55

        // ── EXERCISE 4: Task.WhenAny ──────────────────────────
        // Start 3 tasks with different delays (500ms, 200ms, 800ms).
        // Use Task.WhenAny to get the result of whichever finishes first,
        // then cancel/ignore the others.
        // Print which source won and its result.
        //
        // TODO: use Task.WhenAny

        // var t1 = FetchDataAsync("Slow",   500);
        // var t2 = FetchDataAsync("Fast",   200);
        // var t3 = FetchDataAsync("Slower", 800);
        // var winner = await Task.WhenAny(t1, t2, t3);
        // Console.WriteLine($"Winner: {await winner}"); // [Data from Fast]

        // ── EXERCISE 5: CancellationToken ─────────────────────
        // Implement async Task<string> FetchWithCancelAsync(string source, int delayMs, CancellationToken ct)
        // that checks cancellation and throws OperationCanceledException if cancelled.
        // Use CancellationTokenSource with a 250ms timeout.
        // Call your method with a 500ms delay — it should be cancelled.
        //
        // TODO: implement FetchWithCancelAsync, handle the cancellation

        // using var cts = new CancellationTokenSource(250); // cancel after 250ms
        // try
        // {
        //     var result = await FetchWithCancelAsync("DB", 500, cts.Token);
        //     Console.WriteLine(result);
        // }
        // catch (OperationCanceledException)
        // {
        //     Console.WriteLine("Operation was cancelled"); // expected
        // }

        // ── EXERCISE 6: Exception handling in async ───────────
        // Implement async Task<int> DivideAsync(int a, int b)
        // that waits 50ms then returns a/b, throwing DivideByZeroException if b==0.
        //
        // a) Catch the exception correctly using try/catch around await.
        // b) Show what happens with Task.WhenAll when ONE task fails:
        //    start 3 tasks (one throws), WhenAll throws AggregateException.
        //    Catch it and print all inner exceptions.
        //
        // TODO: implement DivideAsync, demonstrate both cases

        // // Case a:
        // try { Console.WriteLine(await DivideAsync(10, 0)); }
        // catch (DivideByZeroException) { Console.WriteLine("Caught divide by zero"); }
        //
        // // Case b:
        // var t1 = DivideAsync(10, 2);
        // var t2 = DivideAsync(20, 0);  // this will throw
        // var t3 = DivideAsync(30, 3);
        // try { await Task.WhenAll(t1, t2, t3); }
        // catch { /* WhenAll throws the first exception */ }
        // // to see ALL exceptions:
        // var allTasks = new[] { t1, t2, t3 };
        // var errors = allTasks.Where(t => t.IsFaulted).Select(t => t.Exception!.InnerException!.Message);
        // foreach (var err in errors) Console.WriteLine($"Error: {err}");

        // ── EXERCISE 7: async void — why it's dangerous ────────
        // Demonstrate the problem with async void:
        //   - Exceptions from async void cannot be caught by the caller
        //   - The caller has no way to await it or know when it's done
        //
        // Write an async void method BadFireAndForget() that throws after 50ms.
        // Show that wrapping the CALL in try/catch does NOT catch the exception.
        // Then write a correct version using async Task + proper awaiting.
        //
        // DO NOT actually throw unhandled in the process — just add a comment
        // showing the pattern and why Task is always preferred.
        //
        // TODO: write both versions with explanation comments

        // // ❌ BAD — exception is unobservable:
        // // async void BadFireAndForget() { await Task.Delay(50); throw new Exception("lost!"); }
        // // try { BadFireAndForget(); } catch { } // catch NEVER fires!
        //
        // // ✅ GOOD — exception is observable:
        // // async Task GoodFireAndForget() { await Task.Delay(50); throw new Exception("caught!"); }
        // // try { await GoodFireAndForget(); } catch (Exception e) { Console.WriteLine(e.Message); }

        // ── EXERCISE 8: SemaphoreSlim for async rate-limiting ─
        // You have 10 tasks but want max 3 running at the same time.
        // Use SemaphoreSlim(3, 3) to limit concurrency.
        // Each task prints "Starting N", waits 200ms, prints "Done N".
        // Verify that at most 3 tasks run concurrently.
        //
        // TODO: implement using SemaphoreSlim

        // var semaphore = new SemaphoreSlim(3, 3);
        // var tasks = Enumerable.Range(1, 10).Select(async i =>
        // {
        //     await semaphore.WaitAsync();
        //     try
        //     {
        //         Console.WriteLine($"Starting {i}");
        //         await Task.Delay(200);
        //         Console.WriteLine($"Done {i}");
        //     }
        //     finally { semaphore.Release(); }
        // });
        // await Task.WhenAll(tasks);

        // ── EXERCISE 9: IAsyncEnumerable (async stream) ────────
        // Implement async IAsyncEnumerable<int> CountdownAsync(int from)
        // that yields numbers from `from` down to 1, with 100ms between each.
        // Consume it with await foreach.
        //
        // TODO: implement CountdownAsync using yield return + await Task.Delay

        // await foreach (var n in CountdownAsync(5))
        //     Console.Write(n + " "); // 5 4 3 2 1
        // Console.WriteLine();

        // ── EXERCISE 10: Retry with async ────────────────────
        // Implement async Task<T> RetryAsync<T>(Func<Task<T>> operation, int maxAttempts)
        // Retries on any exception, waiting 50ms * attemptNumber between retries.
        // On final failure re-throws the last exception.
        //
        // TODO: implement RetryAsync

        // int attempt = 0;
        // var result = await RetryAsync(async () =>
        // {
        //     attempt++;
        //     if (attempt < 3) throw new HttpRequestException($"Attempt {attempt} failed");
        //     return await ComputeAsync(7, 50);
        // }, maxAttempts: 5);
        // Console.WriteLine($"Result: {result}"); // 49  (7²)
    }
}

// ============================================================
// E04 — Generics  (10 exercises)
// Topics: generic classes, methods, constraints, patterns
// ============================================================

public static class E04_Generics
{
    public static void Run()
    {
        Console.WriteLine("\n========== E04: GENERICS ==========");

        // ── EXERCISE 1: Generic Stack<T> ──────────────────────────
        // Implement class Stack<T> with:
        //   void Push(T item)
        //   T Pop()             → throws InvalidOperationException if empty
        //   T Peek()            → throws InvalidOperationException if empty
        //   int Count
        //   bool IsEmpty
        //
        // TODO: implement Stack<T>

        // var stack = new Stack<int>();
        // stack.Push(1); stack.Push(2); stack.Push(3);
        // Console.WriteLine(stack.Peek());  // 3  (not removed)
        // Console.WriteLine(stack.Pop());   // 3
        // Console.WriteLine(stack.Pop());   // 2
        // Console.WriteLine(stack.Count);   // 1

        // ── EXERCISE 2: Generic method with IComparable ───────────
        // Implement static T Max<T>(T a, T b) where T : IComparable<T>
        // that returns the larger of the two values.
        // Also implement static T Clamp<T>(T value, T min, T max)
        // that constrains value to [min, max].
        //
        // TODO: implement Max<T> and Clamp<T>

        // Console.WriteLine(Max(3, 7));           // 7
        // Console.WriteLine(Max("apple","mango")); // mango
        // Console.WriteLine(Clamp(15, 0, 10));    // 10
        // Console.WriteLine(Clamp(-5, 0, 10));    // 0
        // Console.WriteLine(Clamp(5, 0, 10));     // 5

        // ── EXERCISE 3: Generic Pair<TFirst, TSecond> ─────────────
        // Implement class Pair<TFirst, TSecond> with:
        //   TFirst First  { get; }
        //   TSecond Second { get; }
        //   Constructor that sets both.
        //   Override ToString() → "(First, Second)".
        //   Static factory method: Pair.Create(first, second) → Pair<TFirst, TSecond>
        //     (so callers don't have to write the type parameters explicitly).
        //
        // TODO: implement Pair<TFirst, TSecond>

        // var p1 = new Pair<string, int>("score", 100);
        // var p2 = Pair.Create("level", 5);
        // Console.WriteLine(p1);  // (score, 100)
        // Console.WriteLine(p2);  // (level, 5)

        // ── EXERCISE 4: Generic repository ───────────────────────
        // Define interface IRepository<T> where T : class with:
        //   void Add(T entity)
        //   T? GetById(int id)
        //   IEnumerable<T> GetAll()
        //   void Remove(int id)
        // Implement InMemoryRepository<T> using a List<T> internally.
        // T must implement an IEntity interface that exposes int Id.
        //
        // TODO: implement IEntity, IRepository<T>, InMemoryRepository<T>
        // TODO: create a simple Product class that implements IEntity

        // var repo = new InMemoryRepository<Product>();
        // repo.Add(new Product { Id = 1, Name = "Widget" });
        // repo.Add(new Product { Id = 2, Name = "Gadget" });
        // Console.WriteLine(repo.GetById(1)?.Name);  // Widget
        // Console.WriteLine(repo.GetAll().Count());  // 2
        // repo.Remove(1);
        // Console.WriteLine(repo.GetAll().Count());  // 1

        // ── EXERCISE 5: Generic Result<T> ─────────────────────────
        // Implement class Result<T> that represents either success or failure:
        //   bool IsSuccess
        //   T Value           (only valid when IsSuccess)
        //   string Error      (only valid when !IsSuccess)
        //   static Result<T> Ok(T value)
        //   static Result<T> Fail(string error)
        // Implement string ToString() appropriately for both cases.
        //
        // TODO: implement Result<T>

        // var ok   = Result<int>.Ok(42);
        // var fail = Result<int>.Fail("not found");
        // Console.WriteLine(ok);    // Success: 42
        // Console.WriteLine(fail);  // Error: not found
        // Console.WriteLine(ok.Value);    // 42
        // Console.WriteLine(fail.Error);  // not found

        // ── EXERCISE 6: Generic Filter and Map ────────────────────
        // Implement (without using LINQ):
        //   static List<T>       Filter<T>(IEnumerable<T> source, Func<T,bool> predicate)
        //   static List<TResult> Map<T,TResult>(IEnumerable<T> source, Func<T,TResult> selector)
        //   static TAccum        Reduce<T,TAccum>(IEnumerable<T> source, TAccum seed, Func<TAccum,T,TAccum> fn)
        //
        // TODO: implement Filter, Map, Reduce

        // var nums = new[] { 1, 2, 3, 4, 5, 6 };
        // var evens   = Filter(nums, n => n % 2 == 0);
        // var doubled = Map(nums, n => n * 2);
        // var sum     = Reduce(nums, 0, (acc, n) => acc + n);
        // Console.WriteLine(string.Join(", ", evens));   // 2, 4, 6
        // Console.WriteLine(string.Join(", ", doubled)); // 2, 4, 6, 8, 10, 12
        // Console.WriteLine(sum);                        // 21

        // ── EXERCISE 7: new() constraint ─────────────────────────
        // Implement static List<T> CreateMany<T>(int count) where T : new()
        // that creates a list of `count` default-constructed instances of T.
        // Also implement static T CreateAndInit<T>(Action<T> init) where T : new()
        // that creates one T and runs the init action on it before returning.
        //
        // TODO: implement CreateMany<T> and CreateAndInit<T>

        // var items = CreateMany<System.Text.StringBuilder>(3);
        // Console.WriteLine(items.Count);  // 3
        //
        // var sb = CreateAndInit<System.Text.StringBuilder>(b => b.Append("hello"));
        // Console.WriteLine(sb);  // hello

        // ── EXERCISE 8: Generic event aggregator ──────────────────
        // Implement class EventBus with:
        //   void Subscribe<TEvent>(Action<TEvent> handler)
        //   void Publish<TEvent>(TEvent evt)
        // When Publish is called, all handlers registered for that TEvent type are invoked.
        // Use Dictionary<Type, List<Delegate>> internally.
        //
        // TODO: implement EventBus
        // TODO: create two simple event classes: UserCreated(string Name) and OrderPlaced(int OrderId)

        // var bus = new EventBus();
        // bus.Subscribe<UserCreated>(e  => Console.WriteLine($"User: {e.Name}"));
        // bus.Subscribe<OrderPlaced>(e  => Console.WriteLine($"Order: {e.OrderId}"));
        // bus.Publish(new UserCreated("Alice"));   // User: Alice
        // bus.Publish(new OrderPlaced(42));        // Order: 42

        // ── EXERCISE 9: Generic cache ────────────────────────────
        // Implement class Cache<TKey, TValue> where TKey : notnull with:
        //   TValue GetOrAdd(TKey key, Func<TKey, TValue> factory)
        //     → returns cached value, or calls factory and caches it.
        //   bool TryGet(TKey key, out TValue? value)
        //   void Invalidate(TKey key)
        //   int Count
        //
        // TODO: implement Cache<TKey, TValue>

        // var cache = new Cache<string, int>();
        // int callCount = 0;
        // int getValue(string k) { callCount++; return k.Length; }
        // Console.WriteLine(cache.GetOrAdd("hello", getValue)); // 5
        // Console.WriteLine(cache.GetOrAdd("hello", getValue)); // 5  (from cache)
        // Console.WriteLine(callCount);  // 1  (factory only called once)
        // cache.Invalidate("hello");
        // Console.WriteLine(cache.GetOrAdd("hello", getValue)); // 5
        // Console.WriteLine(callCount);  // 2

        // ── EXERCISE 10: Covariance with IEnumerable ─────────────
        // Demonstrate that IEnumerable<Derived> can be assigned to IEnumerable<Base>
        // because IEnumerable<out T> is covariant.
        // Then show that List<T> is NOT covariant (IList<T> is invariant).
        // Write a method static void PrintAll(IEnumerable<object> items)
        // that prints each item, and call it with a List<string>.
        //
        // Explain in a comment WHY covariance is safe for IEnumerable but NOT for IList.
        //
        // TODO: demonstrate and explain covariance

        // var strings = new List<string> { "a", "b", "c" };
        // PrintAll(strings);  // a / b / c  (works because IEnumerable<out T> is covariant)
        // // IList<object> list = strings;  // would NOT compile — IList<T> is invariant
    }
}

// ============================================================
// E11 — Collections  (10 exercises)
// Topics: List<T>, Dictionary, HashSet, Queue, Stack,
//         SortedDictionary, IEnumerable/ICollection/IList,
//         ConcurrentDictionary, custom sorting
// ============================================================
// COMPLEXITY CHEAT SHEET (know this for interviews):
//   List<T>            Add O(1) amortized | Insert O(n) | Search O(n) | Sort O(n log n)
//   Dictionary<K,V>    Get/Add/Remove O(1) average
//   HashSet<T>         Add/Contains/Remove O(1) average
//   SortedDictionary   Get/Add/Remove O(log n) — keys always sorted
//   Queue<T>           Enqueue/Dequeue O(1)
//   Stack<T>           Push/Pop O(1)
//   LinkedList<T>      Insert/Remove at known node O(1) | Search O(n)
// ============================================================

using System.Collections.Concurrent;

public static class E11_Collections
{
    public static void Run()
    {
        Console.WriteLine("\n========== E11: COLLECTIONS ==========");

        // ── EXERCISE 1: List<T> operations ────────────────────
        // Create a List<int> with values 5,3,8,1,9,2,7,4,6.
        // a) Sort ascending and print.
        // b) BinarySearch for value 7 (list must be sorted first) — print the index.
        // c) Remove all even numbers using RemoveAll.
        // d) Insert 0 at index 2.
        // e) Print final list.
        //
        // TODO: implement steps a-e

        // var list = new List<int> { 5, 3, 8, 1, 9, 2, 7, 4, 6 };
        // list.Sort();
        // Console.WriteLine(string.Join(", ", list));          // 1,2,3,4,5,6,7,8,9
        // Console.WriteLine(list.BinarySearch(7));             // 6
        // list.RemoveAll(n => n % 2 == 0);
        // list.Insert(2, 0);
        // Console.WriteLine(string.Join(", ", list));          // 1,3,0,5,7,9

        // ── EXERCISE 2: Dictionary<K,V> ───────────────────────
        // Build a word frequency counter:
        // Given a string of words, count how many times each word appears.
        // Use TryGetValue to increment safely.
        // Print words sorted by frequency descending.
        //
        // TODO: implement word frequency counter

        // var text = "the quick brown fox jumps over the lazy dog the fox";
        // var freq = new Dictionary<string, int>();
        // foreach (var word in text.Split(' '))
        //     ... // use TryGetValue + increment or GetValueOrDefault
        // foreach (var kv in freq.OrderByDescending(kv => kv.Value))
        //     Console.WriteLine($"{kv.Key}: {kv.Value}");

        // ── EXERCISE 3: HashSet<T> ────────────────────────────
        // a) Create two HashSet<int>: A = {1,2,3,4,5} and B = {3,4,5,6,7}.
        //    Demonstrate: UnionWith, IntersectWith, ExceptWith, IsSubsetOf.
        //    (Note: these methods MUTATE the set — use copies)
        //
        // b) Use a HashSet to deduplicate a list with duplicates and
        //    preserve insertion order (hint: LinkedHashSet doesn't exist in C#
        //    — use a loop with HashSet.Add to filter while iterating).
        //
        // TODO: implement set operations and deduplication

        // var A = new HashSet<int> { 1,2,3,4,5 };
        // var B = new HashSet<int> { 3,4,5,6,7 };
        // var union     = new HashSet<int>(A); union.UnionWith(B);
        // var intersect = new HashSet<int>(A); intersect.IntersectWith(B);
        // var diff      = new HashSet<int>(A); diff.ExceptWith(B);
        // Console.WriteLine($"Union:     {string.Join(",", union)}");     // 1,2,3,4,5,6,7
        // Console.WriteLine($"Intersect: {string.Join(",", intersect)}"); // 3,4,5
        // Console.WriteLine($"Diff A-B:  {string.Join(",", diff)}");      // 1,2

        // ── EXERCISE 4: Queue<T> ──────────────────────────────
        // Simulate a print queue:
        // - Enqueue 5 print jobs ("Doc1"–"Doc5").
        // - Add a priority job "URGENT" to the FRONT (hint: Queue doesn't support this —
        //   you'll need to rebuild the queue or use a different approach. Think about it.)
        // - Dequeue and "print" all jobs one by one with Dequeue.
        // - Show Peek() before dequeue.
        //
        // TODO: implement print queue with priority insertion

        // var queue = new Queue<string>();
        // for (int i = 1; i <= 5; i++) queue.Enqueue($"Doc{i}");
        // // Insert URGENT at front — Queue has no AddFirst, so reconstruct:
        // var withPriority = new Queue<string>();
        // withPriority.Enqueue("URGENT");
        // while (queue.Count > 0) withPriority.Enqueue(queue.Dequeue());
        // Console.WriteLine($"Next: {withPriority.Peek()}"); // URGENT
        // while (withPriority.Count > 0) Console.WriteLine($"Printing: {withPriority.Dequeue()}");

        // ── EXERCISE 5: Stack<T> ──────────────────────────────
        // Implement a bracket validator using Stack<char>:
        //   IsBalanced(string s) → true if all (, [, { are properly closed.
        // Test with: "([]{})", "([)]", "(((" — expected: true, false, false.
        //
        // TODO: implement IsBalanced using Stack<char>

        // Console.WriteLine(IsBalanced("([]{})")); // True
        // Console.WriteLine(IsBalanced("([)]"));   // False
        // Console.WriteLine(IsBalanced("((("));    // False

        // ── EXERCISE 6: SortedDictionary ──────────────────────
        // Create a SortedDictionary<string, int> mapping country names to population.
        // Add: Croatia=4M, Germany=84M, Austria=9M, France=68M, Spain=47M.
        // Iterate and print — notice keys are always in alphabetical order.
        // Then find all countries with population > 40M using LINQ.
        //
        // TODO: implement with SortedDictionary

        // var countries = new SortedDictionary<string, int>
        // {
        //     ["Croatia"] = 4, ["Germany"] = 84, ["Austria"] = 9, ["France"] = 68, ["Spain"] = 47
        // };
        // foreach (var kv in countries) Console.WriteLine($"{kv.Key}: {kv.Value}M");
        // // Austria, Croatia, France, Germany, Spain  (alphabetical!)
        // var big = countries.Where(kv => kv.Value > 40).Select(kv => kv.Key);
        // Console.WriteLine(string.Join(", ", big)); // France, Germany, Spain

        // ── EXERCISE 7: IEnumerable vs ICollection vs IList ───
        // Write three methods that accept different levels of the collection hierarchy:
        //   void PrintAll(IEnumerable<string> items)    — can only iterate
        //   void PrintCount(ICollection<string> items)  — can also get Count, Add, Remove
        //   void PrintAt(IList<string> items)           — can also index: items[2]
        //
        // Explain in comments what each interface adds over the previous.
        // Demonstrate which types are compatible with each parameter.
        //
        // TODO: implement and call with List<string>, string[], HashSet<string>

        // var list  = new List<string> { "a", "b", "c" };
        // var array = new string[] { "x", "y", "z" };
        // var set   = new HashSet<string> { "p", "q", "r" };
        // PrintAll(list);    PrintAll(array);   PrintAll(set);   // all work
        // PrintCount(list);  PrintCount(array); // PrintCount(set) — does HashSet implement ICollection? YES
        // PrintAt(list);     PrintAt(array);    // PrintAt(set) — does this compile? NO — HashSet is not IList

        // ── EXERCISE 8: Custom sorting with IComparer ─────────
        // Create class Person with Name (string) and Age (int).
        // Create three IComparer<Person> implementations:
        //   ByNameAsc      → alphabetical by Name
        //   ByAgeDesc      → oldest first
        //   ByNameLengthAsc → shortest name first
        // Sort the same list three ways and print.
        //
        // TODO: implement Person and the three comparers

        // var people = new List<Person>
        // {
        //     new("Charlie", 30), new("Alice", 25), new("Bob", 35), new("Diana", 28)
        // };
        // people.Sort(new ByNameAsc());
        // Console.WriteLine(string.Join(", ", people.Select(p => p.Name))); // Alice, Bob, Charlie, Diana

        // ── EXERCISE 9: ConcurrentDictionary ──────────────────
        // ConcurrentDictionary is thread-safe — use it when multiple threads
        // read/write the same dictionary.
        //
        // Simulate 10 concurrent tasks all incrementing a counter per key.
        // Use ConcurrentDictionary<string, int> with AddOrUpdate.
        // If you used a regular Dictionary here, you'd get race conditions.
        //
        // TODO: implement concurrent counter with AddOrUpdate

        // var counts = new ConcurrentDictionary<string, int>();
        // var tasks = Enumerable.Range(1, 100).Select(i => Task.Run(() =>
        // {
        //     var key = $"key{i % 5}";  // 5 distinct keys
        //     counts.AddOrUpdate(key, 1, (k, old) => old + 1);
        // }));
        // Task.WaitAll(tasks.ToArray());
        // foreach (var kv in counts.OrderBy(kv => kv.Key))
        //     Console.WriteLine($"{kv.Key}: {kv.Value}"); // each should be ~20

        // ── EXERCISE 10: LinkedList<T> ────────────────────────
        // LinkedList shines when you need fast INSERT/REMOVE in the MIDDLE
        // without shifting elements (unlike List).
        //
        // Create a LinkedList<string> playlist: A → B → C → D → E.
        // a) Insert "X" after "C" using AddAfter.
        // b) Remove "B" using Remove.
        // c) Move "E" to the front using RemoveLast + AddFirst.
        // Print the final playlist after each operation.
        //
        // TODO: implement LinkedList operations

        // var playlist = new LinkedList<string>(new[] { "A","B","C","D","E" });
        // var nodeC = playlist.Find("C")!;
        // playlist.AddAfter(nodeC, "X");
        // Console.WriteLine(string.Join("→", playlist)); // A→B→C→X→D→E
        // playlist.Remove("B");
        // Console.WriteLine(string.Join("→", playlist)); // A→C→X→D→E
        // var last = playlist.Last!.Value;
        // playlist.RemoveLast();
        // playlist.AddFirst(last);
        // Console.WriteLine(string.Join("→", playlist)); // E→A→C→X→D
    }
}

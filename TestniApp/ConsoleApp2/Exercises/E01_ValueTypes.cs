// ============================================================
// E01 — Value Types & Reference Types  (10 exercises)
// Topics: struct/class, boxing, ref/out/in, const/readonly
// ============================================================
// HOW TO USE:
//   Implement each TODO, then uncomment the test block to verify.
// ============================================================

using System.Numerics;
using static E01_ValueTypes;

public static class E01_ValueTypes
{
    public static void PrintType(object entity)
    {
        Console.WriteLine(entity.GetType().IsValueType ? $"Value Type: {entity}" : $"Reference Type: {entity}");
    }
    public static void Run()
    {
        Console.WriteLine("\n========== E01: VALUE TYPES ==========");

        // ── EXERCISE 1: Vector2D struct ──────────────────────────
        // Create a struct called Vector2D with int fields X and Y.
        // Add a method Add(Vector2D other) → returns a new Vector2D (sum of both).
        // Override ToString() → returns "(X, Y)".
        // Verify copy semantics: mutating a copy must NOT affect the original.
        //
        // TODO: implement Vector2D struct

        //var v1 = new Vector2d { x = 1, y = 2 };
        //var v2 = new Vector2d { x = 3, y = 4 };
        //Console.WriteLine(v1.Add(v2));  // (4, 6)
        //var copy = v1;
        //copy.x = 99;
        //Console.WriteLine(v1);          // (1, 2)  ← unchanged

        // ── EXERCISE 2: readonly struct ──────────────────────────
        // Create a readonly struct Temperature with a double _celsius field.
        // Constructor takes celsius value.
        // Property Celsius → returns _celsius.
        // Property Fahrenheit → computed: celsius * 9/5 + 32.
        // Override ToString() → "X°C / Y°F".
        //
        // TODO: implement Temperature readonly struct

        //var t = new Temperature(100);
        //Console.WriteLine(t);              // 100°C / 212°F
        //Console.WriteLine(t.Fahrenheit());   // 212

        // ── EXERCISE 3: Swap with ref ─────────────────────────────
        // Implement static void Swap(ref int a, ref int b)
        // that swaps two integers without using a temp variable.
        // (Hint: XOR swap or arithmetic)
        //
        // TODO: implement Swap

        //int x = 10, y = 25;
        //Swap(ref x, ref y);
        //Console.WriteLine($"{x} {y}");  // 25 10

        // ── EXERCISE 4: TryParse pattern ─────────────────────────
        // Implement static bool TryParsePositiveInt(string input, out int value)
        // Returns true only when input parses to an integer > 0.
        // Sets value to 0 on failure.
        //
        // TODO: implement TryParsePositiveInt

        //string[] tests = { "42", "-5", "abc", "0", "100" };
        //foreach (var s in tests)
        //    Console.WriteLine(TryParsePositiveInt(s, out int v) ? $"OK: {v}" : $"FAIL: {s}");
        // // OK: 42 / FAIL: -5 / FAIL: abc / FAIL: 0 / OK: 100

        // ── EXERCISE 5: MinMax with out ───────────────────────────
        // Implement static void MinMax(int[] arr, out int min, out int max)
        // that returns both the minimum and maximum of the array in one call.
        // Do NOT use LINQ.
        //
        // TODO: implement MinMax

        //MinMax(new[] { 3, 1, 4, 1, 5, 9, 2, 6 }, out int min, out int max);
        //Console.WriteLine($"Min={min} Max={max}");  // Min=1 Max=9

        // ── EXERCISE 6: in parameter ──────────────────────────────
        // Create a struct Matrix2x2 with four double fields: A, B, C, D
        // representing a 2×2 matrix [ [A,B], [C,D] ].
        // Implement static double Determinant(in Matrix2x2 m) → A*D - B*C.
        // Using `in` avoids copying the struct — explain this in a comment.
        //Because we are using a in parameter, we are esentially giving the method an address to the struct but only to read it
        // TODO: implement Matrix2x2 struct and Determinant method

        //var m = new Matrix2x2 { A = 3, B = 8, C = 4, D = 6 };
        //Console.WriteLine(Determinant(in m));  // -14  (3*6 - 8*4)

        // ── EXERCISE 7: const vs static readonly ─────────────────
        // Create a static class AppConfig with:
        //   - const int MaxRetries = 3              (compile-time literal)
        //   - static readonly string Version        (set from Environment or any method call)
        //   - static readonly DateTime StartedAt    (set to DateTime.Now)
        // Print all three. Explain in a comment WHY Version can't be const.
        //
        // TODO: implement AppConfig

        //Console.WriteLine(AppConfig.MaxRetries);   // 3
        //Console.WriteLine(AppConfig.Version);      // (some string)
        //Console.WriteLine(AppConfig.Time);    // current time

        // ── EXERCISE 8: Immutable struct counter ─────────────────
        // Create a readonly struct Counter with int Value.
        // Implement Increment() → returns a NEW Counter with Value + 1.
        // Implement Add(int amount) → returns a NEW Counter with Value + amount.
        // The original Counter must never change.
        //
        // TODO: implement Counter struct

        //var c = new Counter(0);
        //var c1 = c.Increment();
        //var c2 = c1.Increment().Increment();
        //var c3 = c.Add(10);
        //Console.WriteLine(c.Value);   // 0  ← unchanged
        //Console.WriteLine(c1.Value);  // 1
        //Console.WriteLine(c2.Value);  // 3
        //Console.WriteLine(c3.Value);  // 10

        // ── EXERCISE 9: Boxing awareness ─────────────────────────
        // Implement static void PrintType(object value) that prints:
        //   "Value type: X" if the boxed object is a value type
        //   "Reference type: X" otherwise
        // Use value.GetType().IsValueType to check.
        // Call it with int, double, string, DateTime, bool.
        //
        // TODO: implement PrintType

        //PrintType(42);           // Value type: 42
        //PrintType(3.14);         // Value type: 3.14
        //PrintType("hello");      // Reference type: hello
        //PrintType(DateTime.Now); // Value type: [date]
        //PrintType(true);         // Value type: True

        // ── EXERCISE 10: Struct equality ──────────────────────────
        // Create a struct ColorRGB with byte R, G, B.
        // Override Equals(object? obj) and GetHashCode() for value-based equality.
        // Override ToString() → "rgb(R, G, B)".
        // Two ColorRGB values with the same R,G,B must be equal (==).
        // Also overload the == and != operators.
        //
        // TODO: implement ColorRGB struct

        //var red1 = new ColorRGB { R = 255, G = 0, B = 0 };
        //var red2 = new ColorRGB { R = 255, G = 0, B = 0 };
        //var blue = new ColorRGB { R = 0, G = 0, B = 255 };
        //Console.WriteLine(red1 == red2);  // True
        //Console.WriteLine(red1 == blue);  // False
        //Console.WriteLine(red1);          // rgb(255, 0, 0)
    }

    public struct ColorRGB
    {
        public int R, G, B;

        public override bool Equals(object? obj)
        {
            return obj is ColorRGB rGB &&
                   R == rGB.R &&
                   G == rGB.G &&
                   B == rGB.B;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(R, G, B);
        }

        public override string? ToString()
        {
            return $"rgb({R}, {G}, {B})";
        }

        public static bool operator ==(ColorRGB r1, ColorRGB r2) {
            return r1.Equals(r2);
        }

        public static bool operator !=(ColorRGB r1, ColorRGB r2)
        {
            return !r1.Equals(r2);

        }

    }

    private static void MinMax(int[] ints, out int min, out int max)
    {
        min = ints[0];
        max = ints[0];
        foreach (var i in ints) if(i < min) min = i; else if(i > max) max = i;
    }

    private static bool TryParsePositiveInt(string s, out int v)
    {
        if(int.TryParse(s, out v)) return v >0;
        v = 0;
        return false;
    }

    private static void Swap(ref int x, ref int y)
    {
        x = x + y;
        y = x - y;
        x = x - y;
    }
}

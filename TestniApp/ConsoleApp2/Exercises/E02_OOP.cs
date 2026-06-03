// ============================================================
// E02 — Object-Oriented Programming  (10 exercises)
// Topics: inheritance, virtual/override, new (hiding),
//         abstract, interface, explicit impl, sealed
// ============================================================

public static class E02_OOP
{
    public class Vehicle
    {
        public virtual string Describe() => "Vehicle";
    }

    public class Car : Vehicle
    {
        public override string Describe() => "Car";
    }

    public class Bicycle : Vehicle
    {
        public new string Describe() => "Bycicle";
    }

    public static void Run()
    {
        Console.WriteLine("\n========== E02: OOP ==========");

        // ── EXERCISE 1: Basic inheritance + virtual ───────────────
        // Create class Animal with a virtual method Speak() → "...".
        // Create Dog : Animal that overrides Speak() → "Woof".
        // Create Cat : Animal that overrides Speak() → "Meow".
        // Create Parrot : Animal that overrides Speak() → "Squawk".
        // Store all in a List<Animal> and call Speak() on each.
        //
        // TODO: implement Animal, Dog, Cat, Parrot

        //var animals = new List<Animal> { new Dog(), new Cat(), new Parrot() };
        //foreach (var a in animals)
        //    a.Speak();
        // Woof / Meow / Squawk

        // ── EXERCISE 2: virtual/override vs new (hiding) ──────────
        // Create class Vehicle with virtual string Describe() → "Vehicle".
        // Create Car : Vehicle that OVERRIDES Describe() → "Car".
        // Create Bicycle : Vehicle that HIDES Describe() with `new` → "Bicycle".
        // Demonstrate the difference using a Vehicle-typed variable for each.
        //
        // TODO: implement Vehicle, Car, Bicycle
        // (Explain with a comment WHY they print differently)

        //Vehicle v1 = new Car();
        //Vehicle v2 = new Bicycle();
        //Console.WriteLine(v1.Describe()); // Car      ← override: runtime type wins
        //Console.WriteLine(v2.Describe()); // Vehicle  ← new/hiding: compile-time type wins

        // ── EXERCISE 3: abstract class ────────────────────────────
        // Create abstract class Shape with:
        //   - abstract double Area()
        //   - abstract double Perimeter()
        //   - virtual string Describe() → "Shape: area=X perimeter=Y" (uses the two above)
        // Implement Circle(double radius), Rectangle(double w, double h), Triangle(double a, double b, double c).
        //
        // TODO: implement Shape, Circle, Rectangle, Triangle

        // Shape[] shapes = { new Circle(5), new Rectangle(4, 6), new Triangle(3, 4, 5) };
        // foreach (var s in shapes)
        //     Console.WriteLine(s.Describe());

        // ── EXERCISE 4: interfaces ────────────────────────────────
        // Define interfaces IFlyable (Fly() → string) and ISwimmable (Swim() → string).
        // Create Duck : Animal, IFlyable, ISwimmable.
        // Create Eagle : Animal, IFlyable.
        // Create Fish : ISwimmable.
        // Write a method PrintAbilities(object obj) that checks each interface
        // with `is` and prints what the object can do.
        //
        // TODO: implement the interfaces and classes, and PrintAbilities

        // PrintAbilities(new Duck());   // Can fly / Can swim
        // PrintAbilities(new Eagle());  // Can fly
        // PrintAbilities(new Fish());   // Can swim

        // ── EXERCISE 5: explicit interface implementation ─────────
        // Define IPrinter with void Print().
        // Define IScanner with void Print().
        // Create AllInOne that implements BOTH explicitly so each prints
        // a different message depending on which interface you cast to.
        //
        // TODO: implement AllInOne

        // var device = new AllInOne();
        // ((IPrinter)device).Print();  // Printing document...
        // ((IScanner)device).Print();  // Scanning document...

        // ── EXERCISE 6: constructor chaining ──────────────────────
        // Create class Person with fields Name (string) and Age (int).
        // Create three constructors:
        //   Person(string name, int age)
        //   Person(string name)           → calls first with age = 0
        //   Person()                      → calls second with name = "Unknown"
        // Override ToString() → "Person(Name, Age)".
        //
        // TODO: implement Person

        // Console.WriteLine(new Person("Alice", 30));  // Person(Alice, 30)
        // Console.WriteLine(new Person("Bob"));        // Person(Bob, 0)
        // Console.WriteLine(new Person());             // Person(Unknown, 0)

        // ── EXERCISE 7: sealed class ──────────────────────────────
        // Create a class hierarchy: Logger → FileLogger → (sealed) DailyFileLogger.
        // Logger has virtual void Log(string msg).
        // FileLogger overrides it to prefix "[FILE]".
        // DailyFileLogger overrides it to prefix "[DAILY yyyy-MM-dd]".
        // Seal DailyFileLogger so it cannot be further subclassed.
        // Add a comment explaining when you would use sealed.
        //
        // TODO: implement Logger, FileLogger, DailyFileLogger

        // Logger log = new DailyFileLogger();
        // log.Log("startup");  // [DAILY 2025-05-25] startup

        // ── EXERCISE 8: override ToString / Equals / GetHashCode ──
        // Create class Product with int Id and string Name.
        // Two Products are equal when their Id is the same (regardless of Name).
        // Override ToString() → "Product(Id: X, Name: Y)".
        // Override Equals and GetHashCode consistently.
        //
        // TODO: implement Product

        // var p1 = new Product { Id = 1, Name = "Widget" };
        // var p2 = new Product { Id = 1, Name = "Widget v2" };
        // var p3 = new Product { Id = 2, Name = "Gadget" };
        // Console.WriteLine(p1.Equals(p2));  // True  (same Id)
        // Console.WriteLine(p1.Equals(p3));  // False
        // Console.WriteLine(p1);             // Product(Id: 1, Name: Widget)

        // ── EXERCISE 9: abstract template method pattern ──────────
        // Create abstract class DataExporter with a non-overridable
        // method Export(string data) that calls these protected abstract steps in order:
        //   1. Validate(string data) → bool
        //   2. Transform(string data) → string
        //   3. Write(string data)
        // Create CsvExporter and JsonExporter that implement the three steps differently.
        //
        // TODO: implement DataExporter, CsvExporter, JsonExporter

        // DataExporter csv  = new CsvExporter();
        // DataExporter json = new JsonExporter();
        // csv.Export("hello");   // prints transform + write steps for CSV
        // json.Export("hello");  // prints transform + write steps for JSON

        // ── EXERCISE 10: interface default method (C# 8+) ─────────
        // Define interface IGreeter with:
        //   string Greet(string name)           ← must be implemented by class
        //   string GreetAll(string[] names)     ← default impl using Greet()
        // Implement FormalGreeter ("Good day, X") and CasualGreeter ("Hey, X!").
        // Both should inherit GreetAll for free from the interface default.
        //
        // TODO: implement IGreeter, FormalGreeter, CasualGreeter

        // IGreeter formal = new FormalGreeter();
        // IGreeter casual = new CasualGreeter();
        // Console.WriteLine(formal.Greet("Alice"));               // Good day, Alice
        // Console.WriteLine(casual.GreetAll(new[]{"Bob","Carol"})); // Hey, Bob! / Hey, Carol!
    }
}

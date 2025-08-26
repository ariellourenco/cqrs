# C# at Home

This is C# coding style guideline inspired by [.NET team coding guideline](https://github.com/dotnet/corefx/blob/master/Documentation/coding-guidelines/coding-style.md)
and various practices popular among the community. It's a compilation of various naming conventions, usage patterns and tips about how to
use the language in the right way 🧠.

## Coding style

For non-code files (xml, etc.), our current best guidance is consistency. When editing files, keep new code and changes consistent with the
style in the files. For new files, it should conform to the style for that component. If there is a completely new component, anything that
is reasonably broadly accepted is fine.

For C# code our coding conventions follow the Microsoft [C# Coding Conventions](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/inside-a-program/coding-conventions)
and standard [Naming Guidelines](https://docs.microsoft.com/en-us/dotnet/standard/design-guidelines/naming-guidelines).

1. Use [Allman Style](https://en.wikipedia.org/wiki/Indentation_style#Allman_style) braces, where each brace begins on a new line.
  A single line statement block can go without braces, but the block must be properly indented on its own line and must not be nested in
  other statement blocks that use braces (See rule 13 for more details). One exception is that a using statement is permitted to be nested
  within another using statement by starting on the following line at the same indentation level, even if the nested using contains a
  controlled block.
2. Use four spaces of indentation (no tabs).
3. Avoid spurious free spaces. For example, avoid `if (someVar == 0)...`, where the dots mark the spurious free spaces.
4. Avoid more than one empty line at any time. For example, do not have two blank lines between members of a type.
5. Variables and fields that can be made `const` should always be made `const`. If that is not possible, `readonly` can be a suitable alternative.
6. Use `_camelCase` for `internal` and `private` fields.
7. Use `PascalCasing` with no prefix for `public` fields, `const` local variables and `private readonly` fields.
8. Avoid `this.` unless absolutely necessary.
9. Always specify member visibility, even if it's the default (i.e. `private string _foo;` not `string _foo;`)
10. Namespace imports should be specified at the top of the file, outside of `namespace` declarations, and should be sorted alphabetically,
    with the exception of `System.*` namespaces, which are to be placed on top of all others.
11. Fields should be specified at the top within type declarations.
12. We use `nameof(...)` instead of `"..."` whenever possible and relevant.
13. When using a single statement, we follow these conventions:
    - Never use single-line form (for example: `if (source == null) throw new ArgumentNullException("source");`)
    - Braces may be omitted only if the body of every block associated with an `if`/`else if`/.../`else` compound statement is placed on a single line.
    - Using braces is always accepted and required if any block of an `if`/`else if`/.../`else` compound statement uses braces or if a single
      statement body spans multiple lines.
14. Make all internal and private types `static` or `sealed` unless derivation from them is required. As with any implementation detail,
    they can be changed if/when derivation is required in the future. See: [Analyzer Proposal: Seal internal/private types](https://github.com/dotnet/runtime/issues/49944)
15. Asynchronous methods are named with the `Async` suffix.
16. Use any language features available to you (expression-bodied members, throw expressions, tuples, etc.) as long as they make for
    readable, manageable code.

All C# code styles in this repository are aligned with [StyleCop's](https://github.com/DotNetAnalyzers/StyleCopAnalyzers) default conventions,
with one exception: **Rule 9** has been customized to explicitly indicate whether fields are `private` or `internal`. To support consistent
formatting, an [EditorConfig](https://editorconfig.org/) file (`.editorconfig`) is provided at the root of the repository. This enables
automatic formatting in supported IDEs and tools.

Additionally, we’ve enabled `EnforceCodeStyleInBuild`, ensuring that code style violations are treated as build errors, helping maintain
consistency across contributions. We also encourage the use of the [.NET Format](https://github.com/dotnet/sdk/tree/main/src/BuiltInTools/dotnet-format)
tool, which automatically applies formatting rules defined in the EditorConfig, keeping the codebase clean and standardized over time.

Finally, be sure to set the `<AnalysisMode>` property in your project file to, at least, `Recommended`. This will enable checks for security,
performance, and design issues, raising build warnings for any identified problems. For more information, refer to the
[Overview of .NET source code analysis](https://docs.microsoft.com/en-us/dotnet/fundamentals/code-analysis/overview) documentation.

## Coding guidelines

### Async method patterns

The C# `async/await` feature makes it easy to write asynchronous code in a way that resembles synchronous code, but it introduces its own
set of trade-offs. Understanding these costs is key to writing efficient and maintainable async code. Resources like the
[Build](https://learn.microsoft.com/en-us/shows/build-build2011/tool-829t) presentation, the article in
[MSDN Magazine](https://docs.microsoft.com/en-us/archive/msdn-magazine/2011/october/asynchronous-programming-async-performance-understanding-the-costs-of-async-and-await),
[David Fowler’s guidance](https://github.com/davidfowl/AspNetCoreDiagnosticScenarios/blob/master/AsyncGuidance.md), and
[Stephen Toub’s deep dive](https://devblogs.microsoft.com/dotnet/how-async-await-really-works/) offer valuable insights and best practices.

One important aspect of writing robust asynchronous code is **cancellation handling**. By convention, the `CancellationToken` parameter is
typically placed last—unless an `IProgress<T>` parameter is present. It's common to pass cancellation tokens as optional parameters using
`default`, which is equivalent to `CancellationToken.None` (and one of the few valid uses of optional parameters). In web scenarios, the
`HttpContext` often provides its own cancellation token, which should be used when available.

Keep in mind that cancellation is cooperative: every method in the call chain must explicitly accept and pass the `CancellationToken` for
it to be effective. This means developers need to be intentional about propagating the token through all relevant APIs.
[Stephen Cleary discusses this topic in depth in this series on cancellation](https://blog.stephencleary.com/2022/02/cancellation-1-overview.html).

❌ This example neglects to pass the `CancellationToken` to `Stream.ReadAsync` making the operation effectively not cancellable.

```cs
public async Task<string> DoThingAsync(CancellationToken cancellationToken = default)
{
    var buffer = new byte[1024];

   // We forgot to pass flow cancellationToken to ReadAsync
   int read = await _stream.ReadAsync(buffer, 0, buffer.Length);
   return Encoding.UTF8.GetString(buffer, 0, read);
}
```

✅ This example passes the `CancellationToken` into `Stream.ReadAsync`.

```cs
public async Task<string> DoThingAsync(CancellationToken cancellationToken = default)
{
    var buffer = new byte[1024];
   int read = await _stream.ReadAsync(buffer, 0, buffer.Length, cancellationToken);
   return Encoding.UTF8.GetString(buffer, 0, read);
}
```

> [!NOTE]
> When a C# `async` method ends with an `await` as its final operation—meaning the awaited call is the last action before returning—the
> compiler can optimize by **omitting the generation of the usual async state machine**. This optimization reduces overhead and improves
> performance. In such cases, the `async` keyword can be safely omitted, and the method can return the `Task` directly, provided no `await`
> is needed within the method body.

### Prefer ```ValueTask[<T>]``` over ```Task[<T>]``` for ammortized allocation-free

As the name implies, a value task is a value type rather than a reference type (like ordinary tasks). When using the reference type of task
(e.g., `Task<T>`), even if the value is known synchronously (e.g., using `Task.FromResult`), the `Task<T>` wrapper object still needs to be
allocated. Value tasks avoid this allocation because they are value types; when a value is known synchronously, code can create and return
a value task without having to do any allocation. In addition to a clear performance win in the synchronous case, value tasks often produce
more efficient code even in many common asynchronous cases. For further info see:

- [Prefer ValueTask to Task, always; and don't await twice](https://blog.marcgravell.com/2019/08/prefer-valuetask-to-task-always-and.html)
- [Understanding the Whys, Whats, and Whens of ValueTask](https://devblogs.microsoft.com/dotnet/understanding-the-whys-whats-and-whens-of-valuetask/)
- [ValueTask Restrictions](https://blog.stephencleary.com/2020/03/valuetask.html)

> [!NOTE]
> As a rule of thumb, if the underlying API returns a `ValueTask`| `ValueTask<T>`, it is generally safe and acceptable to propagate and
> return a `ValueTask` in the calling method, as long as the calling method itself does not make other asynchronous calls requiring a `Task`
> return. This avoids unnecessary allocations and preserves performance benefits.

### Use C# type keywords in favor of .NET type names

When using a type that has a C# keyword, use it in favor of the .NET type name (e.g., `int`, `string`, `float` instead of `Int32`, `String`,
`Single`, etc) for both type references as well as method calls (e.g., `int.Parse` instead of `Int32.Parse`). For example,
these are correct:

```cs
public string TrimString(string s) => string.IsNullOrEmpty(s)
    ? null : s.Trim();

var intTypeName = nameof(Int32); // can't use C# type keywords with nameof
```

The following are incorrect:

```cs
public String TrimString(String s) => String.IsNullOrEmpty(s)
    ? null : s.Trim();
```

For further detail check: [string vs. String is not a style debate](https://blog.paranoidcoding.com/2019/04/08/string-vs-String-is-not-about-style.html?utm_campaign=featured&utm_medium=email&utm_source=csharpdigest)

### Usage of the var keyword

The `var` keyword is to be used as much as the compiler will allow. However, keep in mind that it should be used when it's obvious what the
variable type is. For example, these are correct:

```cs
var fruit = "Lychee";
var fruits = new List<Fruit>();
var flavor = fruit.GetFlavor();
```

The following are incorrect:

```cs
string fruit = null; // can't use "var" because the type isn't known (though you could do (string)null, don't!)
const string expectedName = "name"; // can't use "var" with const.
var stream = OpenStandardInput();  // it's not obvious what the variable type is.
```

### IEnumerable vs IList vs IReadOnlyList

For inputs use the most restrictive collection type possible, for example, `IReadOnlyCollection`/`IReadOnlyList`/`IEnumerable` as inputs to
methods when the inputs should be immutable. On the other hand, for outputs, if passing ownership of the returned collection to the caller,
prefer `IList` over `IEnumerable`. Otherwise, prefer the most restrictive option.

### Extension methods

✔️ CONSIDER use an extension method when the source of the original class is not available, or else when changing the source is not feasible.

✔️ CONSIDER use an extension method if the functionality being added is a 'core' general feature that would be appropriate to add to the
source of the original class. Liberal use of extension methods has the potential to obfuscate the code, cluttering the APIs of types that
were not designed to have these methods, and create readability issues.

❌ DO NOT put extension methods in the same namespace as the extended type unless it is for adding methods to interfaces or for dependency management.

✔️ CONSIDER using extension methods to provide helper functionality relevant to every implementation of an interface, if said functionality
can be written in terms of the core interface. This is because concrete implementations cannot otherwise be assigned to interfaces.
For example, the `LINQ to Objects` operators are implemented as extension methods for all
[`IEnumerable<T>`](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1?view=net-6.0) types.
Thus, any `IEnumerable<>` implementation is automatically LINQ-enabled.

See also: [Framework Design Guidelines - Extension Methods](https://docs.microsoft.com/en-us/dotnet/standard/design-guidelines/extension-methods)

### String Interpolation `$"{}"` vs `string.Format()` vs `string.Concat()` vs `operator +`

In general, use whatever is easier to read. If performance is a concern, `StringBuilder` will be faster for multiple strings concatenation
and reduce the memory churn. However, ensure that there is no better overload for other string-creation methods. For example, the `string.Concat`
has a dedicated overload for concatenating three strings, it has the best possible implementation for that operation (and if the.NET team
ever found a better way, that method would be improved accordingly).

## Commenting Conventions

- Place the comment on a separate line, not at the end of a line of code.
- Begin comment text with an uppercase letter. It's recommended to end comment text with a period but not required.
- Add comments where the code is not trivial or could be confusing.
- Add comments where a reviewer needs help to understand the code.
- Update/remove existing comments when you are changing the corresponding code.
- Make sure the added/updated comments are meaningful, accurate and easy to understand.

## Documentation comments

- Create documentation using [XML documentation comments](https://learn.microsoft.com/dotnet/csharp/language-reference/xmldoc/) so that
  Visual Studio and other IDEs can use IntelliSense to show quick information about types or members.
- Publicly visible types and their members must be documented. Internal and private members may use doc comments but it is not required.
- Documentation text should be written using complete sentences ending with full stops.
- For keywords like `null`, `true` or `false` use `<see langword="*" />` tags
- The person writing the code will write the DOC comments.

> [!NOTE]
> Public means callable by customer, so it includes protected APIs. However, some public APIs might still be "for internal use only" but
> need to be public for technical reasons. We will still have doc comments for these APIs but they will be documented as appropriate.

## Cross-platform coding

Don't assume we only run (and develop) on Windows or Linux. Code should be sensitive to the differences between OS's. Here are some
specifics to consider.

### Environment Variables

OS's use different variable names to represent similar settings. The code should consider these differences. For example, when looking for
the user's home directory, on Windows the variable is `USERPROFILE` but on most Linux systems it is `HOME`.

```cs
var homeDir = Environment.GetEnvironmentVariable("USERPROFILE") 
                  ?? Environment.GetEnvironmentVariable("HOME");
```

### File path separators

Windows uses `\` and OS X and Linux use `/` to separate directories. Instead of hard coding either type of slash, use `Path.Combine()` or
`Path.DirectorySeparatorChar`.

If this is not possible (such as in scripting), use a forward slash. Windows is more forgiving than Linux in this regard.

### Line breaks

Windows uses `\r\n`, OS X and Linux uses `\n`. When it is important, use `Environment.NewLine` instead of hard coding the line break.

Note: this may not always be possible or necessary. Be aware that these line-endings may cause problems in code when using `@""` text
blocks with line breaks.

## Common Patterns

This section contains common patterns used throughout our code.

### Logging patterns

> Logs are an important interface to your application; they’re the “developer interface”, alongside the user interface, “data interface”,
> or programming interface. Just as we strive to create beautiful and functional pages, SQL schemas, or APIs, we should take ownership of
> our application’s log output and ensure it’s both usable and efficient. - Nicholas Blumhardt

1. Never use string-interpolation ($"Foo {bar}") for log messages. Log message templates are designed so that structured logging systems
  can make individual parameters queryable and string-interpolation breaks this.

## Best Practices

- Avoid hard-coding anything unless it's absolutely necessary.
  - Use `const` variables for literal values that are not expected to change.
  - Use `enum` for a set of related named constants.
  - Use configuration files (e.g. `.json`, `.xml`, etc.) for values that may change based on environment or deployment.
  - Use resource files (`.resx`) for strings that may need to be localized.
- Avoid a method that is too long and complex. In such case, separate it to multiple methods or even a nested class as you see fit.
- Use the `using` statement instead of `try/finally` if the only code in the `finally` block is to call the `Dispose` method.
- Use of object initializers (e.g. `new Example { Name = "Name", ID = 1 }`) is encouraged for better readability, but not required.
- Stick to the `DRY` principle -- Don't Repeat Yourself.
  - Wrap the commonly used code in methods, or even put it in a utility class if that makes sense, so that the same code can be reused (e.g. `StringToBase64Converter.Base64ToString(string)`).
  - Check if the code for the same purpose already exists in the code base before inventing your own wheel.
  - Avoid repeating literal strings in code. Instead, use `const` variable to hold the string.
  - Resource strings used for errors or UI should be put in resource files (`.resx`) so that they can be localized later.
- Consider using the `Interlocked` class instead of the `lock` statement to atomically change simple states. The `Interlocked`
  class provides better performance for updates that must be atomic.

## References

1. [.NET team coding guideline](https://github.com/dotnet/corefx/blob/master/Documentation/coding-guidelines/coding-style.md)
2. [C# Coding Conventions](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions)
3. [C# at Google Style Guide](https://google.github.io/styleguide/csharp-style.html)
4. [Document your code with XML comments](https://docs.microsoft.com/en-us/dotnet/csharp/codedoc)

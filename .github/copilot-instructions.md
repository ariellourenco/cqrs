# GitHub Copilot Instructions

This document provides guidance for GitHub Copilot when generating code for the CQRS project. Follow these guidelines to ensure that 
generated code aligns with the project's coding standards and architectural principles.

If you are not sure, do not guess, just tell that you don't know or ask clarifying questions. Don't copy code that follows the same 
pattern in a different context. Don't rely just on names, evaluate the code based on the implementation and usage. Verify that the 
generated code is correct and compilable.

## Code Style

### General Guidelines

- Follow the [.NET coding guidelines](https://github.com/dotnet/runtime/blob/main/docs/coding-guidelines/coding-style.md) unless explicitly overridden below
- Formatting and naming are governed by the [.editorconfig](../.editorconfig) file and enforced by `dotnet format`. Follow it, and
defer to it for any ambiguous cases
- Use meaningful and descriptive names
- Write code that is clean, maintainable, and easy to understand
- Favor readability over brevity, but keep methods focused and concise
- Only add comments rarely to explain why a non-intuitive solution was used. The code should be self-explanatory otherwise
- Don't add the UTF-8 BOM to files unless they have non-ASCII characters
- Make only high confidence suggestions when reviewing code changes
- All types should be public
- Avoid breaking public APIs. If you need to break a public API, add a new API instead and mark the old one as obsolete. Use
`ObsoleteAttribute` with the message pointing to the new API
- Never change [global.json](../global.json) unless explicitly asked to
- Never change [nuget.config](../nuget.config) files unless explicitly asked to

### C# Specific Guidelines

- Prefer using collection expressions when possible
- Prefer `switch` expressions over `switch` statements when appropriate
- Prefer field-backed property declarations using field contextual keyword instead of an explicit field.
- The projects use implicit namespaces, so do not add `using` directives for namespaces that are already imported by the project
- When verifying that a file doesn't produce compiler errors rebuild the whole project

### Nullability

- Declare variables non-nullable, and check for null at entry points.
- Always use `is null` or `is not null` instead of `== null` or `!= null`.
- Trust the C# null annotations and don't add null checks when the type system says a value cannot be null.

### Testing

- We use xUnit SDK v3 with Microsoft.Testing.Platform (https://learn.microsoft.com/dotnet/core/testing/microsoft-testing-platform-intro)
- We do not use any mocking framework at the moment.
- Copy existing style in nearby files for test method names and capitalization.
- Do not leave newly-added tests commented out. All added tests should be building and passing.
- Do not use Directory.SetCurrentDirectory in tests as it can cause side effects when tests execute concurrently.

## Documentation

- Include XML documentation for all public APIs
- Add proper `<remarks>` tags with links to relevant documentation where helpful
- For keywords like `null`, `true` or `false` use `<see langword="*" />` tags
- Overriding members should inherit the XML documentation from the base type via `/// <inheritdoc />`

## Error Handling

- Use appropriate exception types. 
- Include helpful error messages stored in the .resx file corresponding to the project
- Avoid catching exceptions without rethrowing them

## Asynchronous Programming

- Use the `Async` suffix for asynchronous methods
- Always use `async`/`await`; never block on `.Result` or `.Wait()` (they can cause deadlocks and thread-pool starvation)
- Return `Task` or `ValueTask` from asynchronous methods; use `ValueTask<T>` for hot paths that often complete synchronously
- Use `CancellationToken` parameters to support cancellation
- Avoid async void methods except for event handlers
- In shared library/infrastructure projects call `ConfigureAwait(false)` on awaited calls. This is unnecessary in the ASP.NET Core
service projects, which have no synchronization context

## Performance Considerations

- Be mindful of performance implications, especially for database operations
- Avoid unnecessary allocations
- Consider using more efficient code that is expected to be on the hot path, even if it is less readable

## Implementation Guidelines

- Write code that is secure by default. Avoid exposing potentially private or sensitive data

## Commit Message Guidelines

- Avoid undescriptive one-liner commit messages
- The first line should be short, 50 characters or less, and express intention
- Write the commit message in present imperative tense
- The second line is blank
- Next line optionally defines a summary of changes done and should focus on _Why_, not the _What_.
- Wrap it to 76 columns per line.
- Use emojis when possible

## Documentation Resources

If the `microsoft.docs.mcp` tool is available in your environment, you may use it to:

- Look up current .NET best practices and patterns
- Find official Microsoft documentation for APIs
- Verify modern syntax and recommended approaches
- Research performance optimization techniques

### Query examples

- "ASP.NET Core Minimal API typed results"
- ".NET Aspire service defaults and orchestration"
- "EF Core and event sourcing patterns"
- "async await guidelines C#"

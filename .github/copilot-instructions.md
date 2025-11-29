# GitHub Copilot Instructions

This document provides guidance for GitHub Copilot when generating code for the CQRS project. Follow these guidelines to ensure that 
generated code aligns with the project's coding standards and architectural principles.

If you are not sure, do not guess, just tell that you don't know or ask clarifying questions. Don't copy code that follows the same 
pattern in a different context. Don't rely just on names, evaluate the code based on the implementation and usage. Verify that the 
generated code is correct and compilable.

## Code Style

### General Guidelines

- Follow the [.NET coding guidelines](https://github.com/dotnet/runtime/blob/main/docs/coding-guidelines/coding-style.md) unless explicitly overridden below
- Use the rules defined in the [.editorconfig](../.editorconfig) file in the root of the repository for any ambiguous cases
- Write code that is clean, maintainable, and easy to understand
- Favor readability over brevity, but keep methods focused and concise
- Only add comments rarely to explain why a non-intuitive solution was used. The code should be self-explanatory otherwise
- Don't add the UTF-8 BOM to files unless they have non-ASCII characters
- All types should be public. 
- Avoid breaking public APIs. If you need to break a public API, add a new API instead and mark the old one as obsolete. Use 
`ObsoleteAttribute` with the message pointing to the new API

### Formatting

- Use spaces for indentation (4 spaces)
- Use braces for all blocks except for single-line blocks
- Place braces on new lines
- Limit line length to 140 characters
- Trim trailing whitespace
- All declarations must begin on a new line
- Use a single blank line to separate logical sections of code when appropriate
- Insert a final newline at the end of files

### C# Specific Guidelines

- File scoped namespace declarations
- Use `var` for local variables
- Use expression-bodied members where appropriate
- Prefer using collection expressions when possible
- Use `is` pattern matching instead of `as` and null checks
- Prefer `switch` expressions over `switch` statements when appropriate
- Prefer field-backed property declarations using field contextual keyword instead of an explicit field.
- Prefer range and index from end operators for indexer access
- The projects use implicit namespaces, so do not add `using` directives for namespaces that are already imported by the project
- When verifying that a file doesn't produce compiler errors rebuild the whole project

### Naming Conventions

- Use PascalCase for:
  - Classes, structs, enums, properties, methods, events, namespaces, delegates
  - Public fields
  - Constants
- Use camelCase for:
  - Parameters
  - Local variables
- Use `_camelCase` for instance private fields
- Prefix interfaces with `I`
- Prefix type parameters with `T`
- Use meaningful and descriptive names

### Nullability

- Declare variables non-nullable, and check for null at entry points.
- Always use `is null` or `is not null` instead of `== null` or `!= null`.
- Trust the C# null annotations and don't add null checks when the type system says a value cannot be null.
- Use the null-conditional operator (`?.`) and null-coalescing operator (`??`) when appropriate

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
- Return `Task` or `ValueTask` from asynchronous methods
- Use `CancellationToken` parameters to support cancellation
- Avoid async void methods except for event handlers
- Call `ConfigureAwait(false)` on awaited calls to avoid deadlocks

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

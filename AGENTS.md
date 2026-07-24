---
name: Dev Agent
description: Instructions for AI coding agents (GitHub Copilot, Claude Code, and others) working with the CQRS Journey repository.
---

# Agent Instructions

This document defines the persona, repository-specific workflow, and safety boundaries for AI coding agents working in the **CQRS Journey**
repository. It complements, but does not replace, the detailed coding and style guidance in
[.github/copilot-instructions.md](.github/copilot-instructions.md).

**When writing or reviewing code, always:**

- Use this file for repo context, the build/test workflow, and high-level boundaries.
- Use [.github/copilot-instructions.md](.github/copilot-instructions.md) for C# style, nullability, async, performance, and documentation rules.
- Use [ROADMAP.md](ROADMAP.md) to understand where the project is headed and what the next unit of work is.

## Persona

- You are specialized in writing and reviewing modern C# / .NET code for the **CQRS Journey** solution.
- Your primary goal is to design, implement, and maintain high-quality code and tests, and to help evolve the codebase safely toward the
  target architecture described in [ROADMAP.md](ROADMAP.md).

## Project knowledge

This is a personal learning project that modernizes Microsoft's original [CQRS Journey](https://github.com/microsoftarchive/cqrs-journey)
onto an [eShop](https://github.com/dotnet/eShop)-style architecture. It is **early / work in progress**: today only the solution
scaffolding and a partial **Registration** bounded context exist. Treat [ROADMAP.md](ROADMAP.md) as the plan of record; do not assume a
service, database, or message bus is present unless you can see it in the tree.

### Target architecture

- **Hybrid CQRS** — event-sourced write side for Registration/Seats and Orders; pragmatic EF Core CRUD for Conference Management and Payments.
- **Marten** on PostgreSQL for the event-sourced contexts; **EF Core** on PostgreSQL for the CRUD contexts. *(planned)*
- **RabbitMQ** integration events over the existing `IEventBus` abstraction. *(planned)*
- **.NET Aspire** AppHost as the single local-dev entry point, with OpenTelemetry, health checks, and service discovery. *(planned)*

### Technology stack

- .NET 10.0
- C# with `<LangVersion>preview</LangVersion>` (do not hard-code a C# version number in code or docs)
- xUnit SDK v3 with **Microsoft.Testing.Platform** for testing
- Central package management (`Directory.Packages.props`) and the `.slnx` solution format
- GitHub Actions for CI/CD

### File structure (high level)

- `.github/`: CI/CD workflows, Copilot instructions, and GitHub automation
- `.github/copilot-instructions.md`: detailed coding style, testing, and performance guidance
- `src/Services/<Context>/<Context>.{Api,Domain,Tests}`: one folder per bounded context (currently only `Registration`)
- `src/Infrastructure/`: cross-cutting infrastructure (e.g. `EventBus` integration-event abstractions)
- `Conference.slnx`: main solution file (XML-based `.slnx` format)
- `Directory.Build.props`: shared MSBuild properties across all projects
- `Directory.Packages.props`: centralized package version management
- `global.json`: pins the .NET SDK version and test runner — never modify without an explicit request
- `nuget.config`: package sources — never modify without an explicit request
- `.editorconfig`: formatting rules, null annotations, diagnostic configuration
- `ROADMAP.md`: the milestone-by-milestone plan

## Tools you can use

- **Build:** `dotnet build --configuration Release` at the solution root.
- **Test:** `dotnet test --configuration Release`, keeping all existing tests passing.
- **Format/Analyze:** `dotnet format Conference.slnx --verify-no-changes` to enforce repository conventions
  ([.editorconfig](.editorconfig)). CI runs this and fails on any diff, so run it before committing.

### CRITICAL: Do NOT use VSTest-style --filter with dotnet test

This repo uses [Microsoft.Testing.Platform (MTP)](https://learn.microsoft.com/dotnet/core/testing/microsoft-testing-platform-intro) as
the test runner, not VSTest. The classic `--filter` argument (before `--`) uses VSTest filter syntax and will hang or behave
unexpectedly with MTP.

#### WRONG — VSTest-style filter, will hang with MTP

`dotnet test --project src/Services/Registration/Registration.Tests/Registration.Tests.csproj --filter "FullyQualifiedName~ClassName"`

#### CORRECT — MTP-native filters go after the `--` separator

`dotnet test --project src/Services/Registration/Registration.Tests/Registration.Tests.csproj -- --filter-class "*.ClassName"`
`dotnet test --project src/Services/Registration/Registration.Tests/Registration.Tests.csproj -- --filter-method "*.MethodName"`

All test filtering must use MTP-native switches placed after `--`. For the full set of filter switches, see the official
Microsoft.Testing.Platform CLI documentation: <https://learn.microsoft.com/dotnet/core/testing/microsoft-testing-platform-cli>.

## Boundaries

- ✅ **Always:** Keep code consistent with [.editorconfig](.editorconfig) and repo conventions.
- ✅ **Always:** Keep public APIs stable. If a change is required, prefer adding new APIs and marking old ones obsolete as described in
  [.github/copilot-instructions.md](.github/copilot-instructions.md).
- ✅ **Always:** Add or update tests in the relevant `<Context>.Tests` project when changing behavior.
- ✅ **Always:** Follow the C# style, nullability, async, performance, and documentation guidance in
  [.github/copilot-instructions.md](.github/copilot-instructions.md).
- ⚠️ **Ask first:**
  - Adding new external dependencies
  - Changing [global.json](global.json)
  - Changing [nuget.config](nuget.config)
  - Modifying CI/build configuration
  - Introducing a new bounded context or infrastructure component ahead of the [ROADMAP.md](ROADMAP.md) order
- 🚫 **Never:** Commit secrets or API keys, introduce hard-coded credentials, or bypass existing security-related checks.

## When unsure

- If instructions in this file and in [.github/copilot-instructions.md](.github/copilot-instructions.md) appear to conflict, treat this
  file as the source of truth for repository layout, testing workflow, and safety boundaries, and treat `.github/copilot-instructions.md`
  as the source of truth for how to write C# and .NET code.
- If following both sets of instructions would break existing behavior, tests, or public APIs, stop and ask the user for clarification
  instead of guessing.

## Updating these instructions

When working on a task, if user input is required to complete it or feedback is provided about a specific area of the code, evaluate
that feedback and update this document to incorporate it if it is missing. This document should be a live, evolving set of instructions.

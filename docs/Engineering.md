# Engineering Guidelines

## Branch strategy

In general:

- `main` has the code that is always deployable. This is the branch into which developers normally submit pull requests and merge changes into.
- Use lowercase-with-dashes for naming.
- Use feature branches for all new features and bug fixes. Use a consistent naming convention for your feature branches to identify the
  work done in the branch.
  - `bugfix/description`
  - `features/feature-name`
  - `hotfix/description`
- Follow [Linus' recommendations](https://web.archive.org/web/20230522041845/https://wincent.com/wiki/git_rebase%3A_you're_doing_it_wrong) about history.
   > "People can (and probably should) rebase their private trees (their own work). That's a cleanup. But never other peoples code.
   > That's a 'destroy history'... You must never EVER destroy other peoples history. You must not rebase commits other people did.
   > Basically, if it doesn't have your sign-off on it, it's off limits: you can't rebase it, because it's not yours."
- Open a pull request when you feel your changes are ready to be merged (or even if you aren’t so sure but would like some feedback).
- After the new feature is revised and approved, you can merge it into `main`.

Keep in mind that development MUST be done in [small batches](https://cloud.google.com/solutions/devops/devops-process-working-in-small-batches),
long-lived branches are bad, and code reviews MUST be prioritized to ensure that changes don't have to wait hours, or even days,
to get merged into `main`.

## Commit Message Guidelines

Commits are a historical record of exactly **how** and **why** each line of code came to be. The history of a good repository commit can
help developers track bugs and understand why code looks the way it does. Ultimately, it can even be used for
[automatically generated release notes](https://docs.github.com/en/repositories/releasing-projects-on-github/automatically-generated-release-notes).
Therefore, it is good practice to have some conventions for how our `git commit` should be formatted, which leads to more readable and
easier-to-follow messages when looking at project history.

The conventions for messages are inspired by [http://tbaggery.com/2008/04/19/a-note-about-git-commit-messages.html](http://tbaggery.com/2008/04/19/a-note-about-git-commit-messages.html)

- Avoid undescriptive one-liner commit messages
- The first line should be short (50 characters or less) and express intention _(what does this accomplish?)
- Write the commit message in present imperative tense: "Fix bug" and not "Fixed bug"
- The second line is blank
- Next line optionally defines a summary of changes done and should focus on _Why_, not the _What_
  - Context
  - Justification
  - Implementation

Example:

```text
Capitalized, short (50 chars or less) summary

More detailed explanatory text, if necessary.  Wrap it to 76 columns
per line (or less; 72 is also a common choice). In some contexts, the 
first line is treated as the subject of an email and the rest of the 
text as the body.  The blank line separating the summary from the body 
is critical (unless you omit the body entirely); tools like rebase can 
get confused if you run the two together.

Write your commit message in the imperative: "Fix bug" and not "Fixed bug"
or "Fixes bug."  This convention matches up with commit messages generated
by commands like git merge and git revert.

Further paragraphs come after blank lines.

- Bullet points are okay, too
- Use a hanging indent
```

There are a multitude of blog posts and videos talking about the benefits of good commit messages, and some of the tools that make it easier
to structure your changes around them, below are a few of them:

- [Write Better Commits, Build Better Projects](https://github.blog/2022-06-30-write-better-commits-build-better-projects/)
- [Telling stories through your commits](https://blog.mocoso.co.uk/talks/2015/01/12/telling-stories-through-your-commits/) by Joel Chippindale
- [A branch in time](https://tekin.co.uk/2019/02/a-talk-about-revision-histories) by Tekin Süleyman
- [My favorite Git commit](https://dhwthompson.com/2019/my-favourite-git-commit) by David Thompson

For a more complete convention on top of commit messages, see: [Conventional Commits 1.0.0](https://www.conventionalcommits.org/en/v1.0.0/)

### Polish Your Commits

Before submitting your PR, be sure to read [coding guidelines](https://github.com/ariellourenco/hexagonal/wiki/C%23-at-Home) and check your
code to match as best you can. This can be a lot of effort, but it saves time during review to avoid style issues.

Here are some other tips that we use when cleaning up our commits:

- Check for whitespace errors using `git diff --check [base]...HEAD` or `git log --check`.
- Run `git rebase --whitespace=fix` to correct upstream issues with whitespace.
- Become familiar with interactive rebase (`git rebase -i`) because you will be reordering, [squashing](https://graphite.dev/guides/how-to-squash-git-commits)
  and editing commits as your PR is reviewed.

## Sync your local repository

Use git rebase instead of git merge and git pull, when you're updating your feature-branch.

```bash
# fetch updates all remote branch references in the repository
# --all : tells it to do it for all remotes (handy, when you use your fork)
# -p : tells it to remove obsolete remote branch references (when they are removed from remote)
git fetch --all -p

# rebase on origin/main will rewrite your branch history
git rebase origin/main
```

## Repository Structure

This project is organized to be compliant with [_monorepos_](https://trunkbaseddevelopment.com/monorepos/) structure which means we have
multiple projects in the same repository. The repository organization is depicted below:

### Folders

- `assets` - All static resources in a project such as `.snk`, package logos, images and so on.
- `build` - Build customizations (custom `msbuild` files/psake/fake/albacore/etc) scripts.
- `docs` - Documentation stuff, markdown files, help files etc.
- `src` - Where the source code for the entire application resides.
- `test` - Test projects that exercise the project. XUnit is the current testing framework.

Solution files (`.slnx`) go in the repo root and MUST match the repo name (e.g., `Fruit.sln` in the Fruit repo) and contain solution folders
that match the physical folders (`src`, `test`, etc.).

For example, in the **Fruit** repo with the **Banana** and **Lychee** projects you would have these files checked in:

```text
/Fruit.sln
/src/Banana/Banana.csproj
/src/Banana/Banana.cs
/src/Banana/Util/BananaUtil.cs
/src/Banana/Banana.UnitTests/BananaUnitTests.csproj
/src/Banana/Banana.UnitTests/BananaTest.cs
/src/Banana/Banana.UnitTests/Util/BananaUtilTest.cs
/src/Banana/Banana.FunctionalTests/BananaTest.cs
/src/Lychee/Lychee.csproj
/src/Lychee/Lychee.cs
/src/Lychee/Util/LycheeUtil.cs
```

### Creates a new project

Create a new folder within `/src/` that will house your `.csproj` and other project-related files such as tests projects. If your project is
a service or related to infrastructure, consider create it within the `/services/` or `/infrastructure/` folder instead of `/src/` directly.

## Keep an eye on the CI

These projects use [Github Actions](https://docs.github.com/en/actions/learn-github-actions/understanding-github-actions) for builds which
are triggered when a pull request on the `main` branch is: opened, synchronized, reopened, or closed. When you merge a commit into a `main`
you should keep an eye on CI until your commit has passed it. The process takes about 5-15 minutes to run when it passes, all you really
need to do is ensure that it does.

### **I've broken my build, and I can't get up!**

The build system is brand new, so problems can catch us by surprise. As such, you'll sometimes end up in a broken state and can't build.
The following steps should fix most broken builds:

```bash
# Clean all non-source-controlled files
git clean -xdf

dotnet build
```

Engineering guidelines were inspired by [ASP.NET Core engineering-guidelines](https://github.com/dotnet/aspnetcore/wiki/Engineering-guidelines)
and [Working with Powershell repository](https://github.com/powershell/powershell/tree/master/docs/git).

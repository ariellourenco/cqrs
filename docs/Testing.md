> _"If you don't like unit testing your product, most likely your customers won't like to test it, either."_ - Anonymous-

Software of any complexity can fail in unexpected ways in response of changes. Thus, testing after making changes is required for all but
the most trivial (or least critical) applications. Moreover, tests are more than just making sure your code works, there are numerous benefits
to writing tests; they help with regression, provide documentation, and facilitate good design. However, hard to read and brittle tests can
wreak havoc on your code base. Just by looking at the suite of tests, you should be able to infer the behave of your code without even
looking at the code itself. Additionally, when tests fail, you can see exactly which scenarios do not meet your expectations.

## What to test

A good starting point is to test conditional logic. Anywhere you have a method with behaviour that changes based on a conditional statement
(if-else, switch, and so on), you should be able to come up with at least a couple of tests that confirm the correct behaviour for certain
conditions. If your code has error conditions, it's good to write at least one test for the **"happy path"** through the code (with no errors),
and at least one test for the **"sad path"** (with errors or atypical results) to confirm your application behaves as expected in the face
of errors.

Finally, try to focus on testing things that can fail, rather than focusing on metrics like code coverage. More code coverage is better than
less, generally. However, writing a few more tests of a complex and business-critical method is usually a better use of time than writing
tests for auto-properties just to improve test code coverage metrics. A better technique to assess whether you’re adequately exercising the
lines your tests cover, and adequately asserting on failures, is [mutation](https://research.google/pubs/pub46584/) testing.

> _“Test coverage is a useful tool for finding untested parts of a codebase. Test coverage is of little use as a numeric statement of how
> good your tests are.”_ <br />
> **Martin Fowler** | <https://martinfowler.com/bliki/TestCoverage.html>

## Code Coverage

Code coverage can be a powerful data to assess risk and identify gaps in testing. However, a high code coverage percentage does not guarantee
high quality in the test coverage. It does not guarantee that the covered lines or branches have been tested _correctly_, it just guarantees
that they have been executed by a test.

Although there is no "ideal code coverage number" we can still make a stab at such a value. According to [Codecov](https://about.codecov.io/blog/the-case-against-100-code-coverage),
most repositories find that their code coverage values slide downwards when they are above 80% coverage. Moreover, Google posted a
[blog post](https://testing.googleblog.com/2020/08/code-coverage-best-practices.html) on their best practices where they offer the guidelines
below for code coverage:

> * 60% as “acceptable”,
> * 75% as “commendable”,
> * 90% as “exemplary”

So, it seems to be reasonable aim for somewhere between **75%-85% coverage**. Just keep in mind that more important than the percentage of
line covered is the human judgment over the lines not covered and whether this risk is acceptable or not.

## Guidelines

### Assembly naming

The unit tests for `Hexagonal.Fruit` assembly live in the `Hexagonal.Fruit.Tests` assembly, for integration tests in
`Hexagonal.Fruit.IntegrationTests`, and in `Hexagonal.Fruit.AcceptanceTests` for acceptance tests. In general there should be exactly one
unit test for each product runtime and one integration assembly per repo. Exceptions can be made for both.

### Class naming

The class names end with `Tests` and live in the same namespace as the class being tested. For example, the unit tests for `Hexagonal.Fruit.Banana`
class would be in a `Hexagonal.Fruit.BananaTests` class in the test assembly.

### Test naming

Test method names must be descriptive about _what is being tested_, _under what conditions_, and _what the expectations are_. The structure
has a test class per class being tested and a nested class for each method being tested. Pascal casing and underscores can be used to improve
readability.

The following has two methods for embellishing names with more interesting titles.

```cs
using System;

public class Titleizer
{
    public string Titleize(string name)
    {
        if (string.IsNullOrEmpty(name))
            return "Default name";
        return name + " the awesome hearted";
    }

    public string Knightify(string name, bool male)
    {
        if (string.IsNullOrEmpty(name))
            return "Your name is now Sir Jester";
        return (male ? "Sir" : "Dame") + " " + name;
    }
}
```

Under this system, we’ll have a corresponding top level class, with two embedded classes, one for each method. In each class, we’ll have a
series of tests for that method.

```cs
public class TitleizerTests
{
    public class TitleizerShould
    {
        [Fact]
        public void ReturnsDefaultTitleForNullName()
        {
            // Test code
        }

        [Fact]
        public void AppendsTitleToName()
        {
            // Test code
        }
    }

    public class KnightifyShould
    {
        [Fact]
        public void ReturnsDefaultTitleForNullName()
        {
            // Test code
        }

        [Fact]
        public void AppendsSirToMaleNames()
        {
            // Test code
        }

        [Fact]
        public void AppendsDameToFemaleNames()
        {
            // Test code
        }
    }
}
```

### Test structure

The content of every test should be split into three distinct stages called [AAA-Pattern](https://wiki.c2.com/?ArrangeActAssert). The name
comes from the initials of the three actions usually needed to perform a test:

```cs
// Arrange  
// Act  
// Assert 
```

This clearly separates what is being tested from the arrange and assert steps. The crucial thing here is that the `Act` stage is exactly one
statement. That one statement is nothing more than a call to the one method under test. Keeping that one statement as simple as possible is
also important. For example, this is not ideal:

```cs
int result = myObj.CallSomeMethod(GetComplexParam1(), GetComplexParam2(), GetComplexParam3());
```

This style is not recommended because is way too many things that can go wrong in this one statement. All the `GetComplexParamN()` calls can
throw for a variety of reasons unrelated to the test itself. It is thus unclear for someone running into a problem why the failure occurred.

The ideal pattern is to move the complex parameter building into the `Arrange` section:

```cs
// Arrange
P1 p1 = GetComplexParam1();
P2 p2 = GetComplexParam2();
P3 p3 = GetComplexParam3();

// Act
int result = myObj.CallSomeMethod(p1, p2, p3);

// Assert
Assert.AreEqual(1234, result);
```

Now the only reason the line with `CallSomeMethod()` can fail is if the method itself blew up.

### Testing exception messages

In general testing the specific exception message in a unit test is important. This ensures that the exact desired exception is what is being
tested rather than a different exception of the same type. This can be accomplished by verify the exception message.

To make writing unit tests easier it is recommended to compare the error message to the RESX resource. However, comparing against a string
literal is also permitted.

```cs
var ex = Assert.Throws<InvalidOperationException>(() => fruitBasket.GetBananaById(1234));
Assert.Equal(Strings.FormatInvalidBananaID(1234), ex.Message);
```

### Use xUnit.net's plethora of built-in assertions

xUnit.net includes many kinds of assertions – please use the most appropriate one for your test. This will make the tests a lot more readable
and also allow the test runner report the best possible errors (whether it's local or the CI machine). For example, these are bad:

```cs
Assert.Equal(true, someBool);

Assert.True("abc123" == someString);

Assert.True(list1.Length == list2.Length);

for (int i = 0; i < list1.Length; i++) {
    Assert.True(
        string.Equals
            list1[i],
            list2[i],
            StringComparison.OrdinalIgnoreCase));
}
```

These are good:

```cs
Assert.True(someBool);

Assert.Equal("abc123", someString);

// built-in collection assertions!
Assert.Equal(list1, list2, StringComparer.OrdinalIgnoreCase);
```

### Parallel tests

By default all unit tests assemblies should run in parallel mode,  which is the default. Unit tests shouldn’t depend on any shared state,
and so generally should be runnable in parallel. If the tests fail in parallel, the first thing to do is to figure out _why_; do not just
disable parallel tests!

For functional/integration tests it is reasonable to disable parallel tests.

## Reference

* [Test ASP.NET Core](https://docs.microsoft.com/en-us/dotnet/architecture/modern-web-apps-azure/test-asp-net-core-mvc-apps)
* [Unit testing best practices with .NET Core and .NET Standard](https://docs.microsoft.com/en-us/dotnet/core/testing/unit-testing-best-practices)

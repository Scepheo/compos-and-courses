# Sokoban

## Introduction to features from C# versions 6 and 7

Hello, and welcome to this small course on the new features in C# 6 and 7 (we'll be glossing over
7.1). You'll be building (finishing, really) a small puzzle game using these new features. To get
started, make sure you have Visual Studio 2017 installed (at least 15.3 if you want to use 7.1) and
open `src/Sokoban.sln`.

The title of each segment links to the release information on the feature, click those to get the
information on the feature straight from the horse's mouth. And as always, google is your friend.

Each of the locations where code has to be changed has been marked with a comment saying

```C#
// TODO: Assignment N
```

Open the Task List (View -> Task List) to view all of them and quickly navigate to these positions.

## 1. [`nameof` expressions (C# 6)][nameof-expressions]

We will be using these for the purpose of exceptions. In `Level.cs`, add a check to the constructor
to throw an argument null exception if `null` is passed to the constructor, using `nameof` to pass the parameter name to the exception.

## 2. [String Interpolation (C# 6)][string-interpolation]

String interpolation is also useful for exceptions, primarily the message. Use this to replace the string formatting in the `default` case in `HandleMovement` in `Level.cs`.

## 3. [Read-only auto-properties (C# 6)][read-only-auto-properties]

We will be using readonly properties as replacement of public readonly fields. In `Level.cs`,
replace `Width` and `Height` with get-only properties, and remove the backing fields.

## 4. [Auto-Property Initializers (C# 6)][auto-property-initializers]

Auto-property Initializers can be helpful for setting an initial value for a property, instead of
doing it in the constructor. In the marked locations, refactor the constructor assignments to
auto-property initializers: remove the assignments from the constructor and place them after the
declarations of the properties.

## 5. [Expression-bodied function members (C# 6)][expression-bodied-function-members]

For simple functions or property getters, it can be useful to just use an expression instead of a
full function block. In the marked locations, replace the function block with a return expression.

## 6. [Null-conditional operators (C# 6)][null-conditional-operators]

Helpful for accessing a nested member without having to manually check each component for null.
Combines nicely with the null-coalescing operator (`??`) for setting a default value if the member
is null. We'll be using it to check if an event is null before invoking it: in `EntityBase.cs`,
refactor `TriggerOverlapStart` and `TriggerOverlapEnd` to use the null-conditional operator.

## 7. [Tuples (C# 7)][tuples]

Tuples already existed in the .NET framework for a long time, but the recent addition of
`ValueTuple` and surrounding syntax have made them much more easy to use.

In `Level.cs`, we will be defining four tuples for each of the cardinal directions (up, down, left
and right). Note that the y-coordinates go down, and the x-coordinates go right, and add the four
tuples. Then, in `HandleMovement`, we can assign `xSpeed` and `ySpeed` at the same time with these
tuples.

## 8. [Pattern Matching (C# 7)][pattern-matching]

As can be seen in `Fire.cs`, efficiently working with values of unknown type can be a bit of pain
before C# 7. Instead, we will refactor this to use pattern matching. Everything after the early
return check on `_enabled` will be replaced. Start by pattern matching on `other`:

``` csharp
switch (other)
{
    ...
}
```

then fill in cases for the same types as the original `as`/`if`/`else` statements.

Also, in `MapVector.cs`, use the `is <type> <name>`pattern matching variant to rewrite the `Equals`
method into a one-liner in expression-bodied form.

## 9. [`out` variables (C# 7)][out-variables]

Some nice syntactic sugar, move the declaration of `entity` in `Level.cs`, `GetEntities` inline.

## 10. [Local functions (C# 7)][local-functions]

In order to make sure the entities are rendered correctly when they overlap, we need to sort them
by `TopLayer`, a property indicating whether an item should be drawn over other items. To this end,
`Level.cs` has the method `CompareLayer`. Because this is only used in `GetEntities` and should
never be used anywhere else, it's safer to define it as a local function there. So move
`CompareLayer` into `GetEntities`.

## 11. [Throw expressions (C# 7)][throw-expressions]

Throw expressions often allow us to drastically shorten code that either returns a value or throws.
Combined with inlining an `out` variable and the ternary operator, use this to refactor `GetItem` in
`LevelLoader.cs` to expression-bodied form.

## Done!

That's it for some of the most important new features (in our opinion, anyway). Hopefully this was
enough to whet your appetite and get you experimenting.

[nameof-expressions]: https://docs.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-6#nameof-expressions
[string-interpolation]: https://docs.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-6#string-interpolation
[read-only-auto-properties]: https://docs.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-6#read-only-auto-properties
[auto-property-initializers]: https://docs.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-6#auto-property-initializers
[expression-bodied-function-members]:https://docs.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-6#expression-bodied-function-members
[null-conditional-operators]: https://docs.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-6#null-conditional-operators
[tuples]: https://docs.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-7#tuples
[pattern-matching]: https://docs.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-7#pattern-matching
[out-variables]: https://docs.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-7#out-variables
[local-functions]: https://docs.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-7#local-functions
[throw-expressions]: https://docs.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-7#throw-expressions

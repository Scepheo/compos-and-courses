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
to throw an exception if `null` is passed to the constructor.

## 2. [String Interpolation (C# 6)][string-interpolation]

String interpolation is also useful for exceptions, primarily the message. Use this and a `nameof` expression to throw an exception if the direction passed to `HandleMovement` in `Level.cs` is invalid.

## 3. [Read-only auto-properties (C# 6)][read-only-auto-properties]

We will be using readonly properties as replacement of public readonly fields. In `Level.cs`, replace `Width` and `Height` with get-only properties, and remove the backing fields.

## 4. [Auto-Property Initializers (C# 6)][auto-property-initializers]

Auto-property Initializers can be helpful for setting an initial value for a property, instead of doing it in the constructor. In the marked locations, refactor the constructor assignments to auto-property initializers.

## 5. [Expression-bodied function members (C# 6)][expression-bodied-function-members]

For simple functions or property getters, it can be useful to just use an expression instead of a full function block. In the marked locations, replace the function block with a return expression.

## 6. [Null-conditional operators (C# 6)][null-conditional-operators]

Helpful for accessing a nested member without having to manually check each component for null. Combines nicely with the null-coalescing operator (`??`) for setting a default value if the member is null. We'll be using it to check if an event is null before invoking it: in `EntityBase.cs`, refactor `TriggerOverlapStart` and `TriggerOverlapEnd` to use the null-conditional operator.

[nameof-expressions]: https://docs.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-6#nameof-expressions
[string-interpolation]: https://docs.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-6#string-interpolation
[read-only-auto-properties]: https://docs.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-6#read-only-auto-properties
[auto-property-initializers]: https://docs.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-6#auto-property-initializers
[expression-bodied-function-members]:https://docs.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-6#expression-bodied-function-members
[null-conditional-operators]: https://docs.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-6#null-conditional-operators

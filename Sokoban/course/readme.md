# Sokoban

## Introduction to features from C# versions 6 and 7

Hello, and welcome to this small course on the new features in C# 6 and 7 (we'll be glossing over
7.1). You'll be building (finishing, really) a small puzzle game using these new features. To get
started, make sure you have Visual Studio 2017 installed (at least 15.3 if you want to use 7.1) and
open `src/Sokoban.sln`.

The title of each segment links to the release information on the feature, click those to get the
information on the feature straight from the horse's mouth. And as always, google is your friend.

## [`nameof` expressions (C# 6)][nameof]

We will be using these for the purpose of exceptions. In `Level.cs`, add a check to the constructor
to throw an exception if `null` is passed to the constructor, or an invalid direction is passed to
`HandleMovement`.

[nameof]: https://docs.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-6#nameof-expressions

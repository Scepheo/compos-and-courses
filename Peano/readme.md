# Peano numbers in F#

A short course/tutorial on implementing our own number system, from scratch, in
F#. We will be building our numbers on the [Peano axioms][wiki], without using
any of the functionality of built-in number types (e.g. `int`, `long`, `float`).

## Setting up

This tutorial works from a solution with an F# library project and an XUnit test
project. Make sure the test project has a dependency on the F# project.

## What's this Peano thing?

The Peano axioms are a set of rules that can be used to define numbers. The
wikipedia article linked earlier explains all the correct definitions, but we
won't be dealing with them precisely. What you need to know is this:

> 1. 0 is a natural number
> 2. If _n_ is a natural number, _S(n)_ is a natural number

This function _S_ is known as the successor function (hence the letter S). Note
that this is not a function in the traditional sense: _S(0)_ isn't a function
that does something and gives back 1. The result of applying the successor
function to 0 twice isn't 2, it's _S(S(0))_. We can decide (define) to _call_
_S(S(0))_ 2, but it's important to remember that _we_ decided that: there's no
inherit truth to this.

Anyway, we can now see that this matches the way we often think about numbers:
there's zero, and then the number after zero (which we'll call "one"). Then
there's the number after that (let's call this "two"), the number after _that_
(how does "three" sound?) and so on.

## Putting it into code

So, we now need to turn this into code. Create a file called "Peano.fs" in the
F# project (you can remove any pre-created files) and put this in:

``` fsharp
module Peano
```

Code in F# is organised into modules, so we need to make one. Starting a file
with the module name means that the rest of the file belongs to that module. Now
we need to make a type to define our numbers. Our type will consist of two
options, it's either zero or a successor:

``` fsharp
type Number = Zero | Successor of Number
```

This sort of type is known as a [discriminated union][discriminated-union].
There's a few common types like this, for example the `Option` type, which is
quite a lot like `Nullable` in C#:

``` fsharp
type Option<'T> = None | Some of 'T
```

There's either a value, or nothing. Most functional programming languages
support these kind of type definitions. For comparison, here's (roughly) our
`Number` type in C#:

``` csharp
public abstract class Number
{
    public sealed class Zero : Number {}

    public sealed class Successor : Number
    {
        public Number Item { get; }

        public Successor(Number item)
        {
            Item = item;
        }
    }
}
```

I find the F# to be a lot clearer, anyway.

## And a one, and a two

So, let's make some numbers. These will be more useful to us later on, but for
now they serve simply to drive the point home. We start with one (we've already
got zero, after all):

``` fsharp
let one = Successor Zero
```

That's it. We'll be using the `let` keyword a lot, as that's how you give values
a name (bind) in F#. These values can then be used in the rest of the file. Two
things are important to note here. First, order matters. In F#, you can only use
a value after having bound it. Second, functions are values too.

The right half is a constructor. When we defined the type to (possibly) be
`Successor of Number`, that automatically gave us a constructor we can use to
create a successor of a number.

Anyway, now that we have one, we can define two.

``` fsharp
let two = Successor one
```

Rather than writing out the entire list of `Successor` constructor invocations,
we can just use the previous value (just like you'd do with a variable in C#).
We can then add the rest of the first ten numbers, which (including all
previous lines) gives us the following:

``` fsharp
module Peano

type Number = Zero | Successor of Number

let one   = Successor Zero
let two   = Successor one
let three = Successor two
let four  = Successor three
let five  = Successor four
let six   = Successor five
let seven = Successor six
let eight = Successor seven
let nine  = Successor eight
let ten   = Successor nine
```

Again, note that _we_ are defining these numbers this way: there's no built-in
logic for this whatsoever.

## Test one two

On the testing side of things, we're a bit less strict in our aversion of the
built-in `int` type. After all, it would at least be useful to make it clear
what our tests are doing. So, in the test class, we're going to add two
conversion methods, one for `int`-to-`Number`, and one for `Number`-to-`int`.

``` csharp
private static int ToInt(Peano.Number number)
{
    var result = 0;

    while (number is Peano.Number.Successor successor)
    {
        result++;
        number = successor.Item;
    }

    return result;
}

private static Peano.Number ToNumber(int i)
{
    var result = Peano.Number.Zero;

    while (i > 0)
    {
        result = Peano.Number.NewSuccessor(result);
        i--;
    }

    return result;
}
```

As you can see, quite a bit has happened in the conversion from F# to C#:

* A `Peano` class was created for our module
* A `Number` class was created (under `Peano`) for our `Number` type
* A `Zero` _instance_ was created (under `Number`) for the `Zero` option of our
  `Number` type
* A `Successor` _class_ was created (under `Number`) for the `Successor` option
  of our `Number` type
* A function `NewSuccessor` was created that we can use to create a successor
* A property `Item` was created on `Successor` that we can use to access the
  number it follows

The conversions themselves are fairly straightforward: if we're going from `int`
to `Number`, we start with zero and take its successor `i` times. If we're going
from `Number` to `int`, we keep "unwrapping" the successor until we reach zero,
and keep a count of how often we unwrapped.

To test both these methods and our earlier number definitions, we'll be using an
XUnit theory test. We can't use our `Number`s as `InlineData` for our theories
because they're not compile-time constants (which is also why we're writing
these conversions), so we have to use `MemberData`. Then we want to have a test
for each conversion.

``` csharp
public static IEnumerable<object[]> NumberData()
{
    return new []
    {
        new object[] { 0,  Peano.Number.Zero },
        new object[] { 1,  Peano.one },
        new object[] { 2,  Peano.two },
        new object[] { 3,  Peano.three },
        new object[] { 4,  Peano.four },
        new object[] { 5,  Peano.five },
        new object[] { 6,  Peano.six },
        new object[] { 7,  Peano.seven },
        new object[] { 8,  Peano.eight },
        new object[] { 9,  Peano.nine },
        new object[] { 10, Peano.ten }
    };
}

[Theory, MemberData(nameof(NumberData))]
public void NumberToInt(int expectedInt, Peano.Number number)
{
    var actualInt = ToInt(number);
    Assert.Equal(expectedInt, actualInt);
}

[Theory, MemberData(nameof(NumberData))]
public void IntToNumber(int integer, Peano.Number expectedNumber)
{
    var actualNumber = ToNumber(integer);
    Assert.Equal(expectedNumber, actualNumber);
}
```

Cool, that seems to work.

## One more, please

Let's start with a simple method: incrementing a number by one. What's one more
than a number? Its successor, of course!

``` fsharp
let increment x = Successor x
```

There's two alternative ways we can write this, and both are nice to know.
First, although it's common to leave function application implicit in functional
languages, you _can_ use parentheses to make it explicit:

``` fsharp
let increment(x) = Successor(x)
```

Now, before getting to the other other way of writing it, I'm going to explain
something about functions in (most) functional programming languages. Most
importantly, they're what's known as "first class citizens": a fundamental part
of the language. Functions are values, and you can pass them around as easy as
anything else.

Then there's [currying][currying]. In most functional programming languages,
functions don't actually have multiple arguments. Let's look at the following
function and usage:

``` fsharp
let concat a b = a + b
let cupcake = concat "cup" "cake" // cupcake = "cupcake"
```

That might _look_ like a function that takes two `string`s and returns a new
`string`, but that's not the entire truth of it. What's more true is that it is
a function that takes _one_ `string`, then returns a _function_ that takes a
`string` and returns a `string`. That means that `concat "cup" "cake"` is really
`(concat "cup") "cake"`. This also means that the following is perfectly valid:

``` fsharp
let concat a b = a + b
let say = concat "say "
```

After all, `concat "say "` returns a function that take a single `string` and
return a `string`. Take some time to let that sink in.

The result of all this is that we can also write our `increment` method as such:

``` fsharp
let increment = Successor
```

## One ~~less~~ fewer, please



[wiki]: https://en.wikipedia.org/wiki/Peano_axioms
[discriminated-union]: https://docs.microsoft.com/en-us/dotnet/fsharp/language-reference/discriminated-unions
[currying]: https://en.wikipedia.org/wiki/Currying

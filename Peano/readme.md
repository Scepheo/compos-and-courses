# Peano numbers in F&num;

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
public static IEnumerable<object[]> NumberData =>
    new []
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

With that done, let's write a test! We're going to use a theory with some inline
data to let us easily verify a number of test cases.

``` csharp
[Theory]
[InlineData(0)]
[InlineData(1)]
[InlineData(10)]
public void Increment(int value)
{
    // Test code here
}
```

I'll leave it to you to write the test code. It should be fairly simple: convert
`value` to a `Number`, use `Peano.increment` to increment it, convert it back to
an `int` and assert that it matches the expected value (which is `value + 1`).

## One ~~less~~ fewer, please

Decrementing is a bit more complex than incrementing. There's always two options
when we get a number: either we get zero, or we get the successor of some
number. We can distinguish these cases with pattern matching: we "match" the
received value against a number of "patterns", and the first one to match is
used. That sounds confusing, so let's see what it will look like:

``` fsharp
let decrement x =
    match x with
    | Zero -> // code for when x is zero
    | Successor y -> // code for when x is a successor
```

This time we need to do something with `x`, so we can't leave it out like last
time. You can see that we match `x` against two patterns: `Zero` and
`Successor y`. If it matches `Zero`, that means that we cannot decrement the
number, and we throw an `InvalidOperationException`. In F#, there's a built-in
method to do that:

``` fsharp
let decrement x =
    match x with
    | Zero -> invalidOp "Can't subtract from zero"
    | Successor y -> // code for when x is a successor
```

If `x` matches `Successor y`, that means that `x` is the successor of some
number `y`. When this match happens, this value is "captured", which just means
that the actual value is stored as `y`, and can be used as such. Which is handy,
because if `x` is the successor of `y`, decrementing `x` results in `y`!

``` fsharp
let decrement x =
    match x with
    | Zero -> invalidOp "Can't subtract from zero"
    | Successor y -> y
```

There's one last thing: something mathematicians do, and functional programmers
often do too, is to reuse the same symbol when two values are related. For
example, they might use _T_ to indicate total time, and _t_ for the current
time. Alternatively (and more commonly), the related/derived value will have the
same name, except with an apostrophe at the end. As such, rename `y` to `x'`.
And yes, that's a valid variable name in F#.

For the `Successor` logic, we'll be using a test very similar to the one used
for `increment`, except with different test cases. Again, I'm leaving it up to
you to implement the actual test.

``` csharp
[Theory]
[InlineData(1)]
[InlineData(2)]
[InlineData(11)]
public void Decrement(int value)
{
    // Test code here
}
```

However, we also have to test the zero case, where we simply assert that trying
to decrement zero throws an exception.

``` csharp
[Fact]
public void Decrement_Zero()
{
    Action action = () => Peano.decrement(Peano.Number.Zero);
    Assert.Throws<InvalidOperationException>(action);
}
```

Run the tests to make sure everything passes, and we're ready to move on.

## Add it up

Addition is even more complex than decrement, although not by much. Let's first
go through the logic: adding one to a number is taking its successor. Adding two
to a number is taking its successor twice. Adding three to... Well, you get the
point. Can we generalize? Yes: adding _n_ to a number is taking its successor
_n_ times.

That's hard to program, though; we have no way of doing something _n_ times.
However, doing something _n_ times is the same as doing it once, and then doing
it _n - 1_ times. Unless _n_ is zero, in which case you don't do anything. That
seems familiar...

Turns out it is! Adding _a_ to _b_ is taking _b_'s successor _a_ times. Or, it's
taking _b_'s successor once, and then adding _a - 1_ to that. Unless _a_ is
zero, in which case we're done. Recursion is a magnificent beast.

Now it's time for you to do some real thinking. Here's the layout of the
function and its test, now it's up to you to finish the function and make the
test pass (don't worry if you can't - a full implementation will be included
later).

``` fsharp
let rec add x y =
    match x with
    | Zero -> // if x = 0
    | Successor x' -> // else, x = x' + 1
```

``` csharp
[Theory]
[InlineData(0, 0)]
[InlineData(5, 0)]
[InlineData(0, 3)]
[InlineData(16, 27)]
public void Addition(int left, int right)
{
    var expected = left + right;
    var leftNumber = ToNumber(left);
    var rightNumber = ToNumber(right);
    var result = Peano.add(leftNumber, rightNumber);
    var actual = ToInt(result);
    Assert.Equal(expected, actual);
}
```

Apart from just helping you out with the structure of the method, there's
another reason I gave you the bare bones of the function: the `rec` keyword.
This is used in F# (and other ML-derived languages) to indicate that the
function is recursive. If we didn't add this keyword, the name `add` would not
exist (be bound) within the context of the function, and we couldn't call it.
_Not_ adding the `rec` keyword would, on the other hand, allow us to re-use the
name `add` for e.g. a helper function, which we'll be doing later.

That out of the way, I promised the full implementation. It's fairly
straightforward, once you understand it: if `x` is zero, we are essentially
doing `0 + y`, the result of which is, of course, `y`. If `x` _isn't_ zero, it
is the successor of `x'`, which is equal to `x - 1`. The successor of `y` is
equal to `y + 1`, so using the fact that _x + y = (x - 1) + (y + 1)_, we simply
call `add` again on those two.

Because we keep lowering `x` in every subsequent call, it will eventually reach
zero, and we will be done. More simply put: we keep decrementing `x` and
incrementing `y` by one until `x` is zero, then return `y`.

``` fsharp
let rec add x y =
    match x with
    | Zero -> y
    | Successor x' -> add x' (Successor y)
```

## Take it away now

Subtraction is, like `decrement`, a slightly trickier beast. After all, not
every subtraction results in a natural number. Sure, the result of 5 - 7 is -2,
but that's not a natural number, is it?

Other than that though, calculating `x - y` is very similar to `x + y`, except
instead incrementing the result every step, we decrement it. So, we've got
three rules:

> 1. _x - 0 = x_
> 2. _0 - y, y != 0_ is impossible (throw an exception)
> 3. _x - y = (x - 1) - (y - 1)_

So again, here's the bare bones of a function using that logic, and two tests
you can use to validate your logic: one for the actual subtraction logic, one
for the handling of invalid cases. Again, full code later, don't peek if you'd
like some practice.

``` fsharp
let rec subtract x y =
    match (x, y) with
    | (_, Zero) -> // if y = 0
    | (Zero, _) -> // else if x = 0, y > 0
    | (Successor x', Successor y') -> // else x = x' + 1, y = y' + 1
```

``` csharp
[Fact]
public void Subtraction_CantSubtractLargeFromSmall()
{
    var left = ToNumber(5);
    var right = ToNumber(7);
    Action action = () => Peano.subtract(left, right);
    Assert.Throws<InvalidOperationException>(action);
}

[Theory]
[InlineData(1, 0)]
[InlineData(5, 3)]
[InlineData(24, 13)]
public void Subtraction(int left, int right)
{
    var expected = left - right;
    var leftNumber = ToNumber(left);
    var rightNumber = ToNumber(right);
    var result = Peano.subtract(leftNumber, rightNumber);
    var actual = ToInt(result);
    Assert.Equal(expected, actual);
}
```

This one ends up with one more case to handle (`y` being greater than `x`), but
the recursive case is simpler. So, three cases: if `y` is zero, return `x`
(because _x - 0 = x_). Else, if `x` is zero, throw an invalid operation
exception (because _0 - y, y > 0_ is not allowed). Otherwise, return `x' - y'`,
because _x' = x - 1, y' = y - 1_ and _x - y = (x - 1) - (y - 1)_.

Also note the use of `(x, y)` in the pattern match: we create a tuple from `x`
and `y` so that we can match against them both. The underscores are used to
indicate that we don't care what's in that part of the match: either we already
have the value we're interested in (first case) or we simply don't need it
(second case).

``` fsharp
let rec subtract x y =
    match (x, y) with
    | (_, Zero) -> x
    | (Zero, _) -> invalidOp "Can't subtract a larger number from a smaller number"
    | (Successor x', Successor y') -> subtract x' y'
```

## Go forth and multiply

If addition is repeated incrementation, multiplication is repeated addition: to
multiply _x_ by _y_, you start with 0 and add _x_ to that _y_ times (or the
other way around, of course). That gives us a problem: we have to keep track of
_three_ things this time:

* _a_, which starts at zero and is our answer so far ("a" for "accumulator")
* _x_, so we can add it it _a_
* _y_, which we will use to keep track of how often we still have to add _x_ to
  _a_.

However, our multiplication method only takes _two_ parameters: _x_ and _y_. So
if we want to recurse, how are we going to keep track of _a_?

Well, we create a helper method! Remember how I told you before, that if you're
in a non-recursive function declaration (i.e. without the `rec` keyword), the
name of the function isn't bound to anything? Well, that allows us to re-use the
name for the "real" multiplication function: the one _with_ _a_. We can then
solve our problem by calling that and passing in 0 (which is the starting value
for _a_, remember?).

``` fsharp
let multiply x y =
    let rec multiply a x y =
        ...
    in multiply Zero x y
```

The main reason I'm showing you that you can re-use the name within the function
itself is so you can understand what's going on when you see it written down.
Personally, I like to re-use the system for "alternative variables": appending
an apostrophe:

``` fsharp
let multiply x y =
    let rec multiply' a x y =
        ...
    in multiply' Zero x y
```

So, now it's time to implement the `multiply'` function. Like before, there's
two rules that we will essentially use to define multiplication:

> 1. _0 * y = 0_
> 2. _x * y = y + (x - 1) * y_

So, calling our helper function _m_, which uses _a_ to keep track of the sum so
far, we can rewrite those rules to the following:

> 1. _x * y = m(0, x, y)_
> 2. _m(a, 0, y) = a_
> 3. _m(a, x, y) = m(a + y, x - 1, y)_

Make sure you understand how these two sets of rules mean the same thing, and
you should be able to implement the `multiply'` function yourself. This time I
won't be including the full code later, but here are the tests to help you make
sure your function works as it should.

``` csharp
[Theory]
[InlineData(0, 0)]
[InlineData(5, 0)]
[InlineData(0, 3)]
[InlineData(1, 7)]
[InlineData(8, 9)]
public void Multiplication(int left, int right)
{
    var expected = left * right;
    var leftNumber = ToNumber(left);
    var rightNumber = ToNumber(right);
    var result = Peano.multiply(leftNumber, rightNumber);
    var actual = ToInt(result);
    Assert.Equal(expected, actual);
}
```

Make sure these tests pass (just like before), as later functions will depend on
their working correctly (like `multiply` depends on `add`).

## Divide and conquer

We've got most of the basic arithmetic going now, there's one to go still:
division. Like with subtraction, the result of dividing one natural number with
another isn't always a natural number: what's the result of 1 / 2, or 2 / 0?

For division by zero, there's no real solution: you just can't do that. For
divisions that result in non-whole numbers, there (sort of) is a solution, which
is integer division. Just like with real integer types in most programming
languages, the result of a division is rounded down to the nearest integer. This
means that the result of 27 / 10 is 2 (2.7 rounded down).

Just like addition, subtraction and multiplication, division is also the
repeated application of another operation. In this case: subtraction. After all,
the result of 27 / 10 is the number of times we can subtract 10 from 27: 2
times. However, this means that we have to do something we can't do yet: check
whether we can actually subtract or divisor from the number.

### Less than

So, before we end up subtracting our divisor too often (which throws an error,
remember?), we have to check whether our remaining number is less than the
divisor: if so, we are done with the division. So, how do we compare two
numbers?

Well, comparisons with zero are easy: nothing is less than zero, so if the right
operand is zero, we can return `false`. If the right operand _isn't_ zero, but
the left one _is_, the answer is `true`. That leaves us with the case where both
numbers aren't zero: in that case, we can just subtract one from each and repeat
our comparison on those! After all, _x < y_ is equivalent to _x - 1 < y - 1_.

So, our rules:

> 1. _x < 0 = false_
> 2. _0 < y, y != 0 = true_
> 3. _x < y = x - 1 < y - 1_

By now you should be able to implement this method yourself, so I'm only going
to provide the tests for you. Remember that you can use tuples to match against
multiple values!

``` csharp
[Theory]
[InlineData(0, 0)]
[InlineData(6, 6)]
[InlineData(0, 1)]
[InlineData(1, 0)]
[InlineData(1, 1)]
[InlineData(4, 5)]
[InlineData(7, 19)]
[InlineData(3, 8)]
public void LessThan(int left, int right)
{
    var expected = left < right;
    var leftNumber = ToNumber(left);
    var rightNumber = ToNumber(right);
    var actual = Peano.lessThan(leftNumber, rightNumber);
    Assert.Equal(expected, actual);
}
```

Make sure all the tests pass, and you're good to go on to the next bit.

### Divide

So, division. This is a bit more tricky than before, and I'm going to teach you
a new thing: `if` expressions. If you've programmed in C# before (or any of many
other languages that use it), you might be familiar with the conditional
operator: `a ? b : c`. The difference between it and a regular `if` _statement_
is that an `if` statement doesn't have a value, and might not have an `else`:

``` csharp
if (path == null)
{
    path = DefaultPath;
    Console.WriteLine($"No path set, using default '{DefaultPath}'.");
}
```

As you might have noticed, in F# _everything_ has a value (i.e. is an
expression), and as a result, the `if` _operator_ is more like the `?:`
operator:

``` fsharp
let absolute x = if x < 0 then -x else x
```

And, of course, note the `then` keyword: this is required.

So, on to our divide method. What was the first thing again? Right, _if_ (see?)
the divisor is zero, throw an exception:

``` fsharp
let divide x y =
    if y = Zero then
        invalidOp "Can't divide by zero"
    else
        // y isn't zero here
```

So what do we do if `y` isn't zero? Well, just like with multiplication, we're
going to need to keep track of a third value: how often we've subtracted `y`
from `x`. So, we're going to create another (recursive) helper function. We're
going to use `a` for "accumulator" again, and also like with multiplication, it
starts at zero.

``` fsharp
let divide x y =
    if y = Zero then
        invalidOp "Can't divide by zero"
    else
        let rec divide' a x y =
            // code for division
        in divide' Zero x y
```

The division itself is fairly straightforward: if `x` is less than `y`, we can't
subtract anymore, and we can return `a` (which is how often we've subtracted so
far). If `x` is equal to or greater than `y` (which is just _else_), we _can_
subtract `y` from `x`, we increment `a` and repeat!

``` fsharp
let divide x y =
    if y = Zero then
        invalidOp "Can't divide by zero"
    else
        let rec divide' a x y =
            if lessThan x y then a
            else divide' (increment a) (subtract x y) y
        in divide' Zero x y
```

That's the entire thing, so you don't have to do it yourself - this time. It'll
be useful to remember how this `if`-`then`-`else` thing works, as you will be
writing it yourself later on. However, just to make sure you've still got
something to do, you can finish these tests (have a look at `subtract`'s tests
for some help).

``` csharp
[Fact]
public void Division_CantDivideByZero()
{
    // Test for an invalid operation exception here
}

[Theory]
[InlineData(1, 1)]
[InlineData(5, 3)]
[InlineData(15, 3)]
[InlineData(100, 9)]
public void Division(int left, int right)
{
    // Test for correct division here
}
```

## All that remains

Where division tells you how often the number `y` fits in the number `x`, modulo
tells you which number you'd have left if you actually tried. It's used often
in programming, and we'll be needing it later, so we're going to implement it.

As you might have guessed from its relation to division, modulo can also be
implemented with repeated subtraction. Except where division is _how often_ you
can subtract `y` from `x` before it becomes impossible, module is the number you
have left at that point. So, let's write this down in some rules:

> 1. _x % 0_ is impossible, throw an exception (can't divide by zero, so you
  also can't have anything left)
> 2. _x % y, y > x = x_ (can't subtract `y` from `x` anymore)
> 3. _x % y, x &#x2264; y = (x - y) % y_

I won't be providing the code this time, but as a hint: you'll probably want to
use `if-then-else` statements. And remember: just like in C#, you can put an
`if`-clause in the `else`-clause to make an `else-if`.

Here are some tests you can use to ensure that your code is correct:

``` csharp
[Fact]
public void Modulo_CantDivideByZero()
{
    Action action = () => Peano.modulo(Peano.Number.Zero, Peano.Number.Zero);
    Assert.Throws<InvalidOperationException>(action);
}

[Theory]
[InlineData(1, 1)]
[InlineData(5, 3)]
[InlineData(15, 3)]
[InlineData(100, 9)]
public void Modulo(int left, int right)
{
    var expected = left % right;
    var leftNumber = ToNumber(left);
    var rightNumber = ToNumber(right);
    var result = Peano.modulo(leftNumber, rightNumber);
    var actual = ToInt(result);
    Assert.Equal(expected, actual);
}
```

## Show your work

Of course, we're still missing one of the most important things we can do with
the "normal" integer types in programming language: turning them into strings so
we can print them. So, obviously, that's what we're going to do now.

### Digits

In order to be able to print a whole number, we're first going to have to be
able to print a single digit. We can use the number variables we've defined at
the start of the course for this. The principle is simple: our function
`printDigit` takes a number. If it's equal to `Zero`, we return the string
`"0"`, Else, if it's equal to `one`, we return the string `"1"`, and so on until
we get to nine. In all other cases (i.e. the number is 10 or greater), we throw
an exception.

And here are some more tests for you to verify your code works correctly.

``` csharp
[Fact]
public void PrintDigit_FailsForMoreThanNine()
{
    var number = ToNumber(15);
    Action action = () => Peano.printDigit(number);
    Assert.Throws<InvalidOperationException>(action);
}

public static IEnumerable<object[]> PrintDigitValues =>
    from value in Enumerable.Range(0, 10)
    select new object[] { value };

[Theory, MemberData(nameof(PrintDigitValues))]
public void PrintDigit(int i)
{
    var number = ToNumber(i);
    var result = Peano.printDigit(number);
    Assert.Equal(i.ToString(), result);
}
```

### Numbers

And now, to combine it all! So, how do we print a number? Well, normally you
check how often each power of ten fits in the number, then you write down those
digits in order of greatest power of ten to smallest (i.e. 1). So,
one-hundred-and-twenty-three is _1 * 100 + 2 * 10 + 3 * 1_, which becomes _123_.
We don't write down any leading zeroes, unless the number is zero, in which case
we just write a zero.

Obviously, this is still miles away from actual code (apart from the zero bit,
which is perfect as-is), and we're going to have to do things slightly
differently. Starting with the largest digit is tricky, for one, as that
requires us to first find out what the largest power of ten is that still fits
in our number. No, there's a simpler way: start with the last digit, then
prepend the rest.

So, how do you get the last digit? With `modulo`, of course! How do we print
the last digit? With `printDigit`, of course! How do we get the rest of the
number? With `divide`, of course! How to we print the rest of the number? With
the `print` function we're currently writing, of course! Recursion!

This might be a tricky one, but I'm confident you can do it. To be sure,
however, make sure the following tests actually pass:

``` csharp
public static IEnumerable<object[]> PrintValues =>
    from index in Enumerable.Range(0, 11)
    let value = index * 7
    select new object[] { value };

[Theory, MemberData(nameof(PrintValues))]
public void Print(int i)
{
    var number = ToNumber(i);
    var result = Peano.print(number);
    Assert.Equal(i.ToString(), result);
}
```

## Taking it all in

Writing these numbers out is nice, but what about reading them in? The principle
of parsing is very similar to printing: if the string is just a single digit,
return that digit (again, we've got `one` through `nine` defined to help us
with this). Otherwise, parse the last digit and the rest of the number
separately, then combine their results.

So now, as a last exercise, go and implement a `parse` function. Here's some
more tests to help you verify your solution.

``` csharp
public static IEnumerable<object[]> ParseValues =>
    from index in Enumerable.Range(0, 11)
    let value = index * 7
    let str = value.ToString()
    select new object[] { value };

[Theory, MemberData(nameof(ParseValues))]
public void Parse(string str)
{
    var result = Peano.parse(str);
    var actual = ToInt(result);
    Assert.Equal(int.Parse(str), actual);
}
```

## Proof of concept

Here's a nice final test to show that all the functions work, and that the
numbers act like, well, numbers. Assuming all the previous tests pass, this
will just work.

``` csharp
[Fact]
public void CombineItAll()
{
    const int expected = 12 * 34 - 56 + 78 / 90;

    var result = Peano.add(
        Peano.subtract(Peano.multiply(Peano.parse("12"), Peano.parse("34")), Peano.parse("56")),
        Peano.divide(Peano.parse("78"), Peano.parse("90")));

    var actual = Peano.print(result);

    Assert.Equal(expected.ToString(), actual);
}
```

Well, that's it for this course. I hope you enjoyed writing your own number
system, and I hope you enjoyed using F#.

[wiki]: https://en.wikipedia.org/wiki/Peano_axioms
[discriminated-union]: https://docs.microsoft.com/en-us/dotnet/fsharp/language-reference/discriminated-unions
[currying]: https://en.wikipedia.org/wiki/Currying

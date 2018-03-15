# Dependency injection

Dependency injection is a topic that seems to trouble quite a number of
developers when they first experience it. Explaining it seems to work to the
point where they can use it, but it rarely appears to be properly understood.

This document aims to fix that problem, by having you implement a dependency
injection system yourself.

## How does this thing work

You read it and follow along. It's written in C#, and because it's quite closely
tied to the language capabilities, I recommend using that. For most parts,
you'll be writing along, although I won't be telling you *exactly* what to do -
it's up to you to make sure things are in the correct files and namespace and
that sort of thing.

You're gonna have to set up a solution with a library project and a unit test
project (I'm using XUnit - you can copy my tests if you're using that too, but
it shouldn't be hard to change them to work with MSTest, NUnit or whatever test
framework you like best).

## What is dependency injection?

Also known as "inversion of control", dependency injection is a system whereby,
instead of creating all of an object yourself, you have a magic object (known
as a container) do that for you. To put that in code, whenever you need a
`computer`, you don't do this everywhere:

``` csharp
var screen = new Screen();
var mouse = new LeftHandedMouse();
var earphones = new Earphones();
var microphone = new Microphone();
var headset = new Headset(earphones, microphone);
var printer = new Printer();
var keyboard = new Keyboard("en-US");
var computer = new Computer(mouse, keyboard, screen, headset, printer);
```

Instead, you do this once:

``` csharp
magicContainer.Implement<Mouse, LeftHandedMouse>();
magicContainer.Configure<Keyboard>("en-US");
```

And then, when you need a `Computer`, you do:

``` csharp
var computer = magicContainer.Resolve<Computer>();
```

Or even better, when using constructor dependency injection (which is the type
most often encountered in the wild), if you have a class that requires a
computer (say, a `WorkSpace`), you just add it as a constructor parameter:

``` csharp
public class WorkSpace
{
    private readonly Computer _computer;

    public WorkSpace(Computer computer)
    {
        _computer = computer;
    }
}
```

Obviously, these are just examples to illustrate, and you don't really have to
understand these now. There's also a number of other reasons for using
dependency injection, but these are covered quite well in a variety of articles
and blog posts just a
[google](https://www.google.com/search?q=why+use+dependency+injection) away
(although that won't be stopping me from touching on them later on anyway).

## Planting the dependency tree

Before we can start resolving any dependencies, we're going to need some first.
We'll be creating a number of classes with different constructors, to illustrate
a small variety of use cases. Following the examples above, let's start with our
goal, we want to end up with a computer.

**NOTE:** All this stuff goes into the *unit test* project. After all, our
dependency injection library can't have any knowledge of the types it will be
used for.

``` csharp
public class Computer
{
    public Mouse Mouse { get; }
    public Keyboard Keyboard { get; }
    public Screen Screen { get; }
    public Headset Headset { get; }
    public IPrinter Printer { get; }

    public Computer(
        Mouse mouse,
        Keyboard keyboard,
        Screen screen,
        Headset headset,
        IPrinter printer)
    {
        Mouse = mouse;
        Keyboard = keyboard;
        Screen = screen;
        Headset = headset;
        Printer = printer;
    }
}
```

This is a simple class that has enough dependencies to make it annoying if you'd
have to instantiate them yourself every time. You might have noticed that
`printer` is an interface instead of a class - I'll explain why later.

For now, let's just make sure this actually compiles:

``` csharp
public class Mouse { }
public class Keyboard { }
public class Screen { }
public class Headset { }
public interface IPrinter { }
```

We'll be adding code to `Headset` later, so you might want to put that in its
own file. I'd normally recommend to put all classes (and interfaces and structs)
in their own file, as there's always the possibility of code being added, but
for this tutorial it doesn't really matter.

## Test Driven Development (TDD) is all the rage these days

As the first test for our dependency injection library, let's see if it can
solve a simple case:

``` csharp
[Fact]
public void CanResolveSimpleClass()
{
    var container = new MagicContainer();

    var obj = container.Resolve(typeof(Screen));

    Assert.NotNull(obj);
    Assert.IsType<Screen>(obj);
}
```

Doesn't compile, of course, so let's make our magic container exist (and yes,
I really am calling it that). Time to write our first bit of code in the library
project.

``` csharp
public class MagicContainer
{
    public object Resolve(Type type)
    {
        // We'll be adding the actual code here later

        return null;
    }
}
```

Well, that test is going to fail - we just return null instead of an
instantiated object. So, how do we solve this? We have no idea what `type` is
going to be. In fact, in our test case it's `Screen`, and our library project
doesn't even know that type exists!

This is the sort of problem you solve using a little thing called "reflection":
code that allows you to get information about code (it can "reflect" on itself).
In this case, we're interested in the constructors of the object:

``` csharp
var constructors = type.GetConstructors();
```

For now, we're only going to solve the simple case: an empty constructor. Let's
see if the list of constructors contains one that doesn't take any parameters.
If there isn't one, we'll just throw an exception.

``` csharp
var emptyConstructor = constructors.FirstOrDefault(
    constructor => constructor.GetParameters().Length == 0);

if (emptyConstructor == null)
{
    throw new InvalidOperationException($"Cannot construct type '{type.Name}'");
}
```

If there *is* one, however, we're almost there. All that's left is to "invoke"
it (which just means to run it). This requires an array of all the parameters it
takes, but that's easy: it doesn't take any!

``` csharp
var parameters = new object[0];
return emptyConstructor.Invoke(parameters);
```

If we were serious about writing a dependency injection library, this would be
the time to add tests that make sure that the correct exception is thrown if we
can't construct the type and all those sort of things. But we're not serious, so
never mind all that, this thing is long enough as it is.

## Growing the dependency tree

But what about `Headset`? As we saw in the example, that's a bit more complex,
as it requires both earphones and a microphone!

``` csharp
public class Headset
{
    public Earphones Earphones { get; }
    public Microphone Microphone { get; }

    public Headset(Earphones earphones, Microphone microphone)
    {
        Earphones = earphones;
        Microphone = microphone;
    }
}

public class Earphones { }
public class Microphone { }
```

I'm sure we'll make it work somehow.

``` csharp
[Fact]
public void CanResolveNestedClass()
{
    var container = new MagicContainer();

    var obj = container.Resolve(typeof(Headset));

    Assert.NotNull(obj);
    var headset = Assert.IsType<Headset>(obj);
    Assert.NotNull(headset.Earphones);
    Assert.NotNull(headset.Microphone);
}
```

All right, let's change our earlier `Resolve` method a little bit. Instead of
looking for an empty constructor, just check if there's only one and store it in
a variable called `constructor` (`emptyConstructor` isn't an appropriate name
anymore).

If there is not *exactly* one, throw an exception (the same one as before). A
decent library would probably do something like look at its configuration or
attributes or try them all, but again, we're just here to learn, so an exception
it is.

``` csharp
var constructors = type.GetConstructors();

if (constructors.Length != 1)
{
    throw new InvalidOperationException($"Cannot construct type '{type.Name}'");
}

var constructor = constructors[0];
```

Afterwards, we're going to have to create the parameter array again. This time,
however, we'll actually have to fill it.

``` csharp
var parameterInfos = constructor.GetParameters();
var parameters = new object[parameterInfos.Length];

for (var i = 0; i < parameters.Length; i++)
{
    parameters[i] = ???; // What do we do here???
}

return constructor.Invoke(parameters);
```

So now we need to figure out how we can get an object of that parameter's type.
Wait a second - we already know how to do that! That's the exact method we're
writing! Recursion to the rescue:

``` csharp
parameters[i] = Resolve(parameterInfos[i].ParameterType);
```

Keen readers might notice that this will go on forever if a type takes a
parameter of its own type. This is know as a cyclic dependency, and we *should*
really detect it. However, those same keen readers will also remember that we
don't care about that sort of thing right now.

## Let's pretty that up

Currently, the result of our `Resolve` method is an `object`. Not very pretty,
really, as we'd have to convert it to the correct type wherever we call it. In
the last test, the `Assert.IsType` method does this for us, but having to cast
it everywhere in "real" code isn't very pretty. The solution is simple, though:
we simply put the cast in a generic overload of `Resolve`, using the generic
type parameter as the argument to the original `Resolve`.

``` csharp
public T Resolve<T>() => (T)Resolve(typeof(T));
```

Easy, right? Now the test can look like this:

``` csharp
[Fact]
public void CanResolveNestedClass()
{
    var container = new MagicContainer();

    var headSet = container.Resolve<Headset>();

    Assert.NotNull(headSet);
    Assert.NotNull(headSet.Earphones);
    Assert.NotNull(headSet.Microphone);
}
```

Sure, we're missing the type assertion, but if it wasn't the right type, the
cast would fail anyway. Again, in real library code two methods would have to
mean two sets of tests.

For code that interacts with the types themselves (reflection code, usually), it
can often be useful to have two methods like this: one to use when you know the
type at compile time (in the test), and one to use when you don't (when
resolving for a parameter type).

## Time to add some configuration-ing

Obviously, we might not always want to give back the exact type that's asked
for. We can't even instantiate interfaces for example, so if someone asks for
that, we'll *have* to give back something else. What, though? Well, that's where
configuration comes in: the programmer will have to tell the container how to
resolve certain type. In the spirit of TDD, we start by adding a test.

``` csharp
[Fact]
public void CanResolveInterface()
{
    var container = new MagicContainer();
    container.Implement<IPrinter, Printer>();

    var printer = container.Resolve<IPrinter>();

    Assert.NotNull(printer);
    Assert.IsType<Printer>(printer);
}
```

Oops, forgot to create a `Printer` type.

``` csharp
public class Printer : IPrinter { }
```

Let's add a place to store this configuration to our `MagicContainer`. What will
happen in our updated `Resolve`, is that we'll be asked for one type (the
"requesting" type), look up that type to see what type should be used to
implement it (the "implementing" type) and resolve the latter.

As we'll be doing lookups, a dictionary seems appropriate.

``` csharp
private readonly Dictionary<Type, Type> _typeMappings
    = new Dictionary<Type, Type>();
```

To allow developers to configure certain types, we'll implement the `Implement`
method. As you can see from the test, we want there to be a generic one. Just
like with our `Resolve` methods, the generic one will simply call into the
non-generic one.

For the generic one, we get the added bonus that we can let the compiler
verify the correctness of our configuration, by requiring that `TImplementing`
actually is a subtype of `TRequested`.

``` csharp
public void Implement<TRequested, TImplementing>()
    where TImplementing : TRequested
    => Implement(typeof(TRequested), typeof(TImplementing));

public void Implement(Type requested, Type implementing)
{
    _typeMappings.Add(requested, implementing);
}
```

It might seem like an unnecessary indirection to do it this way, as it's not
much more code to add the mapping directly to the dictionary. However, if we
ever change the way we store the mapping, this way we'll only have to update a
single method.

If you run the tests now, you'll see that the configuration works fine, but the
`Resolve` method fails: after all, you're asking for an interface, and that
doesn't have any constructors. To fix this, we have to start by checking if the
type that's asked for has an implementing type defined. If so, *that's* that
type we want to resolve.

``` csharp
public object Resolve(Type type)
{
    if (_typeMappings.TryGetValue(type, out var implementingType))
    {
        return Resolve(implementingType);
    }
}
```

Again, we're using recursion to resolve that type. We could do something like
reassigning `type`, but what if the implementing type has a mapping *too*? We'd
have to repeat until there's no more mappings defined! But then there might be
other configuration (there will be, later) we have to look at, and after that's
done, there might be mappings again!

To avoid that mess, just use `Resolve` to resolve types. The clue's in the name.

## Here's this other thing we just implemented

You might have noticed that there's nothing to ensure that what's passed to
`Implement` is actually an interface. The reason for that is that we don't
really care. Going back to the example at the start, let's add a
`LeftHandedMouse` and `RightHandedMouse` class.

``` csharp
public class LeftHandedMouse : Mouse { }
public class RightHandedMouse : Mouse { }
```

Now, we only have to configure our container to return the correct type once,
and whenever a mouse is needed, the correct type will be given.

``` csharp
[Fact]
public void CanResolveSubTypeLeft()
{
    var container = new MagicContainer();
    container.Implement<Mouse, LeftHandedMouse>();

    var mouse = container.Resolve<Mouse>();

    Assert.NotNull(mouse);
    Assert.IsType<LeftHandedMouse>(mouse);
}

[Fact]
public void CanResolveSubTypeRight()
{
    var container = new MagicContainer();
    container.Implement<Mouse, RightHandedMouse>();

    var mouse = container.Resolve<Mouse>();

    Assert.NotNull(mouse);
    Assert.IsType<RightHandedMouse>(mouse);
}
```

## How about that `Configure` example?

Well, sometimes you just need a parameter for a class that isn't just another
type. Think of connection strings or log file names. Most libraries will give
you nice methods to specify the values for *some* of the parameters, but we'll
only be implementing the specification of *all* parameters.

For starters, we're going to need a type to use this on. Keyboards come in so
many different configurations that it's not realistic to have a type for each
one, so lets make the culture a member on `Keyboard`.

``` csharp
public class Keyboard
{
    public string Culture { get; }

    public Keyboard(string culture)
    {
        Culture = culture;
    }
}
```

Time for the test!

``` csharp
[Fact]
public void CanResolveWithArguments()
{
    var container = new MagicContainer();
    const string culture = "en-US";
    container.Configure<Keyboard>(culture);

    var keyboard = container.Resolve<Keyboard>();

    Assert.NotNull(keyboard);
    Assert.Equal(culture, keyboard.Culture);
}
```

Which doesn't compile, because we haven't written `Configure` again. Like
before, we'll be receiving a type for which we have to look up whether we've
received a configuration earlier. So again, we'll be using a dictionary. This
time, however, we'll be looking up parameter values, which are `object[]`
(which is passed into `Invoke`).

Also like before, we'll be doing the whole "one is a generic that calls the
other one that isn't generic" thing again.

``` csharp
private readonly Dictionary<Type, object[]> _argumentConfigurations
    = new Dictionary<Type, object[]>();

public void Configure<T>(params object[] arguments)
    => Configure(typeof(T), arguments);

public void Configure(Type type, params object[] arguments)
{
    _argumentConfigurations.Add(type, arguments);
}
```

Well, everything compiles again, but now the test fails. How do we fix this?
In `Resolve`, before building the parameter list for a type's constructor, we'll
have to see if one has been configured. If so, use that instead of the regular
parameters.

``` csharp
var parameterInfos = constructor.GetParameters();
object[] parameters;

if (_argumentConfigurations.TryGetValue(type, out var configuredValues))
{
    parameters = configuredValues;
}
else
{
    parameters = new object[parameterInfos.Length];

    for (var i = 0; i < parameters.Length; i++)
    {
        parameters[i] = Resolve(parameterInfos[i].ParameterType);
    }
}

return constructor.Invoke(parameters);
```

Run the test again, and you'll see it pass.

## Putting it all together

And now for the big finale: creating a computer! Here's the full test:

``` csharp
[Fact]
public void CanResolveComputer()
{
    var container = new MagicContainer();
    container.Implement<IPrinter, Printer>();
    container.Implement<Mouse, RightHandedMouse>();
    const string culture = "en-US";
    container.Configure<Keyboard>(culture);

    var computer = container.Resolve<Computer>();

    Assert.NotNull(computer);
    Assert.NotNull(computer.Mouse);
    Assert.IsType<RightHandedMouse>(computer.Mouse);
    Assert.NotNull(computer.Keyboard);
    Assert.Equal(culture, computer.Keyboard.Culture);
    Assert.NotNull(computer.Screen);
    Assert.NotNull(computer.Headset);
    Assert.NotNull(computer.Headset.Earphones);
    Assert.NotNull(computer.Headset.Microphone);
    Assert.NotNull(computer.Printer);
    Assert.IsType<Printer>(computer.Printer);
}
```

And that's it! Configure it once, and you can get as many computers (and its
constituent parts) as you like.

## In closing

Obviously, there's a lot of stuff a real dependency injection library does that
we didn't cover. Different types of resolutions (singleton, scoped, transient),
better error handling, more configuration options and so on and so on. However,
the point was to teach you something about dependency injection, and hopefully
this has helped you understand that at least a bit better.

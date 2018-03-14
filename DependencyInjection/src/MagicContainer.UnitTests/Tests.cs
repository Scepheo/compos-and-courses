using Xunit;

namespace MagicContainer.UnitTests
{
    public class Tests
    {
        [Fact]
        public void CanResolveSimpleClass()
        {
            var container = new MagicContainer();

            var obj = container.Resolve(typeof(Screen));

            Assert.NotNull(obj);
            Assert.IsType<Screen>(obj);
        }

        [Fact]
        public void CanResolveNestedClass()
        {
            var container = new MagicContainer();

            var headSet = container.Resolve<Headset>();

            Assert.NotNull(headSet);
            Assert.NotNull(headSet.Earphones);
            Assert.NotNull(headSet.Microphone);
        }

        [Fact]
        public void CanResolveInterface()
        {
            var container = new MagicContainer();
            container.Implement<IPrinter, Printer>();

            var printer = container.Resolve<IPrinter>();

            Assert.NotNull(printer);
            Assert.IsType<Printer>(printer);
        }

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
    }
}

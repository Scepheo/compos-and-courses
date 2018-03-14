namespace MagicContainer.UnitTests
{
    public class Computer
    {
        public Mouse Mouse { get; }
        public Keyboard Keyboard { get; }
        public Screen Screen { get; }
        public Headset Headset { get; }
        public IPrinter Printer { get; }

        public Computer(Mouse mouse, Keyboard keyboard, Screen screen, Headset headset, IPrinter printer)
        {
            Mouse = mouse;
            Keyboard = keyboard;
            Screen = screen;
            Headset = headset;
            Printer = printer;
        }
    }
}
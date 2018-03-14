namespace MagicContainer.UnitTests
{
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
}
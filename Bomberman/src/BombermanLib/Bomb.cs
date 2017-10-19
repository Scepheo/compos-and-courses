namespace BombermanLib
{
    public class Bomb
    {
        public readonly int Number;
        public int Turns;
        public readonly Vector Position;

        public Bomb(int number, Vector position)
        {
            Number = number;
            Position = position;
            Turns = 5;
        }
    }
}

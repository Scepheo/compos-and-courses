namespace BombermanLib
{
    public struct Bomb
    {
        public readonly int Number;
        public int Turns;
        public readonly int X;
        public readonly int Y;

        public Bomb(int number, int x, int y)
        {
            Number = number;
            X = x;
            Y = y;
            Turns = 5;
        }
    }
}

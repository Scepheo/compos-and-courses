namespace BombermanLib
{
    public struct Player
    {
        public readonly int Number;
        public readonly int X;
        public readonly int Y;

        public Player(int number, int x, int y)
        {
            Number = number;
            X = x;
            Y = y;
        }
    }
}

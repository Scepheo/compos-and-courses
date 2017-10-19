namespace BombermanLib
{
    public class Player
    {
        public readonly int Number;
        public Vector Position;
        public bool Dead;

        public Player(int number, Vector position)
        {
            Number = number;
            Position = position;
            Dead = false;
        }
    }
}

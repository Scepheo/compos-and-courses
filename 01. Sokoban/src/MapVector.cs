namespace Sokoban
{
    public struct MapVector
    {
        public readonly int X;
        public readonly int Y;

        public MapVector(int x, int y)
        {
            X = x;
            Y = y;
        }

        public EntityVector ToEntityVector() => new EntityVector(X * Level.TileSize, Y * Level.TileSize);

        public static MapVector operator +(MapVector left, MapVector right)
            => new MapVector(left.X + right.X, left.Y + right.Y);

        public override bool Equals(object obj) => obj is MapVector other && other.X == X && other.Y == Y;
        public override int GetHashCode() => (X << 16) | Y;
        public override string ToString() => $"({X}, {Y})";

        public static bool operator ==(MapVector left, MapVector right) => left.X == right.X && left.Y == right.Y;
        public static bool operator !=(MapVector left, MapVector right) => left.X != right.X || left.Y != right.Y;
    }
}

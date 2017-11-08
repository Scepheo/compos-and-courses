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

        // TODO: Assignment 8
        public override bool Equals(object obj)
        {
            if (obj.GetType() != typeof(MapVector))
            {
                return false;
            }

            var other = (MapVector)obj;
            return other.X == X && other.Y == Y;
        }

        public override int GetHashCode() => (X << 16) | Y;
        public override string ToString() => $"({X}, {Y})";

        public static bool operator ==(MapVector left, MapVector right) => left.X == right.X && left.Y == right.Y;
        public static bool operator !=(MapVector left, MapVector right) => left.X != right.X || left.Y != right.Y;

        public void Deconstruct(out int x, out int y)
        {
            x = X;
            y = Y;
        }
    }
}

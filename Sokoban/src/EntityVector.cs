using System;

namespace Sokoban
{
    public struct EntityVector
    {
        public readonly int X;
        public readonly int Y;

        public EntityVector(int x, int y)
        {
            X = x;
            Y = y;
        }

        public MapVector GetMapVector() => new MapVector(X / Level.TileSize, Y / Level.TileSize);
        public MapVector Sign() => new MapVector(Math.Sign(X), Math.Sign(Y));

        public bool IsMapVector => X % Level.TileSize == 0 && Y % Level.TileSize == 0;

        public static readonly EntityVector Zero = new EntityVector(0, 0);

        public static EntityVector operator +(EntityVector left, EntityVector right)
            => new EntityVector(left.X + right.X, left.Y + right.Y);

        public override bool Equals(object obj) => obj is EntityVector other && other.X == X && other.Y == Y;
        public override int GetHashCode() => (X << 16) | Y;
        public override string ToString() => $"({X}, {Y})";

        public static bool operator ==(EntityVector left, EntityVector right) => left.X == right.X && left.Y == right.Y;
        public static bool operator !=(EntityVector left, EntityVector right) => left.X != right.X || left.Y != right.Y;
    }
}

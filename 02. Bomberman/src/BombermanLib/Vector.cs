namespace BombermanLib
{
    public struct Vector
    {
        public readonly int X;
        public readonly int Y;

        public static readonly Vector Zero = new Vector(0, 0);
        public static readonly Vector Up = new Vector(0, -1);
        public static readonly Vector Down = new Vector(0, 1);
        public static readonly Vector Left = new Vector(-1, 0);
        public static readonly Vector Right = new Vector(1, 0);
        public static readonly Vector[] Directions = { Up, Down, Left, Right };

        public Vector(int x, int y)
        {
            X = x;
            Y = y;
        }

        public static Vector operator +(Vector left, Vector right)
        {
            return new Vector(left.X + right.X, left.Y + right.Y);
        }

        public static Vector operator -(Vector vector)
        {
            return new Vector(-vector.X, -vector.Y);
        }

        public static Vector operator -(Vector left, Vector right)
        {
            return new Vector(left.X - right.X, left.Y - right.Y);
        }

        public static Vector operator *(int scalar, Vector vector)
        {
            return new Vector(scalar * vector.X, scalar * vector.Y);
        }

        public override bool Equals(object obj)
        {
            return obj is Vector other && other.X == X && other.Y == Y;
        }

        public override int GetHashCode()
        {
            return unchecked((X << 16) | Y);
        }

        public static bool operator ==(Vector left, Vector right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Vector left, Vector right)
        {
            return !left.Equals(right);
        }
    }
}

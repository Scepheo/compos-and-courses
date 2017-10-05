using System.Collections.Generic;

namespace BombermanLib
{
    public class Level
    {
        public bool[,] Walls { get; private set; }
        public List<Player> Players { get; private set; }
        public List<Bomb> Bombs { get; private set; }
        public List<Box> Boxes { get; private set; }

        private Level() { }

        public static Level TestLevel()
        {
            const bool X = true;
            const bool _ = false;

            return new Level
            {
                Walls = new bool[,]
                {
                    { X, X, X, X, X, X, X, X, X, X },
                    { X, _, _, _, _, X, _, _, _, X },
                    { X, _, _, _, X, X, _, _, _, X },
                    { X, X, X, _, _, _, _, X, _, X },
                    { X, _, X, _, _, _, _, X, X, X },
                    { X, _, _, _, X, X, _, _, _, X },
                    { X, _, _, _, X, _, _, _, _, X },
                    { X, X, X, X, X, X, X, X, X, X }
                },
                Players = new List<Player>
                {
                    new Player(1, 1, 1),
                    new Player(2, 8, 1),
                    new Player(3, 1, 6),
                    new Player(4, 8, 6),
                },
                Boxes = new List<Box>
                {
                    new Box(4, 3),
                    new Box(5, 3),
                    new Box(4, 4),
                    new Box(5, 4)
                },
                Bombs = new List<Bomb>()
            };
        }
    }
}

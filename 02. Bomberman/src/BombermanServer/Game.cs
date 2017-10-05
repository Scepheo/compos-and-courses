using BombermanLib;
using System.Linq;
using System.Text;

namespace BombermanServer
{
    internal class Game
    {
        public bool Running { get; private set; } = true;
        public bool Tie { get; private set; } = false;
        public int Winner { get; private set; } = 0;

        public int Players { get; }
        public int Width { get; }
        public int Height { get; }
        public int MaximumTurns { get; }

        private Level _level;

        public Game(int players, int maximumTurns, int width, int height)
        {
            Players = players;
            MaximumTurns = maximumTurns;
            _level = Level.TestLevel();
            Width = _level.Walls.GetLength(0);
            Height = _level.Walls.GetLength(1);
        }

        public void SendInfo(ClientPool clients)
        {
            clients.SendMessages(Message.Turns, MaximumTurns);
            clients.SendMessages(Message.Players, Players);
            clients.SendNumbers();
            clients.SendMessages(Message.Width, Width);
            clients.SendMessages(Message.Height, Height);

            for (var y = 0; y < Height; y++)
            {
                var line = new StringBuilder();

                for (var x = 0; x < Width; x++)
                {
                    if (_level.Walls[x, y])
                    {
                        line.Append('#');
                    }
                    else if (_level.Players.FirstOrDefault(p => p.X == x && p.Y == y) is var player)
                    {
                        line.Append(player.Number);
                    }
                    else if (_level.Boxes.Any(b => b.X == x && b.Y == y))
                    {
                        line.Append('X');
                    }
                    else
                    {
                        line.Append(' ');
                    }
                }

                clients.SendMessages(line.ToString());
            }
        }

        public string[] Step(string[] messages)
        {
            // TODO
            return new string[0];
        }
    }
}
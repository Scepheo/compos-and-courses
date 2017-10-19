using System.Collections.Generic;
using BombermanLib;
using System.Linq;
using System.Text;

namespace BombermanServer
{
    internal class Game
    {
        public bool Running { get; private set; } = true;
        public bool Tie { get; private set; }
        public int Winner { get; private set; }

        public int Players { get; }
        public int Width { get; }
        public int Height { get; }
        public int MaximumTurns { get; }

        private readonly Level _level;
        private int _turnsLeft;

        public Game(int players, int maximumTurns, int width, int height)
        {
            Players = players;
            MaximumTurns = maximumTurns;
            _turnsLeft = maximumTurns;
            _level = Level.Generate(width, height);
            Width = width;
            Height = height;
        }

        public IEnumerable<string> GetLevelText()
        {
            var line = new StringBuilder();

            for (var y = 0; y < Height; y++)
            {
                line.Clear();

                for (var x = 0; x < Width; x++)
                {
                    var position = new Vector(x, y);

                    if (_level.Walls[x, y])
                    {
                        line.Append('#');
                    }
                    else if (_level.Players.FirstOrDefault(p => p.Position == position) is Player player)
                    {
                        line.Append(player.Number);
                    }
                    else if (_level.Boxes.Any(b => b.Position == position))
                    {
                        line.Append('X');
                    }
                    else if (_level.Bombs.Any(b => b.Position == position))
                    {
                        line.Append('o');
                    }
                    else
                    {
                        line.Append(' ');
                    }
                }

                yield return line.ToString();
            }
        }

        public void SendInfo(ClientPool clients)
        {
            clients.SendMessages(Message.Turns, MaximumTurns);
            clients.SendMessages(Message.Players, Players);
            clients.SendNumbers();
            clients.SendMessages(Message.Width, Width);
            clients.SendMessages(Message.Height, Height);

            foreach (var line in GetLevelText())
            {
                clients.SendMessages(line);
            }
        }

        public string[] Step(string[] messages)
        {
            var results = _level.Step(messages);

            for (var i = 0; i < results.Length; i++)
            {
                results[i] = $"{i + 1} {results[i]}";
            }

            _turnsLeft--;
            var alivePlayers = _level.Players.Count(player => !player.Dead);

            if (_turnsLeft == 0 || alivePlayers == 0)
            {
                Running = false;
                Tie = true;
            }
            else if (alivePlayers == 1)
            {
                Running = false;
                Tie = false;
                Winner = _level.Players.First(player => !player.Dead).Number;
            }

            return results;
        }
    }
}

﻿using BombermanLib;
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
                    var position = new Vector(x, y);

                    if (_level.Walls[x, y])
                    {
                        line.Append('#');
                    }
                    else if (_level.Players.FirstOrDefault(p => p.Position == position) is var player)
                    {
                        line.Append(player.Number);
                    }
                    else if (_level.Boxes.Any(b => b.Position == position))
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
            var results = _level.Step(messages);

            for (var i = 0; i < results.Length; i++)
            {
                results[i] = $"{i + 1} {results[i]}";
            }

            return results;
        }
    }
}
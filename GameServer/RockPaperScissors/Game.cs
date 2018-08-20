using GameServer;
using System.Collections.Generic;
using System.Linq;

namespace RockPaperScissors
{
    internal class Game : IGame
    {
        public bool IsDone { get; private set; }

        private string _winner;

        private readonly Dictionary<string, bool> _connected
            = new Dictionary<string, bool>();

        public ICommand[] Initialize(string[] players)
        {
            foreach (var player in players)
            {
                _connected[player] = true;
            }

            return new ICommand[] { new GlobalCommand("START") };
        }

        public ICommand[] Update(PlayerResponse[] responses)
        {
            var connectedCount
                = _connected.Values.Count(connected => connected);

            if (connectedCount == 0)
            {
                IsDone = true;
                _winner = null;
                return new ICommand[] { new GlobalCommand("END") };
            }

            if (connectedCount == 1)
            {
                _winner = _connected.Single(pair => pair.Value).Key;
                return new ICommand[] { new GlobalCommand("END") };
            }

            var left = responses[0];
            var leftChoice = left.Response.ToUpperInvariant();
            var right = responses[1];
            var rightChoice = right.Response.ToUpperInvariant();

            if (LeftWins(leftChoice, rightChoice))
            {
                IsDone = true;
                _winner = left.Player;
                return new ICommand[] { new GlobalCommand("END") };
            }

            if (LeftWins(rightChoice, leftChoice))
            {
                IsDone = true;
                _winner = right.Player;
                return new ICommand[] { new GlobalCommand("END") };
            }

            return new ICommand[] { new GlobalCommand("AGAIN") };
        }

        public ICommand[] Complete()
        {
            return _winner == null
                ? new ICommand[] { new GlobalCommand("TIE") }
                : new ICommand[] { new GlobalCommand($"WINNER: {_winner}") };
        }

        public GameResults GetResults()
        {
            return new GameResults
            {
                Winner = _winner
            };
        }

        public void PlayerDisconnected(string playerName)
        {
            _connected[playerName] = false;
        }

        private static bool LeftWins(string left, string right)
        {
            if (IsValid(left) && !IsValid(right))
            {
                return true;
            }

            switch (left)
            {
                case "ROCK":
                    return right == "SCISSORS";
                case "PAPER":
                    return right == "ROCK";
                case "SCISSORS":
                    return right == "PAPER";
                default:
                    return false;
            }
        }

        private static bool IsValid(string upper)
        {
            return upper == "ROCK" || upper == "PAPER" || upper == "SCISSORS";
        }
    }
}

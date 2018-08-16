namespace GameServer.UnitTests.Util
{
    internal class RockPaperScissors : IGameLogic
    {
        public bool IsDone { get; private set; }

        private string _winner;

        public ICommand[] Initialize(string[] players)
        {
            return new ICommand[] { new GlobalCommand("START") };
        }

        public ICommand[] Update(PlayerResponse[] responses)
        {
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
            return new ICommand[] { new GlobalCommand($"WINNER: {_winner}") };
        }

        public GameResults GetResults()
        {
            return new GameResults
            {
                Winner = _winner
            };
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

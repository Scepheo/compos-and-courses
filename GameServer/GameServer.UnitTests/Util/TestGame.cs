using System;

namespace GameServer.UnitTests.Util
{
    internal class TestGame : IGameLogic
    {
        public string[] Players { get; private set; }

        public ICommand[] InitialCommands { get; set; }

        public ICommand[] UpdateCommands { get; set; }

        public ICommand[] CompleteCommands { get; set; }

        public bool IsDone { get; set; }

        public GameResults Results { get; set; }

        public ICommand[] Initialize(string[] players)
        {
            Players = players;
            return InitialCommands ?? Array.Empty<ICommand>();
        }

        public ICommand[] Update(PlayerResponse[] responses)
        {
            return UpdateCommands ?? Array.Empty<ICommand>();
        }

        public ICommand[] Complete()
        {
            return CompleteCommands ?? Array.Empty<ICommand>();
        }

        public GameResults GetResults()
        {
            return Results;
        }
    }
}

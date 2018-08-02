using System;

namespace GameServer.UnitTests.Util
{
    internal class TestGame : IGameLogic
    {
        public string[] Players { get; private set; }

        public PlayerCommand[] InitialCommands { get; set; }

        public PlayerCommand[] UpdateCommands { get; set; }

        public PlayerCommand[] CompleteCommands { get; set; }

        public bool IsDone { get; set; }

        public GameResults Results { get; set; }

        public PlayerCommand[] Initialize(string[] players)
        {
            Players = players;
            return InitialCommands ?? Array.Empty<PlayerCommand>();
        }

        public PlayerCommand[] Update(PlayerCommand[] commands)
        {
            return UpdateCommands ?? Array.Empty<PlayerCommand>();
        }

        public PlayerCommand[] Complete()
        {
            return CompleteCommands ?? Array.Empty<PlayerCommand>();
        }

        public GameResults GetResults()
        {
            return Results;
        }
    }
}

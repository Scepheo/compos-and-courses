namespace GameServer.UnitTests.Util
{
    internal class TestGame : IGameLogic
    {
        public string[] InitialCommands { get; set; } = new string[0];

        public string[] UpdateCommands { get; set; } = new string[0];

        public string[] FinalCommands { get; set; } = new string[0];

        public bool IsDone { get; set; }

        public GameResults Results = new GameResults();

        public string[] GetInitialCommands() => InitialCommands;

        public string[] GetUpdateCommands(string[] commands) => UpdateCommands;

        public string[] GetFinalCommands() => FinalCommands;

        public GameResults GetResults() => Results;
    }
}

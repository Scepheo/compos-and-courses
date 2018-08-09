using System;

namespace GameServer
{
    /// <summary>
    /// Configuration of the server
    /// </summary>
    public class Config
    {
        /// <summary>
        /// Port on which the server should listen. Set to 0 to let the lobby
        /// automatically select a free port (the actual port in used can be
        /// retrieved with <see cref="Lobby.Port"/> after starting it).
        /// </summary>
        public int ServerPort { get; set; }

        /// <summary>
        /// Interval between polls while clients are in the lobby, to check
        /// whether they are still connected
        /// </summary>
        public TimeSpan LobbyPollInterval { get; set; }

        /// <summary>
        /// Number of clients required to start a game
        /// </summary>
        public int PlayerCount { get; set; }

        /// <summary>
        /// Factory function to get a new instance of the game logic (i.e. start
        /// a new game)
        /// </summary>
        public Func<IGameLogic> GameLogicFactory { get; set; }
    }
}

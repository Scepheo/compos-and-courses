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
        /// The game factory that will be used to instantiate new games
        /// </summary>
        public IGameFactory GameFactory { get; set; }
    }
}

namespace GameServer
{
    /// <summary>
    /// A command and the player that it will be send to/has been received from
    /// </summary>
    public class PlayerCommand
    {
        /// <summary>
        /// The player who send/wil receive the command
        /// </summary>
        public string Player { get; }

        /// <summary>
        /// The command
        /// </summary>
        public string Command { get; }

        /// <summary>
        /// Instantiates a new command for a given player
        /// </summary>
        /// <param name="player">
        /// The player who has send/will receive the command
        /// </param>
        /// <param name="command">
        /// The command to send to/received from the player
        /// </param>
        public PlayerCommand(string player, string command)
        {
            Player = player;
            Command = command;
        }
    }
}

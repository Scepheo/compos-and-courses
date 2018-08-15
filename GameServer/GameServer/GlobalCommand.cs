namespace GameServer
{
    /// <summary>
    /// A command that will be sent to all players
    /// </summary>
    public class GlobalCommand : ICommand
    {
        /// <summary>
        /// The actual command
        /// </summary>
        public string Command { get; }

        /// <summary>
        /// Instantiates a new global command
        /// </summary>
        /// <param name="command">
        /// The command to send to/received from the player
        /// </param>
        public GlobalCommand(string command)
        {
            Command = command;
        }

        /// <summary>
        /// Indicates whether the given player should receive this command. This
        /// is always true for a global command.
        /// </summary>
        /// <param name="playerName">The player name</param>
        /// <returns>true</returns>
        public bool IsForPlayer(string playerName)
        {
            return true;
        }
    }
}

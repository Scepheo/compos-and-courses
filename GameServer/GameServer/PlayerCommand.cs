namespace GameServer
{
    /// <summary>
    /// A command for a specific player
    /// </summary>
    public class PlayerCommand : ICommand
    {
        /// <summary>
        /// The name of the player form whom the command is intended
        /// </summary>
        private readonly string _player;

        /// <summary>
        /// The actual command
        /// </summary>
        public string Command { get; }

        /// <summary>
        /// Instantiates a new command for a given player
        /// </summary>
        /// <param name="player">
        /// The player who will receive the command
        /// </param>
        /// <param name="command">
        /// The command to send to the player
        /// </param>
        public PlayerCommand(string player, string command)
        {
            _player = player;
            Command = command;
        }

        /// <summary>
        /// Indicates whether the given player is the intended recipient for
        /// this command
        /// </summary>
        /// <param name="playerName">The name of player</param>
        /// <returns>
        /// True if the player is the intended recipient, otherwise false
        /// </returns>
        public bool IsForPlayer(string playerName)
        {
            return string.Equals(playerName, _player);
        }
    }
}

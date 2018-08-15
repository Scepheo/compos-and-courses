namespace GameServer
{
    /// <summary>
    /// Interface for commands to send out
    /// </summary>
    public interface ICommand
    {
        /// <summary>
        /// Indicates whether the command should be sent to the given player
        /// </summary>
        /// <param name="playerName">The name of the player</param>
        /// <returns>
        /// True if the command should be sent to this player, false otherwise
        /// </returns>
        bool IsForPlayer(string playerName);

        /// <summary>
        /// The actual command to send
        /// </summary>
        string Command { get; }
    }
}

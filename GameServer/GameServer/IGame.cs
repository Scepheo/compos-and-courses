namespace GameServer
{
    /// <summary>
    /// Interface that needs to be implemented by any game logic in order to be
    /// able to run it on the server
    /// </summary>
    public interface IGame
    {
        /// <summary>
        /// Returns the initial information send to the clients
        /// </summary>
        /// <param name="players">The names of the players</param>
        /// <returns>
        /// Commands for the clients to build the initial game state
        /// </returns>
        ICommand[] Initialize(string[] players);

        /// <summary>
        /// Uses the commands submitted by the players to determine the new game
        /// state and returns information about the new state
        /// </summary>
        /// <param name="responses">Responses received from the players</param>
        /// <returns>
        /// Commands for the clients to update their game state
        /// </returns>
        ICommand[] Update(PlayerResponse[] responses);

        /// <summary>
        /// Returns the final information send to the clients
        /// </summary>
        /// <returns>
        /// Commands for the clients to determine the final game state
        /// </returns>
        ICommand[] Complete();

        /// <summary>
        /// Indicates whether the game is completed
        /// </summary>
        bool IsDone { get; }

        /// <summary>
        /// Gets the results of the game
        /// </summary>
        /// <returns>The results of the game</returns>
        GameResults GetResults();

        /// <summary>
        /// Used to indicate to the game that the given player has disconnect
        /// </summary>
        /// <param name="playerName">
        /// The name of the player that has disconnected
        /// </param>
        void PlayerDisconnected(string playerName);
    }
}

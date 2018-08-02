namespace GameServer
{
    /// <summary>
    /// Interface that needs to be implemented by any game logic in order to be
    /// able to run it on the server
    /// </summary>
    public interface IGameLogic
    {
        /// <summary>
        /// Returns the initial information send to the clients
        /// </summary>
        /// <returns>
        /// Commands for the clients to build the initial game state
        /// </returns>
        string[] GetInitialCommands();

        /// <summary>
        /// Uses the commands submitted by the players to determine the new game
        /// state and returns information about the new state
        /// </summary>
        /// <param name="commands">Commands submitted by the players</param>
        /// <returns>Commands for the clients to update their game state</returns>
        string[] GetUpdateCommands(string[] commands);

        /// <summary>
        /// Returns the final information send to the clients
        /// </summary>
        /// <returns>
        /// Commands for the clients to determine the final game state
        /// </returns>
        string[] GetFinalCommands();

        /// <summary>
        /// Indicates whether the game is completed
        /// </summary>
        bool IsDone { get; }

        /// <summary>
        /// Gets the results of the game
        /// </summary>
        /// <returns>The results of the game</returns>
        GameResults GetResults();
    }
}

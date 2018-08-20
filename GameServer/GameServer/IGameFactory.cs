namespace GameServer
{
    /// <summary>
    /// Interface that needs to be implemented by a game factory. Needs to be
    /// able to instantiate new games and provide metadata about the game.
    /// </summary>
    public interface IGameFactory
    {
        /// <summary>
        /// Creates a new game instance
        /// </summary>
        /// <returns>A new game</returns>
        IGame CreateGame();

        /// <summary>
        /// The number of players needed for the game
        /// </summary>
        int PlayerCount { get; }
    }
}

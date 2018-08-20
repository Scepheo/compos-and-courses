namespace GameServer
{
    /// <summary>
    /// A response and the player that it has been received from
    /// </summary>
    public class PlayerResponse
    {
        /// <summary>
        /// The player who sent the response
        /// </summary>
        public string Player { get; }

        /// <summary>
        /// The response
        /// </summary>
        public string Response { get; }

        /// <summary>
        /// Instantiates a new response from a given player
        /// </summary>
        /// <param name="player">
        /// The player who has sent the response
        /// </param>
        /// <param name="response">
        /// The response to received from the player
        /// </param>
        public PlayerResponse(string player, string response)
        {
            Player = player;
            Response = response;
        }
    }
}

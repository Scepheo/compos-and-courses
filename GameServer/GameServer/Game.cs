using System;
using System.Threading;
using System.Threading.Tasks;

namespace GameServer
{
    /// <summary>
    /// Class representing a running game
    /// </summary>
    public class Game
    {
        private readonly ClientCollection _players;
        private readonly IGameLogic _gameLogic;

        /// <summary>
        /// Called when the game is done
        /// </summary>
        public event EventHandler<GameResults> OnGameEnd;

        /// <summary>
        /// Creates a new game with the given players and logic
        /// </summary>
        /// <param name="players">The clients in the game</param>
        /// <param name="gameLogic">The game logic to be used</param>
        internal Game(ClientCollection players, IGameLogic gameLogic)
        {
            _players = players;
            _gameLogic = gameLogic;
        }

        /// <summary>
        /// Starts running the game, either until it is done or the token is
        /// cancelled
        /// </summary>
        /// <param name="cancellationToken">
        /// The token that can be used to cancel the task
        /// </param>
        /// <returns>
        /// A task representing the asynchronous running of the game
        /// </returns>
        public async Task Start(CancellationToken cancellationToken)
        {
            var initialCommands = _gameLogic.Initialize(_players.Names);
            await _players.Send(initialCommands);

            while (!cancellationToken.IsCancellationRequested
                && !_gameLogic.IsDone)
            {
                var commands = await _players.Receive();
                var results = _gameLogic.Update(commands);
                await _players.Send(results);
            }

            if (_gameLogic.IsDone)
            {
                var finalCommands = _gameLogic.Complete();
                await _players.Send(finalCommands);
                var results = _gameLogic.GetResults();
                OnGameEnd?.Invoke(this, results);
            }
        }
    }
}

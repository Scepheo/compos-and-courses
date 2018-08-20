using System;
using System.Threading;

namespace GameServer
{
    /// <summary>
    /// Class representing a running game
    /// </summary>
    public class Game
    {
        private readonly ClientCollection _players;
        private readonly IGame _gameLogic;

        private Thread _gameThread;
        private bool _running;

        /// <summary>
        /// Called when the game is done
        /// </summary>
        public event EventHandler<GameResults> OnGameEnd;

        /// <summary>
        /// Creates a new game with the given players and logic
        /// </summary>
        /// <param name="players">The clients in the game</param>
        /// <param name="gameLogic">The game logic to be used</param>
        internal Game(ClientCollection players, IGame gameLogic)
        {
            _players = players;
            _gameLogic = gameLogic;
        }

        /// <summary>
        /// Starts running the game, either until it is done or the token is
        /// cancelled
        /// </summary>
        public void Start()
        {
            _gameThread = new Thread(Run);
            _running = true;
            _gameThread.Start();
        }

        /// <summary>
        /// Stops running the game
        /// </summary>
        public void Stop()
        {
            _running = false;
            _gameThread.Join();
            _gameThread = null;
        }

        private void Run()
        {
#if DEBUG
            // Give the thread a name, for ease of debugging
            var names = string.Join(", ", _players.Names);
            Thread.CurrentThread.Name = $"Game ({names})";
#endif

            var initialCommands = _gameLogic.Initialize(_players.Names);
            _players.Send(initialCommands);

            while (_running && !_gameLogic.IsDone)
            {
                var commands = _players.Receive();
                var results = _gameLogic.Update(commands);
                _players.Send(results);
            }

            if (_gameLogic.IsDone)
            {
                var finalCommands = _gameLogic.Complete();
                _players.Send(finalCommands);
                var results = _gameLogic.GetResults();
                OnGameEnd?.Invoke(this, results);
            }
        }
    }
}

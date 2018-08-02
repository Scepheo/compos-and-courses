using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace GameServer
{
    /// <summary>
    /// A lobby that accepts and holds clients until there are enough to start a
    /// game
    /// </summary>
    public class Lobby
    {
        private readonly TcpListener _tcpListener;
        private readonly List<Client> _clients;
        private readonly TimeSpan _pollInterval;
        private readonly int _playerCount;
        private readonly Func<IGameLogic> _gameLogicFactory;

        /// <summary>
        /// Called when a new game has been created and is ready to be started
        /// </summary>
        public event EventHandler<Game> OnGameCreated; 

        /// <summary>
        /// Creates a new lobby
        /// </summary>
        /// <param name="config">The configuration of the lobby</param>
        public Lobby(Config config)
        {
            _tcpListener = new TcpListener(IPAddress.Any, config.ServerPort);
            _clients = new List<Client>();
            _pollInterval = config.LobbyPollInterval;
            _playerCount = config.PlayerCount;
            _gameLogicFactory = config.GameLogicFactory;
        }

        /// <summary>
        /// Starts the lobby listening for clients
        /// </summary>
        /// <param name="cancellationToken">
        /// Token that can be used to cancel the task
        /// </param>
        public async Task Start(CancellationToken cancellationToken)
        {
            await Task.WhenAll(
                Poll(cancellationToken),
                Listen(cancellationToken));
        }

        private async Task Poll(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                foreach (var client in _clients)
                {
                    client.Poll();
                }

                await Task.Delay(_pollInterval, cancellationToken);
            }
        }

        private async Task Listen(CancellationToken cancellationToken)
        {
            _tcpListener.Start();

            while (!cancellationToken.IsCancellationRequested)
            {
                var tcpClient = await _tcpListener.AcceptTcpClientAsync();
                AcceptClient(tcpClient);
            }
        }

        private void AcceptClient(TcpClient tcpClient)
        {
            var client = new Client(tcpClient);
            _clients.Add(client);
            client.OnDisconnect += ClientDisconnectHandler;
            TryStartGame();
        }

        private void ClientDisconnectHandler(object sender, Client client)
        {
            _clients.Remove(client);
        }

        private void TryStartGame()
        {
            if (_clients.Count < _playerCount)
            {
                return;
            }

            var players = _clients.Take(_playerCount).ToArray();
            _clients.RemoveRange(0, _playerCount);

            foreach (var player in players)
            {
                player.OnDisconnect -= ClientDisconnectHandler;
            }

            var collection = new ClientCollection(players);
            var game = new Game(collection, _gameLogicFactory());
            OnGameCreated?.Invoke(this, game);
        }
    }
}

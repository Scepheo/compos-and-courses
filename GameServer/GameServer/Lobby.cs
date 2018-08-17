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

        private Thread _listenThread;
        private bool _listening;

        private readonly ReaderWriterLockSlim _clientLock =
            new ReaderWriterLockSlim();

        private readonly TaskCompletionSource<int> _portTaskCompletionSource =
            new TaskCompletionSource<int>();

        /// <summary>
        /// Called when a new game has been created and is ready to be started
        /// </summary>
        public event EventHandler<Game> OnGameCreated;

        /// <summary>
        /// The port the lobby is listening on. Differs from the configured port
        /// when that is set to 0, as it will then choose its own port.
        /// </summary>
        public Task<int> Port => _portTaskCompletionSource.Task;

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

            if (config.ServerPort != 0)
            {
                _portTaskCompletionSource.SetResult(config.ServerPort);
            }
        }

        /// <summary>
        /// Starts the lobby listening for clients. Continuously accepts
        /// clients, initializing them and tries to start a new game whenever
        /// there are enough players.
        /// </summary>
        public void Start()
        {
            _listenThread = new Thread(Listen);
            _listening = true;
            _listenThread.Start();
        }

        /// <summary>
        /// Stops the lobby listening to clients.
        /// </summary>
        public void Stop()
        {
            _listening = false;
            _listenThread.Join(_pollInterval);
            _listenThread = null;
        }

        private void PollAllClients()
        {
            _clientLock.EnterReadLock();
            var clients = _clients.ToArray();
            _clientLock.ExitReadLock();

            foreach (var client in clients)
            {
                client.Poll();
            }
        }

        private void Listen()
        {
#if DEBUG
            // Give the thread a name, for ease of debugging
            Thread.CurrentThread.Name = "Lobby listen";
#endif

            _tcpListener.Start();

            if (_tcpListener.LocalEndpoint is IPEndPoint ipEndPoint)
            {
                _portTaskCompletionSource.SetResult(ipEndPoint.Port);
            }

            while (_listening)
            {
                if (_tcpListener.Pending())
                {
                    var tcpClient = _tcpListener.AcceptTcpClient();
                    ThreadPool.QueueUserWorkItem(
                        AcceptClient,
                        tcpClient,
                        false);
                }
                else
                {
                    Thread.Sleep(_pollInterval);
                }
            }
        }

        private void AcceptClient(TcpClient tcpClient)
        {
            var client = new Client(tcpClient);
            client.OnDisconnect += ClientDisconnectHandler;
            InitializeClient(client);
            TryStartGame();
        }

        private void ClientDisconnectHandler(object sender, Client client)
        {
            _clientLock.EnterWriteLock();
            _clients.Remove(client);
            _clientLock.ExitWriteLock();

            client.Dispose();
        }

        private void TryStartGame()
        {
            PollAllClients();

            _clientLock.EnterUpgradeableReadLock();

            if (_clients.Count >= _playerCount)
            {
                var players = _clients.Take(_playerCount).ToArray();

                _clientLock.EnterWriteLock();
                _clients.RemoveRange(0, _playerCount);
                _clientLock.ExitWriteLock();

                foreach (var player in players)
                {
                    player.OnDisconnect -= ClientDisconnectHandler;
                }

                var collection = new ClientCollection(players);
                var game = new Game(collection, _gameLogicFactory());
                OnGameCreated?.Invoke(this, game);
            }

            _clientLock.ExitUpgradeableReadLock();
        }

        private void InitializeClient(Client client)
        {
            client.Send(new[] { "LOBBY" });
            var nameResponse = client.Receive();
            client.Name = nameResponse.Response;

#if DEBUG
            // Give the thread a name, for ease of debugging
            Thread.CurrentThread.Name = $"Accept client ({client.Name})";
#endif

            _clientLock.EnterWriteLock();
            _clients.Add(client);
            _clientLock.ExitWriteLock();
        }
    }
}

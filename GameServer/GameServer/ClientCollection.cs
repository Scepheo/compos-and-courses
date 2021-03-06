﻿using System;
using System.Linq;
using System.Threading.Tasks;

namespace GameServer
{
    /// <summary>
    /// A collection of clients
    /// </summary>
    internal class ClientCollection : IDisposable
    {
        private readonly Client[] _clients;

        /// <summary>
        /// Array containing all names of the clients in the collection
        /// </summary>
        public string[] Names =>
            _clients.Select(client => client.Name).ToArray();

        /// <summary>
        /// Instantiates a new collection containing the given clients
        /// </summary>
        /// <param name="clients">The clients to go in the collection</param>
        public ClientCollection(Client[] clients)
        {
            _clients = clients;
        }

        /// <summary>
        /// Receives a string mesage from each client in the collection
        /// </summary>
        /// <returns>An array containing the messages received</returns>
        public PlayerResponse[] Receive()
        {
            var responses = new PlayerResponse[_clients.Length];
            Parallel.For(
                0,
                _clients.Length,
                i => responses[i] = _clients[i].Receive());
            return responses;
        }

        /// <summary>
        /// Sends one or more string messages to all clients
        /// </summary>
        /// <param name="commands">The messages to send</param>
        public void Send(ICommand[] commands)
        {
            Parallel.ForEach(
                _clients,
                client => SendToClient(client, commands));
        }

        private static void SendToClient(Client client, ICommand[] commands)
        {
            var clientCommands = commands
                .Where(command => command.IsForPlayer(client.Name))
                .Select(command => command.Command)
                .ToArray();

            client.Send(clientCommands);
        }

        /// <summary>
        /// Releases all resources used by the client collection
        /// </summary>
        public void Dispose()
        {
            foreach (var client in _clients)
            {
                client.Dispose();
            }
        }
    }
}

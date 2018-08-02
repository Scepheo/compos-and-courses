using System;
using System.Collections.Generic;
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

        public ClientCollection(Client[] clients)
        {
            _clients = clients;
        }

        /// <summary>
        /// Receives a string mesage from each client in the collection
        /// </summary>
        /// <returns>An array containing the messages received</returns>
        public async Task<string[]> Receive()
        {
            var tasks = _clients.Select(client => client.Receive());
            return await Task.WhenAll(tasks);
        }

        /// <summary>
        /// Sends one or more string messages to all clients
        /// </summary>
        /// <param name="messages">The messages to send</param>
        public async Task Send(IEnumerable<string> messages)
        {
            var tasks = _clients.Select(client => client.Send(messages));
            await Task.WhenAll(tasks);
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

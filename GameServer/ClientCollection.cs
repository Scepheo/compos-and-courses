using System;
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
        public string[] Names => _clients.Select(client => client.Name).ToArray();

        public ClientCollection(Client[] clients)
        {
            _clients = clients;
        }

        /// <summary>
        /// Receives a string mesage from each client in the collection
        /// </summary>
        /// <returns>An array containing the messages received</returns>
        public async Task<PlayerCommand[]> Receive()
        {
            var tasks = _clients.Select(client => client.Receive());
            return await Task.WhenAll(tasks);
        }

        /// <summary>
        /// Sends one or more string messages to all clients
        /// </summary>
        /// <param name="commands">The messages to send</param>
        public async Task Send(PlayerCommand[] commands)
        {
            var tasks = _clients.Select(
                async client =>
                {
                    var clientCommands = commands
                        .Where(command => command.Player == client.Name)
                        .ToArray();

                    await client.Send(clientCommands);
                });

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

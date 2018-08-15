using System;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace GameServer
{
    /// <summary>
    /// Convenience wrapper around a TcpClient
    /// </summary>
    internal class Client : IDisposable
    {
        /// <summary>
        /// Name of the client
        /// </summary>
        public string Name { get; set; }

        private readonly TcpClient _tcpClient;
        private readonly NetworkStream _stream;
        private readonly StreamReader _reader;
        private readonly StreamWriter _writer;

        /// <summary>
        /// Called when the client has disconnected
        /// </summary>
        public event EventHandler<Client> OnDisconnect;

        /// <summary>
        /// Create a wrapper around the TcpClient
        /// </summary>
        /// <param name="tcpClient">The client to wrap</param>
        public Client(TcpClient tcpClient)
        {
            _tcpClient = tcpClient;
            _stream = tcpClient.GetStream();
            _reader = new StreamReader(_stream);
            _writer = new StreamWriter(_stream);
        }

        /// <summary>
        /// Receives a string mesage from the client
        /// </summary>
        /// <returns>The received message</returns>
        public async Task<PlayerResponse> Receive()
        {
            try
            {
                var response = await _reader.ReadLineAsync();
                return new PlayerResponse(Name, response);
            }
            catch (IOException)
            {
                OnDisconnect?.Invoke(this, this);
                return null;
            }
        }

        /// <summary>
        /// Sends one or more string messages to the client
        /// </summary>
        /// <param name="commands">The messages to send</param>
        public async Task Send(string[] commands)
        {
            try
            {
                foreach (var command in commands)
                {
                    await _writer.WriteLineAsync(command);
                    await _writer.FlushAsync();
                }
            }
            catch (IOException)
            {
                OnDisconnect?.Invoke(this, this);
            }
        }

        /// <summary>
        /// Polls the socket to see if the connection is still open, and calls
        /// <see cref="OnDisconnect"/> if the connection is no longer active
        /// </summary>
        public void Poll()
        {
            var canRead = _tcpClient.Client.Poll(1, SelectMode.SelectRead);
            var canWrite = _tcpClient.Client.Poll(1, SelectMode.SelectWrite);

            if (!canRead || !canWrite)
            {
                OnDisconnect?.Invoke(this, this);
            }
        }

        /// <summary>
        /// Releases all resources used by the client
        /// </summary>
        public void Dispose()
        {
            _writer.Dispose();
            _reader.Dispose();
            _stream.Dispose();
            _tcpClient.Dispose();
        }
    }
}

using System;
using System.IO;
using System.Net.Sockets;

namespace BombermanClient
{
    internal class Server : IDisposable
    {
        private readonly TcpClient _tcpClient;
        private readonly TextReader _reader;
        private readonly TextWriter _writer;

        public bool Connected { get; private set; } = true;

        public Server()
        {
            _tcpClient = new TcpClient();
            _tcpClient.Connect(Config.ServerAddress, Config.ServerPort);
            var stream = _tcpClient.GetStream();
            _reader = new StreamReader(stream);
            _writer = new StreamWriter(stream);
        }

        public void SendMessage(string message)
        {
            if (!Connected) return;

            try
            {
                _writer.WriteLine(message);
                _writer.Flush();
            }
            catch (Exception)
            {
                Connected = false;
            }
        }

        public string ReceiveMessage()
        {
            if (!Connected) return string.Empty;

            try
            {
                return _reader.ReadLine();
            }
            catch (Exception)
            {
                Connected = false;
                return string.Empty;
            }
        }

        public void Dispose()
        {
            _tcpClient.Dispose();
            _reader.Dispose();
            _writer.Dispose();
        }
    }
}
using System;
using System.IO;
using System.Net.Sockets;

namespace BombermanServer
{
    internal class Client : IDisposable
    {
        public int Index { get; }

        private TcpClient _tcpClient;
        private TextReader _reader;
        private TextWriter _writer;

        private bool _connected;

        public Client(int index, TcpClient tcpClient)
        {
            Index = index;
            _tcpClient = tcpClient;
            var stream = _tcpClient.GetStream();
            _reader = new StreamReader(stream);
            _writer = new StreamWriter(stream);
            _connected = true;
        }

        public void SendMessage(string message)
        {
            if (!_connected)
            {
                return;
            }

            try
            {
                _writer.WriteLine(message);
                _writer.Flush();
            }
            catch (SocketException)
            {
                _connected = false;
            }
        }

        public string ReceiveMessage()
        {
            if (!_connected)
            {
                return string.Empty;
            }

            try
            {
                return _reader.ReadLine();
            }
            catch (SocketException)
            {
                _connected = false;
                return string.Empty;
            }
        }

        public void Dispose()
        {
            _tcpClient.Dispose();
        }
    }
}
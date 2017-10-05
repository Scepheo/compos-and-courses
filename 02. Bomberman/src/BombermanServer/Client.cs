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

        public Client(int index, TcpClient tcpClient)
        {
            Index = index;
            _tcpClient = tcpClient;
            var stream = _tcpClient.GetStream();
            _reader = new StreamReader(stream);
            _writer = new StreamWriter(stream);
        }

        public void SendMessage(string message)
        {
            _writer.WriteLine(message);
            _writer.Flush();
        }

        public string ReceiveMessage()
        {
            return _reader.ReadLine();
        }

        public void Dispose()
        {
            _tcpClient.Dispose();
        }
    }
}
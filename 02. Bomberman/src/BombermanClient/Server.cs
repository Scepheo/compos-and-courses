using System.IO;
using System.Net.Sockets;

namespace BombermanClient
{
    internal class Server
    {
        private TcpClient _tcpClient;
        private TextReader _reader;
        private TextWriter _writer;

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
            _writer.WriteLine(message);
            _writer.Flush();
        }

        public string ReceiveMessage()
        {
            return _reader.ReadLine();
        }
    }
}
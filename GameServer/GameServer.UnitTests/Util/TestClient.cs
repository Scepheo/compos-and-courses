using System;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace GameServer.UnitTests.Util
{
    internal class TestClient : IDisposable
    {
        private readonly TcpClient _tcpClient;
        private readonly NetworkStream _stream;
        private readonly StreamReader _reader;
        private readonly StreamWriter _writer;

        public TestClient(int port)
        {
            _tcpClient = new TcpClient();
            _tcpClient.Connect(IPAddress.Loopback, port);
            _stream = _tcpClient.GetStream();
            _reader = new StreamReader(_stream);
            _writer = new StreamWriter(_stream);
        }

        public void Send(string command)
        {
            _writer.WriteLine(command);
            _writer.Flush();
        }

        public string Receive()
        {
            return _reader.ReadLine();
        }

        public void Dispose()
        {
            _writer.Close();
            _writer.Dispose();
            _reader.Close();
            _reader.Dispose();
            _stream.Close();
            _stream.Dispose();
            _tcpClient.Close();
            _tcpClient.Dispose();
        }
    }
}

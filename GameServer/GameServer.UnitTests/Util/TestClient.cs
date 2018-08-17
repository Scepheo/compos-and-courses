using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

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

        public async Task Send(string command)
        {
            await _writer.WriteLineAsync(command);
            await _writer.FlushAsync();
        }

        public async Task<string> Receive()
        {
            return await _reader.ReadLineAsync();
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

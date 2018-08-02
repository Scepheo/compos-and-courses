﻿using System;
using System.Collections.Generic;
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

        public void Send(IEnumerable<string> commands)
        {
            foreach (var command in commands)
            {
                _writer.WriteLine(command);
                _writer.Flush();
            }
        }

        public string[] Receive(int count)
        {
            var results = new string[count];

            for (var i = 0; i < count; i++)
            {
                results[i] = _reader.ReadLine();
            }

            return results;
        }

        public void Dispose()
        {
            _writer.Dispose();
            _reader.Dispose();
            _stream.Dispose();
            _tcpClient.Dispose();
        }
    }
}

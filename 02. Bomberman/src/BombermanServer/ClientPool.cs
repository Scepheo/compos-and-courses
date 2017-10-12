using BombermanLib;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace BombermanServer
{
    internal class ClientPool : IDisposable
    {
        private readonly Client[] _clients;

        public ClientPool(int clientCount)
        {
            _clients = new Client[clientCount];
        }

        public void AwaitConnections()
        {
            var tcpListener = new TcpListener(new IPAddress(0x0100007F), 12345);
            tcpListener.Start();

            Parallel.For(
                0,
                _clients.Length,
                index =>
                {
                    {
                        var tcpClient = tcpListener.AcceptTcpClient();
                        _clients[index] = new Client(index, tcpClient);
                        _clients[index].SendMessage(Message.Acknowledge);
                    }
                });

            tcpListener.Stop();
        }

        public void SendMessages(string message)
        {
            foreach (var client in _clients)
            {
                client.SendMessage(message);
            }
        }

        public void SendMessages(string message, object data)
        {
            SendMessages(message + " " + data);
        }

        public void Dispose()
        {
            foreach (var client in _clients)
            {
                client.Dispose();
            }
        }

        public void SendNumbers()
        {
            foreach (var client in _clients)
            {
                client.SendMessage(Message.Number + " " + (client.Index + 1));
            }
        }

        public string[] ReceiveMessages()
        {
            var messages = new string[_clients.Length];
            Parallel.ForEach(_clients, client => messages[client.Index] = client.ReceiveMessage());
            return messages;
        }
    }
}
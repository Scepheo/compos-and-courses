using BombermanLib;
using System;

namespace BombermanClient
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            Config.Parse(args);

            var server = new Server();

            while (true)
            {
                Console.WriteLine(server.ReceiveMessage());
                server.SendMessage(Message.Wait);
            }
        }
    }
}

using BombermanLib;
using System;

namespace BombermanClient
{
    internal static class Program
    {
        private static readonly Random Random = new Random();

        private static readonly string[] Messages =
        {
            Message.Bomb,
            Message.Wait,
            Message.Up,
            Message.Down,
            Message.Left,
            Message.Right
        };

        private static void Main(string[] args)
        {
            Config.Parse(args);

            var server = new Server();
            var done = false;

            while (server.Connected && !done)
            {
                var received = server.ReceiveMessage();
                Console.WriteLine(received);

                var split = received.Split(' ');

                switch (split[0])
                {
                    case Message.Update:
                        var count = int.Parse(split[1]);
                        for (var i = 0; i < count; i++)
                        {
                            Console.WriteLine(server.ReceiveMessage());
                        }
                        break;
                    case Message.End:
                        var result = server.ReceiveMessage();
                        Console.WriteLine(result);

                        if (result == Message.Tie)
                        {
                            Console.WriteLine("It was a tie...");
                        }
                        else
                        {
                            var winner = int.Parse(result.Split(' ')[1]);
                            Console.WriteLine($"Player {winner} won");
                        }

                        Console.WriteLine("Press any key to exit");
                        Console.ReadKey();
                        done = true;
                        break;
                    default:
                        var message = Messages[Random.Next(Messages.Length)];
                        server.SendMessage(message);
                        break;
                }
            }
        }
    }
}

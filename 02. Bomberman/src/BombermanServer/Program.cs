using System;
using BombermanLib;

namespace BombermanServer
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            var game = new Game(4, 100, 31, 23);

            using (var clientPool = new ClientPool(game.Players))
            {
                clientPool.AwaitConnections();

                clientPool.SendMessages(Message.Ready);
                game.SendInfo(clientPool);
                clientPool.SendMessages(Message.Start);

                while (game.Running)
                {
                    var messages = clientPool.ReceiveMessages();
                    var resultMessages = game.Step(messages);

                    if (game.Running)
                    {
                        clientPool.SendMessages(Message.Update, resultMessages.Length);

                        foreach (var resultMessage in resultMessages)
                        {
                            clientPool.SendMessages(resultMessage);
                        }
                    }

                    Console.SetCursorPosition(0, 0);
                    foreach (var line in game.GetLevelText())
                    {
                        Console.WriteLine(line);
                    }
                }

                clientPool.SendMessages(Message.End);

                if (game.Tie)
                {
                    clientPool.SendMessages(Message.Tie);
                }
                else
                {
                    clientPool.SendMessages(Message.Win, game.Winner);
                }
            }
        }
    }
}

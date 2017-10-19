using System;
using System.Net;

namespace BombermanClient
{
    internal static class Config
    {
        public static IPAddress ServerAddress { get; private set; }
        public static ushort ServerPort { get; private set; }

        public static void Parse(string[] args)
        {
            for (var i = 0; i < args.Length; i += 2)
            {
                switch (args[i])
                {
                    case "--ip":
                    case "-i":
                        ServerAddress = IPAddress.Parse(args[i + 1]);
                        break;
                    case "--port":
                    case "-p":
                        ServerPort = ushort.Parse(args[i + 1]);
                        break;
                    default:
                        throw new InvalidOperationException($"Unknown command line parameter {args[i]}");
                }
            }
        }
    }
}

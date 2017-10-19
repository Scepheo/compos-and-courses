using System;

namespace BombermanServer
{
    internal static class Config
    {
        public static ushort Port { get; private set; }
        public static int Width { get; private set; }
        public static int Height { get; private set; }
        public static int Turns { get; private set; }

        public static void Parse(string[] args)
        {
            for (var i = 0; i < args.Length; i += 2)
            {
                switch (args[i])
                {
                    case "--width":
                    case "-w":
                        Width = int.Parse(args[i + 1]);
                        break;
                    case "--height":
                    case "-h":
                        Height = int.Parse(args[i + 1]);
                        break;
                    case "--turns":
                    case "-t":
                        Turns = int.Parse(args[i + 1]);
                        break;
                    case "--port":
                    case "-p":
                        Port = ushort.Parse(args[i + 1]);
                        break;
                    default:
                        throw new InvalidOperationException($"Unknown command line parameter {args[i]}");
                }
            }
        }
    }
}

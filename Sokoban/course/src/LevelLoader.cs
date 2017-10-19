using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Sokoban
{
    public static class LevelLoader
    {
        public static Level Load(string path)
        {
            var text = LoadFile(path);
            var width = text.Max(line => line.Length);
            var height = text.Length;

            var items = new Item[width, height];

            for (var y = 0; y < height; y++)
            {
                var line = text[y];

                for (var x = 0; x < width; x++)
                {
                    var character = x < line.Length ? line[x] : '.';
                    items[x, y] = GetItem(character);
                }
            }

            return new Level(items);
        }

        private static Item GetItem(char character) => Data.CharacterItemMap.TryGetValue(character, out var item)
                ? item
                : throw new ArgumentException($"Invalid item character '{character}'", nameof(character));

        private static string[] LoadFile(string name)
        {
            const string prefix = "Sokoban.levels.";

            var assembly = Assembly.GetExecutingAssembly();

            using (var stream = assembly.GetManifestResourceStream(prefix + name))
            {
                if (stream == null)
                {
                    throw new ArgumentException($"Not a valid file: {name}", nameof(name));
                }

                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd().Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                }
            }
        }
    }
}

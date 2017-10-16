using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;

namespace Sokoban
{
    public static class SpriteSheet
    {
        static SpriteSheet()
        {
            ImageMap = LoadImages();
        }

        public static Image GetImage(Sprite sprite)
        {
            return ImageMap[sprite];
        }

        private static readonly Dictionary<Sprite, Image> ImageMap;

        private static Dictionary<Sprite, Image> LoadImages()
        {
            var map = new Dictionary<Sprite, Image>();

            foreach (var pair in Data.SpriteNameMap)
            {
                var sprite = pair.Key;
                var name = pair.Value;
                var image = LoadImage(name);
                map.Add(sprite, image);
            }

            map.Add(Sprite.Blank, MakeBlankImage());

            return map;
        }

        private static Image MakeBlankImage()
        {
            var image = new Bitmap(Level.TileSize, Level.TileSize);

            using (var graphics = Graphics.FromImage(image))
            {
                graphics.Clear(Color.Transparent);
            }

            return image;
        }

        private static Image LoadImage(string name)
        {
            const string prefix = "Sokoban.img.";
            var assembly = Assembly.GetExecutingAssembly();

            using (var stream = assembly.GetManifestResourceStream(prefix + name))
            {
                if (stream == null)
                {
                    throw new ArgumentException($"Not a valid image: {name}", nameof(name));
                }

                return Image.FromStream(stream);
            }
        }
    }
}

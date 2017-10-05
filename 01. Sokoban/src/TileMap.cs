using System.Drawing;

namespace Sokoban
{
    public class TileMap
    {
        private readonly bool[,] _solid;
        private readonly int _width;
        private readonly int _height;
        private readonly Image _image;

        public TileMap(Item[,] items)
        {
            _width = items.GetLength(0);
            _height = items.GetLength(1);
            var sprites = new Sprite[_width, _height];
            _solid = new bool[_width, _height];

            for (var x = 0; x < _width; x++)
            {
                for (var y = 0; y < _height; y++)
                {
                    sprites[x, y] = GetSprite(items[x, y]);
                    _solid[x, y] = GetSolid(items[x, y]);
                }
            }

            _image = Render(sprites);
        }

        public bool IsSolid(MapVector position) =>
            position.X < 0
            || position.X >= _width
            || position.Y < 0
            || position.Y >= _height
            || _solid[position.X, position.Y];

        public void Draw(Graphics graphics)
        {
            graphics.DrawImage(_image, 0, 0);
        }

        private static Sprite GetSprite(Item item) => item == Item.Wall ? Sprite.Wall : Sprite.Floor;

        private static bool GetSolid(Item item)
        {
            return item == Item.Wall;
        }

        private Image Render(Sprite[,] sprites)
        {
            var image = new Bitmap(_width * Level.TileSize, _height * Level.TileSize);
            var graphics = Graphics.FromImage(image);

            for (var x = 0; x < _width; x++)
            {
                var xTarget = x * Level.TileSize;

                for (var y =0; y < _height; y++)
                {
                    var yTarget = y * Level.TileSize;
                    var sprite = SpriteSheet.GetImage(sprites[x, y]);
                    graphics.DrawImage(sprite, xTarget, yTarget);
                }
            }

            return image;
        }
    }
}

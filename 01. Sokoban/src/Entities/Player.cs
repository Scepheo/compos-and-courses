using System.Drawing;

namespace Sokoban.Entities
{
    public class Player : EntityBase
    {
        public bool Enabled { get; private set; } = true;

        public Player(MapVector position) : base(position) { }

        private Image _image = SpriteSheet.GetImage(Sprite.Player);

        protected override Image Image => _image;

        public void Kill()
        {
            Enabled = false;
            _image = SpriteSheet.GetImage(Sprite.GraveStone);
        }
    }
}

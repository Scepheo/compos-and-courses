using System.Drawing;

namespace Sokoban.Entities
{
    public class Door : EntityBase
    {
        public Door(MapVector position) : base(position)
        {
        }

        protected override Image Image => _image;
        public override bool IsSolid => _isSolid;
        public override bool IsMovable { get; } = false;

        private bool _isSolid = true;
        private Image _image = SpriteSheet.GetImage(Sprite.Door);

        public void Unlock()
        {
            _isSolid = false;
            _image = SpriteSheet.GetImage(Sprite.Blank);
        }
    }
}

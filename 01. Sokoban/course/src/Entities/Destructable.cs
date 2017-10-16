using System.Drawing;

namespace Sokoban.Entities
{
    public abstract class Destructable : EntityBase
    {
        protected Image DestructableImage;

        private bool _solid = true;
        private bool _movable = true;

        protected Destructable(MapVector position, Sprite initialSprite) : base(position)
        {
            DestructableImage = SpriteSheet.GetImage(initialSprite);
        }

        protected override Image Image => DestructableImage;
        public override bool IsSolid => _solid;
        public override bool IsMovable => _movable;

        public void Destroy()
        {
            DestructableImage = SpriteSheet.GetImage(Sprite.Blank);
            _solid = false;
            _movable = false;
        }
    }
}

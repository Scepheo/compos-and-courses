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

        // TODO: Assignment 5
        protected override Image Image { get { return DestructableImage; } }
        public override bool IsSolid  { get { return _solid; } }
        public override bool IsMovable { get { return _movable; } }

        public void Destroy()
        {
            DestructableImage = SpriteSheet.GetImage(Sprite.Blank);
            _solid = false;
            _movable = false;
        }
    }
}

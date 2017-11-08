using System.Collections.Generic;
using System.Drawing;

namespace Sokoban.Entities
{
    public class Fire : EntityBase
    {
        private Image _image = SpriteSheet.GetImage(Sprite.Fire);
        private bool _enabled = true;

        public Fire(MapVector position) : base(position)
        {
            OnOverlapStart += CheckDestroy;
        }

        protected override Image Image => _image;
        public override bool TopLayer { get; } = false;

        public override bool IsSolid { get; } = false;
        public override bool IsMovable { get; } = false;

        private void CheckDestroy(EntityBase other, IEnumerable<EntityBase> allEntities)
        {
            if (!_enabled)
            {
                return;
            }

            switch (other)
            {
                case Bucket bucket when !bucket.IsEmpty:
                    Disable();
                    bucket.Empty();
                    break;
                case Destructable destructable:
                    destructable.Destroy();
                    break;
                case Player player:
                    player.Kill();
                    break;
            }
        }

        private void Disable()
        {
            _enabled = false;
            _image = SpriteSheet.GetImage(Sprite.Blank);
        }
    }
}

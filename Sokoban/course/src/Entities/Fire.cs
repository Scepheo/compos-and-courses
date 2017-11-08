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
        public override bool IsSolid { get; } = false;
        public override bool IsMovable { get; } = false;

        public override bool TopLayer { get; } = false;

        private void CheckDestroy(EntityBase other, IEnumerable<EntityBase> allEntities)
        {
            if (!_enabled)
            {
                return;
            }

            // TODO: Assignment 8
            var bucket = other as Bucket;

            if (bucket != null)
            {
                if (bucket.IsEmpty)
                {
                    bucket.Destroy();
                }
                else
                {
                    Disable();
                    bucket.Empty();
                }
            }
            else
            {
                var player = other as Player;

                if (player != null)
                {
                    player.Kill();
                }
                else
                {
                    var destructable = other as Destructable;

                    if (destructable != null)
                    {
                        destructable.Destroy();
                    }
                }
            }
        }

        private void Disable()
        {
            _enabled = false;
            _image = SpriteSheet.GetImage(Sprite.Blank);
        }
    }
}

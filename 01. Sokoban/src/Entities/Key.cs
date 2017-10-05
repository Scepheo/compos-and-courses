using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Sokoban.Entities
{
    public class Key : EntityBase
    {
        public Key(MapVector position) : base(position)
        {
            OnOverlapStart += CheckUnlock;
        }

        public override bool IsSolid { get; } = false;
        public override bool IsMovable { get; } = false;
        public override bool TopLayer { get; } = false;

        private Image _image = SpriteSheet.GetImage(Sprite.Key);

        protected override Image Image => _image;

        private bool _enabled = true;

        private void CheckUnlock(EntityBase other, IEnumerable<EntityBase> allEntities)
        {
            if (_enabled && other is Player)
            {
                foreach (var door in allEntities.OfType<Door>())
                {
                    door.Unlock();
                    _image = SpriteSheet.GetImage(Sprite.Blank);
                    _enabled = false;
                }
            }
        }
    }
}

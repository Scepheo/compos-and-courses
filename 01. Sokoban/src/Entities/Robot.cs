using System.Collections.Generic;

namespace Sokoban.Entities
{
    public class Robot : Destructable
    {
        public Robot(MapVector position) : base(position, Sprite.BotEmpty)
        {
            OnOverlapStart += CheckCharging;
            OnOverlapEnd += CheckNotCharging;
        }

        public bool Charging { get; private set; }

        private void CheckCharging(EntityBase other, IEnumerable<EntityBase> allEntities)
        {
            if (other is Station)
            {
                Charging = true;
                DestructableImage = SpriteSheet.GetImage(Sprite.BotCharging);
            }
        }

        private void CheckNotCharging(EntityBase other, IEnumerable<EntityBase> allEntities)
        {
            if (other is Station)
            {
                Charging = false;
                DestructableImage = SpriteSheet.GetImage(Sprite.BotEmpty);
            }
        }
    }
}

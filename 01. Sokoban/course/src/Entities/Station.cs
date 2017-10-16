using System.Drawing;

namespace Sokoban.Entities
{
    public class Station : EntityBase
    {
        public Station(MapVector position) : base(position) { }

        protected override Image Image { get; } = SpriteSheet.GetImage(Sprite.ChargingStation);
        public override bool TopLayer { get; } = false;

        public override bool IsSolid { get; } = false;
        public override bool IsMovable { get; } = false;
    }
}

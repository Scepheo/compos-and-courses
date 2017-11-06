using System.Drawing;

namespace Sokoban.Entities
{
    public class Station : EntityBase
    {
        public Station(MapVector position) : base(position)
        {
            // TODO: Assignment 4
            // Override the defaults
            TopLayer = false;
            IsSolid = false;
            IsMovable = false;
        }

        protected override Image Image { get; } = SpriteSheet.GetImage(Sprite.ChargingStation);

        // TODO: Assignment 4
        public override bool TopLayer { get; } = false;

        public override bool IsSolid { get; } = false;
        public override bool IsMovable { get; } = false;
    }
}

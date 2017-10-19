namespace Sokoban.Entities
{
    public class Box : Destructable
    {
        public Box(MapVector position) : base(position, Sprite.Box) { }
    }
}

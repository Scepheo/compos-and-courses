namespace Sokoban.Entities
{
    public class Bucket : Destructable
    {
        public Bucket(MapVector position) : base(position, Sprite.BucketFull) { }

        public bool IsEmpty { get; private set; }

        public void Empty()
        {
            DestructableImage = SpriteSheet.GetImage(Sprite.BucketEmpty);
            IsEmpty = true;
        }
    }
}

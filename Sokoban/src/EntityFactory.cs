using Sokoban.Entities;

namespace Sokoban
{
    public static class EntityFactory
    {
        public static bool TryCreateEntity(Item item, MapVector position, out EntityBase entity)
        {
            switch (item)
            {
                case Item.Box:
                    entity = new Box(position);
                    return true;
                case Item.Player:
                    entity = new Player(position);
                    return true;
                case Item.Key:
                    entity = new Key(position);
                    return true;
                case Item.Door:
                    entity = new Door(position);
                    return true;
                case Item.Station:
                    entity = new Station(position);
                    return true;
                case Item.Robot:
                    entity = new Robot(position);
                    return true;
                case Item.Bucket:
                    entity = new Bucket(position);
                    return true;
                case Item.Fire:
                    entity = new Fire(position);
                    return true;
                default:
                    entity = null;
                    return false;
            }
        }
    }
}

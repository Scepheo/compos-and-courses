using System.Collections.Generic;

namespace Sokoban
{
    public static class Data
    {
        public static readonly Dictionary<char, Item> CharacterItemMap = new Dictionary<char, Item>
        {
            ['B'] = Item.Box,
            ['C'] = Item.Station,
            ['D'] = Item.Door,
            ['K'] = Item.Key,
            ['R'] = Item.Robot,
            ['.'] = Item.None,
            ['@'] = Item.Player,
            ['X'] = Item.Wall,
            ['F'] = Item.Fire,
            ['U'] = Item.Bucket
        };

        public static readonly Dictionary<Sprite, string> SpriteNameMap = new Dictionary<Sprite, string>
        {
            [Sprite.BotCharging]     = "bot_charging.png",
            [Sprite.BotEmpty]        = "bot_empty.png",
            [Sprite.Box]             = "box.png",
            [Sprite.ChargingStation] = "charging_station.png",
            [Sprite.Door]            = "door.png",
            [Sprite.Floor]           = "floor.png",
            [Sprite.Key]             = "key.png",
            [Sprite.Player]          = "player.png",
            [Sprite.Wall]            = "wall.png",
            [Sprite.Fire]            = "fire.png",
            [Sprite.BucketFull]      = "bucket_full.png",
            [Sprite.BucketEmpty]     = "bucket_empty.png",
            [Sprite.GraveStone]      = "gravestone.png",
        };

        public static readonly string[] Levels =
        {
            "level_1.txt",
            "level_2.txt",
            "level_3.txt"
        };
    }
}

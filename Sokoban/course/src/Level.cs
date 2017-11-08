using Sokoban.Entities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Sokoban
{
    public class Level
    {
        // It is important that Speed evenly divides TileSize, or the grid movement detection will not work
        public const int TileSize = 64;
        private const int Speed = 4;

        // TODO: Assignment 3
        private readonly int _width, _height;

        public int Height
        {
            get
            {
                return _height;
            }
        }

        public int Width
        {
            get
            {
                return _width;
            }
        }

        private readonly TileMap _tileMap;
        private readonly EntityBase[] _entities;
        private readonly Player _player;
        private readonly EventQueue _eventQueue = new EventQueue();

        public Level(Item[,] items)
        {
            // TODO: Assignment 1
            // TODO: Assignment 3
            _width = items.GetLength(0);
            _height = items.GetLength(1);

            _tileMap = new TileMap(items);
            _entities = GetEntities(items);
            _player = GetPlayer(_entities);
        }

        public void Step()
        {
            foreach (var entity in _entities)
            {
                entity.Step();
            }

            _eventQueue.Update(_player, _entities);
        }

        public void HandleMovement(Direction direction)
        {
            if (_player.Moving || !_player.Enabled)
            {
                return;
            }

            // TODO: Assignment 7
            int xSpeed, ySpeed;

            switch (direction)
            {
                case Direction.Up:
                    xSpeed = 0;
                    ySpeed = -Speed;
                    break;
                case Direction.Down:
                    xSpeed = 0;
                    ySpeed = Speed;
                    break;
                case Direction.Left:
                    xSpeed = -Speed;
                    ySpeed = 0;
                    break;
                case Direction.Right:
                    xSpeed = Speed;
                    ySpeed = 0;
                    break;
                default:
                    // TODO: Assignment 2
                    xSpeed = 0;
                    ySpeed = 0;
                    break;
            }

            SetMovement(xSpeed, ySpeed);
        }

        private void SetMovement(int xSpeed, int ySpeed)
        {
            var movement = new EntityVector(xSpeed, ySpeed);
            var playerPos = _player.Position.GetMapVector();
            var mapMovement = movement.Sign();

            var targetPos = playerPos + mapMovement;

            // If there's a wall, we can't move
            if (_tileMap.IsSolid(targetPos))
            {
                return;
            }

            // If there's no object in our path, move
            var obstacles = FindEntities(targetPos);

            if (!obstacles.Any(obstacle => obstacle.IsSolid))
            {
                _eventQueue.QueueOverlapStart(_player, obstacles);
                _eventQueue.QueueOverlapEnd(_player, FindEntities(playerPos).Where(obstacle => obstacle != _player));
                _player.SetMovement(xSpeed, ySpeed);
                return;
            }

            // If there's a non-moveable in our path, don't move
            if (obstacles.Any(e => e.IsSolid && !e.IsMovable))
            {
                return;
            }

            var movable = obstacles.Single(obstacle => obstacle.IsMovable);

            // See what's behind the movable
            var behindTargetPos = targetPos + mapMovement;

            // If there's a wall, we can't move
            if (_tileMap.IsSolid(behindTargetPos))
            {
                return;
            }

            // If there's a solid object, we can't move
            var obstaclesBehindMovable = FindEntities(behindTargetPos);

            if (obstaclesBehindMovable.Any(obstacle => obstacle.IsSolid))
            {
                return;
            }
            
            // Otherwise, move and push
            _player.SetMovement(xSpeed, ySpeed);
            _eventQueue.QueueOverlapStart(_player, obstacles.Where(obstacle => obstacle != movable));
            _eventQueue.QueueOverlapEnd(_player, FindEntities(playerPos).Where(obstacle => obstacle != _player));

            movable.SetMovement(xSpeed, ySpeed);
            _eventQueue.QueueOverlapStart(movable, obstaclesBehindMovable);
            _eventQueue.QueueOverlapEnd(movable, obstacles.Where(obstacle => obstacle != movable));
        }


        // TODO: Assignment 5
        public bool IsCompleted()
        {
            return !_player.Moving && _entities.OfType<Robot>().All(robot => robot.Charging);
        }

        private EntityBase[] FindEntities(MapVector position)
        {
            return _entities.Where(entity => entity.Position.GetMapVector() == position).ToArray();
        }

        public void Draw(Graphics graphics)
        {
            _tileMap.Draw(graphics);

            foreach (var entity in _entities)
            {
                entity.Draw(graphics);
            }
        }

        // TODO: Assignment 5
        private IEnumerable<MapVector> GetPositions()
        {
            return
                from x in Enumerable.Range(0, Width)
                from y in Enumerable.Range(0, Height)
                select new MapVector(x, y);
        }
        
        private EntityBase[] GetEntities(Item[,] items)
        {
            var entityList = new List<EntityBase>();

            foreach (var position in GetPositions())
            {
                var item = items[position.X, position.Y];

                // TODO: Assignment 9
                EntityBase entity;

                if (EntityFactory.TryCreateEntity(item, position, out entity))
                {
                    entityList.Add(entity);
                }
            }

            entityList.Sort(CompareLayer);

            return entityList.ToArray();
        }

        // TODO: Assignment 10
        private static int CompareLayer(EntityBase left, EntityBase right)
        {
            if (left.TopLayer == right.TopLayer)
            {
                return 0;
            }
            else if (right.TopLayer)
            {
                return -1;
            }
            else
            {
                return 1;
            }
        }

        private static Player GetPlayer(EntityBase[] entities)
        {
            var players = entities.OfType<Player>().ToArray();

            switch (players.Length)
            {
                case 0:
                    throw new InvalidOperationException("No player in level");
                case 1:
                    return players[0];
                default:
                    throw new InvalidOperationException("More than one player in level");
            }
    }
    }
}

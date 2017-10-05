﻿using Sokoban.Entities;
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

        public int Height { get; }
        public int Width { get; }

        private readonly TileMap _tileMap;
        private readonly EntityBase[] _entities;
        private readonly Player _player;
        private readonly EventQueue _eventQueue = new EventQueue();

        public Level(Item[,] items)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            Width = items.GetLength(0);
            Height = items.GetLength(1);
            _tileMap = new TileMap(items);
            (_entities, _player) = GetEntities(items);
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

            int xSpeed, ySpeed;

            switch (direction)
            {
                case Direction.Up:
                    (xSpeed, ySpeed) = (0, -Speed);
                    break;
                case Direction.Down:
                    (xSpeed, ySpeed) = (0, Speed);
                    break;
                case Direction.Left:
                    (xSpeed, ySpeed) = (-Speed, 0);
                    break;
                case Direction.Right:
                    (xSpeed, ySpeed) = (Speed, 0);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(
                        nameof(direction),
                        direction,
                        $"Invalid direction: '{direction}'");
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

        public bool IsCompleted() => !_player.Moving && _entities.OfType<Robot>().All(robot => robot.Charging);

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

        private (EntityBase[] entities, Player player) GetEntities(Item[,] items)
        {
            var entityList = new List<EntityBase>();
            Player player = null;

            for (var x = 0; x < Width; x++)
            {
                for (var y = 0; y < Height; y++)
                {
                    var item = items[x, y];

                    var position = new MapVector(x, y);

                    if (EntityFactory.TryCreateEntity(item, position, out var entity))
                    {
                        entityList.Add(entity);

                        if (entity is Player entityPlayer)
                        {
                            if (player == null)
                            {
                                player = entityPlayer;
                            }
                            else
                            {
                                throw new InvalidOperationException("Multiple players in level");
                            }
                        }
                    }
                }
            }

            if (player == null)
            {
                throw new InvalidOperationException("No player in level");
            }

            int CompareLayer(EntityBase left, EntityBase right) => left.TopLayer == right.TopLayer
                ? 0
                : right.TopLayer
                    ? -1
                    : 1;

            entityList.Sort(CompareLayer);

            return (entityList.ToArray(), player);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;

namespace BombermanLib
{
    public class Level
    {
        public bool[,] Walls { get; private set; }
        public List<Player> Players { get; private set; }
        public List<Bomb> Bombs { get; private set; }
        public List<Box> Boxes { get; private set; }
        public List<Explosion> Explosions { get; private set; }

        private Level() { }

        public static Level Generate(int width, int height)
        {
            var walls = new bool[width, height];
            var xMax = width - 1;
            var yMax = height - 1;

            // Create walls around the level perimeter
            for (var x = 0; x < width; x++)
            {
                walls[x, 0] = true;
                walls[x, yMax] = true;
            }

            for (var y = 1; y < yMax; y++)
            {
                walls[0, y] = true;
                walls[xMax, y] = true;
            }

            // Create walls in dot patterns
            for (var x = 2; x < xMax; x += 2)
            for (var y = 2; y < yMax; y += 2)
            {
                walls[x, y] = true;
            }

            // Place players in corners
            var players = new List<Player>
            {
                new Player(1, new Vector(1, 1)),
                new Player(2, new Vector(xMax - 1, 1)),
                new Player(3, new Vector(1, yMax - 1)),
                new Player(4, new Vector(xMax - 1, yMax - 1))
            };

            // Place boxes in semi-random patterns
            var random = new Random();
            var boxes = new List<Box>();

            var xLimit = width / 2 + 1;
            var yLimit = height / 2 + 1;

            for (var x = 1; x < xLimit; x++)
            {
                for (var y = 1; y < yLimit; y++)
                {
                    // Don't place a box if it's too close to the player
                    if (x + y < 4) continue;

                    // Do place a box if we're near the center cross
                    var mustPlace = x >= xLimit - 1 || y >= yLimit - 1;

                    // Otherwise, place a box with random chance
                    const double chance = 0.5;
                    if (mustPlace || random.NextDouble() < chance)
                    {
                        boxes.Add(new Box(new Vector(x, y)));
                        boxes.Add(new Box(new Vector(xMax - x, y)));
                        boxes.Add(new Box(new Vector(x, yMax - y)));
                        boxes.Add(new Box(new Vector(xMax - x, yMax - y)));
                    }
                }
            }

            return new Level
            {
                Bombs = new List<Bomb>(),
                Boxes = boxes,
                Explosions = new List<Explosion>(),
                Players = players,
                Walls = walls
            };
        }

        public string[] Step(string[] messages)
        {
            StepBombs();
            StepExplosions();
            return StepPlayers(messages);
        }

        private string[] StepPlayers(string[] messages)
        {
            var results = new string[messages.Length];
            var targetPositions = new Vector[messages.Length];

            for (var i = 0; i < messages.Length; i++)
            {
                switch (messages[i])
                {
                    case Message.Up:
                        targetPositions[i] = Players[i].Position + Vector.Up;
                        break;
                    case Message.Down:
                        targetPositions[i] = Players[i].Position + Vector.Down;
                        break;
                    case Message.Left:
                        targetPositions[i] = Players[i].Position + Vector.Left;
                        break;
                    case Message.Right:
                        targetPositions[i] = Players[i].Position + Vector.Right;
                        break;
                    default:
                        targetPositions[i] = Players[i].Position;
                        break;
                }
            }

            for (var i = 0; i < messages.Length; i++)
            {
                if (Players[i].Dead)
                {
                    results[i] = Message.Dead;
                    continue;
                }

                switch (messages[i])
                {
                    case Message.Bomb:
                        if (Bombs.Any(bomb => bomb.Number == i + 1))
                        {
                            results[i] = Message.Wait;
                        }
                        else
                        {
                            results[i] = Message.Bomb;
                            Bombs.Add(new Bomb(i + 1, Players[i].Position));
                        }
                        break;
                    case Message.Wait:
                        results[i] = Message.Wait;
                        break;
                    case Message.Up:
                    case Message.Down:
                    case Message.Left:
                    case Message.Right:
                        var targetPosition = targetPositions[i];

                        if (Walls[targetPosition.X, targetPosition.Y] ||
                            Bombs.Any(bomb => bomb.Position == targetPosition) ||
                            Boxes.Any(box => box.Position == targetPosition) ||
                            targetPositions.Count(position => position == targetPosition) > 1)
                        {
                            results[i] = Message.Wait;
                        }
                        else
                        {
                            results[i] = messages[i];
                            Players[i].Position = targetPosition;
                        }
                        break;
                }
            }

            return results;
        }

        private void StepBombs()
        {
            foreach (var bomb in Bombs)
            {
                bomb.Turns--;

                if (bomb.Turns == 0)
                {
                    Explosions.Add(new Explosion(bomb.Position));
                }
            }

            Bombs.RemoveAll(bomb => bomb.Turns == 0);
        }

        private struct MovingExplosion
        {
            public readonly Vector Position;
            public readonly Vector Direction;

            public MovingExplosion(Vector position, Vector direction)
            {
                Position = position;
                Direction = direction;
            }
        }

        private void StepExplosions()
        {
            var queue = new Queue<MovingExplosion>();
            var done = new HashSet<Vector>();

            foreach (var explosion in Explosions)
            {
                if (done.Add(explosion.Position))
                {
                    queue.Enqueue(new MovingExplosion(explosion.Position, Vector.Zero));
                }

                foreach (var direction in Vector.Directions)
                {
                    if (done.Add(explosion.Position + direction))
                    {
                        queue.Enqueue(new MovingExplosion(explosion.Position + direction, direction));
                    }
                }
            }

            Explosions.Clear();

            while (queue.Count > 0)
            {
                var movingExplosion = queue.Dequeue();
                var position = movingExplosion.Position;
                var direction = movingExplosion.Direction;

                Bomb hitBomb;
                Box hitBox;
                Player hitPlayer;

                if (Walls[position.X, position.Y])
                {
                    // Do nothing
                }
                else if ((hitBomb = Bombs.Find(bomb => bomb.Position == position)) != null)
                {
                    hitBomb.Turns = 1;
                }
                else if ((hitBox = Boxes.Find(box => box.Position == position)) != null)
                {
                    Boxes.Remove(hitBox);
                }
                else if ((hitPlayer = Players.Find(player => player.Position == position)) != null)
                {
                    hitPlayer.Dead = true;
                }
                else if (done.Add(position + direction))
                {
                    queue.Enqueue(new MovingExplosion(position + direction, Vector.Zero));
                }
            }
        }
    }
}

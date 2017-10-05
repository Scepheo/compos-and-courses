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

        public static Level TestLevel()
        {
            const bool X = true;
            const bool _ = false;

            return new Level
            {
                Walls = new bool[,]
                {
                    { X, X, X, X, X, X, X, X, X, X },
                    { X, _, _, _, _, X, _, _, _, X },
                    { X, _, _, _, X, X, _, _, _, X },
                    { X, X, X, _, _, _, _, X, _, X },
                    { X, _, X, _, _, _, _, X, X, X },
                    { X, _, _, _, X, X, _, _, _, X },
                    { X, _, _, _, X, _, _, _, _, X },
                    { X, X, X, X, X, X, X, X, X, X }
                },
                Players = new List<Player>
                {
                    new Player(1, new Vector(1, 1)),
                    new Player(2, new Vector(8, 1)),
                    new Player(3, new Vector(1, 6)),
                    new Player(4, new Vector(8, 6))
                },
                Boxes = new List<Box>
                {
                    new Box(new Vector(4, 3)),
                    new Box(new Vector(5, 3)),
                    new Box(new Vector(4, 4)),
                    new Box(new Vector(5, 4))
                },
                Bombs = new List<Bomb>(),
                Explosions = new List<Explosion>()
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
                var canMove = direction != Vector.Zero;

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

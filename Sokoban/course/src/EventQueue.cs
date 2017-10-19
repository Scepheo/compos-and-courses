using System;
using System.Collections.Generic;
using System.Linq;
using Sokoban.Entities;

namespace Sokoban
{
    public class EventQueue
    {
        private readonly Queue<(EntityBase, EntityBase[])> _overlapQueue = new Queue<(EntityBase, EntityBase[])>();
        private readonly Queue<(EntityBase, EntityBase[])> _unoverlapQueue = new Queue<(EntityBase, EntityBase[])>();

        public void QueueOverlapStart(EntityBase moving, IEnumerable<EntityBase> overlapped)
        {
            _overlapQueue.Enqueue((moving, overlapped.ToArray()));
        }

        public void QueueOverlapEnd(EntityBase moving, IEnumerable<EntityBase> overlapped)
        {
            _unoverlapQueue.Enqueue((moving, overlapped.ToArray()));
        }

        public void Update(Player player, EntityBase[] allEntities)
        {
            if (player.Moving)
            {
                return;
            }

            EmptyQueue(_overlapQueue, (a, b) => a.TriggerOverlapStart(b, allEntities));
            EmptyQueue(_overlapQueue, (a, b) => a.TriggerOverlapEnd(b, allEntities));
        }

        private static void EmptyQueue(Queue<(EntityBase, EntityBase[])> queue, Action<EntityBase, EntityBase> action)
        {
            while (queue.Count > 0)
            {
                (var main, var others) = queue.Dequeue();

                foreach (var other in others)
                {
                    action(main, other);
                    action(other, main);
                }
            }
        }
    }
}

using System.Collections.Generic;
using System.Drawing;

namespace Sokoban.Entities
{
    public abstract class EntityBase
    {
        protected abstract Image Image { get; }
        public virtual bool TopLayer { get; } = true;

        public virtual bool IsSolid { get; } = true;
        public virtual bool IsMovable { get; } = true;

        public EntityVector Position { get; private set; }

        private EntityVector _velocity;

        protected EntityBase(MapVector position)
        {
            Position = position.ToEntityVector();
        }

        public bool Moving => _velocity != EntityVector.Zero;

        public void SetMovement(int xSpeed, int ySpeed)
        {
            _velocity = new EntityVector(xSpeed, ySpeed);
        }

        public void Step()
        {
            Position += _velocity;

            if (Position.IsMapVector)
            {
                _velocity = EntityVector.Zero;
            }
        }

        public void Draw(Graphics graphics)
        {
            graphics.DrawImage(Image, Position.X, Position.Y);
        }

        protected delegate void OverlapHandler(EntityBase other, IEnumerable<EntityBase> allEntities);

        protected event OverlapHandler OnOverlapStart;
        protected event OverlapHandler OnOverlapEnd;

        public void TriggerOverlapStart(EntityBase other, IEnumerable<EntityBase> allEntities)
        {
            OnOverlapStart?.Invoke(other, allEntities);
        }

        public void TriggerOverlapEnd(EntityBase other, IEnumerable<EntityBase> allEntities)
        {
            OnOverlapEnd?.Invoke(other, allEntities);
        }
    }
}

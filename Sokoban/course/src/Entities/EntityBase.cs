using System.Collections.Generic;
using System.Drawing;

namespace Sokoban.Entities
{
    public abstract class EntityBase
    {
        protected abstract Image Image { get; }

        // TODO: Assignment 4
        public virtual bool TopLayer { get; }

        public virtual bool IsSolid { get; }
        public virtual bool IsMovable { get; }

        public EntityVector Position { get; private set; }

        private EntityVector _velocity;

        protected EntityBase(MapVector position)
        {
            Position = position.ToEntityVector();

            // TODO: Assignment 4
            // Set the defaults
            TopLayer = true;
            IsSolid = true;
            IsMovable = true;
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

        // TODO: Assignment 6
        public void TriggerOverlapStart(EntityBase other, IEnumerable<EntityBase> allEntities)
        {
            if (OnOverlapStart != null)
            {
                OnOverlapStart.Invoke(other, allEntities);
            }
        }

        // TODO: Assignment 6
        public void TriggerOverlapEnd(EntityBase other, IEnumerable<EntityBase> allEntities)
        {
            if (OnOverlapEnd != null)
            {
                OnOverlapEnd.Invoke(other, allEntities);
            }
        }
    }
}

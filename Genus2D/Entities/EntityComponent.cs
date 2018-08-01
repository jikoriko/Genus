using System;

using OpenTK;

namespace Genus2D.Entities
{
    public class EntityComponent
    {
        private Entity _parent;
        public Entity Parent
        {
            get { return _parent; }

            private set { _parent = value; }
        }

        public Transform Transform { get { return Parent.GetTransform(); } }

        public EntityComponent(Entity entity)
        {
            Parent = entity;
            entity.AddComponent(this);
        }

        public virtual void Update(FrameEventArgs e)
        {
        }

        public virtual void LateUpdate(FrameEventArgs e)
        {
        }

        public virtual void Render(FrameEventArgs e)
        {
        }
    }
}

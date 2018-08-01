using System;

using Genus2D.Core;
using Genus2D.Entities;

using OpenTK;

namespace Genus2D.Collision
{
    public class Collider
    {
        protected Entity _entity;
        protected Vector2 _size;
        protected Vector2 _position;

        public bool IsTrigger;
        public bool Draggable;
        public bool Grabbed;

        private Collider(Entity entity, Vector2 size)
        {
            _entity = entity;
            _position = Vector2.Zero;
            _size = size;

            IsTrigger = false;
            Draggable = false;
            Grabbed = false;
        }

        public Collider(Entity entity, float width, float height)
        {
            _entity = entity;
            _position = Vector2.Zero;
            _size = new Vector2(width, height);

            IsTrigger = false;
            Draggable = false;
            Grabbed = false;
        }

        public Entity GetEntity()
        {
            return _entity;
        }

        public Vector2 GetPosition()
        {
            return _position;
        }

        public void SetPosition(Vector2 position)
        {
            _position = position;
        }

        public Vector2 GetSize()
        {
            return _size;
        }

        public void SetSize(float width, float height)
        {
            _size.X = width;
            _size.Y = height;
        }

        public bool MouseInside()
        {
            if (_entity == null)
                return false;

            Vector2 mouse = StateWindow.Instance.GetMousePosition();
            Vector3 pos = _entity.GetTransform().Position;
            pos.X += _position.X;
            pos.Y += _position.Y;
            Vector2 size = _size * _entity.GetTransform().Scale.Xy;

            return (mouse.X < pos.X + size.X && mouse.X >= pos.X && mouse.Y < pos.Y + size.Y && mouse.Y >= pos.Y);
        }

    }
}

using System;
using System.Collections.Generic;
using System.Drawing;

using Genus2D.Core;
using Genus2D.Listeners;
using Genus2D.Collision;
using Genus2D.Graphics;

using OpenTK;
using OpenTK.Input;
using OpenTK.Graphics;

namespace Genus2D.Entities
{
    public class Entity
    {
        protected EntityManager _manager;
        protected EntityLayer _parentLayer;
        protected State _state;

        protected Transform _transform;

        protected int _orderZ;
        protected bool _destroyed, _disabled;

        protected Collider _collider;
        private List<EntityComponent> _components;

        public static Entity CreateInstance(EntityManager manager)
        {
            return (CreateInstance(manager, "Default", Vector3.Zero));
        }

        public static Entity CreateInstance(EntityManager manager, Vector3 pos)
        {
            return (CreateInstance(manager, "Default", pos));
        }

        public static Entity CreateInstance(EntityManager manager, string layerName, Vector3 pos)
        {
            Entity instance = null;

            instance = new Entity(manager, layerName);
            instance.GetTransform().Position = pos;

            return instance;
        }

        private Entity(EntityManager manager, string layerName)
        {
            _manager = manager;
            manager.AddEntity(this, layerName);

            _transform = new Transform(this);

            _orderZ = 0;
            
            _destroyed = _disabled = false;
            _collider = null;
            _components = new List<EntityComponent>();

            Random rand = new Random();
        }

        public EntityManager GetManager()
        {
            return _manager;
        }

        public EntityLayer GetParentLayer()
        {
            return _parentLayer;
        }

        public void SetParentLayer(EntityLayer parentLayer)
        {
            _parentLayer = parentLayer;
        }

        public Transform GetTransform()
        {
            return _transform;
        }

        public virtual int GetOrderZ()
        {
            return _orderZ;
        }

        public virtual void SetOrderZ(int orderZ)
        {
            _orderZ = orderZ;
        }

        public Collider Collider
        {
            get
            {
                return _collider;
            }
            private set
            {
                _collider = value;
            }
        }

        public void SetCollisionSize(float width, float height)
        {
            if (_collider == null)
            {
                _collider = new Collider(this, width, height);
                _manager.CollisionManager.AddCollider(_collider);
            }
            else
            {
                _collider.SetSize(width, height);
            }
        }

        public void SetColliderPosition(Vector2 position)
        {
            if (_collider != null)
            {
                _collider.SetPosition(position);
            }
        }

        public void AddComponent(EntityComponent component)
        {
            if (!_components.Contains(component) && component.Parent == this)
                _components.Add(component);
        }

        public EntityComponent FindComponent<T>()
        {
            foreach (EntityComponent component in _components)
            {
                if (component.GetType() == typeof(T))
                {
                    return component;
                }
            }
            return null;
        }

        public virtual void Enable()
        {
            _disabled = false;
        }

        public virtual void Disable()
        {
            _disabled = true;
        }

        public virtual void Update(FrameEventArgs e)
        {
            if (_destroyed || _disabled)
                return;
            foreach (EntityComponent component in _components)
            {
                component.Update(e);
            }
        }

        public virtual void LateUpdate(FrameEventArgs e)
        {
            if (_destroyed || _disabled)
                return;
            foreach (EntityComponent component in _components)
            {
                component.LateUpdate(e);
            }
        }

        public virtual void Render(FrameEventArgs e)
        {
            if (_destroyed || _disabled)
                return;

            foreach (EntityComponent component in _components)
            {
                component.Render(e);
            }

            if (_collider != null)
            {
                Vector3 pos = _transform.Position;
                pos.X += _collider.GetPosition().X;
                pos.Y += _collider.GetPosition().Y;
                Vector3 size = new Vector3(_collider.GetSize());
                size.Z = 1;
                Color4 colour = Color4.Red;
                Renderer.DrawShape(ShapeFactory.Rectangle, ref pos, ref size, 2f, ref colour);
            }
        }

        public virtual void Destroy()
        {
            if (!_destroyed)
            {
                if (_collider != null)
                {
                    _manager.CollisionManager.RemoveCollider(_collider);
                }
                _destroyed = true;
            }
        }

        public bool Destroyed()
        {
            return _destroyed;
        }

    }
}

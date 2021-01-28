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
        protected State _state;

        protected Transform _transform;

        protected int _orderZ;
        protected bool _destroyed, _disabled;

        private List<EntityComponent> _components;

        public static Entity CreateInstance(EntityManager manager)
        {
            return (CreateInstance(manager, Vector3.Zero));
        }


        public static Entity CreateInstance(EntityManager manager, Vector3 pos)
        {
            Entity instance = null;

            try
            {
                instance = new Entity(manager);
                instance.GetTransform().Position = pos;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }

            return instance;
        }

        private Entity(EntityManager manager)
        {
            if (manager == null)
                throw new Exception("Cannot create an entity with no manager");

            _manager = manager;
            _manager.AddEntity(this);

            _transform = new Transform(this);

            _orderZ = 0;
            
            _destroyed = _disabled = false;
            _components = new List<EntityComponent>();

            Random rand = new Random();
        }

        public EntityManager GetManager()
        {
            return _manager;
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

        public void AddComponent(EntityComponent component)
        {
            if (!_components.Contains(component) && component.Parent == this)
                _components.Add(component);
        }

        public T FindComponent<T>() where T : EntityComponent
        {
            foreach (EntityComponent component in _components)
            {
                if (component.GetType() == typeof(T))
                {
                    return (T)component;
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

        }

        protected virtual void OnDestroy()
        {
            foreach (EntityComponent component in _components)
            {
                component.Destroy();
            }
        }

        public virtual void Destroy()
        {
            if (!_destroyed)
            {
                OnDestroy();
                _destroyed = true;
            }
        }

        public bool Destroyed()
        {
            return _destroyed;
        }

    }
}

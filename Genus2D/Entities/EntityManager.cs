using System;
using System.Collections.Generic;

using Genus2D.Entities;
using Genus2D.Core;
using Genus2D.Graphics;
using Genus2D.Listeners;
using Genus2D.Collision;

using OpenTK;
using OpenTK.Input;

namespace Genus2D.Entities
{
    public class EntityManager
    {
        private State _state;
        private List<Entity> _entities;

        private bool _destroyed;

        public EntityManager(State state)
        {
            _state = state;
            _entities = new List<Entity>();

            _destroyed = false;
        }

        public void AddEntity(Entity entity)
        {
            if (_destroyed)
                return;
            if (entity.GetManager() == this)
            {
                if (!_entities.Contains(entity))
                    _entities.Add(entity);
            }

        }

        public void RemoveEntity(Entity entity)
        {
            _entities.Remove(entity);
        }

        public void UpdateEntities(FrameEventArgs e)
        {
            if (_destroyed)
                return;

            for (int i = 0; i < _entities.Count; i++)
            {
                if (_entities[i].Destroyed())
                {
                    RemoveEntity(_entities[i]);
                    i--;
                }
            }

            for (int i = 0; i < _entities.Count; i++)
            {
                _entities[i].Update(e);
            }

            for (int i = 0; i < _entities.Count; i++)
            {
                _entities[i].LateUpdate(e);
            }
        }

        public void RenderEntities(FrameEventArgs e)
        {
            if (_destroyed)
                return;

            Renderer.PushWorldMatrix();
            for (int i = 0; i < _entities.Count; i++)
            {
                _entities[i].Render(e);
            }
            Renderer.PopWorldMatrix();
        }

        public void Destroy()
        {
            if (!_destroyed)
            {

                foreach (Entity entity in _entities)
                {
                    entity.Destroy();
                }
                _entities.Clear();
                _destroyed = true;
            }
        }

    }
}

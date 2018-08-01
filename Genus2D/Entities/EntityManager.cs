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
        private List<EntityLayer> _entityLayers;
        private List<Entity> _entities;
        private CollisionManager _collisionManager;
        private Entity _selectedEntity;

        private bool _destroyed;

        public EntityManager(State state)
        {
            _state = state;
            _entityLayers = new List<EntityLayer>();
            _entities = new List<Entity>();
            _collisionManager = new CollisionManager(_state);

            GetEntityLayer("Default");
            _destroyed = false;
        }

        public CollisionManager CollisionManager
        {
            get
            {
                return _collisionManager;
            }
            private set
            {
                _collisionManager = value;
            }
        }

        public void CreateLayer(string layerName)
        {
            GetEntityLayer(layerName);
        }

        private EntityLayer GetEntityLayer(string layerName)
        {
            if (_destroyed)
                return null;
            EntityLayer layer;
            for (int i = 0; i < _entityLayers.Count; i++)
            {
                layer = _entityLayers[i];
                if (layer.GetName() == layerName)
                    return layer;
            }
            layer = new EntityLayer(this, layerName);
            _entityLayers.Add(layer);
            layer.SetOrderZ(_entityLayers.Count - 1);
            return layer;
        }

        public void OrderLayers()
        {
            if (_destroyed)
                return;
            int order = 0;
            for (int i = 0; i < _entityLayers.Count; i++)
            {
                _entityLayers[i].SetOrderZ(order);
                order += _entityLayers[i].NumberOfEntities();
            }
        }

        public void AddEntity(Entity entity)
        {
            AddEntity(entity, "Default");
        }

        public void AddEntity(Entity entity, string layerName)
        {
            if (_destroyed)
                return;
            if (entity.GetManager() == this)
            {
                EntityLayer layer = GetEntityLayer(layerName);
                layer.AddEntity(entity);
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

            for (int i = 0; i < _entityLayers.Count; i++)
            {
                EntityLayer layer = _entityLayers[i];
                layer.Update(e);
            }

            for (int i = 0; i < _entityLayers.Count; i++)
            {
                EntityLayer layer = _entityLayers[i];
                layer.LateUpdate(e);
            }
        }

        public void RenderEntities(FrameEventArgs e)
        {
            if (_destroyed)
                return;
            Renderer.PushWorldMatrix();
            for (int i = 0; i < _entityLayers.Count; i++)
            {
                EntityLayer layer = _entityLayers[i];
                layer.Render(e);
            }
        }

        public void Destroy()
        {
            if (!_destroyed)
            {
                foreach (EntityLayer layer in _entityLayers)
                {
                    layer.Destroy();
                }
                _entityLayers.Clear();

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

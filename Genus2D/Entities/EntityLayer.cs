using System;
using System.Collections.Generic;

using OpenTK;

namespace Genus2D.Entities
{
    public class EntityLayer
    {
        private EntityManager _manager;
        private string _name;
        private List<Entity> _entities;
        private int _orderZ;
        private bool _destroyed;

        public EntityLayer(EntityManager manager, string name)
        {
            _manager = manager;
            _name = name;
            _entities = new List<Entity>();
            _orderZ = 0;
            _destroyed = false;
        }

        public void AddEntity(Entity entity)
        {
            if (entity.GetManager() == _manager)
            {
                if (!_entities.Contains(entity))
                {
                    if (entity.GetParentLayer() != null)
                    {
                        entity.GetParentLayer().RemoveEntity(entity);
                    }
                    _entities.Add(entity);
                    entity.SetParentLayer(this);
                    entity.SetOrderZ(_orderZ + _entities.Count - 1);

                }
            }
        }

        public void RemoveEntity(Entity entity)
        {
            _entities.Remove(entity);
            entity.SetParentLayer(null);
        }

        public string GetName()
        {
            return _name;
        }

        public int GetOrderZ()
        {
            return _orderZ;
        }

        public void SetOrderZ(int orderZ)
        {
            _orderZ = orderZ;
            for (int i = 0; i < _entities.Count; i++)
            {
                _entities[i].SetOrderZ(_orderZ + i);
            }
        }

        public int NumberOfEntities()
        {
            return _entities.Count;
        }

        public void Update(FrameEventArgs e)
        {
            for (int i = 0; i < _entities.Count; i++)
                _entities[i].Update(e);
        }

        public void LateUpdate(FrameEventArgs e)
        {
            for (int i = 0; i < _entities.Count; i++)
                _entities[i].LateUpdate(e);
        }

        public void Render(FrameEventArgs e)
        {
            for (int i = 0; i < _entities.Count; i++)
                _entities[i].Render(e);
        }

        public void Destroy()
        {
            if (!_destroyed)
            {
                foreach (Entity entity in _entities)
                {
                    entity.Destroy();
                }
                _destroyed = true;
            }
        }

    }
}

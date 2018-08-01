using System;

using OpenTK;

namespace Genus2D.Entities
{
    public class Transform
    {

        private Entity _entity;
        private Transform _parent;

        private Vector3 _localPosition;
        private Vector3 _localRotation;
        private Vector3 _localScale;

        public Transform(Entity entity)
        {
            _entity = entity;
            _parent = null;
            _localPosition = Vector3.Zero;
            _localRotation = Vector3.Zero;
            _localScale = Vector3.One;
        }

        public Entity Entity
        {
            get { return _entity; }
            private set { _entity = value; }
        }

        public Transform Parent 
        { 
            get 
            { 
                return _parent; 
            }
            set 
            {
                Vector3 pos = Position;
                Vector3 rot = Rotation;
                Vector3 scale = Scale;
                _parent = value;
                Position = pos;
                Rotation = rot;
                Scale = scale;
            } 
        }

        public Vector3 Position 
        { 
            get 
            { 
                Vector3 pos = LocalPosition; 
                if (Parent != null) 
                    pos += Parent.Position; 
                return pos; 
            }
            set 
            {
                Vector3 pos = value; 
                if (Parent != null)
                {
                    pos -= Parent.Position;
                }
                LocalPosition = pos;
            
            } 
        }

        public Vector3 LocalPosition
        {
            get
            {
                return _localPosition;
            }
            set
            {
                _localPosition = value;
            }
        }

        public Vector3 Rotation
        {
            get
            {
                Vector3 rot = LocalRotation;
                if (Parent != null)
                    rot += Parent.Rotation;
                return rot;
            }
            set
            {
                Vector3 rot = value;
                if (Parent != null)
                {
                    rot -= Parent.Rotation;
                }
                LocalRotation = rot;
            }
        }

        public Vector3 LocalRotation
        {
            get
            {
                return _localRotation;
            }
            set
            {
                _localRotation = value;
            }
        }

        public Vector3 Scale
        {
            get
            {
                Vector3 scale = LocalScale;
                if (Parent != null)
                    scale += Parent.Scale;
                return scale;
            }
            set
            {
                Vector3 scale = value;
                if (Parent != null)
                    scale -= Parent.Scale;
                LocalScale = scale;
            }
        }

        public Vector3 LocalScale
        {
            get
            {
                return _localScale;
            }
            set
            {
                _localScale = value;
            }
        }

    }
}

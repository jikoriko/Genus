using System;
using System.Collections.Generic;

using Genus2D.Core;
using Genus2D.Listeners;
using Genus2D.Entities;

using OpenTK;
using OpenTK.Input;

namespace Genus2D.Collision
{
    public class CollisionManager : MouseListener
    {
        private State _state;
        private List<Collider> _colliders;
        private Collider _selectedCollider;

        public CollisionManager(State state)
        {
            _state = state;
            _colliders = new List<Collider>();
            state.AddMouseListener(this);
        }

        public void AddCollider(Collider collider)
        {
            if (!_colliders.Contains(collider))
            {
                _colliders.Add(collider);
            }
        }

        public void RemoveCollider(Collider collider)
        {
            _colliders.Remove(collider);
        }

        public void OnMouseDown(MouseButtonEventArgs e)
        {
            if (e.Button == MouseButton.Left)
            {
                Collider selectedCollider = null;
                for (int i = 0; i < _colliders.Count; i++)
                {
                    if (_colliders[i].MouseInside())
                    {
                        if (selectedCollider == null)
                        {
                            selectedCollider = _colliders[i];
                        }
                        else
                        {
                            if (selectedCollider.GetEntity().GetOrderZ() < _colliders[i].GetEntity().GetOrderZ())
                                selectedCollider = _colliders[i];
                        }
                    }
                }

                if (selectedCollider != null)
                {
                    selectedCollider.Grabbed = true;
                    _selectedCollider = selectedCollider;
                }
            }
        }

        public void OnMouseUp(OpenTK.Input.MouseButtonEventArgs e)
        {
            if (e.Button == MouseButton.Left)
            {
                if (_selectedCollider != null)
                {
                    _selectedCollider.Grabbed = false;
                    if (_selectedCollider.IsTrigger)
                    {

                    }
                    _selectedCollider = null;
                }
            }
        }

        public void OnMouseMove(MouseMoveEventArgs e)
        {
            if (_selectedCollider != null)
            {
                if (_selectedCollider.Draggable)
                {
                    Vector3 pos = _selectedCollider.GetEntity().GetTransform().Position;
                    Vector2 movement = StateWindow.Instance.GetMouseMovement();
                    pos.X = (int)pos.X + movement.X;
                    pos.Y = (int)pos.Y + movement.Y;
                    _selectedCollider.GetEntity().GetTransform().Position = pos;
                }
            }
        }

        public void OnMouseWheel(OpenTK.Input.MouseWheelEventArgs e)
        {
        }
    }
}

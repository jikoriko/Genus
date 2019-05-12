using System;
using System.Collections.Generic;

using Genus2D.Listeners;
using Genus2D.Entities;
using Genus2D.GUI;
using Genus2D.Graphics;
using Genus2D.Collision;

using OpenTK;
using OpenTK.Input;

namespace Genus2D.Core
{
    public class State : KeyListener, MouseListener
    {

        protected List<KeyListener> _keyListeners;
        protected List<MouseListener> _mouseListeners;

        protected EntityManager _entityManager;
        public EntityManager EntityManager 
        { 
            get { return _entityManager; } 
            private set { _entityManager = value; } 
        }
        protected List<Control> _controls;

        protected Control _controlToBringToFront = null;

        public State()
        {
            _keyListeners = new List<KeyListener>();
            _mouseListeners = new List<MouseListener>();

            _entityManager = new EntityManager(this);
            _controls = new List<Control>();
        }

        public virtual void AddKeyListener(KeyListener listener)
        {
            if (!_keyListeners.Contains(listener))
                _keyListeners.Add(listener);
        }

        public virtual void RemoveKeyListener(KeyListener listener)
        {
            _keyListeners.Remove(listener);
        }

        public virtual void AddMouseListener(MouseListener listener)
        {
            if (!_mouseListeners.Contains(listener))
                _mouseListeners.Add(listener);
        }

        public virtual void RemoveMouseListener(MouseListener listener)
        {
            _mouseListeners.Remove(listener);
        }

        public List<Control> GetControls()
        {
            return _controls;
        }

        public virtual void AddControl(Control control)
        {
            if (!_controls.Contains(control))
            {
                _controls.Add(control);
                control.SetOrderZ(_controls.Count - 1);
            }
        }

        public virtual void RemoveControl(Control control)
        {
            if (_controls.Contains(control))
            {
                _controls.Remove(control);
                ReOrderControls();
            }
        }

        public virtual void ControlToFront(Control control)
        {
            if (_controls.Contains(control))
                _controlToBringToFront = control;
        }

        public bool GuiSelectable()
        {
            for (int i = 0; i < _controls.Count; i++)
            {
                if (_controls[i].BodySelectable())
                {
                    return true;
                }
            }
            return false;
        }

        private void ReOrderControls()
        {
            for (int i = 0; i < _controls.Count; i++)
            {
                _controls[i].SetOrderZ(i);
            }
        }

        public virtual void OnUpdateFrame(FrameEventArgs e)
        {

            if (_controlToBringToFront != null)
            {
                _controls.Remove(_controlToBringToFront);
                _controls.Add(_controlToBringToFront);
                _controlToBringToFront = null;
                ReOrderControls();
            }

            _entityManager.UpdateEntities(e);

            for (int i = 0; i < _controls.Count; i++)
            {
                _controls[i].Update(e);
                if (_controls[i] is Panel)
                {
                    Panel panel = (Panel)_controls[i];
                    if (panel.IsClosed())
                    {
                        panel.Destroy();
                        i--;
                    }
                }
            }
        }

        public virtual void OnRenderFrame(FrameEventArgs e)
        {
            Renderer.PushWorldMatrix();
            _entityManager.RenderEntities(e);
            Renderer.PopWorldMatrix();

            Renderer.ClearDepthBits();
            Renderer.ClearStencilBits();
            for (int i = 0; i < _controls.Count; i++)
            {
                _controls[i].Render();
            }
        }

        public virtual void OnKeyPress(KeyPressEventArgs e)
        {
            for (int i = 0; i < _keyListeners.Count; i++)
            {
                _keyListeners[i].OnKeyPress(e);
            }
        }

        public virtual void OnKeyDown(KeyboardKeyEventArgs e)
        {
            for (int i = 0; i < _keyListeners.Count; i++)
            {
                _keyListeners[i].OnKeyDown(e);
            }
        }

        public virtual void OnKeyUp(KeyboardKeyEventArgs e)
        {
            for (int i = 0; i < _keyListeners.Count; i++)
            {
                _keyListeners[i].OnKeyUp(e);
            }
        }

        public virtual void OnMouseDown(MouseButtonEventArgs e)
        {
            for (int i = 0; i < _mouseListeners.Count; i++)
            {
                _mouseListeners[i].OnMouseDown(e);
            }
        }

        public virtual void OnMouseUp(MouseButtonEventArgs e)
        {
            for (int i = 0; i < _mouseListeners.Count; i++)
            {
                _mouseListeners[i].OnMouseUp(e);
            }
        }

        public virtual void OnMouseMove(MouseMoveEventArgs e)
        {
            for (int i = 0; i < _mouseListeners.Count; i++)
            {
                _mouseListeners[i].OnMouseMove(e);
            }
        }

        public virtual void OnMouseWheel(MouseWheelEventArgs e)
        {
            for (int i = 0; i < _mouseListeners.Count; i++)
            {
                _mouseListeners[i].OnMouseWheel(e);
            }
        }

        public virtual void Destroy()
        {
            _entityManager.Destroy();
        }

    }
}

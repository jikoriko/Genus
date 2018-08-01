using System;
using System.Collections.Generic;

using Genus2D.Core;
using Genus2D.Graphics;

using OpenTK;
using OpenTK.Input;
using OpenTK.Graphics;

namespace Genus2D.GUI
{
    public class Panel : Control
    {

        public enum BarMode
        {
            Empty,
            Label,
            Close,
            Drag,
            Close_Drag
        }

        protected BarMode _barMode;
        protected PanelBar _topBar;
        protected bool _closed;
        protected bool _strokeBorder;

        protected List<Control> _controls;

        public Panel(int x, int y, int width, int height, BarMode barMode, State state)
            : base(x, y, width, height, state)
        {
            _controls = new List<Control>();
            _barMode = barMode;
            if (_barMode != BarMode.Empty)
            {
                _topBar = new PanelBar(x + _margin + 1, y + _margin + 1, width - ((_margin + 1) * 2), state, this);
            }
            _strokeBorder = true;
            OnSelection += OnPanelSelection;
            _backgroundGradientMode = Renderer.GradientMode.Vertical;
        }

        protected void OnPanelSelection()
        {
            _state.ControlToFront(this);
        }

        public List<Control> GetControls()
        {
            return _controls;
        }

        public override void SetOrderZ(int orderZ)
        {
            base.SetOrderZ(orderZ);
            if (_topBar != null)
                _topBar.SetOrderZ(orderZ);
            if (_controls != null)
            {
                foreach (Control c in _controls)
                {
                    c.SetOrderZ(_controls.IndexOf(c));
                }
            }
        }

        public virtual void AddControl(Control control)
        {
            if (!_controls.Contains(control))
            {
                _controls.Add(control);
                if (control.GetParent() != null)
                {
                    control.GetParent().RemoveControl(control);
                }
                control.SetParent(this);
                control.SetOrderZ(_orderZ + _controls.Count - 1);
            }
        }

        public virtual void RemoveControl(Control control)
        {
            if (_controls.Contains(control))
            {
                _controls.Remove(control);
                control.Destroy();
                ReOrderControls();
            }
        }

        private void ReOrderControls()
        {
            for (int i = 0; i < _controls.Count; i++)
            {
                _controls[i].SetOrderZ(i);
            }
        }

        public BarMode GetBarMode()
        {
            return _barMode;
        }

        public bool IsDisabled()
        {
            return _disabled;
        }

        public bool IsDraggable()
        {
            return (_barMode == BarMode.Drag || _barMode == BarMode.Close_Drag);
        }

        public bool IsClosable()
        {
            return (_barMode == BarMode.Close || _barMode == BarMode.Close_Drag);
        }

        public override void SetPosition(int x, int y)
        {
            base.SetPosition(x, y);
            if (_topBar != null)
                _topBar.SetPosition((int)_bodyPosition.X + _margin + 1, (int)_bodyPosition.Y + _margin + 1);
        }

        public override void SetSize(int width, int height)
        {
            if (_barMode != BarMode.Empty)
            {
                _topBar.SetSize(width - ((_margin + 1) * 2), 0);
            }
            base.SetSize(width, height);
        }

        public override int GetContentX()
        {
            return base.GetContentX() + 2;
        }

        public override int GetContentY()
        {
            int contentY = base.GetContentY() + 2;
            if (_barMode != BarMode.Empty)
            {
                contentY += PanelBar.BAR_HEIGHT + 2;
            }
            return contentY;
        }

        public override int GetContentWidth()
        {
            return base.GetContentWidth() - 4;
        }

        public override int GetContentHeight()
        {
            int contentHeight = base.GetContentHeight() - 4;
            if (_barMode != BarMode.Empty)
            {
                contentHeight -= PanelBar.BAR_HEIGHT + 2;
            }
            return contentHeight;
        }

        public virtual void Close()
        {
            _closed = true;
            if (OnClose != null)
                OnClose();
            this.Destroy();
        }

        public delegate void OnCloseEvent();
        public OnCloseEvent OnClose;

        public virtual bool IsClosed()
        {
            return _closed;
        }

        public override void SetParent(Panel parent)
        {
            base.SetParent(parent);
            if (_topBar != null)
                _topBar.SetParent(parent);
        }

        public void SetPanelLabel(string label)
        {
            if (_topBar != null)
            {
                _topBar.SetLabel(label);
            }
        }

        public override void Update(FrameEventArgs e)
        {
            if (!_disabled)
            {
                base.Update(e);
                for (int i = 0; i < _controls.Count; i++)
                {
                    _controls[i].Update(e);
                    if (_controls[i].Destroyed())
                    {
                        _controls.RemoveAt(i);
                        i--;
                    }
                }
                if (_topBar != null)
                    _topBar.Update(e);
            }
        }

        public override void Render()
        {
            if (!_disabled)
            {
                base.Render();
                if (_topBar != null)
                    _topBar.Render();
                if (_strokeBorder)
                {
                    Color4 borderColour = Renderer.GetDarkerColour(_borderColour);
                    Renderer.DrawRoundedRectangle(ref _bodyPosition, ref _bodySize, _cornerRadius, _margin, ref borderColour);
                }
            }
        }

        protected override void RenderContent()
        {
            if (!_disabled)
            {
                base.RenderContent();
                for (int i = 0; i < _controls.Count; i++)
                {
                    _controls[i].Render();
                }
            }
        }

        public override void Destroy()
        {
            base.Destroy();
            if (_topBar != null)
                _topBar.Destroy();
            for (int i = 0; i < _controls.Count; i++)
            {
                _controls[i].Destroy();
            }
            OnSelection -= OnPanelSelection;
        }
    }
}

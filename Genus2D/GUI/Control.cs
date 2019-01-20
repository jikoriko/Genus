using System;
using System.Collections.Generic;
using System.Drawing;

using Genus2D.Core;
using Genus2D.Graphics;
using Genus2D.Listeners;

using OpenTK;
using OpenTK.Input;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace Genus2D.GUI
{
    public class Control : MouseListener, KeyListener
    {
        protected Panel _parent = null;

        protected Vector3 _bodyPosition, _bodySize;
        protected Rectangle _worldBody;
        protected int _orderZ;

        protected Color4 _borderColour, _backgroundColour;

        protected int _margin = 2;
        protected float _cornerRadius = 10.0f;

        protected bool _pressed, _disabled;
        protected State _state;
        protected bool _destroyed = false;

        protected Renderer.GradientMode _borderGradientMode = Renderer.GradientMode.None;
        protected Renderer.GradientMode _backgroundGradientMode = Renderer.GradientMode.None;

        protected bool _fillBody = true;
        protected bool _scissorClip = true;

        public Control(int x, int y, int width, int height, State state)
        {
            state.AddMouseListener(this);
            state.AddKeyListener(this);

            _bodyPosition = new Vector3();
            _bodySize = new Vector3(1, 1, 1);
            _worldBody = new Rectangle(0, 0, 1, 1);

            SetPosition(x, y);
            SetSize(width, height);

            _borderColour = new Color4(85, 85, 85, 255);// Color4.RoyalBlue;
            _backgroundColour = new Color4(110, 110, 110, 255);// Color4.LightBlue;

            _pressed = _disabled = false;

            _state = state;
        }

        public void SetBorderGradientMode(Renderer.GradientMode gradientMode)
        {
            _borderGradientMode = gradientMode;
        }

        public void SetBackgroundGradientMode(Renderer.GradientMode gradientMode)
        {
            _backgroundGradientMode = gradientMode;
        }

        public virtual void SetBorderColour(Color4 colour)
        {
            _borderColour = colour;
        }

        public virtual Color4 GetBorderColour()
        {
            return _borderColour;
        }

        public virtual void SetBackgroundColour(Color4 colour)
        {
            _backgroundColour = colour;
        }

        public virtual Color4 GetBackgroundColour()
        {
            return _backgroundColour;
        }

        public virtual void SetParent(Panel parent)
        {
            _parent = parent;
            SetPosition((int)_bodyPosition.X, (int)_bodyPosition.Y);
            SetSize((int)_bodySize.X, (int)_bodySize.Y);
        }

        public Panel GetParent()
        {
            return _parent;
        }

        public virtual Vector3 GetBodyPosition()
        {
            return _bodyPosition;
        }

        public virtual int GetMinX()
        {
            return 0;
        }

        public virtual int GetMaxX()
        {
            Vector2 resolution = Renderer.GetResoultion();
            if (_parent == null)
            {
                return (int)resolution.X - (int)_bodySize.X;
            }
            else
            {
                if (_parent is ScrollPanel)
                {
                    ScrollPanel scrollPanel = (ScrollPanel)_parent;
                    return (int)(scrollPanel.GetScrollableDimensions().X - _bodySize.X);
                }
                else
                {
                    return _parent.GetContentWidth() - (int)_bodySize.X;
                }
            }
        }

        public virtual int GetMinY()
        {
            return 0;
        }

        public virtual int GetMaxY()
        {
            Vector2 resolution = Renderer.GetResoultion();
            if (_parent == null)
            {
                return (int)resolution.Y - (int)_bodySize.Y;
            }
            else
            {
                if (_parent is ScrollPanel)
                {
                    ScrollPanel scrollPanel = (ScrollPanel)_parent;
                    return (int)(scrollPanel.GetScrollableDimensions().Y - _bodySize.Y);
                }
                else
                {
                    return _parent.GetContentHeight() - (int)_bodySize.Y;
                }
            }
        }

        public virtual void SetOrderZ(int orderZ)
        {
            _orderZ = orderZ;
        }

        public virtual int GetOrderZ()
        {
            int orderZ = _orderZ;
            if (_parent != null)
                orderZ += _parent.GetOrderZ();// + 1;
            return orderZ;
        }

        public virtual void SwapOrderZ(Control otherControl)
        {
            int tempZ = _orderZ;
            _orderZ = otherControl.GetOrderZ();
            otherControl.SetOrderZ(tempZ);
        }

        public virtual void Move(int x, int y)
        {
            SetPosition((int)_bodyPosition.X + x, (int)_bodyPosition.Y + y);
        }


        public virtual void SetPosition(int x, int y)
        {
            x = Math.Min(x, GetMaxX());
            x = Math.Max(x, GetMinX());
            y = Math.Min(y, GetMaxY());
            y = Math.Max(y, GetMinY());

            _bodyPosition.X = x;
            _bodyPosition.Y = y;
        }

        /// <summary>
        /// Sets the position of the control.
        /// </summary>
        /// <param name="x">X position.</param>
        /// <param name="y">Y position.</param>
        /// <param name="capPos">Keep control inside window or let control slip beyond window border.</param>
        public virtual void SetPosition(int x, int y, bool capPos)
        {
            if (capPos)
            {
                x = Math.Min(x, GetMaxX());
                x = Math.Max(x, GetMinX());
                y = Math.Min(y, GetMaxY());
                y = Math.Max(y, GetMinY());
            }

            _bodyPosition.X = x;
            _bodyPosition.Y = y;
        }

        public virtual Vector3 GetBodySize()
        {
            return _bodySize;
        }

        public virtual int GetContentX()
        {
            return (int)_bodyPosition.X + _margin;
        }

        public virtual int GetContentY()
        {
            return (int)_bodyPosition.Y + _margin;
        }

        public virtual int GetContentWidth()
        {
            return (int)_bodySize.X - (_margin * 2);
        }

        public virtual int GetContentHeight()
        {
            return (int)_bodySize.Y - (_margin * 2);
        }

        public virtual void SetSize(int width, int height)
        {
            _bodySize.X = width;
            _bodySize.Y = height;
            _worldBody.Width = width;
            _worldBody.Height = height;
        }

        public virtual void SetContentSize(int width, int height)
        {
            int prevContentWidth = GetContentWidth();
            int prevContentHeight = GetContentHeight();

            int baseWidth = (int)GetBodySize().X - prevContentWidth;
            int baseHeight = (int)GetBodySize().Y - prevContentHeight;

            SetSize(baseWidth + width, baseHeight + height);
        }

        public virtual void SetMargin(int margin)
        {
            if (margin <= 0)
                return;
            _margin = margin;
            SetPosition((int)_bodyPosition.X, (int)_bodyPosition.Y);
            SetSize((int)_bodySize.X, (int)_bodySize.Y);
        }

        public virtual Vector3 GetContentPosition()
        {
            return new Vector3(GetContentX(), GetContentY(), _bodyPosition.Z);
        }

        public virtual Vector3 GetContentSize()
        {
            return new Vector3(GetContentWidth(), GetContentHeight(), 1);
        }

        public virtual Vector3 GetWorldBodyPosition()
        {
            Vector3 bodyPos = _bodyPosition;
            if (_parent != null)
                bodyPos += _parent.GetWorldContentPosition();
            if (_parent is ScrollPanel)
            {
                ScrollPanel scrollPanel = (ScrollPanel)_parent;
                bodyPos += new Vector3(scrollPanel.GetScrolledAmount());
            }
            return bodyPos;
        }

        public virtual Vector3 GetWorldContentPosition()
        {
            Vector3 contentPos = GetContentPosition();
            if (_parent != null)
                contentPos += _parent.GetWorldContentPosition();
            if (_parent is ScrollPanel)
            {
                ScrollPanel scrollPanel = (ScrollPanel)_parent;
                Vector3 scroll = new Vector3(scrollPanel.GetScrolledAmount());
                contentPos += scroll;
            }
            return contentPos;
        }

        public virtual void Enable()
        {
            _disabled = false;
        }

        public virtual void Disable()
        {
            _disabled = true;
        }

        public bool IsPressed()
        {
            return _pressed;
        }

        public virtual bool BodySelectable()
        {
            if (_disabled || _destroyed)
                return false;
            else
            {
                if (_parent != null)
                {
                    if (!_parent.ContentSelectable())
                        return false;
                }
                if (MouseInsideBody())
                {
                    List<Control> controls;
                    if (_parent == null)
                    {
                        controls = _state.GetControls();
                    }
                    else
                    {
                        controls = _parent.GetControls();
                    }

                    for (int i = 0; i < controls.Count; i++)
                    {
                        if (controls[i] == this)
                            continue;
                        if (controls[i].MouseInsideBody())
                        {
                            if (controls[i].GetOrderZ() > GetOrderZ())
                                return false;
                        }
                    }
                    return true;
                }
            }

            return false;
        }

        public bool MouseInsideBody()
        {
            Vector2 mouse = StateWindow.Instance.GetMousePosition();
            Vector3 start = GetWorldBodyPosition();
            Vector3 end = start + _bodySize;
            if (mouse.X >= start.X && mouse.X < end.X && mouse.Y >= start.Y && mouse.Y < end.Y)
                return true;
            return false;
        }

        public virtual bool ContentSelectable()
        {
            if (_disabled)
                return false;
            else
            {
                if (!BodySelectable())
                    return false;
                if (_parent != null)
                {
                    if (!_parent.ContentSelectable())
                        return false;
                }
                if (MouseInsideContent())
                {
                    return true;
                }

            }

            return false;
        }

        public bool MouseInsideContent()
        {
            Vector2 mouse = StateWindow.Instance.GetMousePosition();
            Vector3 start = GetWorldContentPosition();
            Vector3 end = start + GetContentSize();
            if (mouse.X >= start.X && mouse.X < end.X && mouse.Y >= start.Y && mouse.Y < end.Y)
                return true;
            return false;
        }

        public virtual void OnMouseDown(MouseButtonEventArgs e)
        {
            if (e.Button == MouseButton.Left)
            {
                if (BodySelectable() && !_disabled)
                {
                    _pressed = true;
                    if (OnSelection != null)
                        OnSelection();
                }
            }
        }

        public virtual void OnMouseUp(MouseButtonEventArgs e)
        {
            if (e.Button == MouseButton.Left)
            {
                if (_pressed && BodySelectable() && !_disabled)
                {
                    if (OnTrigger != null)
                        OnTrigger();
                }
                _pressed = false;
            }
        }

        public virtual void OnMouseMove(MouseMoveEventArgs e)
        {
        }

        public virtual void OnMouseWheel(MouseWheelEventArgs e)
        {
        }

        public virtual void OnKeyPress(KeyPressEventArgs e)
        {
        }

        public virtual void OnKeyDown(KeyboardKeyEventArgs e)
        {
        }

        public virtual void OnKeyUp(KeyboardKeyEventArgs e)
        {
        }

        public delegate void TriggerEventHandler();
        public event TriggerEventHandler OnTrigger;

        public delegate void SelectionEventHandler();
        public event SelectionEventHandler OnSelection;

        public virtual void Update(FrameEventArgs e)
        {
        }

        public virtual void Render()
        {
            if (_disabled)
                return;

            Color4 borderColour = GetBorderColour();
            Color4 backgroundColour = GetBackgroundColour();
            _worldBody.X = (int)GetWorldBodyPosition().X;
            _worldBody.Y = (int)GetWorldBodyPosition().Y;

            Renderer.PushScreenClip(_worldBody, _scissorClip);

            Renderer.SetGradientMode(_borderGradientMode);
            Color4 endColour = Renderer.GetLighterColour(borderColour);

            if (!_fillBody)
                Renderer.DisableColourWrite();

            Renderer.FillRoundedRectangle(ref _bodyPosition, ref _bodySize, _cornerRadius, ref borderColour, ref endColour);
            Renderer.EnableColourWrite();

            Renderer.PushStencilDepth(StencilOp.Incr, StencilFunction.Lequal);
            Vector3 contentPos = GetContentPosition();
            Vector3 contentSize = GetContentSize();
            Renderer.SetGradientMode(_backgroundGradientMode);
            endColour = Renderer.GetLighterColour(backgroundColour, 1.3f);

            if (!_fillBody)
                Renderer.DisableColourWrite();
            Renderer.FillRoundedRectangle(ref contentPos, ref contentSize, _cornerRadius, ref backgroundColour, ref endColour);
            Renderer.EnableColourWrite();

            Renderer.SetStencilOp(StencilOp.Keep);
            Renderer.SetStencilFunction(StencilFunction.Less);

            Renderer.SetGradientMode(Renderer.GradientMode.None);

            Renderer.PushWorldMatrix();
            Renderer.TranslateWorld(contentPos);

            RenderContent();

            Renderer.PopWorldMatrix();
            Renderer.PopStencilDepth();
            Renderer.PopScreenClip();

        }

        protected virtual void RenderContent()
        {
        }

        public virtual void Destroy()
        {
            if (!_destroyed)
            {
                if (_parent == null)
                    _state.RemoveControl(this);
                else
                    _parent.RemoveControl(this);

                _state.RemoveKeyListener(this);
                _state.RemoveMouseListener(this);
                _destroyed = true;
            }
        }

        public bool Destroyed()
        {
            return _destroyed;
        }

    }
}
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Genus2D.Graphics;
using Genus2D.Utililities;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Input;

namespace Genus2D.Core
{
    /// <summary>
    ///                   StateWindow - Handles state managment for a render window, 
    ///                   calls update and render behaviour for current state,
    ///                   also handles event calls to current state as calling events.
    /// </summary>
    public class StateWindow : GameWindow
    {

        public static StateWindow Instance;
        private List<State> _stateList;

        private Vector2 _curMousePosition, _lastMousePosition;
        private bool _mouseInitialized;

        public StateWindow(int width, int height, string title, GameWindowFlags windowFlags)
            : base(width, height, new GraphicsMode(32, 24, 8, 4), title, windowFlags, DisplayDevice.Default, 3, 1, GraphicsContextFlags.ForwardCompatible)
        {
            Instance = this;
            _stateList = new List<State>();
            _mouseInitialized = false;
            Renderer.Initialize();
        }

        public double GetFPS()
        {
            return RenderFrequency;
        }

        public virtual void PushState(State state)
        {
            _stateList.Add(state);
        }

        public virtual void PopState()
        {
            if (_stateList.Count > 1)
                _stateList.RemoveAt(_stateList.Count - 1);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            base.OnKeyPress(e);
            if (!this.Focused) return;
            if (_stateList.Count > 0)
                _stateList[_stateList.Count - 1].OnKeyPress(e);
        }

        protected override void OnKeyDown(KeyboardKeyEventArgs e)
        {
            if (!this.Focused) return;
            base.OnKeyDown(e);
            if (_stateList.Count > 0)
                _stateList[_stateList.Count - 1].OnKeyDown(e);
        }

        protected override void OnKeyUp(KeyboardKeyEventArgs e)
        {
            if (!this.Focused) return;
            base.OnKeyUp(e);
            if (_stateList.Count > 0)
                _stateList[_stateList.Count - 1].OnKeyUp(e);
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            _curMousePosition = new Vector2(Mouse.X, Mouse.Y);
            _lastMousePosition = _curMousePosition;
            _mouseInitialized = true;
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            //do we even need to do anything here?
        }

        protected override void OnMouseMove(MouseMoveEventArgs e)
        {
            base.OnMouseMove(e);
            if (!_mouseInitialized)
            {
                _curMousePosition = new Vector2(Mouse.X, Mouse.Y);
                _lastMousePosition = _curMousePosition;
                _mouseInitialized = true;
            }
            else
            {
                _lastMousePosition.X = _curMousePosition.X;
                _lastMousePosition.Y = _curMousePosition.Y;
                _curMousePosition.X = e.X;
                _curMousePosition.Y = e.Y;
            }

            if (!this.Focused) return;
            if (_stateList.Count > 0)
                _stateList[_stateList.Count - 1].OnMouseMove(e);
        }

        public Vector2 GetMousePosition()
        {
            Vector2 mousePos = _curMousePosition;

            Vector2 viewportPos = Renderer.GetViewportPosition();
            mousePos -= viewportPos;

            Vector2 resolution = Renderer.GetResoultion();
            Vector2 viewportSize = Renderer.GetViewportSize();
            Vector2 scale = new Vector2(resolution.X / viewportSize.X, resolution.Y / viewportSize.Y);
            mousePos *= scale;

            return mousePos;
        }

        public Vector2 GetLastMousePosition()
        {
            Vector2 mousePos = _lastMousePosition;

            Vector2 viewportPos = Renderer.GetViewportPosition();
            mousePos -= viewportPos;

            Vector2 resolution = Renderer.GetResoultion();
            Vector2 viewportSize = Renderer.GetViewportSize();
            Vector2 scale = new Vector2(resolution.X / viewportSize.X, resolution.Y / viewportSize.Y);
            mousePos *= scale;

            return mousePos;
        }

        public Vector2 GetMouseMovement()
        {
            return GetMousePosition() - GetLastMousePosition();
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            if (_stateList.Count > 0)
                _stateList[_stateList.Count - 1].OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);
            if (_stateList.Count > 0)
                _stateList[_stateList.Count - 1].OnMouseUp(e);
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);
            if (!this.Focused) return;
            if (_stateList.Count > 0)
                _stateList[_stateList.Count - 1].OnMouseWheel(e);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            Renderer.SetScreenBounds(this.ClientRectangle);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            if (!this.Focused) return;
            base.OnUpdateFrame(e);

            if (_stateList.Count > 0)
                _stateList[_stateList.Count - 1].OnUpdateFrame(e);
        }
        
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            Renderer.Clear();

            if (_stateList.Count > 0)
                _stateList[_stateList.Count - 1].OnRenderFrame(e);

            this.SwapBuffers();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            Assets.Destroy();
            ShapeFactory.Destroy();
            for (int i = 0; i < _stateList.Count; i++)
            {
                _stateList[i].Destroy();
            }
        }


    }
}

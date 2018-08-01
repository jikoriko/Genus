using System;

using Genus2D.Core;
using Genus2D.Graphics;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Input;

namespace Genus2D.GUI
{
    public class ScrollPanel : Panel
    {

        protected ScrollBar _horizontalBar, _verticalBar;
        protected bool _horizontalScrollEnabled, _verticalScrollEnabled;

        public ScrollPanel(int x, int y, int width, int height, BarMode barMode, State state)
            : base(x, y, width, height, barMode, state)
        {
            _horizontalScrollEnabled = true;
            _verticalScrollEnabled = true;
            _horizontalBar = new ScrollBar(state, this, ScrollBar.SliderType.Horizontal);
            _verticalBar = new ScrollBar(state, this, ScrollBar.SliderType.Vertical);
        }

        public override void SetParent(Panel parent)
        {
            base.SetParent(parent);
            _horizontalBar.SetParent(parent);
            _verticalBar.SetParent(parent);
        }

        public override void SetOrderZ(int orderZ)
        {
            base.SetOrderZ(orderZ);
            _horizontalBar.SetOrderZ(orderZ);
            _verticalBar.SetOrderZ(orderZ);
        }

        public override int GetContentWidth()
        {
            int width = base.GetContentWidth();
            if (_verticalScrollEnabled)
                width -= ScrollBar.BAR_WIDTH;
            return width;
        }

        public override int GetContentHeight()
        {
            int height = base.GetContentHeight();
            if (_horizontalScrollEnabled)
            {
                height -= ScrollBar.BAR_WIDTH;
            }
            return height;
        }

        public bool HorizontalScrollEnabled()
        {
            return _horizontalScrollEnabled;
        }

        public virtual void EnableHorizontalScroll()
        {
            _horizontalScrollEnabled = true;
            SetSize((int)_bodySize.X, (int)_bodySize.Y);
        }

        public virtual void DisableHorizontalScroll()
        {
            _horizontalScrollEnabled = false;
            SetSize((int)_bodySize.X, (int)_bodySize.Y);
        }

        public bool VerticalScrollEnabled()
        {
            return _verticalScrollEnabled;
        }

        public virtual void EnableVerticalScroll()
        {
            _verticalScrollEnabled = true;
            SetSize((int)_bodySize.X, (int)_bodySize.Y);
        }

        public virtual void DisableVerticalScroll()
        {
            _verticalScrollEnabled = false;
            SetSize((int)_bodySize.X, (int)_bodySize.Y);
        }

        public int GetScrollableWidth()
        {
            return _horizontalBar.GetScrollAmount();
        }

        public virtual void SetScrollableWidth(int scrollWidth)
        {
            _horizontalBar.SetScrollAmount(scrollWidth);
        }

        public int GetScrollableHeight()
        {
            return _verticalBar.GetScrollAmount();
        }

        public virtual void SetScrollableHeight(int scrollHeight)
        {
            _verticalBar.SetScrollAmount(scrollHeight);
        }

        public virtual Vector2 GetScrollableDimensions()
        {
            return new Vector2(GetScrollableWidth(), GetScrollableHeight());
        }

        public virtual void SetScrollDimensions(int scrollWidth, int scrollHeight)
        {
            SetScrollableWidth(scrollWidth);
            SetScrollableHeight(scrollHeight);
        }

        public virtual Vector2 GetScrolledAmount()
        {
            return new Vector2(_horizontalBar.GetRelativeScroll(), _verticalBar.GetRelativeScroll());
        }

        public Vector2 GetRelativeContentPosition()
        {
            return GetWorldContentPosition().Xy + GetScrolledAmount();
        }

        public override void Move(int x, int y)
        {
            base.Move(x, y);
            //if (_horizontalBar != null)
            _horizontalBar.RePosition();
            //if (_verticalBar != null)
            _verticalBar.RePosition();
        }

        public override void SetPosition(int x, int y)
        {
            base.SetPosition(x, y);
        }

        public override void SetSize(int width, int height)
        {
            base.SetSize(width, height);
            if (_horizontalBar != null)
                _horizontalBar.Resize();
            if (_verticalBar != null)
                _verticalBar.Resize();
        }

        public override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);
            if (BodySelectable())
            {
                if (e.Delta > 0) // scroll up
                    _verticalBar.ScrollSlider(-10);
                else // scroll down
                    _verticalBar.ScrollSlider(10);
            }
        }

        public override void Update(FrameEventArgs e)
        {
            base.Update(e);
            if (_horizontalScrollEnabled)
                _horizontalBar.Update(e);
            if (_verticalScrollEnabled)
                _verticalBar.Update(e);
        }

        public override void Render()
        {
            base.Render();
            if (_horizontalScrollEnabled)
                _horizontalBar.Render();
            if (_verticalScrollEnabled)
                _verticalBar.Render();
        }

        protected override void RenderContent()
        {
            Renderer.TranslateWorld(new Vector3(_horizontalBar.GetRelativeScroll(), _verticalBar.GetRelativeScroll(), 0));
            base.RenderContent();
        }

        public override void Destroy()
        {
            base.Destroy();
            _horizontalBar.Destroy();
            _verticalBar.Destroy();
        }
    }
}

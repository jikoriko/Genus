using System;

using Genus2D.Core;
using Genus2D.Graphics;

using OpenTK;
using OpenTK.Graphics;

namespace Genus2D.GUI
{
    public class ScrollBar : Control
    {
        public static readonly int BAR_WIDTH = 30;

        protected ScrollPanel _scrollPanel;
        protected Vector3 _sliderPosition, _sliderSize;
        protected bool _sliderInitialized;
        protected int _scrollableAmount;
        protected bool _grabbed;

        public enum SliderType
        {
            Horizontal, Vertical
        }
        protected SliderType _sliderType;

        public ScrollBar(State state, ScrollPanel parentPanel, SliderType sliderType)
            : base(0, 0, 1, 1, state)
        {
            _scrollPanel = parentPanel;
            _sliderType = sliderType;
            _scrollableAmount = 0;

            _sliderPosition = Vector3.Zero;

            _sliderInitialized = false;

            this.SetPosition(GetBodyX(), GetBodyY());
            this.SetSize(GetBodyWidth(), GetBodyHeight());

            _grabbed = false;
            _backgroundColour = Color4.SlateGray;
        }

        public void RePosition()
        {
            this.SetPosition(GetBodyX(), GetBodyY());
        }

        public void Resize()
        {
            this.SetSize(GetBodyWidth(), GetBodyHeight());
        }

        protected int GetBodyX()
        {
            int x;

            if (_sliderType == SliderType.Horizontal)
            {
                x = (int)_scrollPanel.GetContentPosition().X - _margin;
            }
            else
            {
                x = (int)(_scrollPanel.GetContentPosition().X + _scrollPanel.GetContentSize().X) + 1;
            }

            return x;
        }

        protected int GetBodyY()
        {
            int y;

            if (_sliderType == SliderType.Horizontal)
            {
                y = (int)(_scrollPanel.GetContentPosition().Y + _scrollPanel.GetContentSize().Y) + 1;
            }
            else
            {
                y = (int)_scrollPanel.GetContentPosition().Y - _margin;
            }

            return y;
        }

        protected int GetBodyWidth()
        {
            int width;

            if (_sliderType == SliderType.Horizontal)
            {
                width = (int)_scrollPanel.GetContentSize().X + (_margin * 2);
            }
            else
            {
                width = BAR_WIDTH;
            }

            return width;
        }

        protected int GetBodyHeight()
        {
            int height;

            if (_sliderType == SliderType.Horizontal)
            {
                height = BAR_WIDTH;
            }
            else
            {
                height = (int)_scrollPanel.GetContentSize().Y + (_margin * 2);
            }

            return height;
        }

        public override void SetSize(int width, int height)
        {
            base.SetSize(width, height);
            if (!_sliderInitialized)
            {
                _sliderSize = new Vector3(GetContentWidth(), GetContentHeight(), 1);
                _sliderInitialized = true;
            }
            this.SetScrollAmount(_scrollableAmount);

        }

        public int GetScrollAmount()
        {
            return _scrollableAmount;
        }

        protected void CapSliderPosition()
        {
            if (_sliderType == SliderType.Horizontal)
            {
                if (_sliderPosition.X < 0)
                    _sliderPosition.X = 0;
                else if (_sliderPosition.X + _sliderSize.X > GetContentWidth())
                {
                    int difference = (int)(_sliderPosition.X + _sliderSize.X - GetContentWidth());
                    _sliderPosition.X -= difference;
                }
            }
            else
            {
                if (_sliderPosition.Y < 0)
                    _sliderPosition.Y = 0;
                else if (_sliderPosition.Y + _sliderSize.Y > GetContentHeight())
                {
                    int difference = (int)(_sliderPosition.Y + _sliderSize.Y - GetContentHeight());
                    _sliderPosition.Y -= difference;
                }
            }

        }

        public void SetScrollAmount(int amount)
        {
            if (_sliderType == SliderType.Horizontal)
            {
                if (amount < GetContentWidth())
                    amount = (int)GetContentWidth();
                _scrollableAmount = amount;
                _sliderSize.X = this.GetSliderSize();
                CapSliderPosition();
            }
            else
            {
                if (amount < GetContentHeight())
                    amount = (int)GetContentHeight();
                _scrollableAmount = amount;
                _sliderSize.Y = this.GetSliderSize();
                CapSliderPosition();
            }

        }

        public int GetSliderSize()
        {
            float size;
            if (_sliderType == SliderType.Horizontal)
            {
                float percent = GetContentWidth() / (float)_scrollableAmount;
                size = percent * GetContentWidth();
            }
            else
            {
                float percent = GetContentHeight() / (float)_scrollableAmount;
                size = percent * GetContentHeight();
            }

            if (size < 20)
                size = 20;

            return (int)size;
        }

        private bool IsScrollable()
        {
            if (_sliderType == SliderType.Horizontal)
            {
                if (_scrollableAmount <= GetContentWidth())
                    return false;
            }
            else
            {
                if (_scrollableAmount <= GetContentHeight())
                    return false;
            }

            if (_grabbed)
                return true;
            else if (!this.BodySelectable())
                return false;

            Vector2 mouse = StateWindow.Instance.GetMousePosition();

            Vector3 sliderPos = GetWorldContentPosition() + _sliderPosition;

            return (mouse.X >= sliderPos.X && mouse.X < sliderPos.X + _sliderSize.X &&
                            mouse.Y >= sliderPos.Y && mouse.Y < sliderPos.Y + _sliderSize.Y);

        }

        public void SetSliderScroll(int amount)
        {
            if (_sliderType == SliderType.Horizontal)
            {
                _sliderPosition.X = amount;
            }
            else
            {
                _sliderPosition.Y = amount;
            }
            CapSliderPosition();
        }

        public void ScrollSlider(int amount)
        {
            if (_sliderType == SliderType.Horizontal)
            {
                _sliderPosition.X += amount;
            }
            else
            {
                _sliderPosition.Y += amount;
            }
            CapSliderPosition();
        }

        public int GetRelativeScroll()
        {
            float scroll = 0;
            if (_sliderType == SliderType.Horizontal)
            {
                float trackWidth = GetContentWidth() - _sliderSize.X;
                if (trackWidth > 0)
                {
                    float scale = (float)_scrollableAmount - GetContentWidth();
                    scale /= trackWidth;
                    scroll = -(_sliderPosition.X * scale);
                }
            }
            else
            {
                float trackWidth = GetContentHeight() - _sliderSize.Y;
                if (trackWidth > 0)
                {
                    float scale = (float)_scrollableAmount - GetContentHeight();
                    scale /= trackWidth;
                    scroll = -(_sliderPosition.Y * scale);
                }
            }
            return (int)scroll;
        }

        public override void Update(FrameEventArgs e)
        {
            base.Update(e);

            CapSliderPosition();
        }

        public override void OnMouseDown(OpenTK.Input.MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            if (IsScrollable() && e.Button == OpenTK.Input.MouseButton.Left)
                _grabbed = true;
        }

        public override void OnMouseUp(OpenTK.Input.MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);
            if (e.Button == OpenTK.Input.MouseButton.Left)
                _grabbed = false;
        }

        public override void OnMouseMove(OpenTK.Input.MouseMoveEventArgs e)
        {
            base.OnMouseMove(e);
            if (_grabbed)
            {
                Vector2 mouseMovement = StateWindow.Instance.GetMouseMovement();
                if (_sliderType == SliderType.Horizontal)
                {
                    _sliderPosition.X += mouseMovement.X;
                    if (_sliderPosition.X < 0)
                        _sliderPosition.X = 0;
                    else if (_sliderPosition.X + _sliderSize.X > GetContentWidth())
                        _sliderPosition.X = GetContentWidth() - _sliderSize.X;
                }
                else
                {
                    _sliderPosition.Y += mouseMovement.Y;
                    if (_sliderPosition.Y < 0)
                        _sliderPosition.Y = 0;
                    else if (_sliderPosition.Y + _sliderSize.Y > GetContentHeight())
                        _sliderPosition.Y = GetContentHeight() - _sliderSize.Y;
                }
            }
        }

        protected override void RenderContent()
        {

            base.RenderContent();
            Color4 colour = Renderer.GetLighterColour(_borderColour);
            Color4 endColour = Renderer.GetLighterColour(colour);
            if (_sliderType == SliderType.Horizontal)
                Renderer.SetGradientMode(Renderer.GradientMode.VerticalMidBand);
            else
                Renderer.SetGradientMode(Renderer.GradientMode.HorizontalMidBand);
            Renderer.FillRoundedRectangle(ref _sliderPosition, ref _sliderSize, _cornerRadius, ref colour, ref endColour);
            Renderer.SetGradientMode(Renderer.GradientMode.None);
            colour = Renderer.GetDarkerColour(_borderColour);
            Renderer.DrawRoundedRectangle(ref _sliderPosition, ref _sliderSize, _cornerRadius, 2, ref colour);
        }

    }
}

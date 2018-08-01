using System;
using System.Drawing;

using OpenTK;
using OpenTK.Input;
using OpenTK.Graphics;

using Genus2D.States;
using Genus2D.Graphics;

namespace Genus2D.GUI
{
    public class Slider : Control
    {
        public static readonly int SLIDER_HEIGHT = 24;

        private float _maxValue;
        private Rectangle _slider;
        private bool _sliderGrabbed;

        private Color4 _sliderColour;

        public Slider(int x, int y, int width, State state)
            : base(x, y, width, SLIDER_HEIGHT, state)
        {
            _maxValue = 1.0f;
            _slider = new Rectangle(0, 0, GetContentHeight(), GetContentHeight());
            _sliderGrabbed = false;
            _sliderColour = Color4.RoyalBlue;
            _backgroundColour = Color4.LightGray;
        }

        private int GetMaxSliderX()
        {
            return GetContentWidth() - _slider.Width;
        }

        public void SetMaxValue(float max)
        {
            _maxValue = max;
        }

        public void SetValue(float value)
        {
            if (value < 0)
                value = 0;
            else if (value > _maxValue)
                value = _maxValue;

            float percentage = value / _maxValue;

            int x = (int)(GetMaxSliderX() * percentage);
            _slider.X = x;

            if (OnValueChange != null)
                OnValueChange(GetSliderValue());
        }

        public float GetSliderValue()
        {
            float proggression = (float)_slider.X / GetMaxSliderX();
            return proggression * _maxValue;
        }

        public delegate void OnValueChaneEvent(float value);
        public OnValueChaneEvent OnValueChange;

        public void SetSliderColour(Color4 colour)
        {
            _sliderColour = colour;
        }

        public override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button == MouseButton.Left)
            {
                if (ContentSelectable())
                {
                    Vector2 mouse = StateWindow.Instance.GetMousePosition();
                    mouse.X -= GetWorldContentPosition().X;
                    mouse.Y -= GetWorldContentPosition().Y;

                    if (mouse.X >= _slider.X && mouse.X < _slider.Right && mouse.Y >= _slider.Y && mouse.Y < _slider.Bottom)
                    {
                        _sliderGrabbed = true;
                    }
                }
            }
        }

        public override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);

            if (e.Button == MouseButton.Left)
            {
                _sliderGrabbed = false;
            }
        }

        public override void OnMouseMove(MouseMoveEventArgs e)
        {
            base.OnMouseMove(e);
            if (_sliderGrabbed)
            {
                Vector2 mouseMovement = StateWindow.Instance.GetMouseMovement();
                _slider.X += (int)mouseMovement.X;
                _slider.X = Math.Max(0, _slider.X);
                _slider.X = Math.Min(GetMaxSliderX(), _slider.X);

                if (OnValueChange != null)
                    OnValueChange(GetSliderValue());
            }
        }

        protected override void RenderContent()
        {
            base.RenderContent();

            Vector3 sliderPos = new Vector3(_slider.X, _slider.Y, 0);
            Vector3 sliderScale = new Vector3(_slider.Width, _slider.Height, 1);

            Color4 colour = _sliderColour;
            Renderer.FillRoundedRectangle(ref sliderPos, ref sliderScale, sliderScale.X / 2, ref colour);

            colour = Renderer.GetDarkerColour(colour);
            Renderer.DrawRoundedRectangle(ref sliderPos, ref sliderScale, sliderScale.X / 2, 2f, ref colour);

        }
    }
}

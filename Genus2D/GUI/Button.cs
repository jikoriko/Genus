using System;

using Genus2D.Core;
using Genus2D.Graphics;

using OpenTK;
using OpenTK.Graphics;

namespace Genus2D.GUI
{
    public class Button : Control
    {

        protected Color4 _textStartColour, _textEndColour;
        protected Color4 _imageColour;
        protected string _label;
        protected Texture _buttonImage;

        public Button(string label, int x, int y, int width, int height, State state)
            : base(x, y, width, height, state)
        {
            _label = label;
            _textStartColour = Color4.White;
            _textEndColour = Color4.Black;
            //_backgroundColour = Renderer.GetLighterColour(_borderColour);
            _backgroundColour = Color4.RoyalBlue;
            _borderColour = Renderer.GetDarkerColour(_backgroundColour);
            _imageColour = Color4.Black;

            _backgroundGradientMode = Renderer.GradientMode.VerticalMidBand;
        }

        /*
        public override Color4 GetBorderColour()
        {
            if (_pressed)
                return _pressedColour;
            else if (BodySelectable())
                return _hoverColour;
            return base.GetBorderColour();
        }
        */

        public override Color4 GetBackgroundColour()
        {
            if (_pressed)
            {
                return Renderer.GetLighterColour(_backgroundColour);
            }
            else if (BodySelectable())
            {
                return Renderer.GetDarkerColour(_backgroundColour);
            }
            return base.GetBackgroundColour();
        }

        public void SetButtonImage(Texture image)
        {
            _buttonImage = image;
        }

        public void SetImageColour(Color4 colour)
        {
            _imageColour = colour;
        }

        protected override void RenderContent()
        {
            base.RenderContent();

            if (_buttonImage != null)
            {
                int imgWidth = GetContentWidth() - 2;
                int imgHeight = GetContentHeight() - 2;
                int imgX = (GetContentWidth() / 2) - (imgWidth / 2);
                int imgY = (GetContentHeight() / 2) - (imgHeight / 2);
                Vector3 pos = new Vector3(imgX, imgY, 0);
                Vector3 scale = new Vector3(imgWidth, imgHeight, 1);
                Renderer.FillTexture(_buttonImage, ShapeFactory.Rectangle, ref pos, ref scale, ref _imageColour);
            }

            int textWidth = Renderer.GetFont().GetTextWidth(_label);
            int textHeight = Renderer.GetFont().GetTextHeight(_label);
            int cX = (GetContentWidth() / 2) - (textWidth / 2);
            int cY = (GetContentHeight() / 2) - (textHeight / 2);

            Vector3 textPos = new Vector3(cX, cY, 0);
            //Renderer.SetGradientMode(Renderer.GradientMode.Vertical);
            Renderer.PrintText(_label, ref textPos, ref _textStartColour, ref _textEndColour);
            //Renderer.SetGradientMode(Renderer.GradientMode.None);
        }
    }
}

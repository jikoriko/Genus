using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Genus2D.Core;
using Genus2D.Graphics;
using OpenTK.Graphics;
using OpenTK;

namespace Genus2D.GUI
{
    public class Label : Control
    {
        public enum TextAllign
        {
            Left, Center, Right
        }

        protected string _text;
        protected TextAllign _textAllign;
        protected Color4 _textColour;

        public Label(int x, int y, int width, int height, State state) 
            : base(x, y, width, height, state)
        {
            SetMargin(0);
            _cornerRadius = 0;
            _fillBody = false;
            _textAllign = TextAllign.Left;
            _textColour = Color4.Black;
        }

        public string GetText()
        {
            return _text;
        }

        public void SetText(string text)
        {
            _text = text;
        }

        public TextAllign GetTextAllign()
        {
            return _textAllign;
        }

        public void SetTextAllign(TextAllign allign)
        {
            _textAllign = allign;
        }

        public Color4 GetTextColour()
        {
            return _textColour;
        }

        protected override void RenderContent()
        {
            base.RenderContent();

            Vector3 pos = Vector3.Zero;
            switch (_textAllign)
            {
                case TextAllign.Center:
                    pos.X = (GetContentWidth() / 2) - (Renderer.GetFont().GetTextWidth(_text) / 2);
                    break;
                case TextAllign.Right:
                    pos.X = GetContentWidth() - Renderer.GetFont().GetTextWidth(_text);
                    break;
            }
            //pos.Y = (GetContentHeight() / 2) - (Renderer.GetFont().GetTextHeight(_text) / 2);

            Renderer.PrintText(_text, ref pos, ref _textColour);

        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Genus2D.Core;
using Genus2D.Graphics;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Input;

namespace Genus2D.GUI
{
    public class MessageBox : ScrollPanel
    {
        private Label _label;
        float anim = 0.0f;

        private List<string> _options;
        private int _selectedOption;

        public MessageBox(string message, State state, bool closable = true) 
            : base((int)(Renderer.GetResoultion().X / 2) - 150, (int)(Renderer.GetResoultion().Y / 2) - 100, 300, 200, closable ? BarMode.Close_Drag : BarMode.Empty, state)
        {
            DisableHorizontalScroll();

            int textHeight = Renderer.GetFont().GetTextHeight(message) + 20;
            SetScrollableHeight(textHeight);
            
            if (GetScrollableHeight() <= GetContentHeight())
                DisableVerticalScroll();
            else
                EnableVerticalScroll();

            _label = new Label(0, 10, GetContentWidth(), textHeight, state);
            _label.SetText(message);
            _label.SetTextAllign(Label.TextAllign.Center);

            this.AddControl(_label);

            _options = new List<string>();
            _selectedOption = -1;
        }

        public void SetSelectedOption(int selection)
        {
            if (selection > -1 && selection < _options.Count)
            {
                _selectedOption = selection;
            }
            else
            {
                _selectedOption = -1;
            }
        }

        public void AddOption(string option)
        {
            _options.Add(option);
            if (_selectedOption == -1)
                _selectedOption = 0;
        }

        public int GetSelectedOption()
        {
            return _selectedOption;
        }

        public override void OnKeyDown(KeyboardKeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (_options.Count > 0)
            {
                if (e.Key == Key.Right)
                {
                    _selectedOption++;
                    if (_selectedOption >= _options.Count)
                        _selectedOption = 0;

                    SetVerticalScroll(_selectedOption * 40);
                }
                else if (e.Key == Key.Left)
                {
                    _selectedOption--;
                    if (_selectedOption < 0)
                        _selectedOption = _options.Count - 1;
                    SetVerticalScroll(_selectedOption * 40);
                }
            }
        }

        public override void Update(FrameEventArgs e)
        {
            base.Update(e);
            anim += (float)e.Time;
            while (anim > Math.PI)
                anim -= (float)(Math.PI);
        }

        protected override void RenderContent()
        {
            base.RenderContent();

            if (_selectedOption != -1)
            {
                Color4 colour = Color4.Black;
                Vector3 pos = new Vector3(GetContentSize().X / 2, GetContentHeight() / 2, 0);
                string text = _options[_selectedOption];
                Vector3 size = new Vector3(Renderer.GetFont().GetTextWidth(text), 26, 0);
                pos.X -= size.X / 2;
                Renderer.PrintText(text, ref pos, ref colour);

                colour = Color4.Gold;
                colour.A = 0.2f + (float)Math.Sin(anim) * 0.4f;
                pos.X -= 10;
                pos.Y -= 10;
                size.X += 20;
                size.Y += 20;
                Renderer.FillShape(ShapeFactory.Rectangle, ref pos, ref size, ref colour);

                pos.Y += 10;
                colour = Color4.White;
                pos.X -= Renderer.GetFont().GetTextWidth("<") + 10;
                Renderer.PrintText("<", ref pos, ref colour);
                pos.X += Renderer.GetFont().GetTextWidth("<") + Renderer.GetFont().GetTextWidth(">") + 10 + size.X;
                Renderer.PrintText(">", ref pos, ref colour);
            }
        }
    }
}

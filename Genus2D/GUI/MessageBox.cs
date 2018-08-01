using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Genus2D.Core;
using Genus2D.Graphics;
using OpenTK;
using OpenTK.Graphics;

namespace Genus2D.GUI
{
    public class MessageBox : ScrollPanel
    {
        private Label _label;

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

            _label = new Label(0, 0, GetContentWidth(), textHeight, state);
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
            SetScrollableHeight((int)_label.GetBodySize().Y + _options.Count * 60);

            if (GetScrollableHeight() <= GetContentHeight())
                DisableVerticalScroll();
            else
                EnableVerticalScroll();

            Label label = new Label(10, (int)_label.GetBodySize().Y + ((_options.Count - 1) * 60) + 10, GetContentWidth() - 20, 40, _state);
            label.SetText("Option " + (_options.Count) + ": " + option);
            this.AddControl(label);
        }

        protected override void RenderContent()
        {
            base.RenderContent();

            if (_selectedOption != -1)
            {
                Vector3 pos = new Vector3(10, _label.GetBodySize().Y + (_selectedOption * 60), 0);
                Vector3 size = new Vector3(GetContentWidth() - 20, 40, 0);
                Color4 colour = Color4.Gold;
                colour.A = 0.35f;

                Renderer.FillShape(ShapeFactory.Rectangle, ref pos, ref size, ref colour);
            }
        }
    }
}

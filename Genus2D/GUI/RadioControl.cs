using System;
using System.Collections.Generic;
using System.Linq;

using Genus2D.Core;
using Genus2D.Graphics;

using OpenTK;
using OpenTK.Graphics;

namespace Genus2D.GUI
{
    public class RadioControl : Panel
    {
        protected static readonly int X_PADDING = 20;
        protected static readonly int Y_PADDING = 5;

        protected List<string> _items;

        protected int _selectedIndex;

        public RadioControl(int x, int y, List<string> items, State state)
            : base(x, y, 1, 1, BarMode.Empty, state)
        {
            if (items.Count == 0)
                throw new Exception("Cannot create a RadioControl with no items.");
            _items = items;
            Resize();
            AddButtons();
            SetSelection(0);
            //_backgroundColour = Color4.White;
        }

        public RadioControl(int x, int y, string[] items, State state)
            : base(x, y, 1, 1, BarMode.Empty, state)
        {
            if (items.Length == 0)
                throw new Exception("Cannot create a RadioControl with no items.");
            _items = items.ToList();
            Resize();
            AddButtons();
            SetSelection(0);
            //_backgroundColour = Color4.White;
        }

        public void Resize()
        {
            int width = (_margin * 2);
            int height = RadioButton.RADIO_SIZE + Renderer.GetFont().GetLineHeight() + (Y_PADDING * 3) + (_margin * 2);
            for (int i = 0; i < _items.Count; i++)
            {
                width += X_PADDING;
                string item = _items[i];
                width += Renderer.GetFont().GetTextWidth(item);
            }
            width += X_PADDING;
            SetSize(width, height);
        }

        protected void AddButtons()
        {
            int x = 0;
            for (int i = 0; i < _items.Count; i++)
            {
                x += X_PADDING;
                string item = _items[i];
                int textWidth = Renderer.GetFont().GetTextWidth(item);
                x += (textWidth / 2) - (RadioButton.RADIO_SIZE / 2);
                RadioButton button = new RadioButton(x, Y_PADDING, _state);
                button.SetRadioControl(this);
                this.AddControl(button);
                x += (textWidth / 2) + (RadioButton.RADIO_SIZE / 2);
            }
        }

        public void SelectButton(RadioButton button)
        {
            int index = _controls.IndexOf(button);
            SetSelection(index);
        }

        public int GetSelection()
        {
            return _selectedIndex;
        }

        public void SetSelection(int index)
        {
            if (index >= 0 && index < _items.Count)
            {
                _selectedIndex = index;
                for (int i = 0; i < _controls.Count; i++)
                {
                    RadioButton button = (RadioButton)_controls[i];
                    if (i != index)
                    {
                        button.SetCheck(false);
                    }
                    else
                    {
                        button.SetCheck(true);
                    }
                }
            }
        }

        protected override void RenderContent()
        {
            base.RenderContent();

            int x = 0;
            int y = Renderer.GetFont().GetLineHeight() + (Y_PADDING * 2);
            Color4 colour = Color4.Black;
            Vector3 textPos = new Vector3(x, y, 0);
            for (int i = 0; i < _items.Count; i++)
            {
                x += X_PADDING;
                textPos.X = x;
                string item = _items[i];
                Renderer.PrintText(item, ref textPos, ref colour, ref colour);
                int textWidth = Renderer.GetFont().GetTextWidth(item);
                x += textWidth;
            }
        }
    }
}

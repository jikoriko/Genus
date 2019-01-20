using System;
using System.Collections.Generic;
using System.Linq;

using Genus2D.Core;
using Genus2D.Graphics;

using OpenTK;
using OpenTK.Graphics;

namespace Genus2D.GUI
{
    public class DropDownBox : ScrollPanel
    {
        private int _maxItemsVisible = 6;
        private bool _droppedDown = false;
        private int _selectedItem = 0;

        private int _spacing = 12;

        private List<string> _items;

        public DropDownBox(int x, int y, int width, string[] items, State state)
            : base(x, y, width, Renderer.GetFont().GetLineHeight(), BarMode.Empty, state)
        {
            _items = items.ToList();
            SetSize((int)_bodySize.X, GetMinUndroppedHeight());
            DisableHorizontalScroll();
            DisableVerticalScroll();
            _backgroundColour = Color4.White;
            _verticalBar.SetScrollAmount(_items.Count * (Renderer.GetFont().GetLineHeight() + _spacing));
            this.OnTrigger += OnDropTrigger;
            _cornerRadius = 0f;
        }

        public void SetMaxItemsVisible(int maxVisible)
        {
            _maxItemsVisible = Math.Max(1, maxVisible);
            _maxItemsVisible = maxVisible;
        }

        public void OnDropTrigger()
        {
            if (!_droppedDown)
            {
                _droppedDown = true;
                _scissorClip = false;
                if (_items.Count > _maxItemsVisible)
                    EnableVerticalScroll();
            }
            else
            {
                if (ContentSelectable())
                {
                    Vector2 mouse = StateWindow.Instance.GetMousePosition();
                    int target = (int)(mouse.Y - this.GetWorldContentPosition().Y - GetScrolledAmount().Y) / (Renderer.GetFont().GetLineHeight() + _spacing);
                    if (target < _items.Count && target > 0)
                    {
                        if (target <= _selectedItem)
                            target -= 1;
                        _selectedItem = target;
                        _droppedDown = false;
                        _scissorClip = true;
                        if (OnSelectionChange != null)
                            OnSelectionChange();
                    }
                }
            }
        }

        public delegate void SelectionChangeEventHandler();
        public event SelectionChangeEventHandler OnSelectionChange;

        public int GetSelection()
        {
            return _selectedItem;
        }

        public void SetSelection(int index)
        {
            if (index >= 0 && index < _items.Count)
            {
                _selectedItem = index;
            }
        }

        public override void OnMouseDown(OpenTK.Input.MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button == OpenTK.Input.MouseButton.Left)
            {
                if (!BodySelectable())
                {
                    _droppedDown = false;
                }
            }
        }

        private int GetMinDropHeight()
        {
            int height = (_items.Count * Renderer.GetFont().GetLineHeight()) + (_margin * 2);
            height += _spacing * _items.Count;
            return height;
        }

        private int GetMaxDropHeight()
        {
            int height = (_maxItemsVisible * Renderer.GetFont().GetLineHeight()) + (_margin * 2);
            height += _spacing * _maxItemsVisible;
            return height;
        }

        private int GetMinUndroppedHeight()
        {
            return Renderer.GetFont().GetLineHeight() + (_margin * 2) + _spacing;
        }

        public override void Update(FrameEventArgs e)
        {
            base.Update(e);
            if (this._droppedDown)
            {
                int maxHeight = GetMaxDropHeight();
                int minHeight = GetMinDropHeight();
                int targetHeight = Math.Min(maxHeight, minHeight);
                if (_bodySize.Y < targetHeight)
                {
                    SetSize((int)_bodySize.X, (int)(_bodySize.Y + (targetHeight * 0.2f)));
                    if (_bodySize.Y > targetHeight)
                    {
                        int difference = (int)_bodySize.Y - targetHeight;
                        SetSize((int)_bodySize.X, (int)_bodySize.Y - difference);
                    }
                }

            }
            else
            {
                int maxHeight = GetMaxDropHeight();
                int minHeight = GetMinDropHeight();
                int targetHeight = Math.Min(maxHeight, minHeight);
                SetSize((int)_bodySize.X, (int)(_bodySize.Y - (targetHeight * 0.2f)));
                if (_bodySize.Y <= GetMinUndroppedHeight())
                {
                    int difference = GetMinUndroppedHeight() - (int)_bodySize.Y;
                    SetSize((int)_bodySize.X, (int)_bodySize.Y + difference);
                    DisableVerticalScroll();
                }
            }
        }

        protected override void RenderContent()
        {
            base.RenderContent();

            int blockHeight = Renderer.GetFont().GetLineHeight() + _spacing;

            Vector3 position = new Vector3();
            Vector3 scale = new Vector3(GetContentWidth(), Renderer.GetFont().GetLineHeight() + _spacing, 1);
            Color4 colour = Color4.LightSlateGray;
            Renderer.FillShape(ShapeFactory.Rectangle, ref position, ref scale, ref colour);

            if (_items.Count == 0)
                return;

            string item1 = _items[_selectedItem];
            int cx = (GetContentWidth() / 2) - (Renderer.GetFont().GetTextWidth(item1) / 2);
            int cy = (Renderer.GetFont().GetLineHeight() / 2) - (Renderer.GetFont().GetTextHeight(item1) / 2) + (_spacing / 2);
            colour = Color4.White;
            Vector3 textPos = new Vector3(cx, cy, 0);
            Renderer.PrintText(item1, ref textPos, ref colour, ref colour);
            colour = Color4.Black;
            if (_droppedDown)
            {
                int offsetY = Renderer.GetFont().GetLineHeight() + _spacing;
                for (int i = 0; i < _items.Count; i++)
                {
                    Vector3 pos = new Vector3(0, (i * blockHeight) - 1, 0);
                    scale = new Vector3(GetContentWidth(), 2, 1);
                    Renderer.FillShape(ShapeFactory.Rectangle, ref pos, ref scale, ref _borderColour);

                    if (i == _selectedItem) continue;
                    string item = _items[i];
                    cx = (GetContentWidth() / 2) - (Renderer.GetFont().GetTextWidth(item) / 2);
                    cy = (Renderer.GetFont().GetLineHeight() / 2) - (Renderer.GetFont().GetTextHeight(item) / 2) + (_spacing / 2);
                    textPos.X = cx;
                    textPos.Y = cy + offsetY;
                    Renderer.PrintText(item, ref textPos, ref colour, ref colour);
                    offsetY += Renderer.GetFont().GetLineHeight() + _spacing;
                }
            }
        }

        public override void Destroy()
        {
            base.Destroy();
            this.OnTrigger -= OnDropTrigger;
        }
    }
}

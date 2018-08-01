using System;
using System.Collections.Generic;
using System.Linq;

using Genus2D.Core;
using Genus2D.Graphics;

using OpenTK;
using OpenTK.Graphics;

namespace Genus2D.GUI
{
    public class DropDownMenu : ScrollPanel
    {
        private int _maxItemsVisible = 12;
        private bool _droppedDown = false;

        private int _spacing = 12;

        private string _title;
        private List<string> _items;

        public DropDownMenu(int x, int y, int width, string title, string[] items, State state)
            : base(x, y, width, Renderer.GetFont().GetLineHeight(), BarMode.Empty, state)
        {
            _title = title;
            _items = items.ToList();
            SetSize((int)_bodySize.X, GetMinUndroppedHeight());
            DisableHorizontalScroll();
            DisableVerticalScroll();
            _backgroundColour = Color4.LightSlateGray;
            _backgroundGradientMode = Renderer.GradientMode.None;
            _cornerRadius = 0f;
            _verticalBar.SetScrollAmount((_items.Count) * (Renderer.GetFont().GetLineHeight() + _spacing));
            this.OnTrigger += OnDropTrigger;
        }

        public void OnDropTrigger()
        {
            if (!_droppedDown)
            {
                _droppedDown = true;
                if (_items.Count > _maxItemsVisible)
                    EnableVerticalScroll();
            }
            else
            {
                if (ContentSelectable())
                {
                    Vector2 mouse = StateWindow.Instance.GetMousePosition();
                    int target = (int)(mouse.Y - this.GetWorldContentPosition().Y - GetScrolledAmount().Y) / (Renderer.GetFont().GetLineHeight() + _spacing);
                    if (target < _items.Count && target >= 0)
                    {
                        _droppedDown = false;
                        if (OnMenuSelection != null)
                            OnMenuSelection(target);
                    }
                }
            }
        }

        public delegate void SelectionChangeEventHandler(int index);
        public event SelectionChangeEventHandler OnMenuSelection;

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
            Color4 colour = Color4.White;

            if (_droppedDown)
            {
                int offsetY = 0;
                int blockHeight = Renderer.GetFont().GetLineHeight() + _spacing;
                for (int i = 0; i < _items.Count; i++)
                {
                    string item = _items[i];
                    int cx = (GetContentWidth() / 2) - (Renderer.GetFont().GetTextWidth(item) / 2);
                    int cy = (Renderer.GetFont().GetLineHeight() / 2) - (Renderer.GetFont().GetTextHeight(item) / 2) + (_spacing / 2);
                    Vector3 textPos = new Vector3(cx, cy + offsetY, 0);
                    Renderer.PrintText(item, ref textPos, ref colour, ref colour);
                    offsetY += Renderer.GetFont().GetLineHeight() + _spacing;

                    Vector3 pos = new Vector3(0, (i * blockHeight) - 1, 0);
                    Vector3 scale = new Vector3(GetContentWidth(), 2, 1);
                    Renderer.FillShape(ShapeFactory.Rectangle, ref pos, ref scale, ref _borderColour);
                }
            }
            else
            {
                string item = _title;
                int cx = (GetContentWidth() / 2) - (Renderer.GetFont().GetTextWidth(item) / 2);
                int cy = (Renderer.GetFont().GetLineHeight() / 2) - (Renderer.GetFont().GetTextHeight(item) / 2) + (_spacing / 2);
                Vector3 textPos = new Vector3(cx, cy, 0);
                Renderer.PrintText(item, ref textPos, ref colour, ref colour);
            }

        }

        public override void Destroy()
        {
            base.Destroy();
            this.OnTrigger -= OnDropTrigger;
        }
    }
}

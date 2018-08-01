using System;
using System.Collections.Generic;

using Genus2D.Core;
using Genus2D.Graphics;

using OpenTK;
using OpenTK.Graphics;

namespace Genus2D.GUI
{
    public class ListBox : ScrollPanel
    {

        protected int _index;
        protected List<string> _items;
        protected Color4 _selectionColour;
        protected int _maxItems;

        public ListBox(int x, int y, int width, int height, List<string> items, State state)
            : base(x, y, width, height, BarMode.Empty, state)
        {
            _selectionColour = Color4.LightSlateGray;
            _backgroundColour = Color4.AliceBlue;
            this.InitializeItems(items);
            this.SetScrollDimensions(GetContentWidth(), _items.Count * 32);
            DisableHorizontalScroll();
            this.OnTrigger += OnSelectionTrigger;
        }

        public ListBox(int x, int y, int width, int height, string[] items, State state)
            : base(x, y, width, height, BarMode.Empty, state)
        {
            _selectionColour = Color4.LightSlateGray;
            _backgroundColour = Color4.AliceBlue;
            this.InitializeItems(items);
            this.SetScrollDimensions(GetContentWidth(), _items.Count * 32);
            DisableHorizontalScroll();
            this.OnTrigger += OnSelectionTrigger;
        }

        public ListBox(int x, int y, int width, int height, int maxItems, State state)
            : base(x, y, width, height, BarMode.Empty, state)
        {
            _selectionColour = Color4.LightSlateGray;
            _backgroundColour = Color4.AliceBlue;
            this.InitializeItems(maxItems);
            this.SetScrollDimensions(GetContentWidth(), _items.Count * 32);
            DisableHorizontalScroll();
            this.OnTrigger += OnSelectionTrigger;
        }


        private void InitializeItems(List<String> items)
        {
            _maxItems = items.Count;
            if (items.Count > 0)
            {
                _index = 0;
            }
            else
                _index = -1;
            _items = items;
        }

        private void InitializeItems(string[] items)
        {
            List<string> itemsList = new List<string>();
            for (int i = 0; i < items.Length; i++)
            {
                itemsList.Add(items[i]);
            }
            this.InitializeItems(itemsList);
        }

        private void InitializeItems(int maxItems)
        {
            List<string> itemsList = new List<string>();
            for (int i = 0; i < maxItems; i++)
            {
                itemsList.Add("Item " + i);
            }
            this.InitializeItems(itemsList);
        }

        public void SetSelection(int index)
        {
            if (index >= 0 && index < _items.Count)
            {
                _index = index;
                if (OnSelectionChange != null)
                    OnSelectionChange(index);
            }
        }

        public delegate void SelectionChangeEventHandler(int selection);

        public event SelectionChangeEventHandler OnSelectionChange;

        public int GetSelection()
        {
            return _index;
        }

        public List<string> GetItems()
        {
            return _items;
        }

        protected void OnSelectionTrigger()
        {
            if (!this.ContentSelectable())
                return;
            Vector2 mouse = StateWindow.Instance.GetMousePosition();
            int target = (int)(mouse.Y - GetWorldContentPosition().Y - GetScrolledAmount().Y) / 32;
            if (target < _items.Count)
            {
                _index = target;
                if (OnSelectionChange != null)
                    OnSelectionChange(_index);
            }
        }

        public override void Update(FrameEventArgs e)
        {
            base.Update(e);
            if (_maxItems != _items.Count)
            {
                _maxItems = _items.Count;
                this.SetScrollDimensions(GetContentWidth(), _items.Count * 32);
            }
            if (_index >= _items.Count)
            {
                _index = _items.Count - 1;
                if (OnSelectionChange != null)
                    OnSelectionChange(_index);
            }
        }

        protected override void RenderContent()
        {
            base.RenderContent();
            Vector3 position;
            Vector3 scale;
            if (_index >= 0)
            {
                position = new Vector3(0, _index * 32, 0);
                scale = new Vector3(GetContentWidth(), 32, 1);
                Renderer.FillShape(ShapeFactory.Rectangle, ref position, ref scale, ref _selectionColour);
            }
            Vector3 textPos = new Vector3();
            for (int i = 0; i < _items.Count; i++)
            {
                textPos.X = (GetContentWidth() / 2) - (Renderer.GetFont().GetTextWidth(_items[i]) / 2);
                textPos.Y = 16 - (Renderer.GetFont().GetTextHeight(_items[i]) / 2) + (i * 32);
                Color4 textColour = Color4.Black;
                Renderer.PrintText(_items[i], ref textPos, ref textColour, ref textColour);
            }
        }

        public override void Destroy()
        {
            base.Destroy();
            this.OnTrigger -= OnSelectionTrigger;
        }

    }
}

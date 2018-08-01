using System;

using Genus2D.Core;
using Genus2D.Graphics;
using Genus2D.Utililities;

using OpenTK;
using OpenTK.Graphics;

namespace Genus2D.GUI
{
    public class NumberControl : Control
    {

        public static readonly int NUMBER_CONTROL_WIDTH = 80;

        private Button _incrementButton, _decrementButton;
        private int _index = 0;
        private int _minimum = 0, _maximum = 10000;
        private bool _loopIndex = false;
        private string _text = "";

        private float _repeatTimer = 0f;
        private float _repeatMultiplyer = 1f;

        private bool _indexChanged = false;

        public NumberControl(int x, int y, State state)
            : base(x, y, NUMBER_CONTROL_WIDTH, NUMBER_CONTROL_WIDTH / 2, state)
        {
            int buttonSize = (NUMBER_CONTROL_WIDTH / 4) - 4;
            _incrementButton = new Button("", x + (int)(NUMBER_CONTROL_WIDTH * 0.75f) + 2, y + 4, buttonSize, buttonSize, state);
            _incrementButton.SetButtonImage(Assets.GetTexture("GUI_Textures/UpArrow.png"));
            _decrementButton = new Button("", x + (int)(NUMBER_CONTROL_WIDTH * 0.75f) + 2, y + buttonSize + 4, buttonSize, buttonSize, state);
            _decrementButton.SetButtonImage(Assets.GetTexture("GUI_Textures/DownArrow.png"));
            _backgroundColour = Color4.White;
        }

        public string Text
        {
            get { return _text; }
            set { _text = value; }
        }

        public void EnableLoopIndex()
        {
            _loopIndex = true;
        }

        public void DisableLoopIndex()
        {
            _loopIndex = false;
        }

        public override void SetOrderZ(int orderZ)
        {
            base.SetOrderZ(orderZ);
            _incrementButton.SetOrderZ(orderZ);
            _decrementButton.SetOrderZ(orderZ);
        }

        public void SetIndex(int index)
        {
            index = Math.Min(_maximum, index);
            index = Math.Max(_minimum, index);
            if (index != _index)
            {
                _index = index;
                if (OnIndexChange != null)
                    OnIndexChange(_index);
                _indexChanged = true;
            }
        }

        public int GetIndex()
        {
            return _index;
        }

        public delegate void IndexChandedEvent(int index);
        public IndexChandedEvent OnIndexChange;

        public void SetMinimum(int min)
        {
            _minimum = min;
            if (_index < min)
                _index = min;
        }

        public void SetMaximum(int max)
        {
            _maximum = max;
            if (_index > max)
                _index = max;
        }

        public void DecrementIndex()
        {
            if (_index > _minimum)
            {
                _index--;
                if (OnIndexChange != null)
                    OnIndexChange(_index);
            }
            else if (_loopIndex)
            {
                _index = _maximum;
                if (OnIndexChange != null)
                    OnIndexChange(_index);
            }
            _repeatTimer = 0.2f / _repeatMultiplyer;
        }

        public void IncrementIndex()
        {
            if (_index < _maximum)
            {
                _index++;
                if (OnIndexChange != null)
                    OnIndexChange(_index);
            }
            else if (_loopIndex)
            {
                _index = _minimum;
                if (OnIndexChange != null)
                    OnIndexChange(_index);
            }
            _repeatTimer = 0.2f / _repeatMultiplyer;
        }

        public override void SetParent(Panel parent)
        {
            base.SetParent(parent);
            _incrementButton.SetParent(parent);
            _decrementButton.SetParent(parent);
        }

        public override int GetContentWidth()
        {
            return base.GetContentWidth() - (int)((NUMBER_CONTROL_WIDTH - _margin * 2) * 0.25f);
        }

        public override void Update(FrameEventArgs e)
        {
            base.Update(e);

            if (_repeatTimer <= 0f)
            {
                if (!_indexChanged)
                {
                    if (_incrementButton.IsPressed())
                    {
                        IncrementIndex();
                        _repeatMultiplyer *= 1.1f;
                    }
                    else if (_decrementButton.IsPressed())
                    {
                        DecrementIndex();
                        _repeatMultiplyer *= 1.1f;
                    }
                    else
                    {
                        _repeatMultiplyer = 1f;
                    }
                }
            }
            else
            {
                _repeatTimer -= (float)e.Time;
            }

            _text = _index.ToString();
            _indexChanged = false;

        }

        public override void Render()
        {
            base.Render();
            _incrementButton.Render();
            _decrementButton.Render();
        }

        protected override void RenderContent()
        {
            base.RenderContent();
            int cX = (GetContentWidth() / 2) - (Renderer.GetFont().GetTextWidth(_text) / 2);
            int cY = (GetContentHeight() / 2) - (Renderer.GetFont().GetTextHeight(_text) / 2);
            Color4 colour = Color4.Black;
            Vector3 textPos = new Vector3(cX, cY, 0);
            Renderer.PrintText(_text, ref textPos, ref colour, ref colour);
        }

        public override void Destroy()
        {
            base.Destroy();
            _incrementButton.Destroy();
            _decrementButton.Destroy();
        }
    }
}
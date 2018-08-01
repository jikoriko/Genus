using System;

using Genus2D.Core;
using Genus2D.Graphics;

using OpenTK;
using OpenTK.Input;
using OpenTK.Graphics;

namespace Genus2D.GUI
{
    public class TextField : Control
    {

        private string _text;
        private bool _active;
        private int _cursorPos, _maxCharacters, _offsetX;

        public TextField(int x, int y, int width, int height, State state)
            : base(x, y, width, height, state)
        {
            _text = "";
            _active = false;
            _cursorPos = 0;
            _maxCharacters = 1000;
            _backgroundColour = Color4.White;

            _cornerRadius = 0;
            OnTrigger += OnTextTrigger;
        }

        public bool IsActive()
        {
            return _active;
        }

        public override void Disable()
        {
            base.Disable();
            _active = false;
        }
        public void SetMaxChar(int max)
        {
            max = max <= 0 ? 1 : max;
            _maxCharacters = max;
        }

        public void SetText(string text)
        {
            _text = text;
            if (_cursorPos > _text.Length)
            {
                _cursorPos = _text.Length;
            }
        }

        public string GetText()
        {
            return _text;
        }

        private void ScrollCheck()
        {
            int cursorX = (int)_bodyPosition.X + 1 + Renderer.GetFont().GetTextWidth(_text.Substring(0, _cursorPos));
            if (cursorX + _offsetX > (int)(_bodyPosition.X + _bodySize.X) - 3)
            {
                _offsetX = -(cursorX - (int)_bodySize.X - ((int)_bodyPosition.X - 3));
            }
            else if (cursorX + _offsetX < (int)_bodyPosition.X + 1)
            {
                _offsetX = -(cursorX - ((int)_bodyPosition.X + 1));
            }
        }

        private void OnTextTrigger()
        {
            TrueTypeFont font = Renderer.GetFont();
            Vector2 mouse = StateWindow.Instance.GetMousePosition();
            int relativeX = (int)(mouse.X - GetWorldContentPosition().X) - _offsetX;
            _cursorPos = 0;

            for (int i = 0; i < _text.Length; i++)
            {
                if (relativeX > font.GetTextWidth(_text.Substring(0, i + 1)))
                    _cursorPos++;
                else
                    break;
            }
            _cursorPos = Math.Min(_text.Length, _cursorPos);
        }

        public override void OnMouseUp(MouseButtonEventArgs e)
        {
            if (e.Button == MouseButton.Left)
            {
                if (_pressed && this.BodySelectable())
                    _active = true;
                else
                    _active = false;
            }
            base.OnMouseUp(e);
        }

        public override void OnKeyDown(KeyboardKeyEventArgs e)
        {
            base.OnKeyDown(e);
            this.KeyDown(e.Key);
        }

        public override void OnKeyUp(KeyboardKeyEventArgs e)
        {
            base.OnKeyUp(e);
        }

        public override void OnKeyPress(KeyPressEventArgs e)
        {
            base.OnKeyPress(e);
            this.CharDown(e.KeyChar);
        }

        public void KeyDown(Key key)
        {
            if (_active)
            {
                if (key == Key.BackSpace)
                {
                    if (_text.Length != 0 && _cursorPos != 0)
                    {
                        _text = _text.Remove(_cursorPos - 1, 1);
                        _cursorPos--;
                        ScrollCheck();
                    }
                }
                if (key == Key.Delete)
                {
                    if (_text.Length != 0 && _cursorPos < _text.Length)
                    {
                        _text = _text.Remove(_cursorPos, 1);
                        ScrollCheck();
                    }
                }
                else if (key == Key.Left)
                {
                    if (_cursorPos > 0)
                    {
                        _cursorPos--;
                        ScrollCheck();
                    }
                }
                else if (key == Key.Right)
                {
                    if (_cursorPos < _text.Length)
                    {
                        _cursorPos++;
                        ScrollCheck();
                    }
                }
                else if (key == Key.Enter)
                {
                    //this.Triggered = true;
                }

            }
        }

        public void CharDown(char c)
        {
            if (_active)
            {
                if (_text.Length == _maxCharacters)
                    return;

                if (_cursorPos == _text.Length)
                {
                    _text += c;
                    _cursorPos++;
                }
                else
                {
                    _text = _text.Insert(_cursorPos, c.ToString());
                    _cursorPos++;
                }
                ScrollCheck();
            }
        }

        protected override void RenderContent()
        {
            base.RenderContent();

            TrueTypeFont font = Renderer.GetFont();

            Color4 textColour = Color4.Black;

            int cY = (GetContentHeight() / 2) - (font.GetTextHeight(_text) / 2);
            Vector3 textPos = new Vector3();
            textPos.X = _offsetX + 1;
            textPos.Y = cY;
            Renderer.PrintText(_text, ref textPos, ref textColour, ref textColour);

            int cursorX = font.GetTextWidth(_text.Substring(0, _cursorPos));
            cursorX += _offsetX;
            if (_active)
            {
                Vector3 position = new Vector3(cursorX, 1, 0);
                Vector3 scale = new Vector3(2, GetContentHeight() - 2, 1);
                Renderer.FillShape(ShapeFactory.Rectangle, ref position, ref scale, ref textColour);
            }
        }

        public override void Destroy()
        {
            base.Destroy();
            OnTrigger -= OnTextTrigger;
        }


    }
}

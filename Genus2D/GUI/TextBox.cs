using System;
using System.Collections.Generic;

using Genus2D.Core;
using Genus2D.Graphics;

using OpenTK;
using OpenTK.Input;
using OpenTK.Graphics;

namespace Genus2D.GUI
{

    public class TextBox : ScrollPanel
    {
        private List<string> _lines;
        private bool _active;
        private int _cursorRow, _cursorColumn, _lastKey, _maxChar;

        private long _repeatTimer = 0;
        private static readonly int _initialKeyRepeatTimer = 400;
        private static readonly int _keyRepeatTimer = 50;

        public TextBox(int x, int y, int width, int height, State state)
            : base(x, y, width, height, BarMode.Empty, state)
        {
            this.DisableHorizontalScroll();
            _lines = new List<string>();
            _lines.Add("");
            _cursorRow = _cursorColumn = 0;
            _lastKey = -1;
            _maxChar = 10000;
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
            _maxChar = max;
        }

        public void SetText(string text)
        {
            _lines.Clear();
            _cursorRow = _cursorColumn = 0;

            string line = "";
            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];
                if (c == '\n')
                {
                    _lines.Add(line);
                    line = "";
                }
                else
                {
                    if (Renderer.GetFont().GetTextWidth(line + c) > GetContentWidth())
                    {
                        _lines.Add(line);
                        line = "";
                    }
                    line += c;
                }
            }
            _lines.Add(line);
        }

        public override void Update(FrameEventArgs e)
        {
            base.Update(e);
            /*
            if (_lastKey != -1)
            {
                if (Input.KeyDown((Key)_lastKey))
                {
                    if (_repeatTimer < Environment.TickCount)
                    {
                        _repeatTimer = Environment.TickCount + _keyRepeatTimer;
                        this.KeyDown((Key)_lastKey);
                    }
                }
                else
                {
                    _lastKey = -1;
                }
            }
            */
        }

        public void OnTextTrigger()
        {
            if (!ContentSelectable())
                return;
            if (!_active)
            {
                _active = true;
            }
            TrueTypeFont font = Renderer.GetFont();
            Vector2 mouse = StateWindow.Instance.GetMousePosition();
            int relativeY = (int)(mouse.Y - GetWorldContentPosition().Y) - _verticalBar.GetRelativeScroll();
            relativeY /= font.GetLineHeight();
            _cursorRow = Math.Min(relativeY, _lines.Count - 1);

            int relativeX = (int)(mouse.X - GetWorldContentPosition().X);

            string line = _lines[_cursorRow];
            _cursorColumn = 0;
            for (int i = 0; i < line.Length; i++)
            {
                if (relativeX > font.GetTextWidth(line.Substring(0, i + 1)))
                {
                    _cursorColumn++;
                }
                else
                {
                    break;
                }
            }
            _cursorColumn = Math.Min(line.Length, _cursorColumn);
        }

        public override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button == OpenTK.Input.MouseButton.Left)
            {
                if (!BodySelectable())
                    _active = false;
            }
        }

        public void KeyDown(Key key)
        {
            if (_active)
            {
                if (_lastKey != (int)key)
                {
                    _lastKey = (int)key;
                    _repeatTimer = Environment.TickCount + _initialKeyRepeatTimer;
                }
                else
                {
                    _repeatTimer = Environment.TickCount + _keyRepeatTimer;
                }

                if (key == Key.BackSpace)
                {
                    TrueTypeFont font = Renderer.GetFont();
                    string text = _lines[_cursorRow];
                    if (_cursorColumn == 0) // beginning of line
                    {
                        if (_cursorRow > 0) // not first line
                        {
                            _lines.RemoveAt(_cursorRow); // move up one line
                            _cursorRow--;
                            _cursorColumn = _lines[_cursorRow].Length;
                            string line = _lines[_cursorRow] + text;
                            for (int i = 0; i < line.Length; i++)
                            {
                                if (font.GetTextWidth(line.Substring(0, i + 1)) > GetContentWidth())
                                {
                                    _lines.Insert(_cursorRow + 1, line.Substring(i));
                                    line = line.Substring(0, i);
                                    break;
                                }
                            }
                            _lines[_cursorRow] = line;
                        }
                    }
                    else if (text.Length != 0) // there is some text
                    {
                        if (_cursorColumn == text.Length) // end of line
                        {
                            text = text.Substring(0, text.Length - 1);
                            _lines[_cursorRow] = text;
                            _cursorColumn--;
                        }
                        else // middle of line
                        {
                            text = text.Remove(_cursorColumn - 1, 1);
                            _lines[_cursorRow] = text;
                            _cursorColumn--;
                        }
                    }
                    _verticalBar.SetScrollAmount(_lines.Count * font.GetLineHeight());
                }
                else if (key == Key.Delete)
                {
                    TrueTypeFont font = Renderer.GetFont();
                    string text = _lines[_cursorRow];
                    if (_cursorColumn == 0) // beginning of line
                    {
                        if (text.Length != 0) // there is some text
                        {
                            text = text.Remove(_cursorColumn, 1);
                            _lines[_cursorRow] = text;
                        }
                        else // no text
                        {
                            if (_cursorRow + 1 <= _lines.Count - 1) // next line exists
                            {
                                if (_lines[_cursorRow + 1].Length != 0) // remove this line and move next line up
                                {
                                    _lines.RemoveAt(_cursorRow);
                                    text = _lines[_cursorRow];
                                }
                            }
                        }
                    }
                    else if (text.Length != 0) // not beginning of line and with some text
                    {
                        if (_cursorColumn != text.Length) // not end of line
                        {
                            text = text.Remove(_cursorColumn, 1);
                            _lines[_cursorRow] = text;
                        }
                        else // end of line
                        {
                            if (_cursorRow < _lines.Count - 1) // not on last line
                            {
                                text = _lines[_cursorRow + 1];
                                _lines.RemoveAt(_cursorRow + 1);
                                _cursorColumn = _lines[_cursorRow].Length;
                                string line = _lines[_cursorRow] + text;
                                for (int i = 0; i < line.Length; i++)
                                {
                                    if (font.GetTextWidth(line.Substring(0, i + 1)) > GetContentWidth())
                                    {
                                        _lines.Insert(_cursorRow + 1, line.Substring(i));
                                        line = line.Substring(0, i);
                                        break;
                                    }
                                }
                                _lines[_cursorRow] = line;
                            }
                        }
                    }
                    //if (font.GetTextWidth(text) > GetContentWidth()) // text overflow, try wrap
                    //{
                    //    string textToWrap = "";
                    //    int splitIndex = text.LastIndexOf(' ');
                    //    if (splitIndex != -1) // can be wrapped
                    //    {
                    //        textToWrap = text.Substring(splitIndex + 1);
                    //        text = text.Substring(0, splitIndex + 1);
                    //        _lines[_cursorRow] = text;
                    //        _lines.Insert(++_cursorRow, textToWrap);
                    //        _cursorColumn = textToWrap.Length;
                    //    }
                    //    else // can't be wrapped, keep adding text on next line
                    //    {
                    //        _lines.Insert(++_cursorRow, c.ToString());
                    //        _cursorColumn = 1;
                    //    }
                    //    _verticalBar.SetScrollAmount(_lines.Count * font.GetLineHeight());
                    //    _verticalBar.ScrollSlider(font.GetLineHeight());
                    //}
                    _verticalBar.SetScrollAmount(_lines.Count * font.GetLineHeight());
                }
                else if (key == Key.Left)
                {
                    if (_cursorColumn > 0)
                    {
                        _cursorColumn--;
                    }
                    else
                    {
                        if (_cursorRow > 0)
                        {
                            _cursorRow--;
                            _cursorColumn = _lines[_cursorRow].Length;
                            _verticalBar.ScrollSlider(-5);
                        }
                    }
                }
                else if (key == Key.Right)
                {
                    string text = _lines[_cursorRow];
                    if (_cursorColumn < text.Length)
                    {
                        _cursorColumn++;
                    }
                    else
                    {
                        if (_cursorRow < _lines.Count - 1)
                        {
                            _cursorRow++;
                            _cursorColumn = 0;
                            _verticalBar.ScrollSlider(5);
                        }
                    }
                }
                else if (key == Key.Up)
                {
                    if (_cursorRow > 0)
                    {
                        _cursorRow--;
                        if (_cursorColumn > _lines[_cursorRow].Length)
                            _cursorColumn = _lines[_cursorRow].Length;

                        //TrueTypeFont font = Renderer.GetFont();
                        _verticalBar.ScrollSlider(-5); // only works with current font
                    }
                }
                else if (key == Key.Down)
                {
                    if (_cursorRow < _lines.Count - 1)
                    {
                        _cursorRow++;
                        if (_cursorColumn > _lines[_cursorRow].Length)
                            _cursorColumn = _lines[_cursorRow].Length;

                        //TrueTypeFont font = Renderer.GetFont();
                        _verticalBar.ScrollSlider(5); // only works with current font
                    }
                }
                else if (key == Key.Enter)
                {
                    TrueTypeFont font = Renderer.GetFont();
                    if (_cursorColumn != _lines[_cursorRow].Length)
                    {
                        string line = _lines[_cursorRow].Substring(0, _cursorColumn);
                        string line2 = _lines[_cursorRow].Substring(_cursorColumn);
                        _lines[_cursorRow] = line;
                        _lines.Insert(_cursorRow + 1, line2);

                    }
                    else
                    {
                        _lines.Insert(_cursorRow + 1, "");
                    }
                    _cursorRow++;
                    _cursorColumn = 0;
                    _verticalBar.SetScrollAmount(_lines.Count * font.GetLineHeight());
                    _verticalBar.ScrollSlider(font.GetLineHeight());
                }

            }
        }

        public void CharDown(char c)
        {
            if (_active)
            {
                string text = _lines[_cursorRow];
                TrueTypeFont font = Renderer.GetFont();

                if (_cursorColumn == text.Length) // add char to the end of text
                    text += c;
                else // insert char to cursor position
                    text = text.Insert(_cursorColumn, c.ToString());

                if (font.GetTextWidth(text) > GetContentWidth()) // text overflow, try wrap
                {
                    string textToWrap = "";
                    int splitIndex = text.LastIndexOf(' ');
                    if (splitIndex != -1) // can be wrapped
                    {
                        textToWrap = text.Substring(splitIndex + 1);
                        text = text.Substring(0, splitIndex + 1);
                        _lines[_cursorRow] = text;
                        _lines.Insert(++_cursorRow, textToWrap);
                        _cursorColumn = textToWrap.Length;
                    }
                    else // can't be wrapped, keep adding text on next line
                    {
                        _lines.Insert(++_cursorRow, c.ToString());
                        _cursorColumn = 1;
                    }
                    _verticalBar.SetScrollAmount(_lines.Count * font.GetLineHeight());
                    _verticalBar.ScrollSlider(font.GetLineHeight());
                }
                else
                {
                    _lines[_cursorRow] = text;
                    _cursorColumn++;
                }
            }
        }

        protected override void RenderContent()
        {
            base.RenderContent();
            //System.Diagnostics.Debug.WriteLine("Cursor Col: {0}, cursor Row: {1}",_cursorColumn, _cursorRow);
            TrueTypeFont font = Renderer.GetFont();
            int lineHeight = font.GetLineHeight();

            Vector3 textPos = new Vector3();
            for (int i = 0; i < _lines.Count; i++)
            {
                string text = _lines[i];
                Color4 colour = Color4.Black;
                textPos.Y = lineHeight - font.GetTextHeight(text) + (i * lineHeight);
                Renderer.PrintText(text, ref textPos, ref colour, ref colour);
            }
            if (_active)
            {
                try
                {
                    int cursorX = font.GetTextWidth(_lines[_cursorRow].Substring(0, _cursorColumn)) + 1;
                    Vector3 position = new Vector3(cursorX, (_cursorRow * lineHeight) + 1, 0);
                    Vector3 scale = new Vector3(2, lineHeight, 1);
                    Color4 colour = Color4.Black;
                    Renderer.FillShape(ShapeFactory.Rectangle, ref position, ref scale, ref colour);
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine("cursor X error in TextBox.cs render.\n" + e.Message);
                    _cursorColumn--; // emergency fix
                }
            }
        }

        public override void OnKeyDown(KeyboardKeyEventArgs e)
        {
            base.OnKeyDown(e);
            this.KeyDown(e.Key);
        }


        public override void OnKeyPress(KeyPressEventArgs e)
        {
            base.OnKeyPress(e);
            this.CharDown(e.KeyChar);
        }

        public override void Destroy()
        {
            base.Destroy();
            OnTrigger -= OnTextTrigger;
        }
    }

}

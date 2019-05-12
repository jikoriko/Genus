using Genus2D.Core;
using Genus2D.Listeners;
using OpenTK;
using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genus2D
{
    public class ClickBox : MouseListener
    {

        private float _x, _y;
        private float _width, _height;

        private MouseButton _triggerButton;
        private bool _pressed;

        public ClickBox(float x, float y, float width, float height, MouseButton triggerButton)
        {
            _x = x;
            _y = y;
            _width = width;
            _height = height;

            _triggerButton = triggerButton;
            _pressed = false;
        }

        private bool MouseInside()
        {
            Vector2 mouse = StateWindow.Instance.GetMousePosition();
            return (mouse.X < _x + _width && mouse.X >= _x && mouse.Y < _y + _height && mouse.Y >= _y);
        }

        public void OnMouseMove(MouseMoveEventArgs e)
        {
        }

        public void OnMouseDown(MouseButtonEventArgs e)
        {
            if (e.Button == _triggerButton && MouseInside())
            {
                _pressed = true;
            }
        }

        public void OnMouseUp(MouseButtonEventArgs e)
        {
            if (e.Button == _triggerButton && _pressed)
            {
                if (MouseInside())
                {
                    OnTrigger?.Invoke();
                }
                _pressed = false;
            }
        }

        public void OnMouseWheel(MouseWheelEventArgs e)
        {
        }

        public delegate void TriggerEventHandler();
        public event TriggerEventHandler OnTrigger;
    }
}

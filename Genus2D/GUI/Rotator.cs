using System;

using OpenTK;
using OpenTK.Input;
using OpenTK.Graphics;

using Genus2D.States;
using Genus2D.Graphics;

namespace Genus2D.GUI
{
    public class Rotator : Control
    {

        private int _radius;
        private float _rotation;

        public Rotator(int x, int y, int radius, State state)
            : base(x, y, radius * 2, radius * 2, state)
        {
            _radius = radius;
            _rotation = 0f;
            _backgroundColour = Color4.LightGray;
            _cornerRadius = radius;
        }

        public void SetRotation(float rotation, bool radians = true)
        {
            if (!radians)
                rotation = MathHelper.DegreesToRadians(rotation);
            _rotation = rotation;
        }

        public float GetRotation(bool radians = true)
        {
            float rot = _rotation;
            if (!radians)
                rot = MathHelper.RadiansToDegrees(rot);
            return rot;
        }

        public delegate void OnRotateEvent(float rotation);
        public event OnRotateEvent OnRotate;

        public override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            if (_pressed)
            {
                RotateToMouse();
            }
        }

        public override void OnMouseMove(MouseMoveEventArgs e)
        {
            base.OnMouseMove(e);
            if (_pressed)
            {
                RotateToMouse();
            }
        }

        private void RotateToMouse()
        {
            Vector3 center = GetWorldBodyPosition();
            center.X += GetBodySize().X / 2;
            center.Y += GetBodySize().Y / 2;
            Vector3 cursorPos = new Vector3(StateWindow.Instance.GetMousePosition());

            Vector3 dir = cursorPos - center;
            dir.Z = 0;

            if (dir.Length > 0)
            {
                float angle = Vector3.CalculateAngle(Vector3.UnitX, dir);
                if (Vector3.Dot(dir, Vector3.UnitY) < 0)
                    angle = -angle;
                _rotation = angle;

                if (OnRotate != null)
                {
                    OnRotate(_rotation);
                }
            }
        }

        protected override void RenderContent()
        {
            base.RenderContent();

            Vector3 center = Vector3.Zero;
            center.X += GetContentWidth() / 2;
            center.Y += GetContentHeight() / 2;

            Vector3 dir = Vector3.Transform(Vector3.UnitX, Matrix4.CreateRotationZ(_rotation));
            dir.Normalize();

            Vector3 pos = center;
            Color4 colour = _borderColour;
            Renderer.DrawPoint(ref pos, 8f, colour);

            pos += dir * (_radius - 6f);
            colour = Color4.DarkRed;
            Renderer.DrawPoint(ref pos, 6f, colour);


        }
    }
}

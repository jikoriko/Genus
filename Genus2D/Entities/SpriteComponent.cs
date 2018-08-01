using System;
using System.Drawing;

using Genus2D.Graphics;

using OpenTK;
using OpenTK.Graphics;

namespace Genus2D.Entities
{
    public class SpriteComponent : EntityComponent
    {
        public enum SpriteCenter
        {
            Center, Top, Bottom, Left, Right, BottomLeft, BottomRight, TopLeft, TopRight
        }

        private SpriteCenter _spriteCenter;

        private Texture _texture;
        private Color4 _colour;

        private int _xFrames;
        private int _yFrames;
        private int _currentXFrame;
        private int _currentYFrame;

        private double _timer;
        private double _frameTime;
        private bool _animating;
        
        public enum CycleDirection
        {
            Horizontal, vertical
        }
        private CycleDirection _cycleDirection;
        

        public SpriteComponent(Entity entity) 
            : base(entity)
        {
            _spriteCenter = SpriteCenter.Center;
            _texture = null;
            _colour = Color4.White;

            _xFrames = 1;
            _yFrames = 1;
            _currentXFrame = 0;
            _currentYFrame = 0;
            
            _timer = 0;
            _frameTime = 0.3;
            _animating = true;
            _cycleDirection = CycleDirection.Horizontal;
        }

        public void SetSpriteCenter(SpriteCenter spriteCenter)
        {
            _spriteCenter = spriteCenter;
        }

        public Texture GetTexture()
        {
            return _texture;
        }

        public void SetTexture(Texture texture)
        {
            _texture = texture;
        }

        public void SetColour(Color4 colour)
        {
            _colour = colour;
        }

        public int GetXFrame()
        {
            return _currentXFrame;
        }

        public int GetYFrame()
        {
            return _currentYFrame;
        }

        public int GetXFrames()
        {
            return _xFrames;
        }

        public int GetYFrames()
        {
            return _yFrames;
        }

        public void SetXFrames(int amount)
        {
            amount = Math.Max(1, amount);
            _xFrames = amount;
        }

        public void SetYFrames(int amount)
        {
            amount = Math.Max(1, amount);
            _yFrames = amount;
        }

        public void SetYFrame(int frame)
        {
            _currentYFrame = Math.Max(Math.Min(frame, _yFrames - 1), 0);
        }

        public void SetXFrame(int frame)
        {
            _currentXFrame = Math.Max(Math.Min(frame, _xFrames - 1), 0);
        }

        public virtual void IncrementXFrame()
        {
            if (_currentXFrame < _xFrames - 1)
                _currentXFrame++;
            else
                _currentXFrame = 0;
        }

        public virtual void IncrementYFrame()
        {
            if (_currentYFrame < _yFrames - 1)
                _currentYFrame++;
            else
                _currentXFrame = 0;
        }

        public float GetFrameWidth()
        {
            if (_texture != null)
                return (float)_texture.GetWidth() / _xFrames;
            return 0;
        }

        public float GetFrameHeight()
        {
            if (_texture != null)
                return (float)_texture.GetHeight() / _yFrames;
            return 0;
        }

        public void SetAnimating(bool animating)
        {
            _animating = animating;
        }

        public void SetFrameTime(double frameTime)
        {
            frameTime = Math.Max(frameTime, 0.01);
            _frameTime = frameTime;
        }

        public void SetCycleDirection(CycleDirection cycleDirection)
        {
            _cycleDirection = cycleDirection;
        }

        public override void Update(FrameEventArgs e)
        {
            base.Update(e);
            if (_animating)
            {
                if (_timer > 0)
                    _timer -= e.Time;
                while (_timer <= 0)
                {
                    _timer += _frameTime;
                    switch(_cycleDirection)
                    {
                        case CycleDirection.Horizontal:
                            IncrementXFrame();
                            break;
                        case CycleDirection.vertical:
                            IncrementYFrame();
                            break;
                    }
                }

            }
        }

        public override void Render(FrameEventArgs e)
        {
            base.Render(e);
            if (_texture != null)
            {
                int textureWidth = _texture.GetWidth() / _xFrames;
                int textureHeight = _texture.GetHeight() / _yFrames;
                int textureX = _currentXFrame * textureWidth;
                int textureY = _currentYFrame * textureHeight;

                float width = (float)textureWidth * Transform.Scale.X;
                float height = (float)textureHeight * Transform.Scale.Y;

                Vector3 offset = Vector3.Zero;
                switch (_spriteCenter)
                {
                    case SpriteCenter.Center:
                        offset.X = -width / 2;
                        offset.Y = -height / 2;
                        break;
                    case SpriteCenter.Top:
                        offset.X = -width / 2;
                        offset.Y = -height;
                        break;
                    case SpriteCenter.Bottom:
                        offset.X = -width / 2;
                        break;
                    case SpriteCenter.Left:
                        offset.X = -width;
                        offset.Y = -height / 2;
                        break;
                    case SpriteCenter.Right:
                        offset.Y = -height / 2;
                        break;
                    case SpriteCenter.BottomLeft:
                        offset.X = -width;
                        break;
                    case SpriteCenter.BottomRight:
                        break;
                    case SpriteCenter.TopLeft:
                        offset.X = -width;
                        offset.Y = -height;
                        break;
                    case SpriteCenter.TopRight:
                        offset.Y = -height;
                        break;
                }

                Vector3 pos = Transform.Position;
                Vector3 scale = new Vector3(width, height, 1);
                Vector3 rot = Transform.Rotation;
                Rectangle source = new Rectangle(textureX, textureY, textureWidth, textureHeight);
                Renderer.FillTexture(_texture, ShapeFactory.Rectangle, ref pos, ref scale, ref rot, ref offset, ref source, ref _colour);
            }
        }
    }
}

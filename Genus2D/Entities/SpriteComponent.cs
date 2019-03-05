using System;
using System.Drawing;
using Genus2D.GameData;
using Genus2D.Graphics;
using Genus2D.Utililities;
using OpenTK;
using OpenTK.Graphics;

namespace Genus2D.Entities
{
    public class SpriteComponent : EntityComponent
    {
        private int _spriteID;
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
            _spriteID = -1;
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

        public int GetSpriteID()
        {
            return _spriteID;
        }

        public void SetSpriteID(int spriteID)
        {
            _spriteID = spriteID;
        }

        public SpriteData GetSpriteData()
        {
            return SpriteData.GetSpriteData(_spriteID);
        }

        public Texture GetSpriteTexture()
        {
            SpriteData data = GetSpriteData();
            if (data != null)
            {
                return Assets.GetTexture("Sprites/" + data.ImagePath);
            }

            return null;
        }

        public Color4 GetColour()
        {
            return _colour;
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
            Texture texture = GetSpriteTexture();
            if (texture != null)
                return (float)texture.GetWidth() / _xFrames;
            return 0;
        }

        public float GetFrameHeight()
        {
            Texture texture = GetSpriteTexture();
            if (texture != null)
                return (float)texture.GetHeight() / _yFrames;
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

        public virtual bool BushFlag()
        {
            return false;
        }

        public override void Render(FrameEventArgs e)
        {
            base.Render(e);
            Texture texture = GetSpriteTexture();
            if (texture != null)
            {
                int textureWidth = texture.GetWidth() / _xFrames;
                int textureHeight = texture.GetHeight() / _yFrames;
                int textureX = _currentXFrame * textureWidth;
                int textureY = _currentYFrame * textureHeight;

                float width = (float)textureWidth * Transform.Scale.X;
                float height = (float)textureHeight * Transform.Scale.Y;

                Vector3 offset = Vector3.Zero;
                SpriteData data = SpriteData.GetSpriteData(_spriteID);
                if (GetYFrame() == 0 || GetYFrame() == 3)
                {
                    offset.X = -data.VerticalAnchorPoint.X;
                    offset.Y = -data.VerticalAnchorPoint.Y;
                }
                else
                {
                    offset.X = -data.HorizontalAnchorPoint.X;
                    offset.Y = -data.HorizontalAnchorPoint.Y;
                }


                Vector3 pos = Transform.Position;
                Vector3 scale = new Vector3(width, height, 1);
                Vector3 rot = Transform.Rotation;
                Rectangle source = new Rectangle(textureX, textureY, textureWidth, textureHeight);
                if (!BushFlag())
                {
                    Renderer.FillTexture(texture, ShapeFactory.Rectangle, ref pos, ref scale, ref rot, ref offset, ref source, ref _colour);
                }
                else
                {
                    Color4 color2 = new Color4(1, 1, 1, 0.25f);
                    scale.Y -= 12 * Transform.Scale.Y;
                    source.Height -= 12;
                    Renderer.FillTexture(texture, ShapeFactory.Rectangle, ref pos, ref scale, ref rot, ref offset, ref source, ref _colour);
                    pos.Y += scale.Y;
                    scale.Y = 12 * Transform.Scale.Y;
                    source.Y += source.Height;
                    source.Height = 12;
                    Renderer.FillTexture(texture, ShapeFactory.Rectangle, ref pos, ref scale, ref rot, ref offset, ref source, ref color2);
                }
            }
        }
    }
}

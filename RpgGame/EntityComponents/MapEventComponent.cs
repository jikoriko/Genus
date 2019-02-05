using System;

using OpenTK;
using OpenTK.Input;

using Genus2D.Entities;
using Genus2D.Utililities;
using Genus2D.GameData;
using Genus2D.Graphics;
using Genus2D.Networking;

namespace RpgGame.EntityComponents
{
    public class MapEventComponent : SpriteComponent
    {

        private float _spriteTimer, _spriteTimerMax;

        private MapEvent _mapEvent;
        private SpriteData _spriteData;

        public MapEventComponent(Entity entity, MapEvent mapEvent)
            : base(entity)
        {
            if (mapEvent.SpriteID != -1)
            {
                _spriteData = SpriteData.GetSpriteData(mapEvent.SpriteID);
                SetTexture(Assets.GetTexture("Sprites/" + _spriteData.ImagePath));
                SetSpriteCenter(SpriteCenter.Top);
                SetAnimating(false);
                SetXFrames(4);
                SetYFrames(4);
            }
            else
            {
                SetSpriteCenter(SpriteCenter.Center);
                _spriteData = null;
                SetTexture(Assets.GetTexture("GUI_Textures/EventIcon.png"));
                SetColour(new OpenTK.Graphics.Color4(1, 1, 1, 0.65f));
            }

            SetMapEvent(mapEvent);
            _spriteTimer = 0f;
            _spriteTimerMax = 0.3f;
        }

        private void SetMapEvent(MapEvent mapEvent)
        {
            _mapEvent = mapEvent;
            SetRealPosition();
            SetYFrame((int)_mapEvent.EventDirection);
        }

        public void SetRealPosition()
        {
            Vector3 pos = new Vector3(_mapEvent.RealX + 16, _mapEvent.RealY + 16, 0);
            if (_spriteData != null)
            {
                if (_mapEvent.EventDirection == Direction.Left || _mapEvent.EventDirection == Direction.Right)
                {
                    pos.X += -(GetFrameWidth() / 2) + _spriteData.HorizontalAnchorPoint.X;
                    pos.Y += -(GetFrameHeight() / 2) + _spriteData.HorizontalAnchorPoint.Y;
                }
                else
                {
                    pos.X += -(GetFrameWidth() / 2) + _spriteData.VerticalAnchorPoint.X;
                    pos.Y += -(GetFrameHeight() / 2) + _spriteData.VerticalAnchorPoint.Y;
                }
            }
            pos.Z = -(_mapEvent.MapY * 32);
            Transform.LocalPosition = pos;
        }

        public override void Update(OpenTK.FrameEventArgs e)
        {
            base.Update(e);

            SetRealPosition();// we could do some movement prediction here based on target pos, real pos and movement speed?
            SetYFrame((int)_mapEvent.EventDirection);

            if (_spriteTimer > 0)
                _spriteTimer -= (float)e.Time;

            if (!_mapEvent.Moving())
            {
                SetXFrame(0);
            }
            else
            {
                if (_spriteTimer <= 0)
                {
                    _spriteTimer = _spriteTimerMax;
                    IncrementXFrame();

                    Vector2 dir = Vector2.Zero;
                    switch (_mapEvent.EventDirection)
                    {
                        case Direction.Down:
                            dir.Y = 1;
                            break;
                        case Direction.Left:
                            dir.X = -1;
                            break;
                        case Direction.Right:
                            dir.X = 1;
                            break;
                        case Direction.Up:
                            dir.Y = -1;
                            break;
                    }
                }
            }
            
        }

        public override void Render(FrameEventArgs e)
        {
            PlayerPacket localPacket = RpgClientConnection.Instance.GetLocalPlayerPacket();
            if (!(localPacket.PositionX == _mapEvent.MapX && localPacket.PositionY == _mapEvent.MapY))
            {
                base.Render(e);
            }

        }
    }
}

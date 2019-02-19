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
            _spriteData = SpriteData.GetSpriteData(mapEvent.SpriteID);
            SetAnimating(false);
            SetXFrames(4);
            SetYFrames(4);
            SetMapEvent(mapEvent);
            _spriteTimer = 0f;
            _spriteTimerMax = 0.3f;
        }

        private void SetMapEvent(MapEvent mapEvent)
        {
            _mapEvent = mapEvent;
            SetSpriteID(mapEvent.SpriteID);
            SetRealPosition();
            SetYFrame((int)_mapEvent.EventDirection);
        }

        public void SetRealPosition()
        {
            Vector3 pos = new Vector3(_mapEvent.RealX + 16, _mapEvent.RealY + 16, 0);
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
                }
            }
            
        }

        public override bool BushFlag()
        {
            MapData data = MapComponent.Instance.GetMapData();
            for (int i = 0; i < 3; i++)
            {
                float x = _mapEvent.RealX;
                float y = _mapEvent.RealY;
                if ((x % 32) / 32f < 0.5)
                    x = (int)Math.Floor(x / 32);
                else
                    x = (int)Math.Ceiling(x / 32);

                if ((y % 32) / 32f < 0.3)
                    y = (int)Math.Floor(y / 32);
                else
                    y = (int)Math.Ceiling(y / 32);

                Tuple<int, int> tileInfo = data.GetTile(i, (int)x, (int)y);
                TilesetData.Tileset tileset = TilesetData.GetTileset(tileInfo.Item2);
                if (tileset != null)
                {
                    if (tileset.GetBushFlag(tileInfo.Item1))
                        return true;
                }
            }

            return base.BushFlag();
        }

        public override void Render(FrameEventArgs e)
        {
            PlayerPacket localPacket = RpgClientConnection.Instance.GetLocalPlayerPacket();
            if (localPacket != null && !(localPacket.PositionX == _mapEvent.MapX && localPacket.PositionY == _mapEvent.MapY))
            {
                base.Render(e);
            }
            else
            {
                base.Render(e);
            }

        }
    }
}

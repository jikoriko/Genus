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
            if (_mapEvent.Priority == RenderPriority.BelowPlayer)
                pos.Z = -((_mapEvent.MapY + (_mapEvent.OnBridge ? 3 : 0)) * (32 * (_mapEvent.OnBridge ? 3 : 1)));
            else if (_mapEvent.Priority == RenderPriority.AbovePlayer)
                pos.Z = -((_mapEvent.MapY + (_mapEvent.OnBridge ? 3 : 0)) * (32 * (_mapEvent.OnBridge ? 3 : 1))) - 1;
            else
                pos.Z = -((_mapEvent.MapY + 3) * (32 * 5));
            Transform.LocalPosition = pos;
        }

        public override void Update(OpenTK.FrameEventArgs e)
        {
            base.Update(e);
            if (!_mapEvent.Enabled) return;

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
            if (_mapEvent.Priority == RenderPriority.OnTop) return false;
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
                if (tileInfo != null)
                {
                    TilesetData.Tileset tileset = TilesetData.GetTileset(tileInfo.Item2);
                    if (tileset != null)
                    {
                        if (tileset.GetBushFlag(tileInfo.Item1))
                            return true;
                    }
                }
            }

            return base.BushFlag();
        }

        public override void Render(FrameEventArgs e)
        {
            if (!_mapEvent.Enabled) return;

            PlayerPacket localPacket = RpgClientConnection.Instance.GetLocalPlayerPacket();
            if (localPacket == null || !(localPacket.PositionX == _mapEvent.MapX && localPacket.PositionY == _mapEvent.MapY))
            {
                base.Render(e);

                Renderer.PushStencilDepth(OpenTK.Graphics.OpenGL.StencilOp.Keep, OpenTK.Graphics.OpenGL.StencilFunction.Less);
                Renderer.PushWorldMatrix();
                Renderer.SetFlipUV(false, true);

                Renderer.TranslateWorld(0, GetFrameHeight(), 0);
                OpenTK.Graphics.Color4 prevColor = GetColour();
                OpenTK.Graphics.Color4 color = prevColor;
                color.A = 0.2f;

                SetColour(color);
                base.Render(e);
                SetColour(prevColor);

                Renderer.SetFlipUV(false, false);
                Renderer.PopWorldMatrix();
                Renderer.PopStencilDepth();
            }
        }
    }
}

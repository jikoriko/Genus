using System;

using OpenTK;
using OpenTK.Input;

using Genus2D.Entities;
using Genus2D.Utililities;
using Genus2D.GameData;
using Genus2D.Graphics;
using Genus2D.Networking;
using RpgGame.States;

namespace RpgGame.EntityComponents
{
    public class PlayerComponent : SpriteComponent
    {

        private float _spriteTimer, _spriteTimerMax;

        private MapPlayer _mapPlayer;

        public PlayerComponent(Entity entity, MapPlayer mapPlayer)
            : base(entity)
        {
            SetAnimating(false);
            SetXFrames(4);
            SetYFrames(4);

            _spriteTimer = 0f;
            _spriteTimerMax = 0.3f;

            _mapPlayer = mapPlayer;
            SetPlayerPacket(mapPlayer.GetPlayerPacket());
        }

        public MapPlayer GetMapPlayer()
        {
            return _mapPlayer;
        }

        public void SetPlayerPacket(PlayerPacket playerPacket)
        {
            _mapPlayer.SetPlayerPacket(playerPacket);
            SetSpriteID(playerPacket.SpriteID);
            SetRealPosition();
            SetYFrame((int)playerPacket.Direction);
        }

        public void SetRealPosition()
        {
            Vector3 pos = new Vector3(_mapPlayer.GetPlayerPacket().RealX + 16, _mapPlayer.GetPlayerPacket().RealY + 16, 0);
            pos.Z = -(((int)Math.Ceiling(_mapPlayer.GetPlayerPacket().RealY / 32) + (_mapPlayer.GetPlayerPacket().OnBridge ? 3 : 0)) * 2) - 2;
            Transform.LocalPosition = pos;
        }

        public Vector2 GetTargetPosition()
        {
            return new Vector2(_mapPlayer.GetPlayerPacket().PositionX * 32, _mapPlayer.GetPlayerPacket().PositionY * 32);
        }

        public bool Moving()
        {
            int posX = _mapPlayer.GetPlayerPacket().PositionX * 32;
            int posY = _mapPlayer.GetPlayerPacket().PositionY * 32;
            return (posX != _mapPlayer.GetPlayerPacket().RealX || posY != _mapPlayer.GetPlayerPacket().RealY);
        }

        public override void Update(OpenTK.FrameEventArgs e)
        {
            base.Update(e);

            if (_spriteTimer > 0)
                _spriteTimer -= (float)e.Time;

            KeyboardState keyState = Keyboard.GetState();

            bool running = false;
            if (keyState.IsKeyDown(Key.LShift) && _mapPlayer.GetPlayerPacket().Data.Stamina > 0)
                running = true;

            if (!Moving())
            {
                SetXFrame(0);
            }
            else
            {
                if (_spriteTimer <= 0)
                {
                    _spriteTimer = _spriteTimerMax;
                    if (running)
                        _spriteTimer /= 3.0f;
                    IncrementXFrame();
                }
            }

        }

        public override void LateUpdate(FrameEventArgs e)
        {
            base.LateUpdate(e);

            if (_mapPlayer.UpdateMovement((float)e.Time))
            {
                SetRealPosition();
            }

            PlayerPacket playerPacket = MapComponent.Instance.GetLocalPlayerPacket();
            if (playerPacket != null && playerPacket.PlayerID == _mapPlayer.GetPlayerPacket().PlayerID)
            {
                Vector3 mapPos = new Vector3((Renderer.GetResoultion().X / 2) - _mapPlayer.GetPlayerPacket().RealX, ((Renderer.GetResoultion().Y - 200) / 2) - _mapPlayer.GetPlayerPacket().RealY, 0);
                GameState.Instance.MapEntity.GetTransform().LocalPosition = mapPos;
            }
        }

        public override bool BushFlag()
        {
            MapData data = MapComponent.Instance.GetMapInstance().GetMapData();
            if (data == null)
                return false;
            for (int i = 0; i < 3; i++)
            {
                float x = _mapPlayer.GetPlayerPacket().RealX;
                float y = _mapPlayer.GetPlayerPacket().RealY;
                if ((x % 32) / 32f < 0.5)
                    x = (int)Math.Floor(x / 32);
                else
                    x = (int)Math.Ceiling(x / 32);

                if ((y % 32) / 32f < 0.3)
                    y = (int)Math.Floor(y / 32);
                else
                    y = (int)Math.Ceiling(y / 32);

                Tuple<int, int> tileInfo = data.GetTile(i, (int)x, (int)y);
                if (tileInfo == null)
                    continue;
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
            PlayerPacket localPacket = MapComponent.Instance.GetLocalPlayerPacket();
            if (localPacket != null)
            {
                if (!(localPacket.PositionX == _mapPlayer.GetPlayerPacket().PositionX && localPacket.PositionY == _mapPlayer.GetPlayerPacket().PositionY) ||
                    localPacket.PlayerID == _mapPlayer.GetPlayerPacket().PlayerID)
                {
                    base.Render(e);
                    if (GetSpriteID() == -1)
                        return;

                    Vector3 pos = Transform.Position;
                    pos.Y -= GetFrameHeight() + Renderer.GetFont().GetTextHeight(_mapPlayer.GetPlayerPacket().Username) + 21;
                    pos.X -= Renderer.GetFont().GetTextWidth(_mapPlayer.GetPlayerPacket().Username) / 2;
                    pos.Z -= 100;
                    OpenTK.Graphics.Color4 colour = OpenTK.Graphics.Color4.Red;

                    //Renderer.DisableDepthTest();
                    Renderer.PrintText(_mapPlayer.GetPlayerPacket().Username, ref pos, ref colour);

                    int nameHeight = Renderer.GetFont().GetTextHeight(_mapPlayer.GetPlayerPacket().Username);
                    Vector3 size = new Vector3(GetFrameWidth(), 21, 1);
                    pos.X = Transform.Position.X - (GetFrameWidth() / 2);
                    pos.Y += nameHeight + 5;
                    colour = OpenTK.Graphics.Color4.Black;
                    Renderer.FillShape(ShapeFactory.Rectangle, ref pos, ref size, ref colour);

                    pos.X += 2;
                    pos.Y += 2;
                    size.Y = 5;
                    size.X = (GetFrameWidth() - 4) * ((float)_mapPlayer.GetPlayerPacket().Data.HP / _mapPlayer.GetPlayerPacket().Data.GetMaxHP());
                    colour = OpenTK.Graphics.Color4.Red;
                    Renderer.FillShape(ShapeFactory.Rectangle, ref pos, ref size, ref colour);

                    pos.Y += 7;
                    size.X = (GetFrameWidth() - 4) * ((float)_mapPlayer.GetPlayerPacket().Data.MP / _mapPlayer.GetPlayerPacket().Data.GetMaxMP());
                    colour = OpenTK.Graphics.Color4.Blue;
                    Renderer.FillShape(ShapeFactory.Rectangle, ref pos, ref size, ref colour);

                    pos.Y += 7;
                    size.X = (GetFrameWidth() - 4) * ((float)_mapPlayer.GetPlayerPacket().Data.Stamina / _mapPlayer.GetPlayerPacket().Data.GetMaxStamina());
                    colour = OpenTK.Graphics.Color4.Yellow;
                    Renderer.FillShape(ShapeFactory.Rectangle, ref pos, ref size, ref colour);
                    //Renderer.EnableDepthTest();

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
}

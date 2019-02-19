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
    public class PlayerComponent : SpriteComponent
    {

        private float _spriteTimer, _spriteTimerMax;

        private PlayerPacket _playerPacket;

        public PlayerComponent(Entity entity, PlayerPacket playerPacket)
            : base(entity)
        {
            SetAnimating(false);
            SetXFrames(4);
            SetYFrames(4);

            _spriteTimer = 0f;
            _spriteTimerMax = 0.3f;

            SetPlayerPacket(playerPacket);
        }

        public PlayerPacket GetPlayerPacket()
        {
            return _playerPacket;
        }

        public void SetPlayerPacket(PlayerPacket playerPacket)
        {
            _playerPacket = playerPacket;
            SetSpriteID(playerPacket.SpriteID);
            SetRealPosition();
            SetYFrame((int)_playerPacket.Direction);
        }

        public void SetRealPosition()
        {
            Vector3 pos = new Vector3(_playerPacket.RealX + 16, _playerPacket.RealY + 16, 0);
            pos.Z = -(_playerPacket.PositionY * 32);
            Transform.LocalPosition = pos;
        }

        public Vector2 GetTargetPosition()
        {
            return new Vector2(_playerPacket.PositionX * 32, _playerPacket.PositionY * 32);
        }

        public override void Update(OpenTK.FrameEventArgs e)
        {
            base.Update(e);

            if (_spriteTimer > 0)
                _spriteTimer -= (float)e.Time;

            KeyboardState keyState = Keyboard.GetState();

            bool running = false;
            if (keyState.IsKeyDown(Key.LShift))
                running = true;

            Vector2 targetPos = GetTargetPosition();
            if (_playerPacket.RealX == targetPos.X && _playerPacket.RealY == targetPos.Y)
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

                //Transform.LocalPosition = Transform.LocalPosition + new Vector3(dir * (_playerPacket.MovementSpeed * (float)e.Time));
                // we could do some movement prediction here based on target pos, real pos and movement speed?
            }

        }

        public override bool BushFlag()
        {
            MapData data = MapComponent.Instance.GetMapData();
            if (data == null)
                return false;
            for (int i = 0; i < 3; i++)
            {
                float x = _playerPacket.RealX;
                float y = _playerPacket.RealY;
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
            PlayerPacket localPacket = RpgClientConnection.Instance.GetLocalPlayerPacket();
            if (localPacket != null)
            {
                if (!(localPacket.PositionX == _playerPacket.PositionX && localPacket.PositionY == _playerPacket.PositionY) ||
                    localPacket.PlayerID == _playerPacket.PlayerID)
                {
                    base.Render(e);
                    if (GetSpriteID() == -1)
                        return;

                    Vector3 pos = Transform.Position;
                    pos.Y -= GetFrameHeight() + Renderer.GetFont().GetTextHeight(_playerPacket.Username);
                    pos.X -= Renderer.GetFont().GetTextWidth(_playerPacket.Username) / 2;
                    OpenTK.Graphics.Color4 colour = OpenTK.Graphics.Color4.Red;

                    //Renderer.DisableDepthTest();
                    Renderer.PrintText(_playerPacket.Username, ref pos, ref colour);
                    //Renderer.EnableDepthTest();
                }
            }
        }
    }
}

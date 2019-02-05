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
            SetTexture(Assets.GetTexture("Sprites/player.png"));
            SetSpriteCenter(SpriteCenter.Top);
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
            SetRealPosition();
            SetYFrame((int)_playerPacket.Direction);
        }

        public void SetRealPosition()
        {
            Vector3 pos = new Vector3(_playerPacket.RealX + 16, _playerPacket.RealY + 32, 0);
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

                    Vector2 dir = Vector2.Zero;
                    switch (_playerPacket.Direction)
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

                //Transform.LocalPosition = Transform.LocalPosition + new Vector3(dir * (_playerPacket.MovementSpeed * (float)e.Time));
                // we could do some movement prediction here based on target pos, real pos and movement speed?
            }

        }

        public override void Render(FrameEventArgs e)
        {
            PlayerPacket localPacket = RpgClientConnection.Instance.GetLocalPlayerPacket();
            if (!(localPacket.PositionX == _playerPacket.PositionX && localPacket.PositionY == _playerPacket.PositionY) ||
                localPacket.PlayerID == _playerPacket.PlayerID)
            {
                base.Render(e);

                Vector3 pos = Transform.Position;
                pos.Y -= (GetTexture().GetHeight() / 4) + Renderer.GetFont().GetTextHeight(_playerPacket.Username);
                pos.X -= Renderer.GetFont().GetTextWidth(_playerPacket.Username) / 2;
                OpenTK.Graphics.Color4 colour = OpenTK.Graphics.Color4.Red;

                //Renderer.DisableDepthTest();
                Renderer.PrintText(_playerPacket.Username, ref pos, ref colour);
                //Renderer.EnableDepthTest();
            }
        }
    }
}

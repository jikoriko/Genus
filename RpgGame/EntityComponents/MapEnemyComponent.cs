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
    public class MapEnemyComponent : SpriteComponent
    {

        private float _spriteTimer, _spriteTimerMax;

        private MapEnemy _mapEnemy;
        private EnemyData _enemyData;

        public MapEnemyComponent(Entity entity, MapEnemy mapEnemy)
            : base(entity)
        {
            SetAnimating(false);
            SetXFrames(4);
            SetYFrames(4);
            SetMapEnemy(mapEnemy);
            _spriteTimer = 0f;
            _spriteTimerMax = 0.3f;
        }

        private void SetMapEnemy(MapEnemy mapEnemy)
        {
            _mapEnemy = mapEnemy;
            _enemyData = EnemyData.GetEnemy(mapEnemy.GetEnemyID());
            SetSpriteID(_enemyData.SpriteID);
            SetRealPosition();
            SetYFrame((int)_mapEnemy.Direction);
        }

        public void UpdateMapEnemy(int HP, int mapX, int mapY, float realX, float realY, FacingDirection direction, bool onBridge, bool dead)
        {
            _mapEnemy.HP = HP;
            _mapEnemy.MapX = mapX;
            _mapEnemy.MapY = mapY;
            _mapEnemy.RealX = realX;
            _mapEnemy.RealY = realY;
            _mapEnemy.Direction = direction;
            _mapEnemy.OnBridge = onBridge;
            _mapEnemy.Dead = dead;
        }

        public MapEnemy GetMapEnemy()
        {
            return _mapEnemy;
        }

        public void SetRealPosition()
        {
            Vector3 pos = new Vector3(_mapEnemy.RealX + 16, _mapEnemy.RealY + 16, 0);
            pos.Z = -(((int)Math.Ceiling(_mapEnemy.RealY / 32) + (_mapEnemy.OnBridge ? 3 : 0)) * 2);
            //pos.Z = -((_mapEnemy.MapY + (_mapEnemy.OnBridge ? 3 : 0)) * (32 * (_mapEnemy.OnBridge ? 3 : 1)));
            Transform.LocalPosition = pos;
        }

        public override void Update(OpenTK.FrameEventArgs e)
        {
            base.Update(e);
            if (_mapEnemy.Dead) return;

            SetRealPosition();// we could do some movement prediction here based on target pos, real pos and movement speed?
            SetYFrame((int)_mapEnemy.Direction);

            if (_spriteTimer > 0)
                _spriteTimer -= (float)e.Time;

            if (!_mapEnemy.Moving())
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
                float x = _mapEnemy.RealX;
                float y = _mapEnemy.RealY;
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
            if (_mapEnemy.Dead) return;

            PlayerPacket localPacket = RpgClientConnection.Instance.GetLocalPlayerPacket();
            if (localPacket == null || !(localPacket.PositionX == _mapEnemy.MapX && localPacket.PositionY == _mapEnemy.MapY))
            {
                base.Render(e);

                EnemyData data = _mapEnemy.GetEnemyData();
                Vector3 pos = Transform.Position;
                pos.Y -= GetFrameHeight() + Renderer.GetFont().GetTextHeight(data.Name);
                pos.X -= Renderer.GetFont().GetTextWidth(data.Name) / 2;
                OpenTK.Graphics.Color4 colour = OpenTK.Graphics.Color4.Red;

                //Renderer.DisableDepthTest();
                Renderer.PrintText(data.Name, ref pos, ref colour);

                int nameHeight = Renderer.GetFont().GetTextHeight(data.Name);
                Vector3 size = new Vector3(GetFrameWidth(), 9, 1);
                pos.X = Transform.Position.X - (GetFrameWidth() / 2);
                pos.Y += nameHeight + 5;
                colour = OpenTK.Graphics.Color4.Black;
                Renderer.FillShape(ShapeFactory.Rectangle, ref pos, ref size, ref colour);

                pos.X += 2;
                pos.Y += 2;
                size.Y = 5;
                size.X = (GetFrameWidth() - 4) * ((float)_mapEnemy.HP / _mapEnemy.MaxHP);
                colour = OpenTK.Graphics.Color4.Red;
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

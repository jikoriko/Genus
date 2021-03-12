using Genus2D.Entities;
using Genus2D.GameData;
using Genus2D.Graphics;
using Genus2D.Utililities;
using OpenTK;
using OpenTK.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RpgGame.EntityComponents
{
    public class ProjectileComponent : EntityComponent
    {

        private MapProjectile _projectile;
        private ProjectileData _data;

        public ProjectileComponent(Entity entity, MapProjectile projectile) : base(entity)
        {
            SetProjectile(projectile);
        }

        private void SetProjectile(MapProjectile projectile)
        {
            _projectile = projectile;
            _data = ProjectileData.GetProjectileData(projectile.ProjectileID);
            SetRealPosition(_projectile.Position.X, _projectile.Position.Y);
        }

        public void SetRealPosition(float x, float y)
        {
            _projectile.Position.X = x;
            _projectile.Position.Y = y;
            Vector3 pos = new Vector3(x + 16, y + 16, 0);
            pos.Z = -((int)(Math.Ceiling(pos.Y / 32) + (_projectile.OnBridge ? 3 : 0)) * 2) - 2;
            Transform.LocalPosition = pos;

        }

        public void SetOnBridge(bool onBridge)
        {
            _projectile.OnBridge = onBridge;
        }

        public override void LateUpdate(FrameEventArgs e)
        {
            base.LateUpdate(e);

            _projectile.UpdateMovement((float)e.Time);
            SetRealPosition(_projectile.Position.X, _projectile.Position.Y);

        }

        public override void Render(FrameEventArgs e)
        {
            base.Render(e);

            if (_data != null)
            {
                Texture texture = Assets.GetTexture("Icons/" + _data.IconSheetImage);
                Color4 colour = Color4.White;
                Vector3 position = Transform.Position;

                Vector3 offset = new Vector3(-(_data.BoundsWidth / 2), -(_data.BoundsHeight / 2), 0);
                Vector3 rot = Vector3.Zero;
                switch (_projectile.Direction)
                {
                    case FacingDirection.Left:
                        rot.Z = 90;
                        break;
                    case FacingDirection.Right:
                        rot.Z = 270;
                        break;
                    case FacingDirection.Up:
                        rot.Z = 180;
                        break;
                }
                Vector3 size = new Vector3(_data.BoundsWidth, _data.BoundsHeight, 1);
                int sx = ((_data.IconID % 8) * 32) + _data.AnchorX - (_data.BoundsWidth / 2);
                int sy = ((_data.IconID / 8) * 32) + _data.AnchorY - (_data.BoundsHeight / 2);
                Rectangle src = new Rectangle(sx, sy, _data.BoundsWidth, _data.BoundsHeight);
                Renderer.FillTexture(texture, ShapeFactory.Rectangle, ref position, ref size, ref rot, ref offset, ref src, ref colour);
            }
        }
    }
}

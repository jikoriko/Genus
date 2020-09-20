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
    public class MapItemComponent : EntityComponent
    {

        private MapItem _mapItem;
        private ItemData _data;

        public MapItemComponent(Entity entity, MapItem mapItem) : base(entity)
        {
            SetMapItem(mapItem);
        }

        private void SetMapItem(MapItem mapItem)
        {
            _mapItem = mapItem;
            _data = ItemData.GetItemData(mapItem.ItemID);
            SetRealPosition();
        }

        public MapItem GetMapItem()
        {
            return _mapItem;
        }

        public void SetRealPosition()
        {
            Vector3 pos = new Vector3(_mapItem.MapX * 32, _mapItem.MapY * 32, 0);
            pos.Z = -((_mapItem.MapY + (_mapItem.OnBridge ? 3 : 0)) * 2);
            Transform.LocalPosition = pos;

        }

        public override void Render(FrameEventArgs e)
        {
            base.Render(e);

            if (_data != null)
            {
                if (_mapItem.PlayerID == -1 || _mapItem.PlayerID == RpgClientConnection.Instance.GetLocalPlayerPacket().PlayerID)
                {
                    Texture texture = Assets.GetTexture("Icons/" + _data.IconSheetImage);
                    Color4 colour = Color4.White;
                    Vector3 position = Transform.Position;
                    Vector3 size = new Vector3(32, 32, 1);
                    int sx = (_data.IconID % 8) * 32;
                    int sy = (_data.IconID / 8) * 32;
                    Rectangle src = new Rectangle(sx, sy, 32, 32);
                    Renderer.FillTexture(texture, ShapeFactory.Rectangle, ref position, ref size, ref src, ref colour);
                }
            }
        }
    }
}

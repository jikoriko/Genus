using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RpgEditor
{
    public class IconSheetPanel : Panel
    {

        private Genus2D.GameData.ItemData _itemData;
        private Image _iconSheetImage;

        public IconSheetPanel()
        {
            _itemData = null;
            _iconSheetImage = null;
            this.DoubleBuffered = true;
        }

        public void SetItemData(Genus2D.GameData.ItemData data)
        {
            _itemData = data;
            if (_iconSheetImage != null)
            {
                _iconSheetImage.Dispose();
                _iconSheetImage = null;
            }

            if (_itemData != null)
            {
                if (_itemData.IconSheetImage != "")
                {
                    _iconSheetImage = Image.FromFile("Assets/Textures/Icons/" + _itemData.IconSheetImage);
                }
            }

            this.Refresh();
        }

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);
            if (_itemData != null)
            {
                if (((MouseEventArgs)e).Button == MouseButtons.Left)
                {
                    Point mouse = this.PointToClient(Cursor.Position);
                    int x = mouse.X / 32;
                    int y = mouse.Y / 32;

                    int id = x + (y * 8);
                    _itemData.IconID = id;
                    this.Refresh();
                }
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (_iconSheetImage != null)
            {
                e.Graphics.DrawImage(_iconSheetImage, 0, 0, 256, 256);
            }

            if (_itemData != null)
            {
                Rectangle src = new Rectangle(2 + (_itemData.IconID % 8) * 32, 2 + (_itemData.IconID / 8) * 32, 28, 28);
                e.Graphics.DrawRectangle(new Pen(Color.Black, 8), src);
                e.Graphics.DrawRectangle(new Pen(Color.White, 2), src);
            }

        }
    }
}

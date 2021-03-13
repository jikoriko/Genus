using Genus2D.GameData;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RpgEditor
{
    public class ItemSpriteViewerPanel : Panel
    {

        private Genus2D.GameData.ItemData _itemData;
        private Image _sprite;

        public ItemSpriteViewerPanel(Genus2D.GameData.ItemData itemData)
        {
            _itemData = itemData;

            _sprite = null;
            this.AutoScroll = true;
            this.DoubleBuffered = true;
        }

        public void SetSprite(Image sprite)
        {
            _sprite = sprite;
            if (sprite != null)
            {
                this.AutoScrollMinSize = new Size(sprite.Width, sprite.Height);
            }
            else
            {
                this.AutoScrollMinSize = new Size(0, 0);
            }
            this.Refresh();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (_sprite != null)
            {
                e.Graphics.DrawImage(_sprite, 0, 0, _sprite.Width, _sprite.Height);
                int spriteWidth = _sprite.Width / 4;
                int spriteHeight = _sprite.Height / 4;

                for (int x = 0; x < 4; x++)
                {
                    for (int y = 0; y < 4; y++)
                    {
                        Vector2 anchor;
                        if (x == (int)EditEquipableSpriteForm.Insance.GetSelectedFrame() && y == (int)EditEquipableSpriteForm.Insance.GetSelectedDirection())
                        {
                            anchor = EditEquipableSpriteForm.Insance.GetSelectedAnchor();
                        }
                        else
                        {
                            anchor = _itemData.GetEquipableAnchor((FacingDirection)y, x);
                        }
                        int xPos = (x * spriteWidth) + (int)anchor.X;
                        int yPos = (y * spriteHeight) + (int)anchor.Y;

                        e.Graphics.DrawRectangle(new Pen(Color.Black, 1), x * spriteWidth, y * spriteHeight, spriteWidth, spriteHeight);
                        e.Graphics.FillRectangle(new SolidBrush(Color.Red), xPos - 2, yPos - 2, 4, 4);
                    }
                }
            }
        }

    }
}

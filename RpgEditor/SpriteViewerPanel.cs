using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RpgEditor
{
    public class SpriteViewerPanel : Panel
    {

        private Image Sprite;

        public SpriteViewerPanel()
        {
            Sprite = null;
        }

        public void SetSprite(Image sprite)
        {
            Sprite = sprite;
            this.Refresh();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (Sprite != null)
            {
                e.Graphics.DrawImage(Sprite, 0, 0, Sprite.Width, Sprite.Height);
                int spriteWidth = Sprite.Width / 4;
                int spriteHeight = Sprite.Height / 4;
                for (int x = 0; x < 4; x++)
                {
                    for (int y = 0; y < 4; y++)
                    {
                        Point anchor = y == 0 || y == 3 ? EditorForm.Instance.GetSpriteVerticalAnchor() : EditorForm.Instance.GetSpriteHorizontalAnchor();
                        Point bounds = y == 0 || y == 3 ? EditorForm.Instance.GetSpriteVerticalBounds() : EditorForm.Instance.GetSpriteHorizontalBounds();
                        int xPos = (x * spriteWidth) + anchor.X;
                        int yPos = (y * spriteHeight) + anchor.Y;

                        e.Graphics.FillRectangle(new SolidBrush(Color.Red), xPos - 1, yPos - 1, 2, 2);
                        e.Graphics.DrawRectangle(new Pen(Color.Red, 2), xPos - (bounds.X / 2), yPos - (bounds.Y / 2), bounds.X, bounds.Y);
                    }
                }
            }

        }
    }
}

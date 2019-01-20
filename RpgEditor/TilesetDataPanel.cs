using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace RpgEditor
{
    public class TilesetDataPanel : Panel
    {

        private Image _tilesetImage;

        public TilesetDataPanel() : base()
        {
            _tilesetImage = null;

            this.AutoScroll = true;
            this.DoubleBuffered = true;

            //this.Anchor = (AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Right | AnchorStyles.Top);
        }

        public void SetTilesetImage(Image image)
        {
            _tilesetImage = image;
            if (image != null)
            {
                this.AutoScrollMinSize = new Size(256, image.Height);
            }
            else
            {
                this.AutoScrollMinSize = new Size(0, 0);
            }
            this.Refresh();
        }

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);

            if (_tilesetImage == null)
                return;

            MouseEventArgs args = (MouseEventArgs)e;
            if (args.Button == MouseButtons.Left)
            {
                Point mouse = this.PointToClient(Cursor.Position);
                int tileX = (mouse.X + HorizontalScroll.Value) / 32;
                int tileY = (mouse.Y + VerticalScroll.Value) / 32;

                EditorForm.TilesetProperties property = EditorForm.Instance.CurrentTilesetProperty();
                switch (property)
                {
                    case EditorForm.TilesetProperties.Passabilities:
                        bool passable = EditorForm.Instance.GetSelectedTileset().Pasabilities[tileX, tileY] ? false : true;
                        EditorForm.Instance.GetSelectedTileset().SetPassable(tileX, tileY, passable);
                        break;
                    case EditorForm.TilesetProperties.Priorities:
                        int priority = EditorForm.Instance.GetSelectedTileset().Priorities[tileX, tileY] + 1;
                        if (priority > 5)
                            priority = 0;
                        EditorForm.Instance.GetSelectedTileset().SetPriority(tileX, tileY, priority);
                        break;
                }
                this.Refresh();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (_tilesetImage != null)
            {
                e.Graphics.DrawImage(_tilesetImage, -this.HorizontalScroll.Value, -this.VerticalScroll.Value, _tilesetImage.Width, _tilesetImage.Height);

                EditorForm.TilesetProperties property = EditorForm.Instance.CurrentTilesetProperty();
                Font font = new Font("Arial", 22);
                SolidBrush brush = new SolidBrush(Color.Black);


                for (int x = 0; x < 8; x++)
                {
                    for (int y = 0; y < _tilesetImage.Height / 32; y++)
                    {
                        int xPos = (x * 32) - HorizontalScroll.Value;
                        int yPos = (y * 32) - VerticalScroll.Value;
                        string s = "";
                        switch (property)
                        {
                            case EditorForm.TilesetProperties.Passabilities:
                                s = EditorForm.Instance.GetSelectedTileset().Pasabilities[x, y] == true ? "O" : "X";
                                break;
                            case EditorForm.TilesetProperties.Priorities:
                                s = EditorForm.Instance.GetSelectedTileset().Priorities[x, y].ToString();
                                break;
                        }
                        e.Graphics.DrawString(s, font, brush, xPos, yPos);
                    }
                }
            }
        }
    }
}

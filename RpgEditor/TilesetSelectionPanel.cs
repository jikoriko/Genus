using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace RpgEditor
{
    public class TilesetSelectionPanel : Panel
    {

        private Genus2D.GameData.TilesetData.Tileset _tileset;
        private Image _tilesetImage;
        private Image[] _autoTileImages;

        private int _selectedTileStartX = 0;
        private int _selectedTileStartY = 0;
        private int _selectedTileEndX = 0;
        private int _selectedTileEndY = 0;

        private bool _grabbed = false;
        private int _lastX = -1;
        private int _lastY = -1;

        public TilesetSelectionPanel() : base()
        {
            _tileset = null;
            _tilesetImage = null;
            _autoTileImages = new Image[7];

            this.AutoScroll = true;
            this.DoubleBuffered = true;

            this.BackColor = Color.LightGray;

            this.Anchor = (AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Right | AnchorStyles.Top);
        }

        public void SetTileset(Genus2D.GameData.TilesetData.Tileset tileset)
        {
            _tileset = tileset;
            _selectedTileStartX = 0;
            _selectedTileStartY = 0;
            _selectedTileEndX = 0;
            _selectedTileEndY = 0;

            if (_tilesetImage != null)
                _tilesetImage.Dispose();

            if (_tileset != null)
            {
                if (tileset.ImagePath != "")
                {
                    _tilesetImage = Image.FromFile("Assets/Textures/Tilesets/" + tileset.ImagePath);
                    this.AutoScrollMinSize = new Size(256, _tilesetImage.Height + 32);
                }
                else
                {
                    _tilesetImage = null;
                    this.AutoScrollMinSize = new Size(0, 0);
                }

                for (int i = 0; i < _autoTileImages.Length; i++)
                {
                    if (_autoTileImages[i] != null)
                        _autoTileImages[i].Dispose();
                    _autoTileImages[i] = null;

                    if (_tileset.GetAutoTile(i) != "")
                    {
                        _autoTileImages[i] = Image.FromFile("Assets/Textures/AutoTiles/" + _tileset.GetAutoTile(i));
                    }
                }
            }
            else
            {
                this.AutoScrollMinSize = new Size(0, 0);
                for (int i = 0; i < _autoTileImages.Length; i++)
                {
                    if (_autoTileImages[i] != null)
                        _autoTileImages[i].Dispose();
                    _autoTileImages[i] = null;
                }
            }
            this.Refresh();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnClick(e);

            if (e.Button == MouseButtons.Left)
            {
                Point mouse = this.PointToClient(Cursor.Position);
                int startX = (mouse.X + HorizontalScroll.Value) / 32;
                int startY = (mouse.Y + VerticalScroll.Value) / 32;
                if (_tilesetImage == null && startY > 0) return;
                else if (_tilesetImage != null)
                {
                    if (startY >= (_tilesetImage.Height / 32) + 1) return;
                    _grabbed = true;
                    _selectedTileStartX = startX;
                    _selectedTileStartY = startY;
                    _selectedTileEndX = _selectedTileStartX;
                    _selectedTileEndY = _selectedTileStartY;

                    _lastX = _selectedTileStartX;
                    _lastY = _selectedTileStartY;
                }

                this.Refresh();
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (_grabbed)
            {
                Point mouse = this.PointToClient(Cursor.Position);
                int tileX = (mouse.X + HorizontalScroll.Value) / 32;
                int tileY = (mouse.Y + VerticalScroll.Value) / 32;

                if (tileX != _lastX || tileY != _lastY)
                {
                    _selectedTileEndX = tileX;
                    _selectedTileEndY = tileY;

                    _lastX = tileX;
                    _lastY = tileY;

                    this.Refresh();
                }
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            if (_grabbed)
            {
                _grabbed = false;
                _lastX = -1;
                _lastY = -1;

                Rectangle selection = GetSelectionRectangle();
                if (selection.Y == 0 || EditorForm.Instance.GetMapTool() == EditorForm.MapTool.FloodFill)
                {
                    _selectedTileStartX = selection.X;
                    _selectedTileEndX = selection.X;
                    _selectedTileStartY = selection.Y;
                    _selectedTileEndY = selection.Y;
                    this.Refresh();
                }
            }
        }

        public Rectangle GetSelectionRectangle()
        {
            int x = _selectedTileStartX <= _selectedTileEndX ? _selectedTileStartX : _selectedTileEndX;
            int y = _selectedTileStartY <= _selectedTileEndY ? _selectedTileStartY : _selectedTileEndY;
            int width = Math.Abs(_selectedTileStartX - _selectedTileEndX) + 1;
            int height = Math.Abs(_selectedTileStartY - _selectedTileEndY) + 1;

            if (x < 0)
            {
                width += x;
                x = 0;
            }
            if (y < 0)
            {
                height += y;
                y = 0;
            }
            if (width > 8) width = 8;
            if (width > 8 - x) width = 8 - x;

            int maxHeight = _tilesetImage == null ? 1 : (_tilesetImage.Height / 32) + 1;
            if (height > maxHeight) height = maxHeight;
            if (height > maxHeight - y) height = maxHeight - y;

            return new Rectangle(x, y, width, height);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Rectangle rect = new Rectangle(-HorizontalScroll.Value, -VerticalScroll.Value, 256, 32);
            e.Graphics.FillRectangle(new SolidBrush(Color.Black), rect);

            for (int i = 0; i < _autoTileImages.Length; i++)
            {
                Image autoTile = _autoTileImages[i];
                if (autoTile != null)
                {
                    Rectangle source = new Rectangle(0, 0, 32, 32);
                    e.Graphics.DrawImage(autoTile, new Rectangle(32 + (i * 32) - this.HorizontalScroll.Value, -this.VerticalScroll.Value, 32, 32), source, GraphicsUnit.Pixel);
                }
            }

            if (_tilesetImage != null)
            {
                e.Graphics.DrawImage(_tilesetImage, -this.HorizontalScroll.Value, -this.VerticalScroll.Value + 32, _tilesetImage.Width, _tilesetImage.Height);
                e.Graphics.DrawLine(new Pen(Color.Black, 2), -HorizontalScroll.Value, 32 - VerticalScroll.Value, 256 - HorizontalScroll.Value, 32 - VerticalScroll.Value);
            }

            Rectangle selectionRect = GetSelectionRectangle();
            rect = new Rectangle((selectionRect.X * 32) - HorizontalScroll.Value, (selectionRect.Y * 32) - VerticalScroll.Value, selectionRect.Width * 32, selectionRect.Height * 32);
            e.Graphics.DrawRectangle(new Pen(Color.Black, 8), rect);
            e.Graphics.DrawRectangle(new Pen(Color.White, 2), rect);
        }
    }
}

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

        private Image _tilesetImage;

        private int _selectedTileStartX = 0;
        private int _selectedTileStartY = 0;
        private int _selectedTileEndX = 0;
        private int _selectedTileEndY = 0;

        private bool _grabbed = false;
        private int _lastX = -1;
        private int _lastY = -1;

        public TilesetSelectionPanel() : base()
        {
            _tilesetImage = null;

            this.AutoScroll = true;
            this.DoubleBuffered = true;

            this.Anchor = (AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Right | AnchorStyles.Top);
        }

        public void SetTilesetImage(Image image)
        {
            _tilesetImage = image;
            this.AutoScrollMinSize = new Size(256, image.Height);
            this.Refresh();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnClick(e);
            if (_tilesetImage == null) return;

            if (e.Button == MouseButtons.Left)
            {
                _grabbed = true;

                Point mouse = this.PointToClient(Cursor.Position);
                _selectedTileStartX = (mouse.X + HorizontalScroll.Value) / 32;
                _selectedTileStartY = (mouse.Y + VerticalScroll.Value) / 32;
                _selectedTileEndX = _selectedTileStartX;
                _selectedTileEndY = _selectedTileStartY;

                _lastX = _selectedTileStartX;
                _lastY = _selectedTileStartY;

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
            }
        }

        public Rectangle GetSelectionRectangle()
        {
            int x = _selectedTileStartX <= _selectedTileEndX ? _selectedTileStartX : _selectedTileEndX;
            int y = _selectedTileStartY <= _selectedTileEndY ? _selectedTileStartY : _selectedTileEndY;
            int width = Math.Abs(_selectedTileStartX - _selectedTileEndX) + 1;
            int height = Math.Abs(_selectedTileStartY - _selectedTileEndY) + 1;

            if (width > 8) width = 8 - x;
            if (height > _tilesetImage.Height / 32) height = (_tilesetImage.Height / 32) - y;

            return new Rectangle(x, y, width, height);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (_tilesetImage != null)
            {
                e.Graphics.DrawImage(_tilesetImage, -this.HorizontalScroll.Value, -this.VerticalScroll.Value, _tilesetImage.Width, _tilesetImage.Height);

                Rectangle selectionRect = GetSelectionRectangle();

                e.Graphics.DrawRectangle(new Pen(Color.Red), (selectionRect.X * 32) - HorizontalScroll.Value, (selectionRect.Y * 32) - VerticalScroll.Value, selectionRect.Width * 32, selectionRect.Height * 32);
            }
        }
    }
}

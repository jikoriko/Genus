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

        private Genus2D.GameData.TilesetData.Tileset _tileset;
        private Image _tilesetImage;
        private Image[] _autoTileImages;

        public TilesetDataPanel() : base()
        {
            _tileset = null;
            _tilesetImage = null;

            _autoTileImages = new Image[7];

            this.AutoScroll = true;
            this.DoubleBuffered = true;
        }

        public void SetTileset(Genus2D.GameData.TilesetData.Tileset tileset)
        {
            _tileset = tileset;
            if (_tileset != null)
            {
                if (_tilesetImage != null)
                    _tilesetImage.Dispose();
                _tilesetImage = null;
                if (tileset.ImagePath != "")
                    _tilesetImage = Image.FromFile("Assets/Textures/Tilesets/" + tileset.ImagePath);

                for (int i = 0; i < _autoTileImages.Length; i++)
                {
                    if (_autoTileImages[i] != null)
                        _autoTileImages[i].Dispose();
                    _autoTileImages[i] = null;
                    if (tileset.GetAutoTile(i) != "")
                    {
                        _autoTileImages[i] = Image.FromFile("Assets/Textures/AutoTiles/" + tileset.GetAutoTile(i));
                    }
                }

                if (_tilesetImage != null)
                {
                    this.AutoScrollMinSize = new Size(256, _tilesetImage.Height + 32);
                }
                else
                {
                    this.AutoScrollMinSize = new Size(256, 32);
                }
            }
            else
            {
                _tilesetImage = null;
                for (int i = 0; i < _autoTileImages.Length; i++)
                {
                    _autoTileImages[i] = null;
                }
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
            if (args.Button == MouseButtons.Left || args.Button == MouseButtons.Right)
            {
                bool leftMouse = args.Button == MouseButtons.Left;
                EditorForm.TilesetProperties property = EditorForm.Instance.CurrentTilesetProperty();
                if (!leftMouse && property != EditorForm.TilesetProperties.TerrainTags &&
                        property != EditorForm.TilesetProperties.Priorities)
                    return;
                Point mouse = this.PointToClient(Cursor.Position);
                int tileX = (mouse.X + HorizontalScroll.Value) / 32;
                int tileY = (mouse.Y + VerticalScroll.Value) / 32;

                if (tileX == 0 && tileY == 0)
                    return;

                bool passable;
                switch (property)
                {
                    case EditorForm.TilesetProperties.Passabilities:

                        passable = EditorForm.Instance.GetSelectedTileset().GetPassable(tileX, tileY) ? false : true;
                        EditorForm.Instance.GetSelectedTileset().SetPassable(tileX, tileY, passable);

                        break;
                    case EditorForm.TilesetProperties.Passabilities8Dir:

                        float subX = (((mouse.X + HorizontalScroll.Value) % 32) + 1) / 32.0f;
                        float subY = (((mouse.Y + VerticalScroll.Value) % 32) + 1) / 32.0f;

                        int dir = -1;

                        if (subX < 0.33f)
                        {
                            if (subY < 0.33f)
                                dir = (int)Genus2D.GameData.MovementDirection.UpperLeft;
                            else if (subY < 0.66f)
                                dir = (int)Genus2D.GameData.MovementDirection.Left;
                            else
                                dir = (int)Genus2D.GameData.MovementDirection.LowerLeft;
                        }
                        else if (subX < 0.66f)
                        {
                            if (subY < 0.33f)
                                dir = (int)Genus2D.GameData.MovementDirection.Up;
                            else if (subY >= 0.66f)
                                dir = (int)Genus2D.GameData.MovementDirection.Down;
                        }
                        else
                        {
                            if (subY < 0.33f)
                                dir = (int)Genus2D.GameData.MovementDirection.UpperRight;
                            else if (subY < 0.66f)
                                dir = (int)Genus2D.GameData.MovementDirection.Right;
                            else
                                dir = (int)Genus2D.GameData.MovementDirection.LowerRight;
                        }

                        if (dir == -1)
                        {
                            passable = EditorForm.Instance.GetSelectedTileset().GetPassable(tileX, tileY) ? false : true;
                            EditorForm.Instance.GetSelectedTileset().SetPassable(tileX, tileY, passable);
                        }
                        else
                        {
                            Genus2D.GameData.MovementDirection direction = (Genus2D.GameData.MovementDirection)dir;
                            passable = EditorForm.Instance.GetSelectedTileset().GetPassable(tileX, tileY, direction) ? false : true;
                            EditorForm.Instance.GetSelectedTileset().SetPassable(tileX, tileY, direction, passable);
                        }

                        break;
                    case EditorForm.TilesetProperties.Priorities:
                        int priority = EditorForm.Instance.GetSelectedTileset().GetTilePriority(tileX, tileY) + (leftMouse ? 1 : -1);
                        if (priority > 5)
                            priority = 0;
                        else if (priority < 0)
                            priority = 5;
                        EditorForm.Instance.GetSelectedTileset().SetPriority(tileX, tileY, priority);
                        break;
                    case EditorForm.TilesetProperties.TerrainTags:
                        int tag = EditorForm.Instance.GetSelectedTileset().GetTerrainTag(tileX, tileY) + (leftMouse ? 1 : -1);
                        if (tag > 9)
                            tag = 0;
                        else if (tag < 0)
                            tag = 9;
                        EditorForm.Instance.GetSelectedTileset().SetTerrainTag(tileX, tileY, tag);
                        break;
                    case EditorForm.TilesetProperties.BushFlags:
                        passable = EditorForm.Instance.GetSelectedTileset().GetBushFlag(tileX, tileY) ? false : true;
                        EditorForm.Instance.GetSelectedTileset().SetBushFlag(tileX, tileY, passable);
                        break;
                    case EditorForm.TilesetProperties.CounterFlags:
                        passable = EditorForm.Instance.GetSelectedTileset().GetCounterFlag(tileX, tileY) ? false : true;
                        EditorForm.Instance.GetSelectedTileset().SetCounterFlag(tileX, tileY, passable);
                        break;
                }
                this.Refresh();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

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

                EditorForm.TilesetProperties property = EditorForm.Instance.CurrentTilesetProperty();
                Font font = new Font("Arial", property == EditorForm.TilesetProperties.Passabilities8Dir ? 10 : 24, FontStyle.Bold);
                SolidBrush brush = new SolidBrush(Color.Black);


                for (int x = 0; x < 8; x++)
                {
                    for (int y = 0; y < (_tilesetImage.Height / 32) + 1; y++)
                    {
                        if (x == 0 && y == 0)
                            continue;
                        int xPos = (x * 32) - HorizontalScroll.Value;
                        int yPos = (y * 32) - VerticalScroll.Value;
                        string s = "";

                        switch (property)
                        {
                            case EditorForm.TilesetProperties.Passabilities:

                                s = EditorForm.Instance.GetSelectedTileset().GetPassable(x, y) ? "O" : "X";
                                e.Graphics.DrawString(s, font, brush, xPos, yPos);

                                break;
                            case EditorForm.TilesetProperties.Passabilities8Dir:

                                for (int dir = 0; dir < 8; dir++)
                                {
                                    Genus2D.GameData.MovementDirection direction = (Genus2D.GameData.MovementDirection)dir;

                                    int offsetX = 0;
                                    int offsetY = 0;

                                    switch (direction)
                                    {
                                        case Genus2D.GameData.MovementDirection.Down:
                                            offsetX = 10;
                                            offsetY = 20;
                                            break;
                                        case Genus2D.GameData.MovementDirection.Left:
                                            offsetY = 10;
                                            break;
                                        case Genus2D.GameData.MovementDirection.Right:
                                            offsetX = 20;
                                            offsetY = 10;
                                            break;
                                        case Genus2D.GameData.MovementDirection.Up:
                                            offsetX = 10;
                                            break;
                                        case Genus2D.GameData.MovementDirection.UpperRight:
                                            offsetX = 20;
                                            break;
                                        case Genus2D.GameData.MovementDirection.LowerLeft:
                                            offsetY = 20;
                                            break;
                                        case Genus2D.GameData.MovementDirection.LowerRight:
                                            offsetX = 20;
                                            offsetY = 20;
                                            break;
                                    }

                                    s = EditorForm.Instance.GetSelectedTileset().GetPassable(x, y, direction) ? "O" : "X";
                                    e.Graphics.DrawString(s, font, brush, xPos + offsetX, yPos + offsetY);
                                }

                                break;
                            case EditorForm.TilesetProperties.Priorities:
                                s = EditorForm.Instance.GetSelectedTileset().GetTilePriority(x, y).ToString();
                                e.Graphics.DrawString(s, font, brush, xPos, yPos);
                                break;
                            case EditorForm.TilesetProperties.TerrainTags:
                                s = EditorForm.Instance.GetSelectedTileset().GetTerrainTag(x, y).ToString();
                                e.Graphics.DrawString(s, font, brush, xPos, yPos);
                                break;
                            case EditorForm.TilesetProperties.BushFlags:
                                s = EditorForm.Instance.GetSelectedTileset().GetBushFlag(x, y) ? "O" : "X";
                                e.Graphics.DrawString(s, font, brush, xPos, yPos);
                                break;
                            case EditorForm.TilesetProperties.CounterFlags:
                                s = EditorForm.Instance.GetSelectedTileset().GetCounterFlag(x, y) ? "O" : "X";
                                e.Graphics.DrawString(s, font, brush, xPos, yPos);
                                break;
                        }
                    }
                }
            }
        }
    }
}

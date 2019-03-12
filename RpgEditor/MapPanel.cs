using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace RpgEditor
{
    public class MapPanel : Panel
    {

        private Genus2D.GameData.MapData MapData;
        private int MapID;

        private List<Image> _tilesetImages;
        public Dictionary<string, Image> AutoTileImages;

        private bool _leftGrabbed;
        private bool _rightGrabbed;
        private int _lastX;
        private int _lastY;

        private int _startX;
        private int _startY;
        private int _endX;
        private int _endY;

        public MapPanel() : base()
        {
            MapData = null;

            this.AutoScroll = true;
            this.DoubleBuffered = true;
            this.Anchor = (AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Right | AnchorStyles.Top);
            this.BackColor = Color.LightGray;
            _tilesetImages = new List<Image>();
            AutoTileImages = new Dictionary<string, Image>();
        }

        public Genus2D.GameData.MapData GetMapData()
        {
            return MapData;
        }

        public void SetMapData(Genus2D.GameData.MapData data, int mapID)
        {
            MapData = data;
            MapID = mapID;
            if (data != null)
                this.AutoScrollMinSize = new Size(data.GetWidth() * 32, data.GetHeight() * 32);
            else
                this.AutoScrollMinSize = new Size(0, 0);
        }

        public void ClearTilesetImages()
        {
            for (int i = 0; i < _tilesetImages.Count; i++)
            {
                _tilesetImages[i].Dispose();
            }
            _tilesetImages.Clear();
        }

        public void AddTilesetImage(string name)
        {
            if (name == "")
                return;
            Image image = Image.FromFile("Assets/Textures/Tilesets/" + name);
            _tilesetImages.Add(image);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnClick(e);
            if (MapData == null) return;

            if (_leftGrabbed || _rightGrabbed) return;

            if (e.Button == MouseButtons.Left || e.Button == MouseButtons.Right)
            {
                Point mouse = this.PointToClient(Cursor.Position);
                if (mouse.X < MapData.GetWidth() * 32 && mouse.Y < MapData.GetHeight() * 32)
                {
                    int tileX = (mouse.X + HorizontalScroll.Value) / 32;
                    int tileY = (mouse.Y + VerticalScroll.Value) / 32;
                    _lastX = tileX;
                    _lastY = tileY;

                    if (e.Button == MouseButtons.Left)
                    {
                        _leftGrabbed = true;
                        EditorForm.MapTool tool = EditorForm.Instance.GetMapTool();
                        if (tool == EditorForm.MapTool.Rectangle)
                        {
                            _startX = tileX;
                            _startY = tileY;
                            _endX = tileX;
                            _endY = tileY;
                            this.Refresh();
                        }
                        else if (tool == EditorForm.MapTool.Pencil)
                        {
                            PaintSelection(tool);
                            this.Refresh();
                        }
                    }
                    else
                    {
                        _rightGrabbed = true;
                    }

                }
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (_leftGrabbed || _rightGrabbed)
            {
                Point mouse = this.PointToClient(Cursor.Position);
                int tileX = (mouse.X + HorizontalScroll.Value) / 32;
                int tileY = (mouse.Y + VerticalScroll.Value) / 32;
                if (tileX != _lastX || tileY != _lastY)
                {
                    _lastX = tileX;
                    _lastY = tileY;
                    if (_leftGrabbed)
                    {
                        EditorForm.MapTool tool = EditorForm.Instance.GetMapTool();
                        if (tool == EditorForm.MapTool.Rectangle)
                        {
                            _endX = tileX;
                            _endY = tileY;
                        }
                        else if (tool == EditorForm.MapTool.Pencil)
                        {
                            PaintSelection(tool);
                        }
                    }
                }
            }

            this.Refresh();
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            if (_leftGrabbed || _rightGrabbed)
            {
                EditorForm.MapTool tool = EditorForm.Instance.GetMapTool();
                if (_leftGrabbed)
                {
                    if (tool != EditorForm.MapTool.Pencil)
                    {
                        PaintSelection(tool);
                        this.Refresh();
                    }
                }
                else
                {
                    if (tool == EditorForm.MapTool.Event)
                    {
                        for (int i = 0; i < MapData.MapEventsCount(); i++)
                        {
                            Genus2D.GameData.MapEvent mapEvent = MapData.GetMapEvent(i);
                            if (mapEvent.MapX == _lastX && mapEvent.MapY == _lastY)
                            {
                                Genus2D.GameData.MapInfo.SetMapEventsCount(MapID, MapData.MapEventsCount() - 1);
                                MapData.RemoveMapEvent(i);
                                this.Refresh();
                                break;
                            }
                        }
                    }
                    else if (tool == EditorForm.MapTool.SpawnPoint)
                    {
                        for (int i = 0; i < Genus2D.GameData.MapInfo.NumberSpawnPoints(); i++)
                        {
                            Genus2D.GameData.SpawnPoint spawn = Genus2D.GameData.MapInfo.GetSpawnPoint(i);
                            if (spawn.MapID == MapID && spawn.MapX == _lastX && spawn.MapY == _lastY)
                            {
                                Genus2D.GameData.MapInfo.RemoveSpawnPoint(i);
                                this.Refresh();
                                break;
                            }
                        }
                    }
                }

                _lastX = -1;
                _lastY = -1;
                _leftGrabbed = false;
                _rightGrabbed = false;
                this.Refresh();
            }
        }

        private Rectangle GetSelectionRectangle()
        {
            if (EditorForm.Instance.GetMapTool() == EditorForm.MapTool.Pencil)
            {
                Point mouse = this.PointToClient(Cursor.Position);
                int x = (mouse.X + HorizontalScroll.Value) / 32;
                int y = (mouse.Y + VerticalScroll.Value) / 32;
                int width = 1;
                int height = 1;

                int tileset = EditorForm.Instance.GetSelectedMapTileset();
                if (tileset != -1)
                {
                    Rectangle tilesetSelection = EditorForm.Instance.tilesetSelectionPanel.GetSelectionRectangle();
                    width = tilesetSelection.Width;
                    height = tilesetSelection.Height;

                    if (width > MapData.GetWidth() - x)
                        width = MapData.GetWidth() - x;
                    if (height > MapData.GetHeight() - y)
                        height = MapData.GetHeight() - y;
                }

                return new Rectangle(x, y, width, height);
            }
            else if (_leftGrabbed && EditorForm.Instance.GetMapTool() == EditorForm.MapTool.Rectangle)
            {
                int x = _startX <= _endX ? _startX : _endX;
                int y = _startY <= _endY ? _startY : _endY;
                int width = Math.Abs(_startX - _endX) + 1;
                int height = Math.Abs(_startY - _endY) + 1;

                if (width > MapData.GetWidth() - x)
                    width = MapData.GetWidth() - x;
                if (height > MapData.GetHeight() - y)
                    height = MapData.GetHeight() - y;

                return new Rectangle(x, y, width, height);
            }
            else
            {
                Point mouse = this.PointToClient(Cursor.Position);
                int x = (mouse.X + HorizontalScroll.Value) / 32;
                int y = (mouse.Y + VerticalScroll.Value) / 32;
                return new Rectangle(x, y, 1, 1);
            }
        }

        private int TileToID(int x, int y)
        {
            return x + (y * 8);
        }

        private void PaintSelection(EditorForm.MapTool tool)
        {
            Rectangle tilesetSelection = EditorForm.Instance.tilesetSelectionPanel.GetSelectionRectangle();
            int layer = EditorForm.Instance.GetMapLayer();
            int tileset = EditorForm.Instance.GetSelectedMapTileset();
            if (tileset == -1)
                return;

            switch (tool)
            {
                case EditorForm.MapTool.Pencil:
                    for (int x = 0; x < tilesetSelection.Width; x++)
                    {
                        for (int y = 0; y < tilesetSelection.Height; y++)
                        {
                            MapData.SetTile(layer, _lastX + x, _lastY + y, TileToID(tilesetSelection.X + x, tilesetSelection.Y + y), tileset);
                        }
                    }
                    break;
                case EditorForm.MapTool.Rectangle:
                    Rectangle selection = GetSelectionRectangle();

                    for (int x = 0; x < selection.Width; x++)
                    {
                        for (int y = 0; y < selection.Height; y++)
                        {
                            int tileID = TileToID((x % tilesetSelection.Width) + tilesetSelection.X, (y % tilesetSelection.Height) + tilesetSelection.Y);
                            MapData.SetTile(layer, x + selection.X, y + selection.Y, tileID, tileset);
                        }
                    }

                    break;
                case EditorForm.MapTool.FloodFill:
                    FloodFillTile(_lastX, _lastY, layer, ref tilesetSelection);
                    break;
                case EditorForm.MapTool.Event:
                    Genus2D.GameData.MapEvent mapEvent = null;
                    for (int i = 0; i < MapData.MapEventsCount(); i++)
                    {
                        if (MapData.GetMapEvent(i).MapX == _lastX && MapData.GetMapEvent(i).MapY == _lastY)
                        {
                            mapEvent = MapData.GetMapEvent(i);
                            break;
                        }
                    }

                    if (mapEvent == null)
                    {
                        string name = "Map Event " + (MapData.MapEventsCount() + 1).ToString("000");
                        mapEvent = new Genus2D.GameData.MapEvent(name, -1, _lastX, _lastY);
                        Genus2D.GameData.MapInfo.SetMapEventsCount(MapID, MapData.MapEventsCount() + 1);
                        MapData.AddMapEvent(mapEvent);
                    }

                    EditMapEventForm form = new EditMapEventForm(mapEvent);
                    form.ShowDialog(this);
                    break;
                case EditorForm.MapTool.SpawnPoint:
                    int spawnID = Genus2D.GameData.MapInfo.NumberSpawnPoints() + 1;
                    Genus2D.GameData.SpawnPoint spawn = new Genus2D.GameData.SpawnPoint(MapID, _lastX, _lastY, "Spawn " + spawnID.ToString("000"));
                    Genus2D.GameData.MapInfo.AddSpawnPoint(spawn);
                    break;
            }
        }

        private void FloodFillTile(int startX, int startY, int layer, ref Rectangle tilesetSelection)
        {
            int tileset = EditorForm.Instance.GetSelectedMapTileset();
            if (tileset == -1)
                return;

            if (MapData.GetTile(layer, startX, startY).Item1 == TileToID(tilesetSelection.X, tilesetSelection.Y))
                return;

            int floodID = MapData.GetTile(layer, startX, startY).Item1;
            List<Point> pointList = new List<Point>();
            pointList.Add(new Point(startX, startY));

            while (pointList.Count > 0)
            {
                Point prevPoint = pointList[pointList.Count - 1];
                pointList.RemoveAt(pointList.Count - 1);

                int tileX = prevPoint.X - startX;
                while (tileX < 0)
                    tileX += tilesetSelection.Width;
                int tileY = prevPoint.Y - startY;
                while (tileY < 0)
                    tileY += tilesetSelection.Height;

                int tileID = TileToID((tileX % tilesetSelection.Width) + tilesetSelection.X, (tileY % tilesetSelection.Height) + tilesetSelection.Y);
                MapData.SetTile(layer, prevPoint.X, prevPoint.Y, tileID, tileset);

                for (int i = 0; i < 4; i++)
                {

                    Point target = prevPoint;
                    switch (i)
                    {
                        case 0:
                            target.X -= 1;
                            break;
                        case 1:
                            target.Y -= 1;
                            break;
                        case 2:
                            target.X += 1;
                            break;
                        case 3:
                            target.Y += 1;
                            break;
                    }

                    if (target.X < 0 || target.X >= MapData.GetWidth() || target.Y < 0 || target.Y >= MapData.GetHeight())
                        continue;

                    if (MapData.GetTile(layer, target.X, target.Y).Item1 == floodID)
                    {
                            pointList.Add(target);
                    }
                }

            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (MapData != null)
            {
                Rectangle rect = new Rectangle(-HorizontalScroll.Value, -VerticalScroll.Value, MapData.GetWidth() * 32, MapData.GetHeight() * 32);
                e.Graphics.FillRectangle(new SolidBrush(Color.Black), rect);

                for (int layer = 0; layer < 3; layer++)
                {
                    #region tileset rendering
                    for (int x = 0; x < MapData.GetWidth(); x++)
                    {
                        for (int y = 0; y < MapData.GetHeight(); y++)
                        {
                            Tuple<int, int> tileInfo = MapData.GetTile(layer, x, y);
                            int tileID = tileInfo.Item1;
                            int tileset = tileInfo.Item2;

                            if (tileset == -1 || tileset >= _tilesetImages.Count)
                                continue;

                            if (tileID == 0)
                                continue;

                            if (tileID < 8)
                            {
                                string autoTileName = Genus2D.GameData.TilesetData.GetTileset(tileset).GetAutoTile(tileID - 1);
                                if (autoTileName != "")
                                {
                                    int hashCode = 0;
                                    int[] hashValues = new int[] { 1, 2, 4, 8, 16, 32, 64, 128 };
                                    int[,] offsets = new int[,] { { 0, -1 }, { 1, -1 }, { 1, 0 }, { 1, 1 }, { 0, 1 }, { -1, 1 }, { -1, 0 }, { -1, -1 } };
                                    for (int i = 0; i < 8; i++)
                                    {
                                        Tuple<int, int> targetInfo = MapData.GetTile(layer, x + offsets[i, 0], y + offsets[i, 1]);
                                        if (targetInfo == null)
                                            hashCode += hashValues[i];
                                        else if (targetInfo.Item1 == tileInfo.Item1 && targetInfo.Item2 == tileInfo.Item2)
                                            hashCode += hashValues[i];
                                    }

                                    Image autoTile = AutoTileImages[autoTileName];
                                    int destX = (x * 32) - HorizontalScroll.Value;
                                    int destY = (y * 32) - VerticalScroll.Value;
                                    Rectangle[] miniTiles = Genus2D.GameData.TilesetData.Tileset.GetAutoTileSources(hashCode);
                                    e.Graphics.DrawImage(autoTile, new Rectangle(destX, destY, 16, 16), miniTiles[0], GraphicsUnit.Pixel);
                                    e.Graphics.DrawImage(autoTile, new Rectangle(destX + 16, destY, 16, 16), miniTiles[1], GraphicsUnit.Pixel);
                                    e.Graphics.DrawImage(autoTile, new Rectangle(destX, destY + 16, 16, 16), miniTiles[2], GraphicsUnit.Pixel);
                                    e.Graphics.DrawImage(autoTile, new Rectangle(destX + 16, destY + 16, 16, 16), miniTiles[3], GraphicsUnit.Pixel);
                                }
                            }
                            else
                            {
                                int sx = tileID % 8 * 32;
                                int sy = tileID / 8 * 32;
                                Rectangle dest = new Rectangle((x * 32) - HorizontalScroll.Value, (y * 32) - VerticalScroll.Value, 32, 32);
                                Rectangle source = new Rectangle(sx, sy - 32, 32, 32);
                                e.Graphics.DrawImage(_tilesetImages[tileset], dest, source, GraphicsUnit.Pixel);
                            }
                        }
                    }
                    #endregion

                    #region selection rendering
                    EditorForm.MapTool mapTool = EditorForm.Instance.GetMapTool();
                    Rectangle selection = GetSelectionRectangle();
                    if (layer == 2)
                    {
                        if (mapTool == EditorForm.MapTool.Rectangle || mapTool == EditorForm.MapTool.Pencil || mapTool == EditorForm.MapTool.FloodFill)
                        {
                            int tileset = EditorForm.Instance.GetSelectedMapTileset();
                            if (tileset == -1)
                                continue;

                            Rectangle tilesetSelection = EditorForm.Instance.tilesetSelectionPanel.GetSelectionRectangle();
                            if (!(tilesetSelection.X == 0 && tilesetSelection.Y == 0))
                            {

                                for (int x = 0; x < selection.Width; x++)
                                {
                                    for (int y = 0; y < selection.Height; y++)
                                    {
                                        if (tilesetSelection.Y < 1)
                                        {
                                            string autoTileName = Genus2D.GameData.TilesetData.GetTileset(tileset).GetAutoTile(tilesetSelection.X - 1);
                                            if (autoTileName == "")
                                                continue;
                                            Image autoTile = AutoTileImages[autoTileName];
                                            Rectangle dest = new Rectangle(((x + selection.X) * 32) - HorizontalScroll.Value, ((y + selection.Y) * 32) - VerticalScroll.Value, 32, 32);
                                            Rectangle src = new Rectangle(0, 0, 32, 32);
                                            e.Graphics.DrawImage(autoTile, dest, src, GraphicsUnit.Pixel);
                                        }
                                        else
                                        {
                                            Rectangle dest = new Rectangle(((x + selection.X) * 32) - HorizontalScroll.Value, ((y + selection.Y) * 32) - VerticalScroll.Value, 32, 32);
                                            Rectangle src = new Rectangle((tilesetSelection.X + (x % tilesetSelection.Width)) * 32, (tilesetSelection.Y - 1 + (y % tilesetSelection.Height)) * 32, 32, 32);
                                            e.Graphics.DrawImage(_tilesetImages[tileset], dest, src, GraphicsUnit.Pixel);
                                        }
                                    }
                                }
                            }
                        }

                        Rectangle dest2 = new Rectangle((selection.X * 32) - HorizontalScroll.Value, (selection.Y * 32) - VerticalScroll.Value, selection.Width * 32, selection.Height * 32);
                        e.Graphics.DrawRectangle(new Pen(Color.Black, 8), dest2);
                        e.Graphics.DrawRectangle(new Pen(Color.White, 2), dest2);

                    }

                    #endregion
                }

                Font font = new Font("Arial", 22);
                SolidBrush brush = new SolidBrush(Color.Black);

                for (int i = 0; i < MapData.MapEventsCount(); i++)
                {
                    Genus2D.GameData.MapEvent mapEvent = MapData.GetMapEvent(i);
                    int mapX = (mapEvent.MapX * 32) - HorizontalScroll.Value;
                    int mapY = (mapEvent.MapY * 32) - VerticalScroll.Value;

                    e.Graphics.DrawRectangle(new Pen(Color.Black, 4), mapX + 2, mapY + 2, 28, 28);
                    e.Graphics.DrawString("E", font, brush, mapX + 1, mapY);
                }

                for (int i = 0; i < Genus2D.GameData.MapInfo.NumberSpawnPoints(); i++)
                {
                    Genus2D.GameData.SpawnPoint spawn = Genus2D.GameData.MapInfo.GetSpawnPoint(i);
                    if (spawn.MapID == MapID)
                    {
                        int mapX = (spawn.MapX * 32) - HorizontalScroll.Value;
                        int mapY = (spawn.MapY * 32) - VerticalScroll.Value;

                        e.Graphics.DrawRectangle(new Pen(Color.Black, 4), mapX + 2, mapY + 2, 28, 28);
                        e.Graphics.DrawString("S", font, brush, mapX + 1, mapY);
                    }
                }

            }

        }
    }
}

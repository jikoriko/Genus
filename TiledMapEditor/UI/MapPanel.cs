using System;
using System.Drawing;

using OpenTK;
using OpenTK.Graphics;

using Genus2D;
using Genus2D.GUI;
using Genus2D.Graphics;
using Genus2D.Utililities;
using Genus2D.GameData;

using TiledMapEditor.States;
using Genus2D.Core;

namespace TiledMapEditor.UI
{
    public class MapPanel : ScrollPanel
    {

        public enum MapTool
        {
            Pencil,
            Rectangle,
            FloodFill,
            AddMapEvent
        }

        private EditorState _editorState;

        private bool _contentGrabbed;
        private MapTool _mapTool;

        private Vector2 _rectangleStart, _rectangleEnd;

        private MapData _mapData;


        public MapPanel(EditorState state)
            : base(292, 114, StateWindow.Instance.Width - 294, StateWindow.Instance.Height - 114, BarMode.Empty, state)
        {
            _editorState = state;
            SetBackgroundColour(Color4.Black);
            _contentGrabbed = false;
            _mapTool = MapTool.Pencil;
            _mapData = null;
        }

        public void SetMapTool(MapTool tool)
        {
            _mapTool = tool;
        }

        public MapData GetMapData()
        {
            return _mapData;
        }

        public void SetMapData(MapData mapData)
        {
            _mapData = mapData;
        }

        public override void OnMouseDown(OpenTK.Input.MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button == OpenTK.Input.MouseButton.Left)
            {
                if (ContentSelectable())
                {
                    _contentGrabbed = true;
                    if (_mapTool != MapTool.Rectangle)
                        SetMapSelection();
                    else
                    {
                        SetRectangleStart();
                    }
                }
            }
        }

        public override void OnMouseUp(OpenTK.Input.MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);
            if (e.Button == OpenTK.Input.MouseButton.Left)
            {
                if (_contentGrabbed)
                {
                    if (_mapTool == MapTool.Rectangle)
                        SetMapSelection();
                    else if (_mapTool == MapTool.AddMapEvent)
                        AddMapEventSelection();


                    _contentGrabbed = false;
                }
            }
            else if (e.Button == OpenTK.Input.MouseButton.Right)
            {
                if (_mapTool == MapTool.AddMapEvent)
                    RemoveMapEventSelection();
            }
        }

        public override void OnMouseMove(OpenTK.Input.MouseMoveEventArgs e)
        {
            base.OnMouseMove(e);
            if (_contentGrabbed)
            {
                if (_mapTool == MapTool.Pencil)
                    SetMapSelection();
                else if (_mapTool == MapTool.Rectangle)
                {
                    SetRectangleEnd();
                }
            }
        }

        private void SetRectangleStart()
        {
            Vector2 relativeMouse = StateWindow.Instance.GetMousePosition() - GetRelativeContentPosition();
            int relativeX = (int)relativeMouse.X / 32;
            int relativeY = (int)relativeMouse.Y / 32;
            _rectangleStart = new Vector2(relativeX, relativeY);
            _rectangleEnd = _rectangleStart;
        }

        private void SetRectangleEnd()
        {
            Vector2 relativeMouse = StateWindow.Instance.GetMousePosition() - GetRelativeContentPosition();
            int relativeX = (int)relativeMouse.X / 32;
            int relativeY = (int)relativeMouse.Y / 32;
            _rectangleEnd = new Vector2(relativeX, relativeY);
        }

        public Vector2 GetRectangleStart()
        {
            int minX = (int)Math.Min(_rectangleStart.X, _rectangleEnd.X);
            int minY = (int)Math.Min(_rectangleStart.Y, _rectangleEnd.Y);
            return new Vector2(minX, minY);
        }

        public Vector2 GetRectangleEnd()
        {
            int maxX = (int)Math.Max(_rectangleStart.X, _rectangleEnd.X);
            int maxY = (int)Math.Max(_rectangleStart.Y, _rectangleEnd.Y);
            return new Vector2(maxX, maxY);
        }

        private void SetMapSelection()
        {
            if (_mapData == null)
                return;

            switch (_mapTool)
            {
                case MapTool.Pencil:
                        SetPencilSelection();
                    break;
                case MapTool.Rectangle:
                        SetRectangleSelection();
                    break;
                case MapTool.FloodFill:
                        SetFloodFillSelection();
                    break;
            }
        }

        private void SetPencilSelection()
        {
            Vector2 relativeMouse = StateWindow.Instance.GetMousePosition() - GetRelativeContentPosition();
            int layer = _editorState.GetControlPanel().GetLayer();
            int relativeX = (int)relativeMouse.X / 32;
            int relativeY = (int)relativeMouse.Y / 32;

            Vector2 tilesetStartSelection = _editorState.GetTilesetPanel().GetSelectedStartTile();
            Vector2 tilesetEndSelection = _editorState.GetTilesetPanel().GetSelectedEndTile();

            int xTiles = (int)(tilesetEndSelection.X - tilesetStartSelection.X) + 1;
            int yTiles = (int)(tilesetEndSelection.Y - tilesetStartSelection.Y) + 1;

            for (int x = 0; x < xTiles; x++)
            {
                for (int y = 0; y < yTiles; y++)
                {
                    int mapX = relativeX + x;
                    int mapY = relativeY + y;
                    int tilesetX = (int)tilesetStartSelection.X + x;
                    int tilesetY = (int)tilesetStartSelection.Y + y;
                    int tileID = tilesetX + (tilesetY * 8);

                    _mapData.SetTileID(layer, mapX, mapY, tileID);
                }
            }
        }

        private void SetRectangleSelection()
        {
            int layer = _editorState.GetControlPanel().GetLayer();

            Vector2 tilesetStartSelection = _editorState.GetTilesetPanel().GetSelectedStartTile();
            Vector2 tilesetEndSelection = _editorState.GetTilesetPanel().GetSelectedEndTile();
            int xTilesetTiles = (int)(tilesetEndSelection.X - tilesetStartSelection.X) + 1;
            int yTilesetTiles = (int)(tilesetEndSelection.Y - tilesetStartSelection.Y) + 1;

            Vector2 rectangleStart = GetRectangleStart();
            Vector2 rectangleEnd = GetRectangleEnd();
            int xTiles = (int)(rectangleEnd.X - rectangleStart.X) + 1;
            int yTiles = (int)(rectangleEnd.Y - rectangleStart.Y) + 1;

            for (int x = 0; x < xTiles; x++)
            {
                for (int y = 0; y < yTiles; y++)
                {
                    int mapX = (int)rectangleStart.X + x;
                    int mapY = (int)rectangleStart.Y + y;
                    int tilesetX = (int)tilesetStartSelection.X + (x % xTilesetTiles);
                    int tilesetY = (int)tilesetStartSelection.Y + (y % yTilesetTiles);
                    int tileID = tilesetX + (tilesetY * 8);

                    _mapData.SetTileID(layer, mapX, mapY, tileID);
                }
            }

        }

        private void SetFloodFillSelection()
        {
            //need to program flood fill fuctionality
        }

        private void AddMapEventSelection()
        {
            if (_mapData == null)
                return;

            if (MapEventData.MapEventsDataCount() == 0)
                return;

            Vector2 relativeMouse = StateWindow.Instance.GetMousePosition() - GetRelativeContentPosition();
            int relativeX = (int)relativeMouse.X / 32;
            int relativeY = (int)relativeMouse.Y / 32;

            if (relativeX > _mapData.GetWidth() || relativeY > _mapData.GetHeight())
                return;

            MapEvent mapEvent = null;

            for (int i = 0; i < _mapData.MapEventsCount(); i++)
            {
                if (_mapData.GetMapEvent(i).MapX == relativeX && _mapData.GetMapEvent(i).MapY == relativeY)
                {
                    mapEvent = _mapData.GetMapEvent(i);
                }
            }

            if (mapEvent == null)
            {
                mapEvent = new MapEvent(0, relativeX, relativeY);
                _mapData.AddMapEvent(mapEvent);
            }

            EditMapEventPanel panel = new EditMapEventPanel(_state, mapEvent);
            _state.AddControl(panel);
        }

        private void RemoveMapEventSelection()
        {
            if (_mapData == null)
                return;

            Vector2 relativeMouse = StateWindow.Instance.GetMousePosition() - GetRelativeContentPosition();
            int relativeX = (int)relativeMouse.X / 32;
            int relativeY = (int)relativeMouse.Y / 32;

            for (int i = 0; i < _mapData.MapEventsCount(); i++)
            {
                if (_mapData.GetMapEvent(i).MapX == relativeX && _mapData.GetMapEvent(i).MapY == relativeY)
                {
                    _mapData.RemoveMapEvent(i);
                    break;
                }
            }
        }

        protected override void RenderContent()
        {
            base.RenderContent();

            if (_mapData != null)
            {
                int startX = ((int)-GetScrolledAmount().X / 32);
                int startY = ((int)-GetScrolledAmount().Y / 32);
                int endX = startX + (GetContentWidth() / 32) + 2;
                int endY = startY + (GetContentHeight() / 32) + 2;

                Texture tileset = Assets.GetTexture(TilesetData.GetTileset(_mapData.GetTilesetID()).TexturePath);
                Vector3 pos = new Vector3();
                Vector3 scale = new Vector3(_mapData.GetWidth() * 32, _mapData.GetHeight() * 32, 1);
                Color4 colour = Color4.DarkGray;

                Renderer.FillShape(ShapeFactory.Rectangle, ref pos, ref scale, ref colour);

                scale.X = scale.Y = 32;
                System.Drawing.Rectangle source = new System.Drawing.Rectangle(0, 0, 32, 32);
                colour = Color4.White;

                for (int layer = 0; layer < 3; layer++)
                {
                    for (int x = startX; x < endX; x++)
                    {
                        if (x < 0)
                            continue;
                        else if (x >= _mapData.GetWidth())
                            break;
                        for (int y = 0; y < endY; y++)
                        {
                            if (y < 0)
                                continue;
                            else if (y >= _mapData.GetHeight())
                                break;

                            int id = _mapData.GetTileID(layer, x, y);

                            pos.X = x * 32;
                            pos.Y = y * 32;
                            source.X = id % 8 * 32;
                            source.Y = id / 8 * 32;
                            Renderer.FillTexture(tileset, ShapeFactory.Rectangle, ref pos, ref scale, ref source, ref colour);
                        }
                    }
                }

                for (int i = 0; i < _mapData.MapEventsCount(); i++)
                {
                    MapEvent mapEvent = _mapData.GetMapEvent(i);
                    Texture eventTexture = Assets.GetTexture("GUI_Textures/EventIcon.png");
                    pos.X = mapEvent.MapX * 32;
                    pos.Y = mapEvent.MapY * 32;
                    colour = Color4.White;
                    colour.A = 0.65f;
                    Renderer.FillTexture(eventTexture, ShapeFactory.Rectangle, ref pos, ref colour);
                }

                RenderToolPlacement();

            }
        }

        private void RenderToolPlacement()
        {
            switch (_mapTool)
            {
                case MapTool.Pencil:
                    RenderPencilPlacement();
                    break;
                case MapTool.Rectangle:
                    RenderRectanglePlacement();
                    break;
                case MapTool.FloodFill:
                    RenderFloodFillPlacement();
                    break;
            }
        }

        private void RenderPencilPlacement()
        {
            if (MouseInsideContent())
            {
                Texture tileset = Assets.GetTexture(TilesetData.GetTileset(_mapData.GetTilesetID()).TexturePath);

                Vector2 relativeMouse = StateWindow.Instance.GetMousePosition() - GetWorldContentPosition().Xy;
                int layer = _editorState.GetControlPanel().GetLayer();
                int relativeX = (int)relativeMouse.X / 32;
                int relativeY = (int)relativeMouse.Y / 32;

                Vector2 tilesetStartSelection = _editorState.GetTilesetPanel().GetSelectedStartTile();
                Vector2 tilesetEndSelection = _editorState.GetTilesetPanel().GetSelectedEndTile();

                int xTiles = (int)(tilesetEndSelection.X - tilesetStartSelection.X) + 1;
                int yTiles = (int)(tilesetEndSelection.Y - tilesetStartSelection.Y) + 1;

                Vector3 pos = new Vector3(relativeX * 32, relativeY * 32, 0);
                pos.Xy -= GetScrolledAmount();
                Vector3 scale = new Vector3(32 * xTiles, 32 * yTiles, 1);
                Rectangle source = new Rectangle((int)tilesetStartSelection.X * 32, (int)tilesetStartSelection.Y * 32, 32 * xTiles, 32 * yTiles);
                Color4 colour = Color4.White;

                Renderer.FillTexture(tileset, ShapeFactory.Rectangle, ref pos, ref scale, ref source, ref colour);

                colour = Color.Black;
                Renderer.DrawShape(ShapeFactory.Rectangle, ref pos, ref scale, 2f, ref colour);
            }
        }

        private void RenderRectanglePlacement()
        {
            if (_contentGrabbed || MouseInsideBody())
            {
                Texture tileset = Assets.GetTexture(TilesetData.GetTileset(_mapData.GetTilesetID()).TexturePath);

                Vector2 tilesetStartSelection = _editorState.GetTilesetPanel().GetSelectedStartTile();
                Vector2 tilesetEndSelection = _editorState.GetTilesetPanel().GetSelectedEndTile();
                int xTilesetTiles = (int)(tilesetEndSelection.X - tilesetStartSelection.X) + 1;
                int yTilesetTiles = (int)(tilesetEndSelection.Y - tilesetStartSelection.Y) + 1;

                Vector2 rectangleStart = GetRectangleStart();
                Vector2 rectangleEnd = GetRectangleEnd();
                int xTiles = (int)(rectangleEnd.X - rectangleStart.X) + 1;
                int yTiles = (int)(rectangleEnd.Y - rectangleStart.Y) + 1;

                Color4 colour = Color4.White;
                Vector3 pos = new Vector3();
                Vector3 scale = new Vector3(32, 32, 1);
                Rectangle source = new Rectangle(0, 0, 32, 32);

                if (_contentGrabbed)
                {
                    for (int x = 0; x < xTiles; x++)
                    {
                        for (int y = 0; y < yTiles; y++)
                        {
                            source.X = ((int)tilesetStartSelection.X + (x % xTilesetTiles)) * 32;
                            source.Y = ((int)tilesetStartSelection.Y + (y % yTilesetTiles)) * 32;

                            pos.X = (rectangleStart.X + x) * 32;
                            pos.Y = (rectangleStart.Y + y) * 32;
                            Renderer.FillTexture(tileset, ShapeFactory.Rectangle, ref pos, ref scale, ref source, ref colour);
                        }
                    }

                    //draw border


                }
                else
                {
                    Vector2 relativeMouse = StateWindow.Instance.GetMousePosition() - GetWorldContentPosition().Xy;
                    int relativeX = (int)relativeMouse.X / 32;
                    int relativeY = (int)relativeMouse.Y / 32;

                    source.X = (int)tilesetStartSelection.X * 32;
                    source.Y = (int)tilesetStartSelection.Y * 32;
                    pos.X = relativeX * 32;
                    pos.Y = relativeY * 32;
                    pos.Xy -= GetScrolledAmount();

                    Renderer.FillTexture(tileset, ShapeFactory.Rectangle, ref pos, ref scale, ref source, ref colour);

                    colour = Color.Black;
                    Renderer.DrawShape(ShapeFactory.Rectangle, ref pos, ref scale, 2f, ref colour);

                }
            }
        }

        private void RenderFloodFillPlacement()
        {

        }

    }
}

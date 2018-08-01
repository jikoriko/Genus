using System;

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
    public class TilesetPanel : ScrollPanel
    {
        private EditorState _editorState;

        private Texture _tileset;

        private bool _tilesetGrabbed;
        private int _selectedStartX, _selectedStartY;
        private int _selectedEndX, _selectedEndY;

        public TilesetPanel(EditorState state)
            : base(0, 114, 294, StateWindow.Instance.Height - 114, BarMode.Empty, state)
        {
            _editorState = state;
            SetBackgroundColour(Color4.White);
            this.DisableHorizontalScroll();

            _tileset = null;

            _tilesetGrabbed = false;
            _selectedStartX = 0;
            _selectedStartY = 0;
            _selectedEndX = 0;
            _selectedEndY = 0;
        }

        public Vector2 GetSelectedStartTile()
        {
            int minX = Math.Min(_selectedStartX, _selectedEndX);
            int minY = Math.Min(_selectedStartY, _selectedEndY);
            return new Vector2(minX, minY);
        }

        public Vector2 GetSelectedEndTile()
        {
            int maxX = Math.Max(_selectedStartX, _selectedEndX);
            int maxY = Math.Max(_selectedStartY, _selectedEndY);
            return new Vector2(maxX, maxY);
        }

        public override void OnMouseDown(OpenTK.Input.MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button == OpenTK.Input.MouseButton.Left)
            {
                if (ContentSelectable())
                {
                    Vector2 relativeMouse = StateWindow.Instance.GetMousePosition() - GetRelativeContentPosition();
                    int relativeX = (int)relativeMouse.X / 32;
                    int relativeY = (int)relativeMouse.Y / 32;

                    _selectedStartX = relativeX;
                    _selectedEndX = relativeX;
                    _selectedStartY = relativeY;
                    _selectedEndY = relativeY;

                    _tilesetGrabbed = true;
                }
            }
        }

        public override void OnMouseUp(OpenTK.Input.MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);
            if (e.Button == OpenTK.Input.MouseButton.Left)
            {
                _tilesetGrabbed = false;
            }
        }

        public override void OnMouseMove(OpenTK.Input.MouseMoveEventArgs e)
        {
            base.OnMouseMove(e);
            if (_tilesetGrabbed)
            {
                if (MouseInsideContent())
                {
                    Vector2 relativeMouse = StateWindow.Instance.GetMousePosition() - GetRelativeContentPosition();
                    int relativeX = (int)relativeMouse.X / 32;
                    int relativeY = (int)relativeMouse.Y / 32;

                    _selectedEndX = relativeX;
                    _selectedEndY = relativeY;
                }
            }
        }

        public void SetTileset(int tilesetID)
        {
            TilesetData.Tileset tileset = TilesetData.GetTileset(tilesetID);
            if (tileset != null)
            {
                _tileset = Assets.GetTexture(tileset.TexturePath);
                if (_tileset != null)
                {
                    SetScrollDimensions(_tileset.GetWidth(), _tileset.GetHeight());
                }
            }
        }

        public Texture GetTileset()
        {
            return _tileset;
        }

        protected override void RenderContent()
        {
            base.RenderContent();

            if (_tileset != null)
            {
                Vector3 pos = Vector3.Zero;
                Color4 colour = Color4.White;
                Renderer.FillTexture(_tileset, ShapeFactory.Rectangle, ref pos, ref colour);

                Vector2 selectionStart = GetSelectedStartTile();
                Vector2 selectionEnd = GetSelectedEndTile();
                Vector3 selectionPos = new Vector3(selectionStart.X * 32, selectionStart.Y * 32, 0);
                int selectionWidth = (int)(selectionEnd.X - selectionStart.X + 1) * 32;
                int selectionHeight = (int)(selectionEnd.Y - selectionStart.Y + 1) * 32;
                Vector3 selectionScale = new Vector3(selectionWidth, selectionHeight, 1);
                colour = Color4.Black;

                Renderer.DrawShape(ShapeFactory.Rectangle, ref selectionPos, ref selectionScale, 4f, ref colour);
                colour = Color4.White;
                Renderer.DrawShape(ShapeFactory.Rectangle, ref selectionPos, ref selectionScale, 1f, ref colour);

                /*
                pos.X = _selectedTile % 8 * 32;
                pos.Y = _selectedTile / 8 * 32;
                Vector3 scale = new Vector3(32, 32, 1);
                colour = Color4.Black;
                Renderer.DrawShape(ShapeFactory.Rectangle, ref pos, ref scale, 4f, ref colour);
                colour = Color4.White;
                Renderer.DrawShape(ShapeFactory.Rectangle, ref pos, ref scale, 1f, ref colour);
                */
            }
        }

    }
}

using System;

using OpenTK;
using OpenTK.Graphics;

using Genus2D;
using Genus2D.GUI;
using Genus2D.Graphics;

using TiledMapEditor.States;
using Genus2D.GameData;
using Genus2D.Utililities;
using Genus2D.Core;

namespace TiledMapEditor.UI
{
    public class TilesetPropertiesPanel : ScrollPanel
    {
        private EditorState _editorState;
        private TilesetEditorPanel _tilesetEditor;

        public enum EditPropertyMode
        {
            Passabilities,
            Priorities
        }

        private EditPropertyMode _editPropertyMode;

        public TilesetPropertiesPanel(TilesetEditorPanel tilesetEditor, EditorState state)
            : base(400, 100, 294, 400, BarMode.Empty, state)
        {
            _editorState = state;
            _tilesetEditor = tilesetEditor;

            SetBackgroundColour(Color4.White);
            this.DisableHorizontalScroll();

            SetEditPropertyMode(EditPropertyMode.Passabilities);
        }

        public EditPropertyMode GetEditPropertyMode()
        {
            return _editPropertyMode;
        }

        public void SetEditPropertyMode(EditPropertyMode mode)
        {
            _editPropertyMode = mode;
        }

        public override void OnMouseDown(OpenTK.Input.MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button == OpenTK.Input.MouseButton.Left)
            {
                if (ContentSelectable())
                {
                    TilesetData.Tileset tileset = _tilesetEditor.GetSelectedTileset();

                    if (tileset != null)
                    {
                        Vector2 relativeMouse = StateWindow.Instance.GetMousePosition() - GetWorldContentPosition().Xy;
                        relativeMouse -= this.GetScrolledAmount();
                        int relativeX = (int)relativeMouse.X / 32;
                        int relativeY = (int)relativeMouse.Y / 32;

                        if (_editPropertyMode == EditPropertyMode.Passabilities)
                        {
                            bool passable = tileset.GetPassable(relativeX, relativeY);
                            tileset.SetPassable(relativeX, relativeY, passable ? false : true);
                        }
                        else if (_editPropertyMode == EditPropertyMode.Priorities)
                        {
                            int priority = tileset.GetTilePriority(relativeX, relativeY);
                            priority = priority == 5 ? 0 : priority + 1;
                            tileset.SetPriority(relativeX, relativeY, priority);
                        }
                    }

                }
            }
        }

        public override void OnMouseUp(OpenTK.Input.MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);
            if (e.Button == OpenTK.Input.MouseButton.Left)
            {

            }
        }

        public override void OnMouseMove(OpenTK.Input.MouseMoveEventArgs e)
        {
            base.OnMouseMove(e);
        }

        public override void Update(FrameEventArgs e)
        {
            base.Update(e);
            
            TilesetData.Tileset tileset = _tilesetEditor.GetSelectedTileset();
            if (tileset != null)
            {
                Texture tilesetTexture = Assets.GetTexture(tileset.TexturePath);
                if (tilesetTexture != null)
                {
                    SetScrollableHeight(tilesetTexture.GetHeight());
                }
            }

        }

        protected override void RenderContent()
        {
            base.RenderContent();

            TilesetData.Tileset tileset = _tilesetEditor.GetSelectedTileset();
            if (tileset != null)
            {
                Texture tilesetTexture = Assets.GetTexture(tileset.TexturePath);

                if (tilesetTexture != null)
                {
                    Vector3 pos = Vector3.Zero;
                    Color4 colour = Color4.White;
                    Renderer.FillTexture(tilesetTexture, ShapeFactory.Rectangle, ref pos, ref colour);

                    for (int x = 0; x < tileset.Pasabilities.GetLength(0); x++)
                    {
                        for (int y = 0; y < tileset.Pasabilities.GetLength(1); y++)
                        {
                            pos = new Vector3(x * 32, y * 32, 0);
                            colour = Color4.Black;

                            String text = "";

                            if (_editPropertyMode == EditPropertyMode.Passabilities)
                            {
                                if (tileset.GetPassable(x, y))
                                    text = "O";
                                else
                                    text = "X";
                            }
                            else if (_editPropertyMode == EditPropertyMode.Priorities)
                            {
                                text = tileset.GetTilePriority(x, y).ToString();
                            }

                            pos.X += (32 - Renderer.GetFont().GetTextWidth(text)) / 2;
                            pos.Y += (32 - Renderer.GetFont().GetTextHeight(text)) / 2;
                            Renderer.PrintText(text, ref pos, ref colour);
                        }
                    }



                }
            }
        }
    }
}

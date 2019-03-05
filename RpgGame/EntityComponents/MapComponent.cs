using System;

using OpenTK;
using OpenTK.Graphics;

using Genus2D.Core;
using Genus2D.Entities;
using Genus2D.GameData;
using Genus2D.Graphics;
using Genus2D.Utililities;
using System.Drawing;
using Genus2D;
using System.Collections.Generic;
using RpgGame.States;

namespace RpgGame.EntityComponents
{
    public class MapComponent : EntityComponent
    {

        public static MapComponent Instance { get; private set; }

        public int MapID;
        private MapData _mapData;
        private List<Entity> _mapEvents;
        private bool _mapDataChanged;

        private float _autoTileAnimationTimer;

        public MapComponent(Entity entity)
            : base(entity)
        {
            Instance = this;
            MapID = -1;
            _mapData = null;
            _mapDataChanged = false;
            _mapEvents = new List<Entity>();

            _autoTileAnimationTimer = 0;
        }

        public MapData GetMapData()
        {
            return _mapData;
        }

        public void SetMapData(int id, MapData mapData)
        {
            MapID = id;
            _mapData = mapData;
            _mapDataChanged = true;
            for (int i = 0; i < _mapEvents.Count; i++)
            {
                _mapEvents[i].Destroy();
            }
            _mapEvents.Clear();
        }

        public void ChangeMapEventSprite(int mapEvent, int spriteID)
        {
            if (mapEvent >= 0 && mapEvent < _mapEvents.Count)
            {
                ((MapEventComponent)_mapEvents[mapEvent].FindComponent<MapEventComponent>()).SetSpriteID(spriteID);
                _mapData.GetMapEvent(mapEvent).SpriteID = spriteID;
            }
        }

        public void ChangeMapEventRenderPriority(int mapEvent, RenderPriority priority)
        {
            if (mapEvent >= 0 && mapEvent < _mapEvents.Count)
            {
                _mapData.GetMapEvent(mapEvent).Priority = priority;
            }
        }

        public void ChangeMapEventEnabled(int mapEvent, bool enabled)
        {
            if (mapEvent >= 0 && mapEvent < _mapEvents.Count)
            {
                _mapData.GetMapEvent(mapEvent).Enabled = enabled;
            }
        }

        public override void Update(FrameEventArgs e)
        {
            base.Update(e);

            if (_mapDataChanged)
            {
                if (_mapData != null)
                {
                    for (int i = 0; i < _mapData.MapEventsCount(); i++)
                    {
                        Entity entity = Entity.CreateInstance(GameState.Instance.EntityManager);
                        entity.GetTransform().Parent = this.Parent.GetTransform();
                        MapEventComponent component = new MapEventComponent(entity, _mapData.GetMapEvent(i));
                        _mapEvents.Add(entity);
                    }
                }
                _mapDataChanged = false;
            }
        }

        public override void LateUpdate(OpenTK.FrameEventArgs e)
        {
            base.Update(e);

            StateWindow.Instance.Title = "FPS: " + StateWindow.Instance.GetFPS();
        }

        public override void Render(OpenTK.FrameEventArgs e)
        {
            base.Render(e);
            if (_mapDataChanged) return;
            if (_mapData != null)
            {
                Renderer.PushWorldMatrix();
                Renderer.TranslateWorld((int)Transform.Position.X, (int)Transform.Position.Y, 0);

                Vector3 pos = Vector3.Zero;
                Vector3 scale = new Vector3(32, 32, 1);
                Vector3 autoTileScale = new Vector3(16, 16, 1);
                Rectangle source = new Rectangle(0, 0, 32, 32);
                Color4 colour = Color4.White;

                int startX = ((int)-Transform.Position.X / 32);
                int startY = ((int)-Transform.Position.Y / 32);
                int endX = startX + (int)(Renderer.GetResoultion().X / 32) + 2;
                int endY = startY + (int)(Renderer.GetResoultion().Y / 32) + 2;

                for (int layer = 0; layer < MapData.NUM_LAYERS; layer++)
                {
                    for (int x = startX; x < endX; x++)
                    {
                        for (int y = startY; y < endY; y++)
                        {
                            Tuple<int, int> tileInfo = _mapData.GetTile(layer, x, y);
                            if (tileInfo == null)
                                continue;
                            int tileID = tileInfo.Item1;
                            if (tileID == 0)
                                continue;

                            int tilesetID = tileInfo.Item2;
                            TilesetData.Tileset tileset = TilesetData.GetTileset(tilesetID);
                            Texture tilesetTexture = Assets.GetTexture("Tilesets/" + tileset.ImagePath);

                            pos.X = x * 32;
                            pos.Y = y * 32;
                            int tilePriority = tileset.GetTilePriority(tileID);
                            pos.Z = -((y + layer) * (tilePriority * 32));

                            if (tileset.GetReflectionFlag(tileID))
                            {
                                Renderer.PushStencilDepth(OpenTK.Graphics.OpenGL.StencilOp.Incr, OpenTK.Graphics.OpenGL.StencilFunction.Equal);
                            }

                            if (tileID < 8)
                            {
                                string autoTileName = tileset.GetAutoTile(tileID - 1);
                                if (autoTileName != "")
                                {
                                    int hashCode = 0;
                                    int[] hashValues = new int[] { 1, 2, 4, 8, 16, 32, 64, 128 };
                                    int[,] offsets = new int[,] { { 0, -1 }, { 1, -1 }, { 1, 0 }, { 1, 1 }, { 0, 1 }, { -1, 1 }, { -1, 0 }, { -1, -1 } };
                                    for (int i = 0; i < 8; i++)
                                    {
                                        Tuple<int, int> targetInfo = _mapData.GetTile(layer, x + offsets[i, 0], y + offsets[i, 1]);
                                        if (targetInfo == null)
                                            hashCode += hashValues[i];
                                        else if (targetInfo.Item1 == tileInfo.Item1 && targetInfo.Item2 == tileInfo.Item2)
                                            hashCode += hashValues[i];
                                    }

                                    Texture autoTile = Assets.GetTexture("AutoTiles/" + autoTileName);
                                    Rectangle[] miniTiles = TilesetData.Tileset.GetAutoTileSources(hashCode);
                                    int tileFrames = autoTile.GetWidth() / 96;
                                    _autoTileAnimationTimer += (float)e.Time * (tileset.GetAutoTileTimer(tileID - 1) / 200f);
                                    while (_autoTileAnimationTimer > 1f)
                                        _autoTileAnimationTimer -= 1f;
                                    int frame = (int)(_autoTileAnimationTimer * (tileFrames - 1));
                                    for (int i = 0; i < miniTiles.Length; i++)
                                    {
                                        miniTiles[i].X += 96 * frame;
                                    }

                                    Renderer.FillTexture(autoTile, ShapeFactory.Rectangle, ref pos, ref autoTileScale, ref miniTiles[0], ref colour);
                                    pos.X += 16;
                                    Renderer.FillTexture(autoTile, ShapeFactory.Rectangle, ref pos, ref autoTileScale, ref miniTiles[1], ref colour);
                                    pos.X -= 16;
                                    pos.Y += 16;
                                    Renderer.FillTexture(autoTile, ShapeFactory.Rectangle, ref pos, ref autoTileScale, ref miniTiles[2], ref colour);
                                    pos.X += 16;
                                    Renderer.FillTexture(autoTile, ShapeFactory.Rectangle, ref pos, ref autoTileScale, ref miniTiles[3], ref colour);
                                }
                            }
                            else
                            {
                                source.X = tileID % 8 * 32;
                                source.Y = (tileID / 8 * 32) - 32;
                                Renderer.FillTexture(tilesetTexture, ShapeFactory.Rectangle, ref pos, ref scale, ref source, ref colour);
                            }

                            if (tileset.GetReflectionFlag(tileID))
                            {
                                Renderer.PopStencilDepth();
                            }
                        }
                    }
                }

                Renderer.PopWorldMatrix();
            }
        }
    }
}

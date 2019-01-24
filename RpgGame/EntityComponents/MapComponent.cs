﻿using System;

using OpenTK;
using OpenTK.Graphics;

using Genus2D.Core;
using Genus2D.Entities;
using Genus2D.GameData;
using Genus2D.Graphics;
using Genus2D.Utililities;
using System.Drawing;
using Genus2D;

namespace RpgGame.EntityComponents
{
    public class MapComponent : EntityComponent
    {

        public static MapComponent Instance { get; private set; }

        private MapData _mapData;

        public MapComponent(Entity entity)
            : base(entity)
        {
            Instance = this;
            _mapData = null;
        }

        public MapData getMapData()
        {
            return _mapData;
        }

        public void SetMapData(MapData mapData)
        {
            _mapData = mapData;
        }

        public override void LateUpdate(OpenTK.FrameEventArgs e)
        {
            base.Update(e);

            StateWindow.Instance.Title = "FPS: " + StateWindow.Instance.GetFPS();
        }

        public override void Render(OpenTK.FrameEventArgs e)
        {
            base.Render(e);
            if (_mapData != null)
            {
                Renderer.PushWorldMatrix();
                Renderer.TranslateWorld((int)Transform.Position.X, (int)Transform.Position.Y, 0);

                Vector3 pos = Vector3.Zero;
                Vector3 scale = new Vector3(32, 32, 1);
                Rectangle source = new Rectangle(0, 0, 32, 32);
                Color4 colour = Color4.White;
                String textureName = TilesetData.GetTileset(_mapData.GetTilesetID()).ImagePath;
                Texture tileset = Assets.GetTexture("Tilesets/" + textureName);

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
                            int tileID = _mapData.GetTileID(layer, x, y);

                            if (tileID != -1)
                            {
                                pos.X = x * 32;
                                pos.Y = y * 32;
                                int tilePriority = TilesetData.GetTileset(_mapData.GetTilesetID()).GetTilePriority(tileID);
                                pos.Z = -(y + tilePriority) * (layer * 32);
                                source.X = tileID % 8 * 32;
                                source.Y = tileID / 8 * 32;
                                Renderer.FillTexture(tileset, ShapeFactory.Rectangle, ref pos, ref scale, ref source, ref colour);
                            }
                        }
                    }
                }

                for (int i = 0; i < _mapData.MapEventsCount(); i++)
                {
                    MapEvent mapEvent = _mapData.GetMapEvent(i);
                    int spriteID = MapEventData.GetMapEventData(mapEvent.EventID).GetSpriteID();
                    Texture eventTexture = null;
                    colour = Color4.White;

                    if (spriteID != -1)
                    {
                        SpriteData sprite = SpriteData.GetSpriteData(spriteID);
                        eventTexture = Assets.GetTexture("Sprites/" + sprite.ImagePath);

                        scale.X = eventTexture.GetWidth() / 4;
                        scale.Y = eventTexture.GetHeight() / 4;
                        source.X = 0;
                        source.Y = 0;
                        source.Width = (int)scale.X;
                        source.Height = (int)scale.Y;

                        pos.X = (mapEvent.MapX * 32) + 16 - (sprite.VerticalAnchorPoint.X);
                        pos.Y = (mapEvent.MapY * 32) + (scale.Y - sprite.VerticalAnchorPoint.Y) - (scale.Y / 2);
                        pos.Z = -(mapEvent.MapY * 32);

                        colour.A = 1f;
                    }
                    else
                    {
                        eventTexture = Assets.GetTexture("GUI_Textures/EventIcon.png");

                        pos.X = mapEvent.MapX * 32;
                        pos.Y = mapEvent.MapY * 32;
                        pos.Z = -(mapEvent.MapY * 32);
                        scale.X = 32;
                        scale.Y = 32;
                        source.X = 0;
                        source.Y = 0;
                        source.Width = 32;
                        source.Height = 32;

                        colour.A = 0.65f;
                    }
                    
                    Renderer.FillTexture(eventTexture, ShapeFactory.Rectangle, ref pos, ref scale, ref source, ref colour);
                }

                Renderer.PopWorldMatrix();
            }
        }
    }
}

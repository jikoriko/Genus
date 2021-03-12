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
using System.Linq;
using Genus2D.Networking;

namespace RpgGame.EntityComponents
{
    public class MapComponent : EntityComponent
    {

        private class MapTileMeshData
        {
            private bool _isReflections;
            private bool _isAutoTiles;
            private TilesetData.Tileset _tileset;
            private Texture _texture;

            private List<float> _vertices;
            private List<float> _textureCoords;
            private List<int> _indices;

            private Shape _shape;

            public bool Ready;
            public int NumFrames;
            public int AutoTileID;

            public MapTileMeshData(bool isReflections, bool isAutoTiles, TilesetData.Tileset tileset, Texture texture)
            {
                _isReflections = isReflections;
                _isAutoTiles = isAutoTiles;
                _tileset = tileset;
                _texture = texture;

                _vertices = new List<float>();
                _textureCoords = new List<float>();
                _indices = new List<int>();
                _shape = null;

                Ready = false;
                NumFrames = 1;
                AutoTileID = -1;
            }

            public void AddRegularMapTile(int tileID, int x, int y, int layer)
            {
                if (_shape != null)
                {
                    _shape.Destroy();
                    _shape = null;
                    Ready = false;
                }

                float rX = x;
                float rY = y;
                int tilePriority = _tileset.GetTilePriority(tileID);
                float rZ = -((y + tilePriority) * 2) - (layer / 2f);

                Rectangle source = new Rectangle(0, 0, 32, 32);
                source.X = tileID % 8 * 32;
                source.Y = (tileID / 8 * 32) - 32;

                float tX = (float)(source.X + 0.5f) / _texture.GetWidth();
                float tY = (float)(source.Y + 0.5f) / _texture.GetHeight();
                float tW = (float)(source.Width - 1) / _texture.GetWidth();
                float tH = (float)(source.Height - 1) / _texture.GetHeight();

                float[] verts = new float[] {
                    rX, rY, rZ,
                    rX + 1f, rY, rZ,
                    rX, rY + 1f, rZ,
                    rX + 1f, rY + 1f, rZ
                };

                float[] texCoords = new float[]
                {
                    tX, tY,
                    tX + tW, tY,
                    tX, tY + tH,
                    tX + tW, tY + tH
                };

                int offset = _indices.Count;

                int[] indices = new int[]
                {
                    offset, offset + 1, offset + 3, offset + 2
                };

                _vertices.AddRange(verts);
                _textureCoords.AddRange(texCoords);
                _indices.AddRange(indices);
            }

            public void AddAutoMapTile(int tileID, int x, int y, int layer, int hashcode)
            {
                int tilePriority = _tileset.GetTilePriority(tileID);
                float rZ = -((y + (layer / 3f) + tilePriority) * 2);

                Rectangle[] miniTiles = TilesetData.Tileset.GetAutoTileSources(hashcode);

                for (int i = 0; i < miniTiles.Length; i++)
                {
                    float tX = (float)(miniTiles[i].X + 0.5f) / _texture.GetWidth();
                    float tY = (float)(miniTiles[i].Y + 0.5f) / _texture.GetHeight();
                    float tW = (float)(miniTiles[i].Width - 1) / _texture.GetWidth();
                    float tH = (float)(miniTiles[i].Height - 1) / _texture.GetHeight();

                    float rX = x + ((miniTiles[i].X % 32f) / 32f);
                    float rY = y + ((miniTiles[i].Y % 32f) / 32f);

                    float[] verts = new float[] {
                        rX, rY, rZ,
                        rX + 0.5f, rY, rZ,
                        rX, rY + 0.5f, rZ,
                        rX + 0.5f, rY + 0.5f, rZ
                    };

                    float[] texCoords = new float[]
                    {
                        tX, tY,
                        tX + tW, tY,
                        tX, tY + tH,
                        tX + tW, tY + tH
                    };

                    int offset = _indices.Count;

                    int[] indices = new int[]
                    {
                        offset, offset + 1, offset + 3, offset + 2
                    };

                    _vertices.AddRange(verts);
                    _textureCoords.AddRange(texCoords);
                    _indices.AddRange(indices);
                }

            }

            public bool IsReflections()
            {
                return _isReflections;
            }

            public bool IsAutoTiles()
            {
                return _isAutoTiles;
            }

            public TilesetData.Tileset GetTileset()
            {
                return _tileset;
            }

            public Texture GetTexture()
            {
                return _texture;
            }

            public Shape GetShape()
            {
                if (_shape == null && _vertices.Count > 0 && Ready)
                {
                    _shape = new Shape(_vertices.ToArray(), _textureCoords.ToArray(), _indices.ToArray(), OpenTK.Graphics.OpenGL.PrimitiveType.Quads);
                    _shape.IgnoreTextureCoordTransforms = true;
                }

                return _shape;
            }

            public bool Valid()
            {
                return _vertices.Count > 0;
            }

        }

        public static MapComponent Instance { get; private set; }

        private MapInstance _mapInstance;
        private bool _mapDataChanged;
        private List<MapTileMeshData> _MapTileMeshes;
        private float _autoTileAnimationTimer;

        private PlayerPacket _localPlayerPacket;
        MapPlayer _localMapPlayer;

        public List<Entity> MapEventEntities { get; private set; }
        public Dictionary<int, Entity> PlayerEntities { get; private set; }
        public List<Entity> EnemyEntites { get; private set; }
        public List<Entity> ProjectileEntites { get; private set; }
        public List<Entity> MapItemEntities { get; private set; }

        public MapComponent(Entity entity)
            : base(entity)
        {
            Instance = this;

            _mapInstance = null;
            _mapDataChanged = false;
            _MapTileMeshes = new List<MapTileMeshData>();
            _autoTileAnimationTimer = 0;

            _localPlayerPacket = null;
            _localMapPlayer = null;

            MapEventEntities = new List<Entity>();
            PlayerEntities = new Dictionary<int, Entity>();
            EnemyEntites = new List<Entity>();
            ProjectileEntites = new List<Entity>();
            MapItemEntities = new List<Entity>();
        }

        public MapInstance GetMapInstance()
        {
            return _mapInstance;
        }

        public void SetMapInstance(MapPacket packet)
        {
            _mapInstance = new MapInstance(packet);

            for (int i = 0; i < MapEventEntities.Count; i++)
                MapEventEntities[i].Destroy();
            MapEventEntities.Clear();

            //
            // DESTROY players here (when they are in the map packet)
            //

            for (int i = 0; i < EnemyEntites.Count; i++)
                EnemyEntites[i].Destroy();
            EnemyEntites.Clear();

            for (int i = 0; i < ProjectileEntites.Count; i++)
                ProjectileEntites[i].Destroy();
            ProjectileEntites.Clear();

            for (int i = 0; i < MapItemEntities.Count; i++)
                MapItemEntities[i].Destroy();
            MapItemEntities.Clear();

            _mapDataChanged = true;
        }

        public PlayerPacket GetLocalPlayerPacket()
        {
            return _localPlayerPacket;
        }

        private struct TileInfo
        {
            public int Layer, X, Y, TileID, TilesetID;
        }

        private void GenerateMapMeshes()
        {
            _MapTileMeshes.Clear();
            Dictionary<string, List<TileInfo>> tiles = new Dictionary<string, List<TileInfo>>();

            for (int layer = 0; layer < MapData.NUM_LAYERS; layer++)
            {
                for (int x = 0; x < _mapInstance.GetMapData().GetWidth(); x++)
                {
                    for (int y = 0; y < _mapInstance.GetMapData().GetHeight(); y++)
                    {
                        Tuple<int, int> tileInfo = _mapInstance.GetMapData().GetTile(layer, x, y);
                        if (tileInfo == null)
                            continue;
                        int tileID = tileInfo.Item1;
                        if (tileID == 0)
                            continue;
                        int tilesetID = tileInfo.Item2;
                        if (tilesetID == -1)
                            continue;

                        TilesetData.Tileset tileset = TilesetData.GetTileset(tilesetID);

                        TileInfo tileInfo2;
                        tileInfo2.Layer = layer;
                        tileInfo2.X = x;
                        tileInfo2.Y = y;
                        tileInfo2.TileID = tileID;
                        tileInfo2.TilesetID = tilesetID;

                        string key;

                        if (tileID < 8)
                        {
                            key = "AutoTiles/" + tileset.GetAutoTile(tileID - 1) + "," + tilesetID + "," + tileID;
                        }
                        else
                        {
                            key = "Tilesets/" + tileset.ImagePath + "," + tilesetID;
                        }
                        
                        if (!tiles.ContainsKey(key))
                        {
                            tiles.Add(key, new List<TileInfo>());
                        }

                        tiles[key].Add(tileInfo2);
                    }
                }
            }

            int[] hashValues = new int[] { 1, 2, 4, 8, 16, 32, 64, 128 };
            int[,] offsets = new int[,] { { 0, -1 }, { 1, -1 }, { 1, 0 }, { 1, 1 }, { 0, 1 }, { -1, 1 }, { -1, 0 }, { -1, -1 } };
            Tuple<int, int> targetInfo;

            for (int i = 0; i < tiles.Count; i++)
            {
                string key = tiles.ElementAt(i).Key;
                string textureName = key.Split(',')[0];
                int tilesetID = tiles.ElementAt(i).Value[0].TilesetID;
                TilesetData.Tileset tileset = TilesetData.GetTileset(tilesetID);
                Texture texture = Assets.GetTexture(textureName);
                bool isAutoTiles = textureName.StartsWith("AutoTiles/");

                MapTileMeshData regularMeshData = new MapTileMeshData(false, isAutoTiles, tileset, texture);
                MapTileMeshData reflectionMeshData = new MapTileMeshData(true, isAutoTiles, tileset, texture);

                for (int j = 0; j < tiles[key].Count; j++)
                {
                    int layer = tiles[key][j].Layer;
                    int x = tiles[key][j].X;
                    int y = tiles[key][j].Y;
                    int tileID = tiles[key][j].TileID;
                    int hashCode = -1;

                    if (isAutoTiles)
                    {
                        hashCode = 0;
                        for (int k = 0; k < 8; k++)
                        {
                            targetInfo = _mapInstance.GetMapData().GetTile(layer, x + offsets[k, 0], y + offsets[k, 1]);
                            if (targetInfo == null)
                                hashCode += hashValues[k];
                            else if (targetInfo.Item1 == tileID && targetInfo.Item2 == tilesetID)
                                hashCode += hashValues[k];
                        }
                    }

                    bool isReflection = tileset.GetReflectionFlag(tileID);
                    if (isReflection)
                    {
                        if (isAutoTiles)
                        {
                            reflectionMeshData.AddAutoMapTile(tileID, x, y, layer, hashCode);
                            reflectionMeshData.NumFrames = texture.GetWidth() / 96;
                            reflectionMeshData.AutoTileID = tileID;
                        }
                        else
                            reflectionMeshData.AddRegularMapTile(tileID, x, y, layer);
                    }
                    else
                    {
                        if (isAutoTiles)
                        {
                            regularMeshData.AddAutoMapTile(tileID, x, y, layer, hashCode);
                            regularMeshData.NumFrames = texture.GetWidth() / 96;
                            regularMeshData.AutoTileID = tileID;
                        }
                        else
                            regularMeshData.AddRegularMapTile(tileID, x, y, layer);
                    }

                }

                regularMeshData.Ready = true;
                reflectionMeshData.Ready = true;

                if (regularMeshData.Valid())
                    _MapTileMeshes.Add(regularMeshData);
                if (reflectionMeshData.Valid())
                    _MapTileMeshes.Add(reflectionMeshData);

            }
        }

        public void SetPlayers(Dictionary<int, PlayerPacket> playerPackets)
        {
            for (int i = 0; i < PlayerEntities.Count; i++)
            {
                int id = PlayerEntities.ElementAt(i).Key;
                if (playerPackets.ContainsKey(id) == false)
                {
                    PlayerEntities[id].Destroy();
                    PlayerEntities.Remove(id);
                    _mapInstance.RemoveMapPlayer(i);
                    i--;
                }
            }

            for (int i = 0; i < playerPackets.Count; i++)
            {
                 PlayerPacket packet = playerPackets.ElementAt(i).Value;
                if (packet.PlayerID == RpgClientConnection.Instance.GetLocalPlayerID())
                {
                    _localPlayerPacket = packet;
                }

                if (PlayerEntities.ContainsKey(packet.PlayerID))
                {
                    Entity clientEntity = PlayerEntities[packet.PlayerID];
                    PlayerComponent playerComponent = (PlayerComponent)clientEntity.FindComponent<PlayerComponent>();
                    playerComponent.SetPlayerPacket(packet);
                }
                else
                {
                    Entity clientEntity = Entity.CreateInstance(this.Parent.GetManager());
                    clientEntity.GetTransform().Parent = this.Parent.GetTransform();
                    MapPlayer mapPlayer = new MapPlayer(packet, null);
                    _mapInstance.AddMapPlayer(mapPlayer);
                    new PlayerComponent(clientEntity, mapPlayer);
                    PlayerEntities.Add(packet.PlayerID, clientEntity);

                    if (_localPlayerPacket == packet)
                    {
                        _localMapPlayer = mapPlayer;
                    }
                }
            }

        }

        public void MovePlayer(MovementDirection direction)
        {
            if (_localMapPlayer != null)
            {
                _localMapPlayer.Move(direction);
            }
        }

        public void ChangeMapEventSprite(int mapEvent, int spriteID)
        {
            if (mapEvent >= 0 && mapEvent < MapEventEntities.Count)
            {
                ((MapEventComponent)MapEventEntities[mapEvent].FindComponent<MapEventComponent>()).SetSpriteID(spriteID);
                _mapInstance.GetMapData().GetMapEvent(mapEvent).SpriteID = spriteID;
            }
        }

        public void ChangeMapEventRenderPriority(int mapEvent, RenderPriority priority)
        {
            if (mapEvent >= 0 && mapEvent < MapEventEntities.Count)
            {
                _mapInstance.GetMapData().GetMapEvent(mapEvent).Priority = priority;
            }
        }

        public void ChangeMapEventEnabled(int mapEvent, bool enabled)
        {
            if (mapEvent >= 0 && mapEvent < MapEventEntities.Count)
            {
                _mapInstance.GetMapData().GetMapEvent(mapEvent).Enabled = enabled;
            }
        }

        private void AddMapEvent(MapEvent mapEvent, bool addToMap = true)
        {
            Entity entity = Entity.CreateInstance(GameState.Instance.EntityManager);
            entity.GetTransform().Parent = this.Parent.GetTransform();
            new MapEventComponent(entity, mapEvent);
            if (addToMap)
                MapEventEntities.Add(entity);
        }

        //public void AddMapPlayer()

        public void AddMapEnemy(MapEnemy mapEnemy, bool addToMap = true)
        {
            Entity entity = Entity.CreateInstance(GameState.Instance.EntityManager);
            entity.GetTransform().Parent = this.Parent.GetTransform();
            new MapEnemyComponent(entity, mapEnemy);
            EnemyEntites.Add(entity);
            if (addToMap)
                _mapInstance.AddMapEnemy(mapEnemy);
        }

        public void AddMapProjectile(MapProjectile projectile, bool addToMap = true)
        {
            Entity entity = Entity.CreateInstance(GameState.Instance.EntityManager);
            entity.GetTransform().Parent = this.Parent.GetTransform();
            new ProjectileComponent(entity, projectile);
            ProjectileEntites.Add(entity);
            if (addToMap)
                _mapInstance.AddMapProjectile(projectile);
        }

        public void AddMapItem(MapItem mapItem, bool addToMap = true)
        {
            Entity entity = Entity.CreateInstance(GameState.Instance.EntityManager);
            entity.GetTransform().Parent = this.Parent.GetTransform();
            new MapItemComponent(entity, mapItem);
            MapItemEntities.Add(entity);
            if (addToMap)
                _mapInstance.AddMapItem(mapItem);
        }

        public override void Update(FrameEventArgs e)
        {
            base.Update(e);
            _autoTileAnimationTimer += (float)e.Time;

            if (_mapDataChanged)
            {
                if (_mapInstance != null)
                {
                    for (int i = 0; i < _mapInstance.GetMapData().MapEventsCount(); i++)
                    {
                        AddMapEvent(_mapInstance.GetMapData().GetMapEvent(i), false);
                    }

                    //
                    // add players here (when they are in the map packet)
                    //

                    for (int i = 0; i < _mapInstance.GetMapPacket().Enemies.Count; i++)
                    {
                        AddMapEnemy(_mapInstance.GetMapPacket().Enemies[i], false);
                    }

                    for (int i = 0; i < _mapInstance.GetMapPacket().Projectiles.Count; i++)
                    {
                        AddMapProjectile(_mapInstance.GetMapPacket().Projectiles[i], false);
                    }

                    for (int i = 0; i < _mapInstance.GetMapPacket().Items.Count; i++)
                    {
                        AddMapItem(_mapInstance.GetMapPacket().Items[i], false);
                    }

                }
                _mapDataChanged = false;
                GenerateMapMeshes();
            }
        }

        public override void LateUpdate(OpenTK.FrameEventArgs e)
        {
            base.Update(e);

            StateWindow.Instance.Title = "FPS: " + StateWindow.Instance.GetFPS();
        }

        //*
        public override void Render(FrameEventArgs e)
        {
            base.Render(e);
            if (_mapDataChanged) return;
            if (_mapInstance != null)
            {
                Renderer.PushWorldMatrix();
                Renderer.TranslateWorld((int)Transform.Position.X, (int)Transform.Position.Y, 0);

                for (int i = 0; i < _MapTileMeshes.Count; i++)
                {
                    Shape shape = _MapTileMeshes[i].GetShape();
                    if (shape != null)
                    {
                        Vector3 pos = Vector3.Zero;
                        Vector3 scale = new Vector3(32, 32, 1);
                        Color4 colour = Color4.White;
                        Texture texture = _MapTileMeshes[i].GetTexture();
                        bool reflections = _MapTileMeshes[i].IsReflections();
                        bool autoTiles = _MapTileMeshes[i].IsAutoTiles();

                        if (reflections)
                        {
                            Renderer.PushStencilDepth(OpenTK.Graphics.OpenGL.StencilOp.Incr, OpenTK.Graphics.OpenGL.StencilFunction.Equal);
                        }

                        if (autoTiles)
                        {
                            int numFrames = _MapTileMeshes[i].NumFrames;
                            TilesetData.Tileset tileset = _MapTileMeshes[i].GetTileset();
                            int tileID = _MapTileMeshes[i].AutoTileID;

                            int frame = (int)(_autoTileAnimationTimer * tileset.GetAutoTileTimer(tileID - 1)) % numFrames;
                            float offsetX = (frame * 96f) / texture.GetWidth();
                            Renderer.SetTextureOffset(offsetX, 0);
                        }

                        Renderer.FillTexture(texture, shape, ref pos, ref scale, ref colour);

                        if (reflections)
                        {
                            Renderer.PopStencilDepth();
                        }

                        if (autoTiles)
                        {
                            Renderer.SetTextureOffset(0, 0);
                        }
                    }
                }

                Renderer.PopWorldMatrix();
            }
        }
        //*/

        /*
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

                Tuple<int, int> tileInfo;
                int tilesetID;
                TilesetData.Tileset tileset;
                Texture tilesetTexture;
                int tilePriority;
                string autoTileName;
                Tuple<int, int> targetInfo;

                int hashCode;
                int[] hashValues = new int[] { 1, 2, 4, 8, 16, 32, 64, 128 };
                int[,] offsets = new int[,] { { 0, -1 }, { 1, -1 }, { 1, 0 }, { 1, 1 }, { 0, 1 }, { -1, 1 }, { -1, 0 }, { -1, -1 } };
                Texture autoTile;
                Rectangle[] miniTiles;
                int tileFrames;
                int frame;

                for (int layer = 0; layer < MapData.NUM_LAYERS; layer++)
                {
                    for (int x = startX; x < endX; x++)
                    {
                        for (int y = startY; y < endY; y++)
                        {
                            tileInfo = _mapData.GetTile(layer, x, y);
                            if (tileInfo == null)
                                continue;
                            int tileID = tileInfo.Item1;
                            if (tileID == 0)
                                continue;

                            tilesetID = tileInfo.Item2;
                            tileset = TilesetData.GetTileset(tilesetID);
                            tilesetTexture = Assets.GetTexture("Tilesets/" + tileset.ImagePath);

                            pos.X = x * 32;
                            pos.Y = y * 32;
                            tilePriority = tileset.GetTilePriority(tileID);
                            pos.Z = -((y + tilePriority) * 2);

                            if (tileset.GetReflectionFlag(tileID))
                            {
                                Renderer.PushStencilDepth(OpenTK.Graphics.OpenGL.StencilOp.Incr, OpenTK.Graphics.OpenGL.StencilFunction.Equal);
                            }

                            if (tileID < 8)
                            {
                                autoTileName = tileset.GetAutoTile(tileID - 1);
                                if (autoTileName != "")
                                {
                                    hashCode = 0;
                                    for (int i = 0; i < 8; i++)
                                    {
                                        targetInfo = _mapData.GetTile(layer, x + offsets[i, 0], y + offsets[i, 1]);
                                        if (targetInfo == null)
                                            hashCode += hashValues[i];
                                        else if (targetInfo.Item1 == tileInfo.Item1 && targetInfo.Item2 == tileInfo.Item2)
                                            hashCode += hashValues[i];
                                    }

                                    autoTile = Assets.GetTexture("AutoTiles/" + autoTileName);
                                    miniTiles = TilesetData.Tileset.GetAutoTileSources(hashCode);

                                    tileFrames = autoTile.GetWidth() / 96;
                                    _autoTileAnimationTimer += (float)e.Time;


                                    frame = (int)(_autoTileAnimationTimer * (tileset.GetAutoTileTimer(tileID - 1) / 20)) % tileFrames;
                                    //int frame = (int)(time) * tileFrames;
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
        //*/
    }
}

using Genus2D.Networking;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Genus2D.GameData
{
    public class MapInstance
    {
        private MapPacket _mapPacket;

        private List<Dictionary<MapEnemy, float>> _enemyTrackers;
        private float _updateMapTimer;
        private Random _random = new Random();

        public MapInstance(int mapID)
        {
            Initialize(new MapPacket(mapID, MapInfo.LoadMap(mapID)));
        }

        public MapInstance(MapPacket mapPacket)
        {
            Initialize(mapPacket);
        }

        private void Initialize(MapPacket mapPacket)
        {
            _mapPacket = mapPacket;

            _enemyTrackers = new List<Dictionary<MapEnemy, float>>();
            for (int i = 0; i < GetMapData().MapEventsCount(); i++)
            {
                _enemyTrackers.Add(new Dictionary<MapEnemy, float>());
            }
            _updateMapTimer = 0.0f;
        }

        public MapPacket GetMapPacket()
        {
            return _mapPacket;
        }

        public MapData GetMapData()
        {
            return _mapPacket.MapData;
        }

        public MapPlayer FindMapPlayer(int id)
        {
            for (int i = 0; i < _mapPacket.Players.Count; i++)
            {
                if (_mapPacket.Players[i].PlayerID == id)
                    return _mapPacket.Players[i];
            }
            return null;
        }

        public List<MapPlayer> GetMapPlayers()
        {
            List<MapPlayer> players = new List<MapPlayer>();
            foreach (MapPlayer player in _mapPacket.Players)
            {
                if (player != null)
                    players.Add(player);
            }
            return players;
        }

        public Dictionary<MapEnemy, float> GetEnemyTracker(MapEvent mapEvent)
        {
            int mapEventIndex = -1;
            for (int i = 0; i < GetMapData().MapEventsCount(); i++)
            {
                if (GetMapData().GetMapEvent(i) == mapEvent)
                {
                    mapEventIndex = i;
                    break;
                }
            }

            if (mapEventIndex != -1)
            {
                return _enemyTrackers[mapEventIndex];
            }
            return null;
        }

        public MapEnemy FindMapEnemy(int enemyID)//enemy signature?
        {
            if (enemyID >= 0 && enemyID < _mapPacket.Enemies.Count)
                return _mapPacket.Enemies[enemyID];
            return null;
        }

        public bool TileInsideMap(int x, int y)
        {
            if (x < 0 || y < 0 || x >= GetMapData().GetWidth() || y >= GetMapData().GetHeight())
                return false;
            return true;
        }

        public bool MapTilesetPassable(int x, int y)
        {
            if (!TileInsideMap(x, y))
                return false;

            for (int i = MapData.NUM_LAYERS - 1; i >= 0; i--)
            {
                Tuple<int, int> tileInfo = GetMapData().GetTile(i, x, y);
                if (tileInfo.Item2 == -1)
                    continue;

                if (!TilesetData.GetTileset(tileInfo.Item2).GetPassable(tileInfo.Item1))
                    return false;
            }

            return true;
        }

        public bool MapTilePassable(int x, int y, bool checkDirections, bool onBridge, bool bridgeEntry, MovementDirection dir)
        {
            if (!TileInsideMap(x, y))
                return false;

            for (int i = MapData.NUM_LAYERS - 1; i >= 0; i--)
            {
                Tuple<int, int> tileInfo = GetMapData().GetTile(i, x, y);
                if (tileInfo.Item2 == -1)
                    continue;

                if (TilesetData.GetTileset(tileInfo.Item2).GetBridgeFlag(tileInfo.Item1))
                {
                    if (onBridge)
                        break;
                }
                else
                {
                    if (onBridge)
                    {
                        if (bridgeEntry && TilesetData.GetTileset(tileInfo.Item2).GetPassable(tileInfo.Item1))
                            return true;
                        else if (GetBridgeFlag(x, y))
                            return true;
                        else
                            return false;
                    }
                    else
                    {
                        if (checkDirections)
                        {
                            if (!TilesetData.GetTileset(tileInfo.Item2).GetPassable(tileInfo.Item1, dir))
                            {
                                return false;
                            }
                        }
                        else
                        {
                            if (!TilesetData.GetTileset(tileInfo.Item2).GetPassable(tileInfo.Item1))
                            {
                                return false;
                            }
                        }
                    }
                }
            }

            return true;
        }

        public bool MapTileCharacterPassable(int x, int y, bool checkDirections, bool onBridge, bool bridgeEntry, MovementDirection dir)
        {
            if (!MapTilePassable(x, y, checkDirections, onBridge, bridgeEntry, dir))
                return false;

            int tileEvent = TileHasNonePassableEvent(x, y, -1);
            if (tileEvent != -1)
            {
                MapEvent mapEvent = GetMapData().GetMapEvent(tileEvent);
                if (onBridge == mapEvent.OnBridge)
                {
                    return false;
                }
            }

            return true;
        }

        public bool MapTileEventPassable(int x, int y, int eventID, bool checkDirections, bool onBridge, bool bridgeEntry, MovementDirection dir)
        {
            MapEvent mapEvent = GetMapData().GetMapEvent(eventID);
            if (mapEvent.Passable && TileInsideMap(x, y))
                return true;

            if (!MapTilePassable(x, y, checkDirections, onBridge, bridgeEntry, dir))
                return false;


            if (TileHasPlayers(x, y, onBridge))
                return false;

            int tileEvent = TileHasNonePassableEvent(x, y, eventID);
            if (tileEvent != -1)
            {
                mapEvent = GetMapData().GetMapEvent(tileEvent);
                if (onBridge == mapEvent.OnBridge)
                {
                    return false;
                }
            }

            return true;
        }

        public bool MapTileProjectilePassable(int x, int y, bool onBridge, bool bridgeEntry, MovementDirection dir)
        {
            if (!MapTilePassable(x, y, true, onBridge, bridgeEntry, dir))
                return false;

            int tileEvent = TileHasNonePassableEvent(x, y, -1);
            if (tileEvent != -1)
            {
                MapEvent mapEvent = GetMapData().GetMapEvent(tileEvent);
                if (onBridge == mapEvent.OnBridge)
                {
                    return false;
                }
            }

            return true;
        }

        public int TileHasNonePassableEvent(int mapX, int mapY, int ignoreID)
        {
            int id = -1;
            for (int i = 0; i < GetMapData().MapEventsCount(); i++)
            {
                if (i == ignoreID)
                    continue;
                MapEvent mapEvent = GetMapData().GetMapEvent(i);
                if (!mapEvent.Passable && mapEvent.Enabled)
                {
                    if (mapEvent.MapX == mapX && mapEvent.MapY == mapY)
                    {
                        id = i;
                        break;
                    }
                }
            }
            return id;
        }

        public bool TileHasPlayers(int mapX, int mapY, bool onBridge)
        {
            for (int i = 0; i < _mapPacket.Players.Count; i++)
            {
                PlayerPacket packet = _mapPacket.Players[i].GetPlayerPacket();
                if (packet.PositionX == mapX && packet.PositionY == mapY && packet.OnBridge == onBridge)
                    return true;
            }
            return false;
        }

        public bool GetBridgeFlag(int mapX, int mapY)
        {
            for (int i = MapData.NUM_LAYERS - 1; i >= 0; i--)
            {
                Tuple<int, int> tileInfo = GetMapData().GetTile(i, mapX, mapY);
                if (tileInfo.Item2 == -1)
                    continue;

                if (TilesetData.GetTileset(tileInfo.Item2).GetBridgeFlag(tileInfo.Item1))
                    return true;
            }
            return false;
        }

        public bool TerrainTagCheck(int x, int y, int tag)
        {
            for (int i = 0; i < MapData.NUM_LAYERS; i++)
            {
                Tuple<int, int> tileInfo = GetMapData().GetTile(i, x, y);
                if (tileInfo.Item2 != -1)
                {
                    if (TilesetData.GetTileset(tileInfo.Item2).GetTerrainTag(tileInfo.Item1) == tag)
                        return true;
                }
            }
            return false;
        }

        public delegate void MapEventHandler(MapEvent mapEvent, int index);
        public event MapEventHandler OnChangeMapEvent;

        public bool ChangeMapEvent(int eventIndex, EventCommand command)
        {
            bool changed = false;
            MapEvent mapEvent = GetMapData().GetMapEvent(eventIndex);

            ChangeMapEventProperty property = (ChangeMapEventProperty)command.GetParameter("Property");
            switch (property)
            {
                case ChangeMapEventProperty.Teleport:
                    int mapX = (int)command.GetParameter("MapX");
                    int mapY = (int)command.GetParameter("MapY");
                    changed = TeleportMapEvent(eventIndex, mapX, mapY);
                    break;
                case ChangeMapEventProperty.Move:
                    MovementDirection movementDirection = (MovementDirection)command.GetParameter("MovementDirection");
                    changed = MoveMapEvent(eventIndex, movementDirection);
                    break;
                case ChangeMapEventProperty.Direction: 
                    FacingDirection facingDirection = (FacingDirection)command.GetParameter("FacingDirection");
                    changed = ChangeMapEventDirection(mapEvent, facingDirection);
                    break;
                case ChangeMapEventProperty.Sprite:
                    int spriteID = (int)command.GetParameter("SpriteID");
                    changed = ChangeMapEventSprite(mapEvent, spriteID);
                    break;
                case ChangeMapEventProperty.RenderPriority:
                    RenderPriority priority = (RenderPriority)command.GetParameter("RenderPriority");
                    changed = ChangeMapEventRenderPriority(mapEvent, priority);
                    break;
                case ChangeMapEventProperty.MovementSpeed:
                    MovementSpeed speed = (MovementSpeed)command.GetParameter("MovementSpeed");
                    mapEvent.Speed = speed;
                    changed = true;
                    break;
                case ChangeMapEventProperty.MovementFrequency:
                    MovementFrequency frequency = (MovementFrequency)command.GetParameter("MovementFrequency");
                    mapEvent.Frequency = frequency;
                    changed = true;
                    break;
                case ChangeMapEventProperty.Passable:
                    bool passable = (bool)command.GetParameter("Passable");
                    GetMapData().GetMapEvent(eventIndex).Passable = passable;
                    changed = true;
                    break;
                case ChangeMapEventProperty.RandomMovement:
                    bool randomMovement = (bool)command.GetParameter("RandomMovement");
                    mapEvent.RandomMovement = randomMovement;
                    changed = true;
                    break;
                case ChangeMapEventProperty.Enabled:
                    bool enabled = (bool)command.GetParameter("Enabled");
                    changed = ChangeMapEventEnabled(mapEvent, enabled);
                    break;
            }

            if (changed) OnChangeMapEvent?.Invoke(mapEvent, eventIndex);

            return changed;
        }

        public bool TeleportMapEvent(int eventIndex, int mapX, int mapY)
        {
            MapEvent mapEvent = GetMapData().GetMapEvent(eventIndex);
            if (mapEvent.Moving()) return false;

            if (MapTileEventPassable(mapX, mapY, eventIndex, false, false, false, MovementDirection.Down))
            {
                mapEvent.MapX = mapX;
                mapEvent.MapY = mapY;
                mapEvent.RealX = mapX * 32;
                mapEvent.RealY = mapY * 32;
                mapEvent.PositionChanged = true;

                return true;
            }
            return false;
        }

        public bool MoveMapEvent(int eventIndex, MovementDirection direction)
        {
            MapEvent mapEvent = GetMapData().GetMapEvent(eventIndex);
            if (mapEvent.Moving() || !mapEvent.Enabled) return false;

            int prevX = mapEvent.MapX;
            int prevY = mapEvent.MapY;
            int targetX = prevX;
            int targetY = prevY;
            FacingDirection facingDirection = FacingDirection.Down;
            MovementDirection entryDirection = MovementDirection.Down;

            switch (direction)
            {
                case MovementDirection.UpperLeft:
                    targetX -= 1;
                    targetY -= 1;
                    facingDirection = FacingDirection.Left;
                    entryDirection = MovementDirection.LowerRight;
                    break;
                case MovementDirection.Up:
                    targetY -= 1;
                    facingDirection = FacingDirection.Up;
                    entryDirection = MovementDirection.Down;
                    break;
                case MovementDirection.UpperRight:
                    targetX += 1;
                    targetY -= 1;
                    facingDirection = FacingDirection.Right;
                    entryDirection = MovementDirection.LowerLeft;
                    break;
                case MovementDirection.Left:
                    targetX -= 1;
                    facingDirection = FacingDirection.Left;
                    entryDirection = MovementDirection.Right;
                    break;
                case MovementDirection.Right:
                    targetX += 1;
                    facingDirection = FacingDirection.Right;
                    entryDirection = MovementDirection.Left;
                    break;
                case MovementDirection.LowerLeft:
                    targetX -= 1;
                    targetY += 1;
                    facingDirection = FacingDirection.Left;
                    entryDirection = MovementDirection.UpperRight;
                    break;
                case MovementDirection.Down:
                    targetY += 1;
                    facingDirection = FacingDirection.Down;
                    entryDirection = MovementDirection.Up;
                    break;
                case MovementDirection.LowerRight:
                    targetX += 1;
                    targetY += 1;
                    facingDirection = FacingDirection.Right;
                    entryDirection = MovementDirection.UpperLeft;
                    break;
            }

            bool bridgeEntry = MapTilesetPassable(prevX, prevY) && mapEvent.OnBridge && !mapEvent.Passable;
            if (MapTileEventPassable(prevX, prevY, eventIndex, true, mapEvent.OnBridge, bridgeEntry, direction) &&
                MapTileEventPassable(targetX, targetY, eventIndex, true, mapEvent.OnBridge, bridgeEntry, entryDirection))
            {
                if (mapEvent.Move(targetX, targetY))
                {
                    if (GetBridgeFlag(targetX, targetY))
                    {
                        if (MapTilesetPassable(targetX, targetY))
                            mapEvent.OnBridge = true;
                    }
                    else
                    {
                        mapEvent.OnBridge = false;
                    }
                    mapEvent.EventDirection = facingDirection;
                    mapEvent.PositionChanged = true;
                    return true;
                }
            }
            return false;
        }

        private bool ChangeMapEventDirection(MapEvent mapEvent, FacingDirection direction)
        {
            if (mapEvent.EventDirection != direction)
            {
                mapEvent.EventDirection = direction;
                mapEvent.DirectionChanged = true;
                return true;
            }
            return false;
        }

        private bool ChangeMapEventSprite(MapEvent mapEvent, int spriteID)
        {
            if (mapEvent.SpriteID != spriteID)
            {
                mapEvent.SpriteID = spriteID;
                mapEvent.SpriteChanged = true;
                return true;
            }
            return false;
        }

        private bool ChangeMapEventRenderPriority(MapEvent mapEvent, RenderPriority priority)
        {
            if (mapEvent.Priority != priority)
            {
                mapEvent.Priority = priority;
                mapEvent.RenderPriorityChanged = true;
                return true;
            }
            return false;
        }

        private bool ChangeMapEventEnabled(MapEvent mapEvent, bool enabled)
        {
            if (mapEvent.Enabled != enabled)
            {
                mapEvent.Enabled = enabled;
                mapEvent.EnabledChanged = true;
                return true;
            }
            return false;
        }

        public delegate void MapPlayerHandler(MapPlayer mapPlayer, int index);
        public event MapPlayerHandler OnAddMapPlayer, OnRemoveMapPlayer, OnUpdateMapPlayer;

        public void AddMapPlayer(MapPlayer player)
        {
            _mapPacket.Players.Add(player);
            player.SetMapInstance(this);
            OnAddMapPlayer?.Invoke(player, _mapPacket.Players.Count - 1);
        }

        public void RemoveMapPlayer(MapPlayer mapPlayer)
        {
            if (_mapPacket.Players.Contains(mapPlayer))
            {
                RemoveMapPlayer(_mapPacket.Players.IndexOf(mapPlayer));
            }
        }

        public void RemoveMapPlayer(int index)
        {
            MapPlayer mapPlayer = GetMapPlayer(index);
            if (mapPlayer != null)
            {
                _mapPacket.Players.RemoveAt(index);
                OnRemoveMapPlayer?.Invoke(mapPlayer, index);
            }
        }

        public MapPlayer GetMapPlayer(int index)
        {
            if (index >= 0 && index < _mapPacket.Players.Count)
                return _mapPacket.Players[index];

            return null;
        }

        public delegate void MapEnemyHandler(MapEnemy mapEnemy, int index);
        public event MapEnemyHandler OnAddMapEnemy, OnRemoveMapEnemy, OnUpdateMapEnemy;

        public void AddMapEnemy(MapEnemy mapEnemy)
        {
            _mapPacket.Enemies.Add(mapEnemy);
            OnAddMapEnemy?.Invoke(mapEnemy, _mapPacket.Enemies.Count - 1);
        }

        public void RemoveMapEnemy(int index)
        {
            MapEnemy mapEnemy = GetMapEnemy(index);
            if (mapEnemy != null)
            {
                _mapPacket.Enemies.RemoveAt(index);
                OnRemoveMapEnemy?.Invoke(mapEnemy, index);
            }
        }

        public int GetMapEnemyIndex(MapEnemy mapEnemy)
        {
            if (_mapPacket.Enemies.Contains(mapEnemy))
            {
                return _mapPacket.Enemies.IndexOf(mapEnemy);
            }
            return -1;
        }

        public MapEnemy GetMapEnemy(int index)
        {
            if (index >= 0 && index < _mapPacket.Enemies.Count)
                return _mapPacket.Enemies[index];

            return null;
        }

        public delegate void MapProjectileHandler(MapProjectile mapProjectile, int index);
        public event MapProjectileHandler OnAddMapProjectile, OnRemoveMapProjectile, OnUpdateMapProjectile;

        public void AddMapProjectile(MapProjectile projectile)
        {
            _mapPacket.Projectiles.Add(projectile);
            OnAddMapProjectile?.Invoke(projectile, _mapPacket.Projectiles.Count - 1);
        }

        public bool RemoveMapProjectile(int index)
        {
            MapProjectile mapProjectile = GetMapProjectile(index);
            if (mapProjectile != null)
            {
                _mapPacket.Projectiles.RemoveAt(index);
                OnRemoveMapProjectile?.Invoke(mapProjectile, index);
                return true;
            }
            return false;
        }

        public MapProjectile GetMapProjectile(int index)
        {
            if (index >= 0 && index < _mapPacket.Projectiles.Count)
                return _mapPacket.Projectiles[index];

            return null;
        }

        public delegate void MapItemHandler(MapItem mapItem, int index);
        public event MapItemHandler OnAddMapItem, OnRemoveMapItem, OnUpdateMapItem;

        public void AddMapItem(MapItem mapItem)
        {
            _mapPacket.Items.Add(mapItem);
            OnAddMapItem?.Invoke(mapItem, _mapPacket.Items.Count - 1);
        }

        public bool RemoveMapItem(int index)
        {
            MapItem mapItem = GetMapItem(index);
            if (mapItem != null)
            {
                _mapPacket.Items.RemoveAt(index);
                OnRemoveMapItem?.Invoke(mapItem, index);

                return true;
            }
            return false;
        }

        public MapItem GetMapItem(int index)
        {
            if (index >= 0 && index < _mapPacket.Items.Count)
                return _mapPacket.Items[index];

            return null;
        }

        public delegate void EventTriggerHandler(MapPlayer mapPlayer, MapEvent mapEvent);
        public event EventTriggerHandler OnEventTrigger;

        public void Update(float deltaTime)
        {
            //update map packet timer
            if (_updateMapTimer > 0f)
            {
                _updateMapTimer -= deltaTime;
            }

            //update enemies
            for (int i = 0; i < _mapPacket.Enemies.Count; i++)
            {
                MapEnemy mapEnemy = _mapPacket.Enemies[i];

                if (mapEnemy.AttackTimer > 0)
                    mapEnemy.AttackTimer -= deltaTime;

                if (mapEnemy.MovementTimer > 0)
                    mapEnemy.MovementTimer -= deltaTime;

                UpdateMapEnemy(mapEnemy, i);
                mapEnemy.Update(deltaTime);

                //update enemy packet
                if (_updateMapTimer <= 0)
                {
                    OnUpdateMapEnemy?.Invoke(mapEnemy, i);
                    if (mapEnemy.Dead)
                    {
                        _mapPacket.Enemies.RemoveAt(i);
                        i--;
                    }
                }
            }


            //update enemy death/respawn trackers
            for (int i = 0; i < _enemyTrackers.Count; i++)
            {
                for (int j = 0; j < _enemyTrackers[i].Count; j++)
                {
                    MapEnemy mapEnemy = _enemyTrackers[i].ElementAt(j).Key;
                    if (mapEnemy.Dead)
                    {
                        _enemyTrackers[i][mapEnemy] -= deltaTime;
                        if (_enemyTrackers[i][mapEnemy] <= 0)
                        {
                            _enemyTrackers[i].Remove(mapEnemy);
                            j--;
                        }
                    }
                }
            }

            //update map events
            for (int i = 0; i < GetMapData().MapEventsCount(); i++)
            {
                MapEvent mapEvent = GetMapData().GetMapEvent(i);

                if (mapEvent.RandomMovement)
                {
                    int dir = _random.Next(0, 7);
                    MoveMapEvent(i, (MovementDirection)dir);
                }

                if (mapEvent.TriggerType == EventTriggerType.Autorun)
                {
                    OnEventTrigger?.Invoke(null, mapEvent);
                }

                mapEvent.UpdateMovement(deltaTime);

                if (_updateMapTimer <= 0f && mapEvent.PositionChanged)
                {
                    OnChangeMapEvent?.Invoke(mapEvent, i);

                }
            }

            //update projectiles
            for (int i = 0; i < _mapPacket.Projectiles.Count; i++)
            {
                MapProjectile projectile = _mapPacket.Projectiles[i];
                if (!projectile.Destroyed)
                {
                    UpdateProjectile(projectile, i, deltaTime);
                }

                if (_updateMapTimer <= 0f || projectile.Destroyed)
                {
                    OnUpdateMapProjectile?.Invoke(projectile, i);
                }

                if (projectile.Destroyed)
                {
                    _mapPacket.Projectiles.RemoveAt(i);
                    i--;
                }
            }

            //update map items
            for (int i = 0; i < _mapPacket.Items.Count; i++)
            {
                MapItem mapItem = _mapPacket.Items[i];
                if (mapItem.PickedUp)
                {
                    RemoveMapItem(i);
                    i--;
                }
                else
                {
                    if (mapItem.PlayerLockTimer > 0)
                    {
                        mapItem.PlayerLockTimer -= deltaTime;
                        if (mapItem.PlayerLockTimer <= 0)
                        {
                            mapItem.PlayerID = -1;
                            mapItem.Changed = true;
                        }
                    }
                    else
                    {
                        mapItem.DespawnTimer -= deltaTime;
                        if (mapItem.DespawnTimer <= 0)
                        {
                            RemoveMapItem(i);
                            i--;
                        }
                    }
                }

                if (mapItem.Changed)
                {
                    UpdateMapItem(mapItem, i);
                }
            }

            //refresh packet timer
            if (_updateMapTimer <= 0)
            {
                _updateMapTimer = 0.1f;
            }

        }

        public void UpdateMapItem(MapItem mapItem, int index)
        {
            if (mapItem.Changed) OnUpdateMapItem?.Invoke(mapItem, index);
        }

        private void UpdateMapEnemy(MapEnemy mapEnemy, int index)
        {
            EnemyData enemyData = mapEnemy.GetEnemyData();
            if (mapEnemy.TargetPlayerID == -1)
            {
                //find player
                for (int i = 0; i < _mapPacket.Players.Count; i++)
                {
                    MapPlayer mapPlayer = GetMapPlayer(i);
                    if (mapPlayer.EnemyCanAttack(CharacterType.Enemy, _mapPacket.Enemies.IndexOf(mapEnemy), _mapPacket.MapData.MultiCombat))
                    {
                        PlayerPacket playerPacket = mapPlayer.GetPlayerPacket();

                        if (playerPacket.Data.Level <= enemyData.AgroLvl)
                        {
                            int xDistance = playerPacket.PositionX - mapEnemy.MapX;
                            int yDistance = playerPacket.PositionY - mapEnemy.MapY;
                            int sxDistance = playerPacket.PositionX - mapEnemy.SpawnX;
                            int syDistance = playerPacket.PositionY - mapEnemy.SpawnY;
                            int distance = (int)new Vector2(xDistance, yDistance).Length;
                            int spawnDistance = (int)new Vector2(sxDistance, syDistance).Length;
                            if (distance <= enemyData.VisionRage && spawnDistance <= enemyData.WanderRange + 1)
                            {
                                mapEnemy.TargetPlayerID = playerPacket.PlayerID;
                                break;
                            }
                        }
                    }
                }
            }
            else
            {
                //update/track target player
                bool canFight = false;
                int distance = 0;
                int spawnDistance = 0;
                MapPlayer mapPlayer = null;
                PlayerPacket playerPacket = null;
                for (int i = 0; i < _mapPacket.Players.Count; i++)
                {
                    if (_mapPacket.Players[i].EnemyCanAttack(CharacterType.Enemy, _mapPacket.Enemies.IndexOf(mapEnemy), _mapPacket.MapData.MultiCombat))
                    {
                        mapPlayer = _mapPacket.Players[i];
                        playerPacket = mapPlayer.GetPlayerPacket();

                        if (playerPacket.PlayerID == mapEnemy.TargetPlayerID)
                        {
                            int xDistance = playerPacket.PositionX - mapEnemy.MapX;
                            int yDistance = playerPacket.PositionY - mapEnemy.MapY;
                            int sxDistance = playerPacket.PositionX - mapEnemy.SpawnX;
                            int syDistance = playerPacket.PositionY - mapEnemy.SpawnY;
                            distance = (int)new Vector2(xDistance, yDistance).Length;
                            spawnDistance = (int)new Vector2(sxDistance, syDistance).Length;
                            if (distance <= enemyData.VisionRage && spawnDistance <= enemyData.WanderRange + 1)
                            {
                                canFight = true;
                            }
                            break;
                        }
                    }
                }

                if (canFight)
                {
                    if (distance == 0)
                    {
                        MoveEnemyRandomly(mapEnemy);
                    }
                    else
                    {
                        if (EnemyAttackCheck(mapEnemy, playerPacket, distance))
                        {
                            TurnEnemyTowardsPlayer(mapEnemy, playerPacket);

                            switch (mapEnemy.GetEnemyData().AtkStyle)
                            {
                                case AttackStyle.Melee:
                                    EnemyMeleeAttack(mapEnemy, mapPlayer);
                                    break;
                                case AttackStyle.Ranged:
                                    EnemyRangeAttack(mapEnemy, mapPlayer);
                                    break;
                                case AttackStyle.Magic:
                                    EnemyMagicAttack(mapEnemy, mapPlayer);
                                    break;
                            }

                        }
                        else
                        {
                            MoveEnemyTowardsPlayer(mapEnemy, mapPlayer);
                        }
                    }
                }
                else
                {
                    mapEnemy.TargetPlayerID = -1;
                }
            }

            if (mapEnemy.TargetPlayerID == -1 && mapEnemy.MovementTimer <= 0)
            {
                MoveEnemyRandomly(mapEnemy);
                mapEnemy.MovementTimer = (float)_random.NextDouble() * 3;
            }
        }

        private void UpdateProjectile(MapProjectile projectile, int index, float deltaTime)
        {
            ProjectileData data = projectile.GetData();
            projectile.Lifespan += deltaTime;
            if (projectile.Lifespan >= data.Lifespan)
            {
                projectile.Destroyed = true;
            }
            else
            {
                int mapX = (int)(projectile.Position.X / 32);
                int mapY = (int)(projectile.Position.Y / 32);
                Vector2 targetPos = projectile.Position + (projectile.Velocity * deltaTime);
                int targetX = (int)(targetPos.X / 32);
                int targetY = (int)(targetPos.Y / 32);

                bool canMove = false;

                if (targetX == mapX && targetY == mapY)
                {
                    canMove = true;
                }
                else
                {
                    MovementDirection exitDir = MovementDirection.Down;
                    MovementDirection entryDir = MovementDirection.Up;
                    switch (projectile.Direction)
                    {
                        case FacingDirection.Left:
                            exitDir = MovementDirection.Left;
                            entryDir = MovementDirection.Right;
                            break;
                        case FacingDirection.Right:
                            exitDir = MovementDirection.Right;
                            entryDir = MovementDirection.Left;
                            break;
                        case FacingDirection.Up:
                            exitDir = MovementDirection.Up;
                            entryDir = MovementDirection.Down;
                            break;
                    }

                    bool bridgeEntry = MapTilesetPassable(mapX, mapY) && projectile.OnBridge;
                    if (MapTileProjectilePassable(mapX, mapY, projectile.OnBridge, bridgeEntry, exitDir) &&
                        MapTileProjectilePassable(targetX, targetY, projectile.OnBridge, bridgeEntry, entryDir))
                        canMove = true;
                }

                if (canMove)
                {
                    projectile.UpdateMovement(deltaTime);
                    CheckProjectileHit(projectile);
                    if (GetBridgeFlag(targetX, targetY))
                    {
                        if (MapTilesetPassable(targetX, targetY))
                            projectile.OnBridge = true;
                    }
                    else
                    {
                        projectile.OnBridge = false;
                    }
                }
                else
                {
                    projectile.Destroyed = true;
                }


            }

            projectile.Changed = true;
        }



        private void TurnEnemyTowardsPlayer(MapEnemy mapEnemy, PlayerPacket player)
        {
            if (player.PositionY < mapEnemy.MapY)
                mapEnemy.Direction = FacingDirection.Up;
            else if (player.PositionY > mapEnemy.MapY)
                mapEnemy.Direction = FacingDirection.Down;

            if (player.PositionX < mapEnemy.MapX)
                mapEnemy.Direction = FacingDirection.Left;
            else if (player.PositionX > mapEnemy.MapX)
                mapEnemy.Direction = FacingDirection.Right;

            mapEnemy.Changed = true;
        }

        private void MoveEnemyTowardsPlayer(MapEnemy mapEnemy, MapPlayer mapPlayer)
        {
            if (mapEnemy.Moving()) return;

            PlayerPacket playerPacket = mapPlayer.GetPlayerPacket();

            MovementDirection direction = MovementDirection.Down;
            MovementDirection entryDirection = MovementDirection.Up;
            int targetX = mapEnemy.MapX;
            int targetY = mapEnemy.MapY;
            int xDist = Math.Abs(mapEnemy.MapX - playerPacket.PositionX);
            int yDist = Math.Abs(mapEnemy.MapY - playerPacket.PositionY);

            bool move = false;
            if (xDist > 0 || yDist > 0)
            {
                if (playerPacket.PositionX < mapEnemy.MapX)
                    targetX--;
                else if (playerPacket.PositionX > mapEnemy.MapX)
                    targetX++;
                move = true;
            }

            if ((xDist != 1 && yDist > 0) || yDist > 1)
            {
                if (playerPacket.PositionY < mapEnemy.MapY)
                    targetY--;
                else if (playerPacket.PositionY > mapEnemy.MapY)
                    targetY++;
                move = true;
            }

            if (targetX < mapEnemy.MapX && targetY < mapEnemy.MapY)
            {
                direction = MovementDirection.UpperLeft;
                entryDirection = MovementDirection.LowerRight;
            }
            if (targetX == mapEnemy.MapX && targetY < mapEnemy.MapY)
            {
                direction = MovementDirection.Up;
                entryDirection = MovementDirection.Down;
            }
            if (targetX > mapEnemy.MapX && targetY < mapEnemy.MapY)
            {
                direction = MovementDirection.UpperRight;
                entryDirection = MovementDirection.LowerLeft;
            }
            if (targetX < mapEnemy.MapX && targetY == mapEnemy.MapY)
            {
                direction = MovementDirection.Left;
                entryDirection = MovementDirection.Right;
            }
            if (targetX > mapEnemy.MapX && targetY == mapEnemy.MapY)
            {
                direction = MovementDirection.Right;
                entryDirection = MovementDirection.Left;
            }
            if (targetX < mapEnemy.MapX && targetY > mapEnemy.MapY)
            {
                direction = MovementDirection.LowerLeft;
                entryDirection = MovementDirection.UpperRight;
            }
            if (targetX == mapEnemy.MapX && targetY > mapEnemy.MapY)
            {
                direction = MovementDirection.Down;
                entryDirection = MovementDirection.Up;
            }
            if (targetX > mapEnemy.MapX && targetY > mapEnemy.MapY)
            {
                direction = MovementDirection.LowerRight;
                entryDirection = MovementDirection.UpperRight;
            }

            if (move)
            {
                bool bridgeEntry = MapTilesetPassable(mapEnemy.MapX, mapEnemy.MapY) && mapEnemy.OnBridge;
                if (MapTileCharacterPassable(mapEnemy.MapX, mapEnemy.MapY, true, mapEnemy.OnBridge, bridgeEntry, direction) &&
                    MapTileCharacterPassable(targetX, targetY, true, mapEnemy.OnBridge, bridgeEntry, entryDirection))
                {
                    if (mapEnemy.Move(direction))
                    {
                        if (GetBridgeFlag(targetX, targetY))
                        {
                            if (MapTilesetPassable(targetX, targetY))
                                mapEnemy.OnBridge = true;
                        }
                        else
                        {
                            mapEnemy.OnBridge = false;
                        }
                    }
                }

            }
        }

        private void MoveEnemyRandomly(MapEnemy mapEnemy)
        {
            MovementDirection direction = (MovementDirection)_random.Next(0, 7);
            MovementDirection entryDirection = MovementDirection.Down;
            int targetX = mapEnemy.MapX;
            int targetY = mapEnemy.MapY;

            switch (direction)
            {
                case MovementDirection.UpperLeft:
                    targetX -= 1;
                    targetY -= 1;
                    entryDirection = MovementDirection.LowerRight;
                    break;
                case MovementDirection.Up:
                    targetY -= 1;
                    entryDirection = MovementDirection.Down;
                    break;
                case MovementDirection.UpperRight:
                    targetX += 1;
                    targetY -= 1;
                    entryDirection = MovementDirection.LowerLeft;
                    break;
                case MovementDirection.Left:
                    targetX -= 1;
                    entryDirection = MovementDirection.Right;
                    break;
                case MovementDirection.Right:
                    targetX += 1;
                    entryDirection = MovementDirection.Left;
                    break;
                case MovementDirection.LowerLeft:
                    targetX -= 1;
                    targetY += 1;
                    entryDirection = MovementDirection.UpperRight;
                    break;
                case MovementDirection.Down:
                    targetY += 1;
                    entryDirection = MovementDirection.Up;
                    break;
                case MovementDirection.LowerRight:
                    targetX += 1;
                    targetY += 1;
                    entryDirection = MovementDirection.UpperLeft;
                    break;
            }

            bool bridgeEntry = MapTilesetPassable(mapEnemy.MapX, mapEnemy.MapY) && mapEnemy.OnBridge;
            if (MapTileCharacterPassable(mapEnemy.MapX, mapEnemy.MapY, true, mapEnemy.OnBridge, bridgeEntry, direction) &&
                MapTileCharacterPassable(targetX, targetY, true, mapEnemy.OnBridge, bridgeEntry, entryDirection))
            {
                int spawnDistance = (int)new Vector2(mapEnemy.SpawnX - targetX, mapEnemy.SpawnY - targetY).Length;
                if (spawnDistance <= mapEnemy.GetEnemyData().WanderRange)
                {
                    if (mapEnemy.Move(direction))
                    {
                        if (GetBridgeFlag(targetX, targetY))
                        {
                            if (MapTilesetPassable(targetX, targetY))
                                mapEnemy.OnBridge = true;
                        }
                        else
                        {
                            mapEnemy.OnBridge = false;
                        }
                    }
                }
            }
        }

        private bool EnemyAttackCheck(MapEnemy enemy, PlayerPacket player, int distance)
        {
            if (enemy.OnBridge != player.OnBridge || enemy.Moving())
                return false;

            if (enemy.MapX == player.PositionX || enemy.MapY == player.PositionY)
            {
                switch (enemy.GetEnemyData().AtkStyle)
                {
                    case AttackStyle.Melee:
                        if (distance == 1)
                            return true;
                        break;
                    case AttackStyle.Ranged:
                    case AttackStyle.Magic:
                        if (distance <= enemy.GetEnemyData().AttackRange)
                            return true;
                        break;
                }
            }

            return false;
        }

        private void EnemyMeleeAttack(MapEnemy enemy, MapPlayer mapPlayer)
        {
            if (enemy.AttackTimer > 0) return;

            PlayerPacket playerPacket = mapPlayer.GetPlayerPacket();
            Random rand = new Random();
            CombatStats stats1 = enemy.GetEnemyData().BaseStats;
            CombatStats stats2 = playerPacket.Data.GetCombinedCombatStats();
            int critModifier = rand.Next(1, 6) > 4 ? (int)(stats1.Strength * 0.2f) : 0;
            double maxAccuracy = 1;// Math.Min(stats1.Agility / stats1.Strength, 1.0);
            double accuracy = rand.NextDouble() * maxAccuracy;
            int meleePower = (int)((stats1.Strength + critModifier) * accuracy) - (stats2.MeleeDefence / 2);
            meleePower = Math.Max(meleePower, 0);
            mapPlayer.TakeDamage(CharacterType.Enemy, _mapPacket.Enemies.IndexOf(enemy), meleePower, _mapPacket.MapData.MultiCombat);
            enemy.AttackTimer = Math.Max((1 / stats1.Agility) - 1.0f, 0.1f) * 10;
        }

        private void EnemyRangeAttack(MapEnemy enemy, MapPlayer mapPlayer)
        {
            if (enemy.AttackTimer > 0) return;

            Hitbox hitbox = enemy.GetHitbox();
            Vector2 position = new Vector2(hitbox.X, hitbox.Y);
            int projectileID = enemy.GetEnemyData().ProjectileID;
            int enemyID = _mapPacket.Enemies.IndexOf(enemy);
            MapProjectile projectile = new MapProjectile(projectileID, CharacterType.Enemy, enemyID, position, enemy.Direction);
            //client.TakeDamage(CharacterType.Enemy, enemyID, 0);
            projectile.OnBridge = enemy.OnBridge;
            projectile.TargetType = CharacterType.Player;
            projectile.TargetID = mapPlayer.PlayerID;
            projectile.Style = AttackStyle.Ranged;

            Random rand = new Random();
            CombatStats stats = enemy.GetEnemyData().BaseStats;
            int critModifier = rand.Next(1, 6) > 4 ? (int)(stats.Strength * 0.2f) : 0;
            double maxAccuracy = 1;// Math.Min(stats.Agility / stats.Strength, 1.0);
            double accuracy = rand.NextDouble() * maxAccuracy;
            int rangePower = (int)((stats.Strength + critModifier) * accuracy);
            projectile.AttackPower = rangePower;

            AddMapProjectile(projectile);
            enemy.AttackTimer = Math.Max((1 / stats.Agility) - 1.0f, 0.1f) * 10;
        }

        private void EnemyMagicAttack(MapEnemy enemy, MapPlayer mapPlayer)
        {
            if (enemy.AttackTimer > 0) return;

            Hitbox hitbox = enemy.GetHitbox();
            Vector2 position = new Vector2(hitbox.X, hitbox.Y);
            int projectileID = enemy.GetEnemyData().ProjectileID;
            int enemyID = _mapPacket.Enemies.IndexOf(enemy);
            MapProjectile projectile = new MapProjectile(projectileID, CharacterType.Enemy, enemyID, position, enemy.Direction);
            //client.TakeDamage(CharacterType.Enemy, enemyID, 0);
            projectile.OnBridge = enemy.OnBridge;
            projectile.TargetType = CharacterType.Player;
            projectile.TargetID = mapPlayer.PlayerID;
            projectile.Style = AttackStyle.Magic;

            Random rand = new Random();
            CombatStats stats = enemy.GetEnemyData().BaseStats;
            int critModifier = rand.Next(1, 6) > 4 ? (int)(stats.Inteligence * 0.2f) : 0;
            double maxAccuracy = 1;// Math.Min(stats.Agility / stats.Inteligence, 1.0);
            double accuracy = rand.NextDouble() * maxAccuracy;
            int magicPower = (int)((stats.Inteligence + critModifier) * accuracy);
            projectile.AttackPower = magicPower;

            AddMapProjectile(projectile);
            enemy.AttackTimer = Math.Max((1 / stats.Agility) - 1.0f, 0.1f) * 10;
        }

        private void CheckProjectileHit(MapProjectile projectile)
        {
            Hitbox hitBox = projectile.GetHitBox();
            if (projectile.Direction == FacingDirection.Left || projectile.Direction == FacingDirection.Right)
            {
                float width = hitBox.Width;
                hitBox.Width = hitBox.Height;
                hitBox.Height = width;
            }


            if ((GetMapData().PvpEnabled && projectile.ParentType == CharacterType.Player) || projectile.ParentType == CharacterType.Enemy)
            {
                if (projectile.TargetID == -1 || projectile.TargetType == CharacterType.Player)
                {
                    List<MapPlayer> players = GetMapPlayers();
                    foreach (MapPlayer mapPlayer in players)
                    {
                        //if not this players projectile
                        if (!(projectile.ParentType == CharacterType.Player && projectile.CharacterID == mapPlayer.GetPlayerPacket().PlayerID))
                        {
                            if ((projectile.TargetID == -1 || projectile.TargetID == mapPlayer.GetPlayerPacket().PlayerID) &&
                                mapPlayer.EnemyCanAttack(projectile.ParentType, projectile.CharacterID, _mapPacket.MapData.MultiCombat))
                            {
                                Hitbox playerHitbox = mapPlayer.GetHitbox();
                                if (hitBox.Intersects(playerHitbox))
                                {
                                    CombatStats stats = mapPlayer.GetPlayerPacket().Data.GetCombinedCombatStats();
                                    int defence = projectile.Style == AttackStyle.Ranged ? stats.RangeDefence : stats.MagicDefence;
                                    int attackPower = projectile.AttackPower - (defence / 2);
                                    attackPower = Math.Max(attackPower, 0);
                                    mapPlayer.TakeDamage(projectile.ParentType, projectile.CharacterID, attackPower, _mapPacket.MapData.MultiCombat);
                                    projectile.Destroyed = true;
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            if (!projectile.Destroyed)
            {
                //it is actually possible for enemy projectiles to target other enemies with this code - if ever needed? just need to add logic to enemy targeting
                if ((projectile.TargetID == -1 || projectile.TargetType == CharacterType.Enemy))
                {
                    for (int i = 0; i < _mapPacket.Enemies.Count; i++)
                    {
                        MapEnemy enemy = _mapPacket.Enemies[i];
                        //if not this enemies projectile
                        if (!(projectile.ParentType == CharacterType.Enemy && projectile.CharacterID == i))
                        {
                            MapPlayer mapPlayer = null;
                            if (projectile.ParentType == CharacterType.Player)
                            {
                                mapPlayer = FindMapPlayer(projectile.CharacterID);
                                if (mapPlayer == null || !mapPlayer.EnemyCanAttack(CharacterType.Enemy, i, _mapPacket.MapData.MultiCombat))
                                    continue;
                            }

                            if ((projectile.TargetID == -1 || projectile.TargetID == i) &&
                                enemy.EnemyCanAttack(projectile.ParentType, projectile.CharacterID, _mapPacket.MapData.MultiCombat))
                            {
                                Hitbox enemyHitBox = enemy.GetHitbox();
                                if (hitBox.Intersects(enemyHitBox))
                                {
                                    CombatStats stats = enemy.GetEnemyData().BaseStats;
                                    int defence = projectile.Style == AttackStyle.Ranged ? stats.RangeDefence : stats.MagicDefence;
                                    int attackPower = projectile.AttackPower - (defence / 2);
                                    attackPower = Math.Max(attackPower, 0);
                                    enemy.TakeDamage(projectile.ParentType, projectile.CharacterID, attackPower, _mapPacket.MapData.MultiCombat);
                                    projectile.Destroyed = true;

                                    if (enemy.Dead)
                                    {
                                        if (mapPlayer != null)
                                            mapPlayer.GainExperience(enemy.GetEnemyData().Experience);
                                        DropItem(enemy, mapPlayer);
                                    }

                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }

        public void DropItem(MapEnemy enemy, MapPlayer mapPlayer)
        {
            DropTableData dropTable = DropTableData.GetDropTable(enemy.GetEnemyData().DropTable);
            if (dropTable != null)
            {
                Random random = new Random();
                int chance = 100 - random.Next(0, 99);
                for (int k = 0; k < dropTable.TableItems.Count; k++)
                {
                    DropTableData.DropTableItem item = dropTable.TableItems[k];
                    if (item.ItemID >= 0 && item.ItemID < ItemData.GetItemDataCount())
                    {
                        if (item.Chance >= chance)
                        {
                            int playerID = mapPlayer == null ? -1 : mapPlayer.PlayerID;
                            MapItem mapItem = new MapItem(item.ItemID, item.ItemCount, enemy.MapX, enemy.MapY, playerID, enemy.OnBridge);
                            AddMapItem(mapItem);
                        }
                    }
                }
            }
        }
    }
}

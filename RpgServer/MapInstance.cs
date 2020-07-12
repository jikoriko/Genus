using System;
using System.Collections.Generic;
using System.Linq;

using Genus2D.GameData;
using Genus2D.Networking;
using OpenTK;

namespace RpgServer
{
    public class MapInstance
    {
        private Server _server;
        private int _mapID;
        private MapData _mapData;

        private MapPacket _mapPacket;
        private List<GameClient> _clients;
        private List<GameClient> _clientsToAdd;
        private List<GameClient> _clientsToRemove;

        private EventInterpreter _eventInterpreter;
        private List<Dictionary<MapEnemy, float>> _enemyTrackers;
        private float _updateMapTimer;
        private Random _random = new Random();

        public MapInstance(Server server, int mapID)
        {
            _server = server;
            _mapID = mapID;
            _mapData = MapInfo.LoadMap(mapID);

            _mapPacket = new MapPacket(_mapID, _mapData);

            _clients = new List<GameClient>();
            _clientsToAdd = new List<GameClient>();
            _clientsToRemove = new List<GameClient>();

            _eventInterpreter = new EventInterpreter(this);
            _enemyTrackers = new List<Dictionary<MapEnemy, float>>();
            for (int i = 0; i < _mapData.MapEventsCount(); i++)
            {
                _enemyTrackers.Add(new Dictionary<MapEnemy, float>());
            }

            _updateMapTimer = 0.0f;
        }

        public Server GetServer()
        {
            return _server;
        }

        public void AddClient(GameClient client)
        {
            if (!_clients.Contains(client) && !_clientsToAdd.Contains(client))
            {
                _clientsToAdd.Add(client);
                client.SetMapInstance(this);
            }
        }

        public void RemoveClient(GameClient client)
        {
            if (_clients.Contains(client) && !_clientsToRemove.Contains(client))
            {
                _clientsToRemove.Add(client);
            }
        }

        public int NumClients()
        {
            return _clients.Count;
        }

        public GameClient[] GetClients()
        {
            return _clients.ToArray();
        }

        public GameClient FindGameClient(int playerID)
        {
            for (int i = 0; i < _clients.Count; i++)
            {
                if (_clients[i].GetPacket().PlayerID == playerID)
                    return _clients[i];
            }
            return null;
        }

        public MapEnemy FindMapEnemy(int enemyID)//enemy signature?
        {
            if (enemyID >= 0 && enemyID < _mapPacket.Enemies.Count)
                return _mapPacket.Enemies[enemyID];
            return null;
        }

        public MapPacket GetMapPacket()
        {
            return _mapPacket;
        }

        public MapData GetMapData()
        {
            return _mapData;
        }

        public EventInterpreter GetEventInterpreter()
        {
            return _eventInterpreter;
        }

        public Dictionary<MapEnemy, float> GetEnemyTracker(MapEvent mapEvent)
        {
            int mapEventIndex = -1;
            for (int i = 0; i < _mapData.MapEventsCount(); i++)
            {
                if (_mapData.GetMapEvent(i) == mapEvent)
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

        public bool TileInsideMap(int x, int y)
        {
            if (x < 0 || y < 0 || x >= _mapData.GetWidth() || y >= _mapData.GetHeight())
                return false;
            return true;
        }

        public bool MapTilesetPassable(int x, int y)
        {
            if (!TileInsideMap(x, y))
                return false;

            for (int i = MapData.NUM_LAYERS - 1; i >= 0; i--)
            {
                Tuple<int, int> tileInfo = _mapData.GetTile(i, x, y);
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
                Tuple<int, int> tileInfo = _mapData.GetTile(i, x, y);
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
                MapEvent mapEvent = _mapData.GetMapEvent(tileEvent);
                if (onBridge == mapEvent.OnBridge)
                {
                    return false;
                }
            }

            return true;
        }

        public bool MapTileEventPassable(int x, int y, int eventID, bool checkDirections, bool onBridge, bool bridgeEntry, MovementDirection dir)
        {
            if (!MapTilePassable(x, y, checkDirections, onBridge, bridgeEntry, dir))
                return false;

            MapEvent mapEvent = _mapData.GetMapEvent(eventID);
            if (mapEvent.Passable)
                return true;
            else
            {
                if (TileHasPlayers(x, y, onBridge))
                    return false;

                int tileEvent = TileHasNonePassableEvent(x, y, eventID);
                if (tileEvent != -1)
                {
                    mapEvent = _mapData.GetMapEvent(tileEvent);
                    if (onBridge == mapEvent.OnBridge)
                    {
                        return false;
                    }
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
                MapEvent mapEvent = _mapData.GetMapEvent(tileEvent);
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
            for (int i = 0; i < _mapData.MapEventsCount(); i++)
            {
                if (i == ignoreID)
                    continue;
                MapEvent mapEvent = _mapData.GetMapEvent(i);
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
            for (int i = 0; i < _clients.Count; i++)
            {
                PlayerPacket packet = _clients[i].GetPacket();
                if (packet.PositionX == mapX && packet.PositionY == mapY && packet.OnBridge == onBridge)
                    return true;
            }
            return false;
        }

        public bool GetBridgeFlag(int mapX, int mapY)
        {
            for (int i = MapData.NUM_LAYERS - 1; i >= 0; i--)
            {
                Tuple<int, int> tileInfo = _mapData.GetTile(i, mapX, mapY);
                if (tileInfo.Item2 == -1)
                    continue;

                if (TilesetData.GetTileset(tileInfo.Item2).GetBridgeFlag(tileInfo.Item1))
                    return true;
            }
            return false;
        }

        public bool ChangeMapEvent(int eventID, EventCommand command)
        {
            ChangeMapEventProperty property = (ChangeMapEventProperty)command.GetParameter("Property");
            switch (property)
            {
                case ChangeMapEventProperty.Teleport:
                    if (GetMapData().GetMapEvent(eventID).Moving())
                    {
                        return false;
                    }
                    int mapX = (int)command.GetParameter("MapX");
                    int mapY = (int)command.GetParameter("MapY");
                    if (!TeleportMapEvent(eventID, mapX, mapY))
                        return false;
                    break;
                case ChangeMapEventProperty.Move:
                    MovementDirection movementDirection = (MovementDirection)command.GetParameter("MovementDirection");
                    if (!MoveMapEvent(eventID, movementDirection))
                        return false;
                    break;
                case ChangeMapEventProperty.Direction:
                    if (GetMapData().GetMapEvent(eventID).Moving())
                    {
                        return false;
                    }
                    FacingDirection facingDirection = (FacingDirection)command.GetParameter("FacingDirection");
                    ChangeMapEventDirection(eventID, facingDirection);
                    break;
                case ChangeMapEventProperty.Sprite:
                    int spriteID = (int)command.GetParameter("SpriteID");
                    ChangeMapEventSprite(eventID, spriteID);
                    break;
                case ChangeMapEventProperty.RenderPriority:
                    RenderPriority priority = (RenderPriority)command.GetParameter("RenderPriority");
                    ChangeMapEventRenderPriority(eventID, priority);
                    break;
                case ChangeMapEventProperty.MovementSpeed:
                    MovementSpeed speed = (MovementSpeed)command.GetParameter("MovementSpeed");
                    GetMapData().GetMapEvent(eventID).Speed = speed;
                    break;
                case ChangeMapEventProperty.MovementFrequency:
                    MovementFrequency frequency = (MovementFrequency)command.GetParameter("MovementFrequency");
                    GetMapData().GetMapEvent(eventID).Frequency = frequency;
                    break;
                case ChangeMapEventProperty.Passable:
                    bool passable = (bool)command.GetParameter("Passable");
                    GetMapData().GetMapEvent(eventID).Passable = passable;
                    break;
                case ChangeMapEventProperty.RandomMovement:
                    bool randomMovement = (bool)command.GetParameter("RandomMovement");
                    GetMapData().GetMapEvent(eventID).RandomMovement = randomMovement;
                    break;
                case ChangeMapEventProperty.Enabled:
                    bool enabled = (bool)command.GetParameter("Enabled");
                    ChangeMapEventEnabled(eventID, enabled);
                    break;
            }
            return true;
        }

        public bool TeleportMapEvent(int eventID, int mapX, int mapY)
        {
            if (MapTileEventPassable(mapX, mapY, eventID, false, false, false, MovementDirection.Down))
            {
                MapEvent mapEvent = GetMapData().GetMapEvent(eventID);
                mapEvent.MapX = mapX;
                mapEvent.MapY = mapY;
                mapEvent.RealX = mapX * 32;
                mapEvent.RealY = mapY * 32;
                UpdateMapEventOnClients(eventID);
                return true;
            }
            return false;
        }

        public bool MoveMapEvent(int eventID, MovementDirection direction)
        {
            MapEvent mapEvent = GetMapData().GetMapEvent(eventID);
            if (mapEvent.Moving() || !mapEvent.Enabled)
                return false;
            int x = mapEvent.MapX;
            int y = mapEvent.MapY;
            int targetX = x;
            int targetY = y;
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

            bool bridgeEntry = MapTilesetPassable(x, y) && mapEvent.OnBridge;
            if (MapTileEventPassable(x, y, eventID, true, mapEvent.OnBridge, bridgeEntry, direction) &&
                MapTileEventPassable(targetX, targetY, eventID, true, mapEvent.OnBridge, bridgeEntry, entryDirection))
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
                    UpdateMapEventOnClients(eventID);
                    return true;
                }
            }
            return false;
        }

        private void ChangeMapEventDirection(int eventID, FacingDirection direction)
        {
            if (GetMapData().GetMapEvent(eventID).EventDirection != direction)
            {
                GetMapData().GetMapEvent(eventID).EventDirection = direction;
                ServerCommand serverCommand = new ServerCommand(ServerCommand.CommandType.ChangeMapEventDirection);
                serverCommand.SetParameter("EventID", eventID);
                serverCommand.SetParameter("MapID", _mapID);
                serverCommand.SetParameter("Direction", (int)direction);
                for (int i = 0; i < _clients.Count; i++)
                {
                    _clients[i].AddServerCommand(serverCommand);
                }
            }
        }

        private void ChangeMapEventSprite(int eventID, int spriteID)
        {
            if (GetMapData().GetMapEvent(eventID).SpriteID != spriteID)
            {
                GetMapData().GetMapEvent(eventID).SpriteID = spriteID;
                ServerCommand serverCommand = new ServerCommand(ServerCommand.CommandType.ChangeMapEventSprite);
                serverCommand.SetParameter("EventID", eventID);
                serverCommand.SetParameter("MapID", _mapID);
                serverCommand.SetParameter("SpriteID", spriteID);
                for (int i = 0; i < _clients.Count; i++)
                {
                    _clients[i].AddServerCommand(serverCommand);
                }
            }
        }

        private void ChangeMapEventRenderPriority(int eventID, RenderPriority priority)
        {
            if (GetMapData().GetMapEvent(eventID).Priority != priority)
            {
                GetMapData().GetMapEvent(eventID).Priority = priority;
                ServerCommand serverCommand = new ServerCommand(ServerCommand.CommandType.ChangeMapEventRenderPriority);
                serverCommand.SetParameter("EventID", eventID);
                serverCommand.SetParameter("MapID", _mapID);
                serverCommand.SetParameter("RenderPriority", (int)priority);
                for (int i = 0; i < _clients.Count; i++)
                {
                    _clients[i].AddServerCommand(serverCommand);
                }
            }
        }

        private void ChangeMapEventEnabled(int eventID, bool enabled)
        {
            if (GetMapData().GetMapEvent(eventID).Enabled != enabled)
            {
                GetMapData().GetMapEvent(eventID).Enabled = enabled;
                ServerCommand serverCommand = new ServerCommand(ServerCommand.CommandType.ChangeMapEventEnabled);
                serverCommand.SetParameter("EventID", eventID);
                serverCommand.SetParameter("MapID", _mapID);
                serverCommand.SetParameter("Enabled", enabled);
                for (int i = 0; i < _clients.Count; i++)
                {
                    _clients[i].AddServerCommand(serverCommand);
                }
            }
        }

        public void AddMapEnemy(MapEnemy mapEnemy)
        {
            _mapPacket.Enemies.Add(mapEnemy);
            ServerCommand serverCommand = new ServerCommand(ServerCommand.CommandType.AddMapEnemy);
            serverCommand.SetParameter("EnemyID", mapEnemy.GetEnemyID());
            serverCommand.SetParameter("MapID", _mapID);
            serverCommand.SetParameter("MapX", mapEnemy.MapX);
            serverCommand.SetParameter("MapY", mapEnemy.MapY);
            serverCommand.SetParameter("OnBridge", mapEnemy.OnBridge);

            for (int i = 0; i < _clients.Count; i++)
            {
                _clients[i].AddServerCommand(serverCommand);
            }
        }

        public void UpdateMapEnemyOnClient(int index)
        {
            MapEnemy mapEnemy = _mapPacket.Enemies[index];
            ServerCommand serverCommand = new ServerCommand(ServerCommand.CommandType.UpdateMapEnemy);
            serverCommand.SetParameter("EnemyIndex", index);
            serverCommand.SetParameter("MapID", _mapID);
            serverCommand.SetParameter("HP", mapEnemy.HP);
            serverCommand.SetParameter("MapX", mapEnemy.MapX);
            serverCommand.SetParameter("MapY", mapEnemy.MapY);
            serverCommand.SetParameter("RealX", mapEnemy.RealX);
            serverCommand.SetParameter("RealY", mapEnemy.RealY);
            serverCommand.SetParameter("Direction", (int)mapEnemy.Direction);
            serverCommand.SetParameter("OnBridge", mapEnemy.OnBridge);
            serverCommand.SetParameter("Dead", mapEnemy.Dead);

            for (int i = 0; i < _clients.Count; i++)
            {
                _clients[i].AddServerCommand(serverCommand);
            }
        }

        public void UpdateMapEventOnClients(int eventID)
        {
            ServerCommand serverCommand = new ServerCommand(ServerCommand.CommandType.UpdateMapEvent);
            serverCommand.SetParameter("EventID", eventID);
            serverCommand.SetParameter("MapID", _mapID);
            MapEvent mapEvent = GetMapData().GetMapEvent(eventID);
            serverCommand.SetParameter("MapX", mapEvent.MapX);
            serverCommand.SetParameter("MapY", mapEvent.MapY);
            serverCommand.SetParameter("RealX", mapEvent.RealX);
            serverCommand.SetParameter("RealY", mapEvent.RealY);
            serverCommand.SetParameter("Direction", (int)mapEvent.EventDirection);
            serverCommand.SetParameter("OnBridge", mapEvent.OnBridge);
            for (int i = 0; i < _clients.Count; i++)
            {
                _clients[i].AddServerCommand(serverCommand);
            }
        }

        public void AddProjectile(Projectile projectile)
        {
            _mapPacket.Projectiles.Add(projectile);

            ServerCommand command = new ServerCommand(ServerCommand.CommandType.AddProjectile);
            command.SetParameter("MapID", _mapID);
            command.SetParameter("DataID", projectile.ProjectileID);
            command.SetParameter("RealX", projectile.Position.X);
            command.SetParameter("RealY", projectile.Position.Y);
            command.SetParameter("VelocityX", projectile.Velocity.X);
            command.SetParameter("VelocityY", projectile.Velocity.Y);
            command.SetParameter("Direction", (int)projectile.Direction);
            command.SetParameter("OnBridge", projectile.OnBridge);

            for (int i = 0; i < _clients.Count; i++)
            {
                _clients[i].AddServerCommand(command);
            }
        }

        public void UpdateProjectileOnClients(int projectileID)
        {
            ServerCommand serverCommand = new ServerCommand(ServerCommand.CommandType.UpdateProjectile);
            Projectile projectile = _mapPacket.Projectiles[projectileID];
            serverCommand.SetParameter("MapID", _mapID);
            serverCommand.SetParameter("ProjectileID", projectileID);
            serverCommand.SetParameter("RealX", projectile.Position.X);
            serverCommand.SetParameter("RealY", projectile.Position.Y);
            serverCommand.SetParameter("OnBridge", projectile.OnBridge);
            serverCommand.SetParameter("Destroyed", projectile.Destroyed);
            for (int i = 0; i < _clients.Count; i++)
            {
                _clients[i].AddServerCommand(serverCommand);
            }
        }

        public MapItem GetMapItem(int index)
        {
            if (index >= 0 && index < _mapPacket.Items.Count)
                return _mapPacket.Items[index];

            return null;
        }

        public void AddMapItem(MapItem mapItem)
        {
            _mapPacket.Items.Add(mapItem);
            ServerCommand command = new ServerCommand(ServerCommand.CommandType.AddMapItem);
            command.SetParameter("MapID", _mapPacket.MapID);
            command.SetParameter("ItemID", mapItem.ItemID);
            command.SetParameter("Count", mapItem.Count);
            command.SetParameter("MapX", mapItem.MapX);
            command.SetParameter("MapY", mapItem.MapY);
            command.SetParameter("PlayerID", mapItem.PlayerID);
            command.SetParameter("OnBridge", mapItem.OnBridge);
            for (int i = 0; i < _clients.Count; i++)
            {
                _clients[i].AddServerCommand(command);
            }
        }

        public void RemoveMapItem(int index)
        {
            if (index >= 0 && index < _mapPacket.Items.Count)
            {
                _mapPacket.Items.RemoveAt(index);
                ServerCommand command = new ServerCommand(ServerCommand.CommandType.RemoveMapItem);
                command.SetParameter("MapID", _mapPacket.MapID);
                command.SetParameter("ItemIndex", index);
                for (int i = 0; i < _clients.Count; i++)
                {
                    _clients[i].AddServerCommand(command);
                }
            }
        }

        public void UpdateMapItem(int index)
        {
            if (index >= 0 && index < _mapPacket.Items.Count)
            {
                ServerCommand command = new ServerCommand(ServerCommand.CommandType.UpdateMapItem);
                command.SetParameter("MapID", _mapPacket.MapID);
                command.SetParameter("ItemIndex", index);
                command.SetParameter("PlayerID", _mapPacket.Items[index].PlayerID);
                command.SetParameter("Count", _mapPacket.Items[index].Count);
                for (int i = 0; i < _clients.Count; i++)
                {
                    _clients[i].AddServerCommand(command);
                }
            }
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

        public void Update(float deltaTime)
        {
            //add clients
            while (_clientsToAdd.Count > 0)
            {
                GameClient client = _clientsToAdd[0];
                if (client != null)
                {
                    _clients.Add(client);
                    _clientsToAdd.RemoveAt(0);
                }
            }

            //remove clients
            while (_clientsToRemove.Count > 0)
            {
                GameClient client = _clientsToRemove[0];
                if (client != null)
                {
                    _clients.Remove(client);
                    _clientsToRemove.RemoveAt(0);
                }
            }

            //update interpreter
            _eventInterpreter.Update(deltaTime);

            //update clients
            for (int i = 0; i < _clients.Count; i++)
            {
                _clients[i].Update(deltaTime);
                if (!_clients[i].Connected())
                    RemoveClient(_clients[i]);
            }

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

                UpdateMapEnemy(mapEnemy);
                mapEnemy.Update(deltaTime);

                //update enemy packet
                if (_updateMapTimer <= 0)
                {
                    UpdateMapEnemyOnClient(i);
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
                    _eventInterpreter.TriggerEventData(null, mapEvent);
                }

                mapEvent.UpdateMovement(deltaTime);

                if (_updateMapTimer <= 0f && mapEvent.Moved)
                {
                    UpdateMapEventOnClients(i);
                    mapEvent.Moved = false;
                }
            }

            //update projectiles
            for (int i = 0; i < _mapPacket.Projectiles.Count; i++)
            {
                Projectile projectile = _mapPacket.Projectiles[i];
                if (_updateMapTimer <= 0f || projectile.Destroyed)
                {
                    UpdateProjectileOnClients(i);
                }

                if (!projectile.Destroyed)
                {
                    UpdateProjectile(projectile, deltaTime);
                }
                else
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
                            UpdateMapItem(i);
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
            }

            //refresh packet timer
            if (_updateMapTimer <= 0)
            {
                _updateMapTimer = 0.05f;
            }

        }

        private void UpdateProjectile(Projectile projectile, float deltaTime)
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
                    projectile.Position = targetPos;
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
        }

        private void UpdateMapEnemy(MapEnemy mapEnemy)
        {
            EnemyData enemyData = mapEnemy.GetEnemyData();
            if (mapEnemy.TargetPlayerID == -1)
            {
                //find player
                for (int i = 0; i < _clients.Count; i++)
                {
                    if (_clients[i].EnemyCanAttack(CharacterType.Enemy, _mapPacket.Enemies.IndexOf(mapEnemy)))
                    {
                        PlayerPacket player = _clients[i].GetPacket();

                        if (player.Data.Level <= enemyData.AgroLvl)
                        {
                            int xDistance = player.PositionX - mapEnemy.MapX;
                            int yDistance = player.PositionY - mapEnemy.MapY;
                            int sxDistance = player.PositionX - mapEnemy.SpawnX;
                            int syDistance = player.PositionY - mapEnemy.SpawnY;
                            int distance = (int)new Vector2(xDistance, yDistance).Length;
                            int spawnDistance = (int)new Vector2(sxDistance, syDistance).Length;
                            if (distance <= enemyData.VisionRage && spawnDistance <= enemyData.WanderRange + 1)
                            {
                                mapEnemy.TargetPlayerID = _clients[i].GetPacket().PlayerID;
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
                GameClient client = null;
                PlayerPacket player = null;
                for (int i = 0; i < _clients.Count; i++)
                {
                    if (_clients[i].EnemyCanAttack(CharacterType.Enemy, _mapPacket.Enemies.IndexOf(mapEnemy)))
                    {
                        client = _clients[i];
                        player = client.GetPacket();

                        if (player.PlayerID == mapEnemy.TargetPlayerID)
                        {
                            int xDistance = player.PositionX - mapEnemy.MapX;
                            int yDistance = player.PositionY - mapEnemy.MapY;
                            int sxDistance = player.PositionX - mapEnemy.SpawnX;
                            int syDistance = player.PositionY - mapEnemy.SpawnY;
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
                        if (EnemyAttackCheck(mapEnemy, client.GetPacket(), distance))
                        {
                            TurnEnemyTowardsPlayer(mapEnemy, client.GetPacket());

                            switch (mapEnemy.GetEnemyData().AtkStyle)
                            {
                                case AttackStyle.Melee:
                                    EnemyMeleeAttack(mapEnemy, client);
                                    break;
                                case AttackStyle.Ranged:
                                    EnemyRangeAttack(mapEnemy, player);
                                    break;
                                case AttackStyle.Magic:
                                    EnemyMagicAttack(mapEnemy, player);
                                    break;
                            }

                        }
                        else
                        {
                            MoveEnemyTowardsPlayer(mapEnemy, player);
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
        }

        private void MoveEnemyTowardsPlayer(MapEnemy mapEnemy, PlayerPacket player)
        {
            if (mapEnemy.Moving()) return;

            MovementDirection direction = MovementDirection.Down;
            MovementDirection entryDirection = MovementDirection.Up;
            int targetX = mapEnemy.MapX;
            int targetY = mapEnemy.MapY;
            int xDist = Math.Abs(mapEnemy.MapX - player.PositionX);
            int yDist = Math.Abs(mapEnemy.MapY - player.PositionY);

            bool move = false;
            if (xDist > 0 || yDist > 0)
            {
                if (player.PositionX < mapEnemy.MapX)
                    targetX--;
                else if (player.PositionX > mapEnemy.MapX)
                    targetX++;
                move = true;
            }

            if ((xDist != 1 && yDist > 0) || yDist > 1)
            {
                if (player.PositionY < mapEnemy.MapY)
                    targetY--;
                else if (player.PositionY > mapEnemy.MapY)
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

        private void EnemyMeleeAttack(MapEnemy enemy, GameClient client)
        {
            if (enemy.AttackTimer > 0) return;

            PlayerPacket player = client.GetPacket();
            Random rand = new Random();
            CombatStats stats1 = enemy.GetEnemyData().BaseStats;
            CombatStats stats2 = player.Data.GetCombinedCombatStats();
            int critModifier = rand.Next(1, 6) > 4 ? (int)(stats1.Strength * 0.2f) : 0;
            double maxAccuracy = 1;// Math.Min(stats1.Agility / stats1.Strength, 1.0);
            double accuracy = rand.NextDouble() * maxAccuracy;
            int meleePower = (int)((stats1.Strength + critModifier) * accuracy) - (stats2.MeleeDefence / 2);
            meleePower = Math.Max(meleePower, 0);
            client.TakeDamage(CharacterType.Enemy, _mapPacket.Enemies.IndexOf(enemy), meleePower);
            enemy.AttackTimer = Math.Max((1 / stats1.Agility) - 1.0f, 0.1f) * 10;
        }

        private void EnemyRangeAttack(MapEnemy enemy, PlayerPacket player)
        {
            if (enemy.AttackTimer > 0) return;

            Hitbox hitbox = enemy.GetHitbox();
            Vector2 position = new Vector2(hitbox.X, hitbox.Y);
            int projectileID = enemy.GetEnemyData().ProjectileID;
            int enemyID = _mapPacket.Enemies.IndexOf(enemy);
            Projectile projectile = new Projectile(projectileID, CharacterType.Enemy, enemyID, position, enemy.Direction);
            projectile.OnBridge = enemy.OnBridge;
            projectile.TargetType = CharacterType.Player;
            projectile.TargetID = player.PlayerID;
            projectile.Style = AttackStyle.Ranged;

            Random rand = new Random();
            CombatStats stats = enemy.GetEnemyData().BaseStats;
            int critModifier = rand.Next(1, 6) > 4 ? (int)(stats.Strength * 0.2f) : 0;
            double maxAccuracy = 1;// Math.Min(stats.Agility / stats.Strength, 1.0);
            double accuracy = rand.NextDouble() * maxAccuracy;
            int rangePower = (int)((stats.Strength + critModifier) * accuracy);
            projectile.AttackPower = rangePower;

            AddProjectile(projectile);
            enemy.AttackTimer = Math.Max((1 / stats.Agility) - 1.0f, 0.1f) * 10;
        }

        private void EnemyMagicAttack(MapEnemy enemy, PlayerPacket player)
        {
            if (enemy.AttackTimer > 0) return;

            Hitbox hitbox = enemy.GetHitbox();
            Vector2 position = new Vector2(hitbox.X, hitbox.Y);
            int projectileID = enemy.GetEnemyData().ProjectileID;
            int enemyID = _mapPacket.Enemies.IndexOf(enemy);
            Projectile projectile = new Projectile(projectileID, CharacterType.Enemy, enemyID, position, enemy.Direction);
            projectile.OnBridge = enemy.OnBridge;
            projectile.TargetType = CharacterType.Player;
            projectile.TargetID = player.PlayerID;
            projectile.Style = AttackStyle.Magic;

            Random rand = new Random();
            CombatStats stats = enemy.GetEnemyData().BaseStats;
            int critModifier = rand.Next(1, 6) > 4 ? (int)(stats.Inteligence * 0.2f) : 0;
            double maxAccuracy = 1;// Math.Min(stats.Agility / stats.Inteligence, 1.0);
            double accuracy = rand.NextDouble() * maxAccuracy;
            int magicPower = (int)((stats.Inteligence + critModifier) * accuracy);
            projectile.AttackPower = magicPower;

            AddProjectile(projectile);
            enemy.AttackTimer = Math.Max((1 / stats.Agility) - 1.0f, 0.1f) * 10;
        }

        private void CheckProjectileHit(Projectile projectile)
        {
            GameClient[] clients = GetClients();
            Hitbox hitBox = projectile.GetHitBox();
            if (projectile.Direction == FacingDirection.Left || projectile.Direction == FacingDirection.Right)
            {
                float width = hitBox.Width;
                hitBox.Width = hitBox.Height;
                hitBox.Height = width;
            }

            if (projectile.TargetID == -1 || projectile.TargetType == CharacterType.Player)
            {
                foreach (GameClient client in clients)
                {
                    if (!(projectile.ParentType == CharacterType.Player && projectile.CharacterID == client.GetPacket().PlayerID))
                    {
                        if (projectile.TargetID == -1 || projectile.TargetID == client.GetPacket().PlayerID)
                        {
                            Hitbox playerHitbox = client.GetHitbox();
                            if (hitBox.Intersects(playerHitbox))
                            {
                                CombatStats stats = client.GetPacket().Data.GetCombinedCombatStats();
                                int defence = projectile.Style == AttackStyle.Ranged ? stats.RangeDefence : stats.MagicDefence;
                                int attackPower = projectile.AttackPower - (defence / 2);
                                attackPower = Math.Max(attackPower, 0);
                                client.TakeDamage(projectile.ParentType, projectile.CharacterID, attackPower);
                                projectile.Destroyed = true;
                                break;
                            }
                        }
                    }
                }
            }

            //it is actually possible for enemy projectiles to target other enemies with this code - if ever needed?
            if (projectile.TargetID == -1 || projectile.TargetType == CharacterType.Enemy)
            {
                for(int i = 0; i < _mapPacket.Enemies.Count; i++)
                {
                    MapEnemy enemy = _mapPacket.Enemies[i];
                    if (!(projectile.ParentType == CharacterType.Enemy && projectile.CharacterID == i))
                    {
                        if (projectile.TargetID == -1 || projectile.TargetID == i)
                        {
                            Hitbox enemyHitBox = enemy.GetHitbox();
                            if (hitBox.Intersects(enemyHitBox))
                            {
                                CombatStats stats = enemy.GetEnemyData().BaseStats;
                                int defence = projectile.Style == AttackStyle.Ranged ? stats.RangeDefence : stats.MagicDefence;
                                int attackPower = projectile.AttackPower - (defence / 2);
                                attackPower = Math.Max(attackPower, 0);
                                enemy.TakeDamage(projectile.ParentType, projectile.CharacterID, attackPower);
                                projectile.Destroyed = true;

                                if (enemy.Dead && projectile.ParentType == CharacterType.Player)
                                {
                                    GameClient client = Server.Instance.FindClientByID(projectile.CharacterID);
                                    if (client != null)
                                        client.GainExperience(enemy.GetEnemyData().Experience);
                                }

                                break;
                            }
                        }
                    }
                }
            }
        }
        
        public List<PlayerPacket> GetPlayerPackets()
        {
            List<PlayerPacket> packets = new List<PlayerPacket>();
            foreach (GameClient client in _clients)
            {
                if (client != null)
                    packets.Add(client.GetPacket());
            }
            return packets;
        }

    }
}

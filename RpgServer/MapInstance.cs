using System;
using System.Collections.Generic;

using Genus2D.GameData;
using Genus2D.Networking;

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
        private float _updateMapTimer;
        private Random _random = new Random();

        public MapInstance(Server server, int mapID)
        {
            _server = server;
            _mapID = mapID;
            _mapData = MapInfo.LoadMap(mapID);

            _mapPacket = new MapPacket();
            _mapPacket.MapID = _mapID;
            _mapPacket.mapData = _mapData;
            _clients = new List<GameClient>();
            _clientsToAdd = new List<GameClient>();
            _clientsToRemove = new List<GameClient>();

            _eventInterpreter = new EventInterpreter(this);
            _updateMapTimer = 0.06f;
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

        public bool TileInsideMap(int x, int y)
        {
            if (x < 0 || y < 0 || x >= _mapData.GetWidth() || y >= _mapData.GetHeight())
                return false;
            return true;
        }

        public bool MapTilePassable(int x, int y)
        {
            if (!TileInsideMap(x, y))
                return false;

            for (int i = MapData.NUM_LAYERS - 1; i >= 0; i--)
            {
                Tuple<int, int> tileInfo = _mapData.GetTile(i, x, y);
                if (tileInfo.Item2 == -1)
                    continue;
                if (!TilesetData.GetTileset(tileInfo.Item2).GetPassable(tileInfo.Item1))
                {
                    return false;
                }
            }

            return true;
        }

        public bool MapTilePassable(int x, int y, int eventID, bool onBridge, bool bridgeEntry)
        {
            return MapTilePassable(x, y, MovementDirection.Down, eventID, onBridge, bridgeEntry, false);
        }

        public bool MapTilePassable(int x, int y, MovementDirection dir, int eventID, bool onBridge, bool bridgeEntry, bool checkDirections = true, bool checkCharacters = true)
        {
            if (!TileInsideMap(x, y))
                return false;

            if (eventID != -1)
            {
                MapEvent mapEvent = _mapData.GetMapEvent(eventID);
                if (mapEvent.Passable)
                    return true;
            }

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
                        if (bridgeEntry && MapTilePassable(x, y))
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

            if (checkCharacters)
            {
                int tileEvent = TileHasEvent(x, y, eventID);
                if (tileEvent != -1)
                {
                    MapEvent mapEvent = _mapData.GetMapEvent(tileEvent);
                    if (onBridge == mapEvent.OnBridge)
                    {
                        return false;
                    }
                }

                if (eventID != -1)
                {
                    MapEvent mapEvent = _mapData.GetMapEvent(eventID);
                    if (TileHasPlayers(x, y, onBridge) && !mapEvent.Passable)
                        return false;
                }
            }

            return true;
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

        public int TileHasEvent(int mapX, int mapY, int ignoreID)
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
            if (MapTilePassable(mapX, mapY, eventID, false, false))
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

            bool bridgeEntry = MapTilePassable(x, y) && mapEvent.OnBridge;
            if ((MapTilePassable(x, y, direction, eventID, mapEvent.OnBridge, bridgeEntry, true, false) && 
                MapTilePassable(targetX, targetY, entryDirection, eventID, mapEvent.OnBridge, bridgeEntry)))
            {
                if (mapEvent.Move(targetX, targetY))
                {
                    if (GetBridgeFlag(targetX, targetY))
                    {
                        if (MapTilePassable(targetX, targetY))
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
                serverCommand.SetParameter("EventID", eventID.ToString());
                serverCommand.SetParameter("MapID", _mapID.ToString());
                serverCommand.SetParameter("Direction", ((int)direction).ToString());
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
                serverCommand.SetParameter("EventID", eventID.ToString());
                serverCommand.SetParameter("MapID", _mapID.ToString());
                serverCommand.SetParameter("SpriteID", spriteID.ToString());
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
                serverCommand.SetParameter("EventID", eventID.ToString());
                serverCommand.SetParameter("MapID", _mapID.ToString());
                serverCommand.SetParameter("RenderPriority", ((int)priority).ToString());
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
                serverCommand.SetParameter("EventID", eventID.ToString());
                serverCommand.SetParameter("MapID", _mapID.ToString());
                serverCommand.SetParameter("Enabled", (enabled ? 1 : 0).ToString());
                for (int i = 0; i < _clients.Count; i++)
                {
                    _clients[i].AddServerCommand(serverCommand);
                }
            }
        }

        public void UpdateMapEventOnClients(int eventID)
        {
            ServerCommand serverCommand = new ServerCommand(ServerCommand.CommandType.UpdateMapEvent);
            serverCommand.SetParameter("EventID", eventID.ToString());
            serverCommand.SetParameter("MapID", _mapID.ToString());
            MapEvent mapEvent = GetMapData().GetMapEvent(eventID);
            serverCommand.SetParameter("MapX", (mapEvent.MapX).ToString());
            serverCommand.SetParameter("MapY", (mapEvent.MapY).ToString());
            serverCommand.SetParameter("RealX", (mapEvent.RealX).ToString());
            serverCommand.SetParameter("RealY", (mapEvent.RealY).ToString());
            serverCommand.SetParameter("Direction", ((int)mapEvent.EventDirection).ToString());
            serverCommand.SetParameter("OnBridge", (mapEvent.OnBridge ? 1 : 0).ToString());
            for (int i = 0; i < _clients.Count; i++)
            {
                _clients[i].AddServerCommand(serverCommand);
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
            while (_clientsToAdd.Count > 0)
            {
                GameClient client = _clientsToAdd[0];
                if (client != null)
                {
                    _clients.Add(client);
                    _clientsToAdd.RemoveAt(0);
                }
            }

            while (_clientsToRemove.Count > 0)
            {
                GameClient client = _clientsToRemove[0];
                if (client != null)
                {
                    _clients.Remove(client);
                    _clientsToRemove.RemoveAt(0);
                }
            }

            _eventInterpreter.Update(deltaTime);

            for (int i = 0; i < _clients.Count; i++)
            {
                _clients[i].Update(deltaTime);
                if (!_clients[i].Connected())
                    RemoveClient(_clients[i]);
            }

            if (_updateMapTimer > 0f)
            {
                _updateMapTimer -= deltaTime;
            }

            bool movedMapEvent = false;

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
                    movedMapEvent = true;
                    mapEvent.Moved = false;
                }
            }

            if (movedMapEvent && _updateMapTimer <= 0)
            {
                _updateMapTimer = 0.06f;
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

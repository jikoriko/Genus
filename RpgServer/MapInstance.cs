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

            _eventInterpreter = new EventInterpreter();
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

        public bool MapTilePassable(int x, int y, int eventID)
        {
            return MapTilePassable(x, y, MovementDirection.Down, eventID, false);
        }

        public bool MapTilePassable(int x, int y, MovementDirection dir, int eventID, bool checkDirections = true)
        {
            if (x < 0 || y < 0 || x >= _mapData.GetWidth() || y >= _mapData.GetHeight())
                return false;

            for (int i = 0; i < MapData.NUM_LAYERS; i++)
            {
                Tuple<int, int> tileInfo = _mapData.GetTile(i, x, y);
                if (tileInfo.Item2 == -1)
                    continue;

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

            int tileEvent = TileHasEvent(x, y, eventID);
            if (tileEvent != -1)
            {
                MapEvent mapEvent = _mapData.GetMapEvent(tileEvent);
                if (!mapEvent.Passable)
                    return false;
            }

            if (eventID != -1)
            {
                MapEvent mapEvent = _mapData.GetMapEvent(eventID);
                if (TileHasPlayers(x, y) && !mapEvent.Passable)
                    return false;
            }

            return true;
        }

        public int TileHasEvent(int mapX, int mapY, int ignoreID)
        {
            int id = -1;
            for (int i = 0; i < _mapData.MapEventsCount(); i++)
            {
                if (i == ignoreID)
                    continue;
                MapEvent mapEvent = _mapData.GetMapEvent(i);
                if (mapEvent.MapX == mapX && mapEvent.MapY == mapY)
                {
                    id = i;
                }
            }
            return id;
        }

        public bool TileHasPlayers(int mapX, int mapY)
        {
            for (int i = 0; i < _clients.Count; i++)
            {
                PlayerPacket packet = _clients[i].GetPacket();
                if (packet.PositionX == mapX && packet.PositionY == mapY)
                    return true;
            }
            return false;
        }

        public bool TeleportMapEvent(int eventID, int mapX, int mapY)
        {
            if (MapTilePassable(mapX, mapY, eventID))
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

            if (MapTilePassable(x, y, direction, eventID) && MapTilePassable(targetX, targetY, entryDirection, eventID))
            {
                mapEvent.EventDirection = facingDirection;
                mapEvent.MapX = targetX;
                mapEvent.MapY = targetY;
                UpdateMapEventOnClients(eventID);
                return true;
            }
            else
            {
                ChangeMapEventDirection(eventID, facingDirection);
                return false;
            }
        }

        public void ChangeMapEventDirection(int eventID, FacingDirection direction)
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

        public void ChangeMapEventSprite(int eventID, int spriteID)
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
            for (int i = 0; i < _clients.Count; i++)
            {
                _clients[i].AddServerCommand(serverCommand);
            }
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


            for (int i = 0; i < _clients.Count; i++)
            {
                _clients[i].Update(deltaTime);
                if (!_clients[i].Connected())
                    RemoveClient(_clients[i]);
            }

            _eventInterpreter.Update(deltaTime);

            if (_updateMapTimer > 0f)
            {
                _updateMapTimer -= deltaTime;
            }

            for (int i = 0; i < GetMapData().MapEventsCount(); i++)
            {
                MapEvent mapEvent = GetMapData().GetMapEvent(i);
                if (mapEvent.TriggerType == EventTriggerType.Autorun)
                {
                    _eventInterpreter.TriggerEventData(null, mapEvent);
                }

                mapEvent.UpdateMovement(deltaTime);

                if (_updateMapTimer <= 0f && mapEvent.Moved)
                {
                    UpdateMapEventOnClients(i);
                    _updateMapTimer = 0.06f;
                    mapEvent.Moved = false;
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

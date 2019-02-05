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
            Console.WriteLine(client);
            if (!_clients.Contains(client) && !_clientsToAdd.Contains(client))
            {
                _clientsToAdd.Add(client);
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
            if (x < 0 || y < 0 || x >= _mapData.GetWidth() || y >= _mapData.GetHeight())
                return false;

            int tilesetID = _mapData.GetTilesetID();
            for (int i = 0; i < MapData.NUM_LAYERS; i++)
            {
                int tileID = _mapData.GetTileID(i, x, y);
                if (!TilesetData.GetTileset(tilesetID).GetPassable(tileID))
                    return false;
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

        public bool MoveMapEvent(int eventID, Direction direction)
        {
            MapEvent mapEvent = GetMapData().GetMapEvent(eventID);
            int x = direction == Direction.Left ? -1 : direction == Direction.Right ? 1 : 0;
            int y = direction == Direction.Up ? -1 : direction == Direction.Down ? 1 : 0;
            x += mapEvent.MapX;
            y += mapEvent.MapY;

            if (MapTilePassable(x, y, eventID))
            {
                mapEvent.EventDirection = direction;
                mapEvent.MapX = x;
                mapEvent.MapY = y;
                UpdateMapEventOnClients(eventID);
                return true;
            }
            else
            {
                ChangeMapEventDirection(eventID, direction);
                return false;
            }
        }

        public void ChangeMapEventDirection(int eventID, Direction direction)
        {
            if (GetMapData().GetMapEvent(eventID).EventDirection != direction)
            {
                GetMapData().GetMapEvent(eventID).EventDirection = direction;
                ClientCommand clientCommand = new ClientCommand(ClientCommand.CommandType.ChangeMapEventDirection);
                clientCommand.SetParameter("EventID", eventID.ToString());
                clientCommand.SetParameter("MapID", _mapID.ToString());
                clientCommand.SetParameter("Direction", ((int)direction).ToString());
                for (int i = 0; i < _clients.Count; i++)
                {
                    _clients[i].AddClientCommand(clientCommand);
                }
            }
        }

        public void UpdateMapEventOnClients(int eventID)
        {
            ClientCommand clientCommand = new ClientCommand(ClientCommand.CommandType.UpdateMapEvent);
            clientCommand.SetParameter("EventID", eventID.ToString());
            clientCommand.SetParameter("MapID", _mapID.ToString());
            MapEvent mapEvent = GetMapData().GetMapEvent(eventID);
            clientCommand.SetParameter("MapX", (mapEvent.MapX).ToString());
            clientCommand.SetParameter("MapY", (mapEvent.MapY).ToString());
            clientCommand.SetParameter("RealX", (mapEvent.RealX).ToString());
            clientCommand.SetParameter("RealY", (mapEvent.RealY).ToString());
            clientCommand.SetParameter("Direction", ((int)mapEvent.EventDirection).ToString());
            for (int i = 0; i < _clients.Count; i++)
            {
                _clients[i].AddClientCommand(clientCommand);
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
                    client.SetMapInstance(this);
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
                if (_clients[i] == null)
                {
                    continue;
                }
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

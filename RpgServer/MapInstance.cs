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

        private float _updateMapTimer;

        public MapInstance(Server server, int mapID)
        {
            _server = server;
            _mapID = mapID;
            _mapData = MapInfo.LoadMap(mapID);

            _mapPacket = new MapPacket();
            _mapPacket.mapData = _mapData;
            _clients = new List<GameClient>();
            _clientsToAdd = new List<GameClient>();
            _clientsToRemove = new List<GameClient>();

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

        public bool MapTilePassable(int x, int y)
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

            for (int i = 0; i < _mapData.MapEventsCount(); i++)
            {
                MapEvent mapEvent = _mapData.GetMapEvent(i);
                if ((mapEvent.MapX == x && mapEvent.MapY == y) && !mapEvent.GetMapEventData().Passable())
                    return false;
            }

            return true;
        }

        public void TeleportMapEvent(int eventID, int mapX, int mapY)
        {
            if (MapTilePassable(mapX, mapY))
            {
                GetMapData().GetMapEvent(eventID).MapX = mapX;
                GetMapData().GetMapEvent(eventID).MapY = mapY;
                GetMapData().GetMapEvent(eventID).RealX = mapX * 32;
                GetMapData().GetMapEvent(eventID).RealY = mapY * 32;
                UpdateMapEventOnClients(eventID);
            }
        }

        public void MoveMapEvent(int eventID, Direction direction)
        {
            int x = direction == Direction.Left ? -1 : direction == Direction.Right ? 1 : 0;
            int y = direction == Direction.Up ? -1 : direction == Direction.Down ? 1 : 0;
            x += GetMapData().GetMapEvent(eventID).MapX;
            y += GetMapData().GetMapEvent(eventID).MapY;

            if (MapTilePassable(x, y))
            {
                GetMapData().GetMapEvent(eventID).EventDirection = direction;
                GetMapData().GetMapEvent(eventID).MapX = x;
                GetMapData().GetMapEvent(eventID).MapY = y;
                UpdateMapEventOnClients(eventID);
            }
            else
            {
                ChangeMapEventDirection(eventID, direction);
            }
        }

        public void ChangeMapEventDirection(int eventID, Direction direction)
        {
            GetMapData().GetMapEvent(eventID).EventDirection = direction;
            ClientCommand clientCommand = new ClientCommand(ClientCommand.CommandType.ChangeMapEventDirection);
            clientCommand.SetParameter("EventID", eventID.ToString());
            clientCommand.SetParameter("Direction", ((int)direction).ToString());
            for (int i = 0; i < _clients.Count; i++)
            {
                _clients[i].AddClientCommand(clientCommand);
            }
        }

        public void UpdateMapEventOnClients(int eventID)
        {
            ClientCommand clientCommand = new ClientCommand(ClientCommand.CommandType.UpdateMapEvent);
            clientCommand.SetParameter("EventID", eventID.ToString());
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

            if (_updateMapTimer > 0f)
            {
                _updateMapTimer -= deltaTime;
            }

            for (int i = 0; i < GetMapData().MapEventsCount(); i++)
            {
                if (GetMapData().GetMapEvent(i).Moving())
                {
                    GetMapData().GetMapEvent(i).UpdateMovement(deltaTime);
                }

                if (_updateMapTimer <= 0f)
                {
                    UpdateMapEventOnClients(i);
                    _updateMapTimer = 0.06f;
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

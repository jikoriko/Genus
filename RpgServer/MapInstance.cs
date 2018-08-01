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

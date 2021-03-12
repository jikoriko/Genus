using System;
using System.Collections.Generic;
using System.Linq;

using Genus2D.GameData;
using Genus2D.Networking;
using OpenTK;

namespace RpgServer
{
    public class ServerMap
    {
        private Server _server;
        private int _mapID;
        private MapInstance _mapInstance;

        private List<GameClient> _clients;
        private List<GameClient> _clientsToAdd;
        private List<GameClient> _clientsToRemove;
        private EventInterpreter _eventInterpreter;

        public ServerMap(Server server, int mapID)
        {
            _server = server;
            _mapID = mapID;
            _mapInstance = new MapInstance(mapID);

            _mapInstance.OnChangeMapEvent += OnChangeMapEvent;

            _mapInstance.OnAddMapPlayer += OnAddMapPlayer;
            _mapInstance.OnRemoveMapPlayer += OnRemoveMapPlayer;
            _mapInstance.OnUpdateMapPlayer += OnUpdateMapPlayer;
            _mapInstance.OnAddMapEnemy += OnAddMapEnemy;
            _mapInstance.OnRemoveMapEnemy += OnRemoveMapEnemy;
            _mapInstance.OnUpdateMapEnemy += OnUpdateMapEnemy;
            _mapInstance.OnAddMapProjectile += OnAddMapProjectile;
            _mapInstance.OnRemoveMapProjectile += OnRemoveMapProjectile;
            _mapInstance.OnUpdateMapProjectile += OnUpdateMapProjectile;
            _mapInstance.OnAddMapItem += OnAddMapItem;
            _mapInstance.OnRemoveMapItem += OnRemoveMapItem;
            _mapInstance.OnUpdateMapItem += OnUpdateMapItem;

            _mapInstance.OnEventTrigger += OnEventTrigger;

            _clients = new List<GameClient>();
            _clientsToAdd = new List<GameClient>();
            _clientsToRemove = new List<GameClient>();
            _eventInterpreter = new EventInterpreter(this);
        }

        public Server GetServer()
        {
            return _server;
        }

        public MapInstance GetMapInstance()
        {
            return _mapInstance;
        }

        public void AddClient(GameClient client)
        {
            if (!_clients.Contains(client) && !_clientsToAdd.Contains(client))
            {
                _clientsToAdd.Add(client);
                client.SetServerMap(this);
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

        public EventInterpreter GetEventInterpreter()
        {
            return _eventInterpreter;
        }

        private void OnChangeMapEvent(MapEvent mapEvent, int index)
        {
            OnUpdateMapEventMovement(mapEvent, index);
            OnUpdateMapEventDirection(mapEvent, index);
            OnUpdateMapEventSprite(mapEvent, index);
            OnUpdateMapEventRenderPriority(mapEvent, index);
            OnUpdateMapEventEnabled(mapEvent, index);
        }

        private void OnUpdateMapEventMovement(MapEvent mapEvent, int eventIndex)
        {
            if (mapEvent.PositionChanged)
            {
                ServerCommand serverCommand = new ServerCommand(ServerCommand.CommandType.UpdateMapEvent);
                serverCommand.SetParameter("EventID", eventIndex);
                serverCommand.SetParameter("MapID", _mapID);
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
                mapEvent.PositionChanged = false;
            }
        }

        private void OnUpdateMapEventDirection(MapEvent mapEvent, int eventIndex)
        {
            if (mapEvent.DirectionChanged)
            {
                ServerCommand serverCommand = new ServerCommand(ServerCommand.CommandType.ChangeMapEventDirection);
                serverCommand.SetParameter("EventID", eventIndex);
                serverCommand.SetParameter("MapID", _mapID);
                serverCommand.SetParameter("Direction", (int)mapEvent.EventDirection);

                for (int i = 0; i < _clients.Count; i++)
                {
                    _clients[i].AddServerCommand(serverCommand);
                }
                mapEvent.DirectionChanged = false;
            }
        }

        private void OnUpdateMapEventSprite(MapEvent mapEvent, int eventIndex)
        {
            if (mapEvent.SpriteChanged)
            {
                ServerCommand serverCommand = new ServerCommand(ServerCommand.CommandType.ChangeMapEventSprite);
                serverCommand.SetParameter("EventID", eventIndex);
                serverCommand.SetParameter("MapID", _mapID);
                serverCommand.SetParameter("SpriteID", mapEvent.SpriteID);

                for (int i = 0; i < _clients.Count; i++)
                {
                    _clients[i].AddServerCommand(serverCommand);
                }
                mapEvent.SpriteChanged = false;
            }
        }

        private void OnUpdateMapEventRenderPriority(MapEvent mapEvent, int eventIndex)
        {
            if (mapEvent.RenderPriorityChanged)
            {
                ServerCommand serverCommand = new ServerCommand(ServerCommand.CommandType.ChangeMapEventRenderPriority);
                serverCommand.SetParameter("EventID", eventIndex);
                serverCommand.SetParameter("MapID", _mapID);
                serverCommand.SetParameter("RenderPriority", (int)mapEvent.Priority);

                for (int i = 0; i < _clients.Count; i++)
                {
                    _clients[i].AddServerCommand(serverCommand);
                }
                mapEvent.RenderPriorityChanged = false;
            }
        }

        private void OnUpdateMapEventEnabled(MapEvent mapEvent, int eventIndex)
        {
            if (mapEvent.EnabledChanged)
            {
                ServerCommand serverCommand = new ServerCommand(ServerCommand.CommandType.ChangeMapEventEnabled);
                serverCommand.SetParameter("EventID", eventIndex);
                serverCommand.SetParameter("MapID", _mapID);
                serverCommand.SetParameter("Enabled", mapEvent.Enabled);

                for (int i = 0; i < _clients.Count; i++)
                {
                    _clients[i].AddServerCommand(serverCommand);
                }
                mapEvent.EnabledChanged = false;
            }
        }

        private void OnAddMapPlayer(MapPlayer mapPlayer, int index)
        {

        }

        private void OnRemoveMapPlayer(MapPlayer mapPlayer, int index)
        {
            
        }

        private void OnUpdateMapPlayer(MapPlayer mapPlayer, int index)
        {

        }

        private void OnAddMapEnemy(MapEnemy mapEnemy, int index)
        {
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

        private void OnRemoveMapEnemy(MapEnemy mapEnemy, int index)
        {

        }

        private void OnUpdateMapEnemy(MapEnemy mapEnemy, int index)
        {
            if (mapEnemy.Changed)
            {
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
        }

        public void OnAddMapProjectile(MapProjectile mapProjectile, int index)
        {
            ServerCommand command = new ServerCommand(ServerCommand.CommandType.AddProjectile);
            command.SetParameter("MapID", _mapID);
            command.SetParameter("DataID", mapProjectile.ProjectileID);
            command.SetParameter("RealX", mapProjectile.Position.X);
            command.SetParameter("RealY", mapProjectile.Position.Y);
            command.SetParameter("VelocityX", mapProjectile.Velocity.X);
            command.SetParameter("VelocityY", mapProjectile.Velocity.Y);
            command.SetParameter("Direction", (int)mapProjectile.Direction);
            command.SetParameter("OnBridge", mapProjectile.OnBridge);

            for (int i = 0; i < _clients.Count; i++)
            {
                _clients[i].AddServerCommand(command);
            }
        }

        public void OnRemoveMapProjectile(MapProjectile mapProjectile, int index)
        {

        }

        public void OnUpdateMapProjectile(MapProjectile mapProjectile, int index)
        {
            if (mapProjectile.Changed)
            {
                ServerCommand serverCommand = new ServerCommand(ServerCommand.CommandType.UpdateProjectile);
                serverCommand.SetParameter("MapID", _mapID);
                serverCommand.SetParameter("ProjectileID", index);
                serverCommand.SetParameter("RealX", mapProjectile.Position.X);
                serverCommand.SetParameter("RealY", mapProjectile.Position.Y);
                serverCommand.SetParameter("OnBridge", mapProjectile.OnBridge);
                serverCommand.SetParameter("Destroyed", mapProjectile.Destroyed);
                for (int i = 0; i < _clients.Count; i++)
                {
                    _clients[i].AddServerCommand(serverCommand);
                }
            }
        }

        public void OnAddMapItem(MapItem mapItem, int index)
        {
            if (mapItem.Changed)
            {
                ServerCommand command = new ServerCommand(ServerCommand.CommandType.AddMapItem);
                command.SetParameter("MapID", _mapID);
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
        }

        public void OnRemoveMapItem(MapItem mapItem, int index)
        {
            ServerCommand command = new ServerCommand(ServerCommand.CommandType.RemoveMapItem);
            command.SetParameter("MapID", _mapID);
            command.SetParameter("ItemIndex", index);
            for (int i = 0; i < _clients.Count; i++)
            {
                _clients[i].AddServerCommand(command);
            }
        }

        public void OnUpdateMapItem(MapItem mapItem, int index)
        {
            ServerCommand command = new ServerCommand(ServerCommand.CommandType.UpdateMapItem);
            command.SetParameter("MapID", _mapID);
            command.SetParameter("ItemIndex", index);
            command.SetParameter("PlayerID", mapItem.PlayerID);
            command.SetParameter("Count", mapItem.Count);
            for (int i = 0; i < _clients.Count; i++)
            {
                _clients[i].AddServerCommand(command);
            }

        }

        public void OnEventTrigger(MapPlayer mapPlayer, MapEvent mapEvent)
        {
            _eventInterpreter.TriggerEventData(mapPlayer, mapEvent);
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
                    _mapInstance.AddMapPlayer(client.GetMapPlayer());
                    _clientsToAdd.RemoveAt(0);
                }
            }

            //remove clients
            while (_clientsToRemove.Count > 0)
            {
                GameClient client = _clientsToRemove[0];
                if (client != null)
                {
                    int index = _clients.IndexOf(client);
                    _mapInstance.RemoveMapPlayer(client.GetMapPlayer());
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

            //update map instance
            _mapInstance.Update(deltaTime);
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

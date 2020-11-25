using Genus2D.Entities;
using Genus2D.GameData;
using Genus2D.Graphics;
using Genus2D.GUI;
using Genus2D.Networking;
using OpenTK;
using RpgGame.EntityComponents;
using RpgGame.GUI;
using RpgGame.States;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Xml;

namespace RpgGame
{
    public class RpgClientConnection
    {
        public static RpgClientConnection Instance { get; private set; }
        private XmlDocument _settingsXml;

        private TcpClient _client;
        private string _ipAddress;
        private int _port;
        private NetworkStream _stream;
        private LoginResult _loginResult;
        private bool _connected = false;

        private Dictionary<int, PlayerPacket> _playerPackets;
        public Dictionary<int, Entity> PlayerEntities { get; private set; }
        public List<Entity> EnemyEntites { get; private set; }
        public List<Entity> ProjectileEntites { get; private set; }
        public List<Entity> MapItemEntities { get; private set; }
        private bool _playersUpdated = false;

        private Thread _recievePacketsThread;
        private List<MessagePacket> _messages;
        private List<ClientCommand> _clientCommands;

        private MessageBox _messageBox = null;

        States.GameState _gameState;

        public RpgClientConnection(States.GameState state, string username, string password, bool register)
        {
            Instance = this;
            _gameState = state;
            if (_gameState != null)
                _gameState.SetRpgClientConnection(this);

            try
            {
                _settingsXml = new XmlDocument();
                _settingsXml.Load("Data/ServerSettings.xml");
                XmlElement xElement = _settingsXml.DocumentElement["Port"];
                _port = int.Parse(xElement.InnerText);
                xElement = _settingsXml.DocumentElement["ExternalIpAddress"];
                _ipAddress = xElement.InnerText;

                _client = new TcpClient();
                _client.Connect(_ipAddress, _port);
                _stream = _client.GetStream();

                LoginRequest request = new LoginRequest();
                request.Username = username;
                request.Password = password;

                if (register)
                {
                    SendRegisterRequest(_stream, request);
                    RegisterResult result = RecieveRegisterResult(_stream);
                    if (!result.Registered)
                    {
                        MessageBox messageBox = new MessageBox("Failed to register." + '\n' + result.Reason, LoginState.INSTANCE);
                        LoginState.INSTANCE.AddControl(messageBox);
                    }
                    else
                    {
                        MessageBox messageBox = new MessageBox("Account registered.", LoginState.INSTANCE);
                        LoginState.INSTANCE.AddControl(messageBox);
                    }
                    Disconnect();
                }
                else
                {
                    SendLoginRequest(_stream, request);
                    _loginResult = RecieveLoginResult(_stream);

                    if (_loginResult.LoggedIn)
                    {
                        _connected = true;
                        _playerPackets = new Dictionary<int, PlayerPacket>();
                        PlayerEntities = new Dictionary<int, Entity>();
                        EnemyEntites = new List<Entity>();
                        ProjectileEntites = new List<Entity>();
                        MapItemEntities = new List<Entity>();

                        _recievePacketsThread = new Thread(new ThreadStart(RecievePackets));
                        _recievePacketsThread.Start();
                        _messages = new List<MessagePacket>();
                        _clientCommands = new List<ClientCommand>();
                        Console.WriteLine("Client logged in: " + _loginResult.PlayerID + ", " + request.Username);
                    }
                    else
                    {
                        MessageBox messageBox = new MessageBox("Failed to login.", LoginState.INSTANCE);
                        LoginState.INSTANCE.AddControl(messageBox);
                        Disconnect();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public PlayerPacket GetLocalPlayerPacket()
        {
            if (_playerPackets.ContainsKey(_loginResult.PlayerID))
                return _playerPackets[_loginResult.PlayerID];
            return null;
        }

        private void SendRegisterRequest(NetworkStream stream, LoginRequest request)
        {
            byte[] requestBytes = request.GetBytes();
            byte[] sizeBytes = BitConverter.GetBytes(requestBytes.Length);

            stream.Write(BitConverter.GetBytes((int)PacketType.RegisterRequest), 0, sizeof(int));
            stream.Flush();
            stream.Write(sizeBytes, 0, sizeof(int));
            stream.Flush();
            stream.Write(requestBytes, 0, requestBytes.Length);
            stream.Flush();
        }

        private RegisterResult RecieveRegisterResult(NetworkStream stream)
        {
            byte[] bytes = ReadData(sizeof(int), _stream);
            int size = BitConverter.ToInt32(bytes, 0);

            bytes = ReadData(size, _stream);
            RegisterResult result = RegisterResult.FromBytes(bytes);

            return result;
        }

        private void SendLoginRequest(NetworkStream stream, LoginRequest request)
        {
            byte[] requestBytes = request.GetBytes();
            byte[] sizeBytes = BitConverter.GetBytes(requestBytes.Length);

            stream.Write(BitConverter.GetBytes((int)PacketType.LoginRequest), 0, sizeof(int));
            stream.Flush();
            stream.Write(sizeBytes, 0, sizeof(int));
            stream.Flush();
            stream.Write(requestBytes, 0, requestBytes.Length);
            stream.Flush();
        }

        private LoginResult RecieveLoginResult(NetworkStream stream)
        {
            byte[] bytes = ReadData(sizeof(int), _stream);
            int size = BitConverter.ToInt32(bytes, 0);

            bytes = ReadData(size, _stream);
            LoginResult result = LoginResult.FromBytes(bytes);

            return result;
        }

        public void Update()
        {
            if (_playersUpdated)
            {
                for (int i = 0; i < PlayerEntities.Count; i++)
                {
                    int id = PlayerEntities.ElementAt(i).Key;
                    if (_playerPackets.ContainsKey(id) == false)
                    {
                        PlayerEntities[id].Destroy();
                        PlayerEntities.Remove(id);
                    }

                }

                for (int i = 0; i < _playerPackets.Count; i++)
                {
                    PlayerPacket packet = _playerPackets.ElementAt(i).Value;
                    if (_loginResult.PlayerID == packet.PlayerID)
                    {
                        Vector3 mapPos = new Vector3((Renderer.GetResoultion().X / 2) - (packet.RealX + 16), ((Renderer.GetResoultion().Y - 200) / 2) - packet.RealY, 0);
                        _gameState.MapEntity.GetTransform().LocalPosition = mapPos;
                    }

                    if (PlayerEntities.ContainsKey(packet.PlayerID))
                    {
                        Entity clientEntity = PlayerEntities[packet.PlayerID];
                        PlayerComponent playerComponent = (PlayerComponent)clientEntity.FindComponent<PlayerComponent>();
                        playerComponent.SetPlayerPacket(packet);
                    }
                    else
                    {
                        Entity clientEntity = Entity.CreateInstance(_gameState.EntityManager);
                        clientEntity.GetTransform().Parent = _gameState.MapEntity.GetTransform();
                        new PlayerComponent(clientEntity, packet);
                        PlayerEntities.Add(packet.PlayerID, clientEntity);
                    }

                   
                }
                _playersUpdated = false;
            }

        }

        private void RecievePackets()
        {
            while (true)
            {
                if (!Connected())
                    break;

                try
                {
                    byte[] bytes = ReadData(sizeof(int), _stream);
                    PacketType type = (PacketType)BitConverter.ToInt32(bytes, 0);

                    switch (type)
                    {
                        case PacketType.PlayerPacket:
                            RecievePlayerPackets();
                            break;
                        case PacketType.MapPacket:
                            RecieveMapPackets();
                            break;
                        case PacketType.ServerCommand:
                            RecieveServerCommand();
                            break;
                        case PacketType.ClientCommand:
                            SendClientCommands();
                            break;
                        case PacketType.SendMessagePackets:
                            SendMessagePackets();
                            break;
                        case PacketType.RecieveMessagePackets:
                            RecieveMessagePackets();
                            break;
                        case PacketType.BankPacket:
                            RecieveBankPacket();
                            break;

                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Console.WriteLine(e.StackTrace);
                }
            }

        }

        private void RecievePlayerPackets()
        {
            byte[] bytes = ReadData(sizeof(int), _stream);
            int numClients = BitConverter.ToInt32(bytes, 0);

            Dictionary<int, PlayerPacket> playerPackets = new Dictionary<int, PlayerPacket>();

            for (int i = 0; i < numClients; i++)
            {
                bytes = ReadData(sizeof(int), _stream);
                int packetSize = BitConverter.ToInt32(bytes, 0);

                bytes = ReadData(packetSize, _stream);
                PlayerPacket packet = PlayerPacket.FromBytes(bytes);

                try
                {
                    playerPackets.Add(packet.PlayerID, packet);
                }
                catch (Exception e) { }
            }

            if (!_playersUpdated)
            {
                _playerPackets = playerPackets;
                _playersUpdated = true;
            }
        }

        private void RecieveMapPackets()
        {
            byte[] bytes = ReadData(sizeof(int), _stream);
            int packetSize = BitConverter.ToInt32(bytes, 0);

            bytes = ReadData(packetSize, _stream);

            MapPacket packet = MapPacket.FromBytes(bytes);
            MapComponent.Instance.SetMapData(packet.MapID, packet.mapData);

            for (int i = 0; i < EnemyEntites.Count; i++)
                EnemyEntites[i].Destroy();
            EnemyEntites.Clear();

            for (int i = 0; i < packet.Enemies.Count; i++)
            {
                Entity entity = Entity.CreateInstance(GameState.Instance.EntityManager);
                entity.GetTransform().Parent = _gameState.MapEntity.GetTransform();
                entity.AddComponent(new MapEnemyComponent(entity, packet.Enemies[i]));
                EnemyEntites.Add(entity);
            }

            for (int i = 0; i < ProjectileEntites.Count; i++)
                ProjectileEntites[i].Destroy();
            ProjectileEntites.Clear();
            for (int i = 0; i < packet.Projectiles.Count; i++)
            {
                Entity entity = Entity.CreateInstance(GameState.Instance.EntityManager);
                entity.GetTransform().Parent = _gameState.MapEntity.GetTransform();
                entity.AddComponent(new ProjectileComponent(entity, packet.Projectiles[i]));
                ProjectileEntites.Add(entity);
            }

            for (int i = 0; i < MapItemEntities.Count; i++)
                MapItemEntities[i].Destroy();
            MapItemEntities.Clear();
            for (int i = 0; i < packet.Items.Count; i++)
            {
                Entity entity = Entity.CreateInstance(GameState.Instance.EntityManager);
                entity.GetTransform().Parent = _gameState.MapEntity.GetTransform();
                entity.AddComponent(new MapItemComponent(entity, packet.Items[i]));
                MapItemEntities.Add(entity);
            }
        }

        private void RecieveBankPacket()
        {
            byte[] bytes = ReadData(sizeof(int), _stream);
            int packetSize = BitConverter.ToInt32(bytes, 0);
            bytes = ReadData(packetSize, _stream);

            if (BankPanel.Instance != null)
            {
                BankData data = BankData.FromBytes(bytes);
                BankPanel.Instance.SetBankData(data);
            }
        }

        public void CloseMessageBox()
        {
            if (_messageBox != null)
            {
                if (_messageBox.GetSelectedOption() != -1)
                {
                    ClientCommand command = new ClientCommand(ClientCommand.CommandType.SelectOption);
                    command.SetParameter("Option", _messageBox.GetSelectedOption());
                    this.AddClientCommand(command);
                }
                else
                {
                    ClientCommand command = new ClientCommand(ClientCommand.CommandType.CloseMessage);
                    this.AddClientCommand(command);
                }
                _messageBox.Close();
                _messageBox = null;
            }
        }

        private void RecieveServerCommand()
        {
            byte[] bytes = ReadData(sizeof(int), _stream);
            int packetSize = BitConverter.ToInt32(bytes, 0);

            bytes = ReadData(packetSize, _stream);
            ServerCommand command = ServerCommand.FromBytes(bytes);

            int eventID;
            int enemyID;
            int enemyIndex;
            int mapID;
            int mapX;
            int mapY;
            float realX;
            float realY;
            FacingDirection direction;
            MapData map;
            int projectileID;
            int playerID;
            string playerName;
            int itemIndex;
            int itemID;
            int count;

            switch (command.GetCommandType())
            {
                case ServerCommand.CommandType.Disconnect:
                    this.Disconnect();
                    Console.WriteLine("Server Disconnected");
                    break;
                case ServerCommand.CommandType.ShowMessage:

                    if (_messageBox == null)
                    {
                        _messageBox = new MessageBox((string)command.GetParameter("Message"), _gameState, false);
                        _gameState.AddControl(_messageBox);
                    }

                    break;
                case ServerCommand.CommandType.ShowOptions:

                    if (_messageBox == null)
                    {
                        string message = (string)command.GetParameter("Message");
                        _messageBox = new MessageBox(message, _gameState, false);
                        string[] options = ((string)command.GetParameter("Options")).Split(',');
                        for (int i = 0; i < options.Length; i++)
                            _messageBox.AddOption(options[i]);
                        _messageBox.SetSelectedOption(0);
                        _gameState.AddControl(_messageBox);
                    }

                    break;
                case ServerCommand.CommandType.AddMapEnemy:

                    enemyID = (int)command.GetParameter("EnemyID");
                    mapID = (int)command.GetParameter("MapID");
                    if (_gameState.MapEntity.FindComponent<MapComponent>().MapID == mapID)
                    {
                        mapX = (int)command.GetParameter("MapX");
                        mapY = (int)command.GetParameter("MapY");
                        bool onBridge = (bool)command.GetParameter("OnBridge");
                        MapEnemy mapEnemy = new MapEnemy(enemyID, mapX, mapY, onBridge);
                        Entity entity = Entity.CreateInstance(GameState.Instance.EntityManager);
                        entity.GetTransform().Parent = _gameState.MapEntity.GetTransform();
                        entity.AddComponent(new MapEnemyComponent(entity, mapEnemy));
                        EnemyEntites.Add(entity);
                    }

                    break;
                case ServerCommand.CommandType.UpdateMapEnemy:

                    enemyIndex = (int)command.GetParameter("EnemyIndex");
                    mapID = (int)command.GetParameter("MapID");
                    if ((_gameState.MapEntity.FindComponent<MapComponent>()).MapID == mapID)
                    {
                        int HP = (int)command.GetParameter("HP");
                        mapX = (int)command.GetParameter("MapX");
                        mapY = (int)command.GetParameter("MapY");
                        realX = (float)command.GetParameter("RealX");
                        realY = (float)command.GetParameter("RealY");
                        direction = (FacingDirection)command.GetParameter("Direction");
                        bool onBridge = (bool)command.GetParameter("OnBridge");
                        bool dead = (bool)command.GetParameter("Dead");
                        MapEnemyComponent enemyComponent = EnemyEntites[enemyIndex].FindComponent<MapEnemyComponent>();
                        enemyComponent.UpdateMapEnemy(HP, mapX, mapY, realX, realY, direction, onBridge, dead);

                        if (dead)
                        {
                            EnemyEntites[enemyIndex].Destroy();
                            EnemyEntites.RemoveAt(enemyIndex);
                        }
                    }

                    break;
                case ServerCommand.CommandType.UpdateMapEvent:

                    eventID = (int)command.GetParameter("EventID");
                    mapID = (int)command.GetParameter("MapID");
                    if (_gameState.MapEntity.FindComponent<MapComponent>().MapID == mapID)
                    {
                        mapX = (int)command.GetParameter("MapX");
                        mapY = (int)command.GetParameter("MapY");
                        realX = (float)command.GetParameter("RealX");
                        realY = (float)command.GetParameter("RealY");
                        direction = (FacingDirection)command.GetParameter("Direction");
                        bool onBridge = (bool)command.GetParameter("OnBridge");

                        map = _gameState.MapEntity.FindComponent<MapComponent>().GetMapData();
                        if (map != null)
                        {
                            map.GetMapEvent(eventID).MapX = mapX;
                            map.GetMapEvent(eventID).MapY = mapY;
                            map.GetMapEvent(eventID).RealX = realX;
                            map.GetMapEvent(eventID).RealY = realY;
                            map.GetMapEvent(eventID).EventDirection = direction;
                            map.GetMapEvent(eventID).OnBridge = onBridge;
                        }
                    }

                    break;
                case ServerCommand.CommandType.ChangeMapEventDirection:

                    eventID = (int)command.GetParameter("EventID");
                    mapID = (int)command.GetParameter("MapID");
                    if (_gameState.MapEntity.FindComponent<MapComponent>().MapID == mapID)
                    {
                        direction = (FacingDirection)command.GetParameter("Direction");

                        map = _gameState.MapEntity.FindComponent<MapComponent>().GetMapData();
                        if (map != null)
                        {
                            map.GetMapEvent(eventID).EventDirection = direction;
                        }
                    }

                    break;
                case ServerCommand.CommandType.ChangeMapEventSprite:

                    mapID = (int)command.GetParameter("MapID");
                    if (_gameState.MapEntity.FindComponent<MapComponent>().MapID == mapID)
                    {
                        eventID = (int)command.GetParameter("EventID");
                        int spriteID = (int)command.GetParameter("SpriteID");
                        _gameState.MapEntity.FindComponent<MapComponent>().ChangeMapEventSprite(eventID, spriteID);
                    }

                    break;
                case ServerCommand.CommandType.ChangeMapEventRenderPriority:

                    mapID = (int)command.GetParameter("MapID");
                    if (_gameState.MapEntity.FindComponent<MapComponent>().MapID == mapID)
                    {
                        eventID = (int)command.GetParameter("EventID");
                        RenderPriority priority = (RenderPriority)command.GetParameter("RenderPriority");
                        _gameState.MapEntity.FindComponent<MapComponent>().ChangeMapEventRenderPriority(eventID, priority);
                    }

                    break;
                case ServerCommand.CommandType.ChangeMapEventEnabled:

                    mapID = (int)command.GetParameter("MapID");
                    if (_gameState.MapEntity.FindComponent<MapComponent>().MapID == mapID)
                    {
                        eventID = (int)command.GetParameter("EventID");
                        bool enabled = (bool)command.GetParameter("Enabled");
                        _gameState.MapEntity.FindComponent<MapComponent>().ChangeMapEventEnabled(eventID, enabled);
                    }

                    break;
                case ServerCommand.CommandType.AddProjectile:

                    mapID = (int)command.GetParameter("MapID");
                    if (_gameState.MapEntity.FindComponent<MapComponent>().MapID == mapID)
                    {
                        {
                            int dataID = (int)command.GetParameter("DataID");
                            realX = (float)command.GetParameter("RealX");
                            realY = (float)command.GetParameter("RealY");
                            direction = (FacingDirection)command.GetParameter("Direction");
                            bool onBridge = (bool)command.GetParameter("OnBridge");

                            Projectile projectile = new Projectile(dataID, CharacterType.Player, -1, new Vector2(realX, realY), direction);
                            projectile.OnBridge = onBridge;
                            Entity entity = Entity.CreateInstance(GameState.Instance.EntityManager);
                            entity.GetTransform().Parent = _gameState.MapEntity.GetTransform();
                            entity.AddComponent(new ProjectileComponent(entity, projectile));
                            ProjectileEntites.Add(entity);
                        }

                    }

                    break;
                case ServerCommand.CommandType.UpdateProjectile:

                    mapID = (int)command.GetParameter("MapID");
                    if (_gameState.MapEntity.FindComponent<MapComponent>().MapID == mapID)
                    {
                        projectileID = (int)command.GetParameter("ProjectileID");
                        if (projectileID < ProjectileEntites.Count)
                        {
                            realX = (float)command.GetParameter("RealX");
                            realY = (float)command.GetParameter("RealY");
                            bool onBridge = (bool)command.GetParameter("OnBridge");
                            bool destroyed = (bool)command.GetParameter("Destroyed");
                            ProjectileComponent component = ProjectileEntites[projectileID].FindComponent<ProjectileComponent>();
                            component.SetRealPosition(realX, realY);
                            component.SetOnBridge(onBridge);
                            if (destroyed)
                            {
                                ProjectileEntites[projectileID].Destroy();
                                ProjectileEntites.RemoveAt(projectileID);
                            }
                        }
                    }

                    break;
                case ServerCommand.CommandType.AddMapItem:

                    mapID = (int)command.GetParameter("MapID");
                    MapComponent mapComponent = _gameState.MapEntity.FindComponent<MapComponent>();
                    if (mapComponent.MapID == mapID)
                    {
                        itemID = (int)command.GetParameter("ItemID");
                        count = (int)command.GetParameter("Count");
                        mapX = (int)command.GetParameter("MapX");
                        mapY = (int)command.GetParameter("MapY");
                        playerID = (int)command.GetParameter("PlayerID");
                        bool onBridge = (bool)command.GetParameter("OnBridge");
                        MapItem mapItem = new MapItem(itemID, count, mapX, mapY, playerID, onBridge);
                        Entity entity = Entity.CreateInstance(GameState.Instance.EntityManager);
                        entity.GetTransform().Parent = _gameState.MapEntity.GetTransform();
                        entity.AddComponent(new MapItemComponent(entity, mapItem));
                        MapItemEntities.Add(entity);
                    }

                    break;
                case ServerCommand.CommandType.RemoveMapItem:

                    mapID = (int)command.GetParameter("MapID");
                    if (_gameState.MapEntity.FindComponent<MapComponent>().MapID == mapID)
                    {
                        itemIndex = (int)command.GetParameter("ItemIndex");
                        if (itemIndex < MapItemEntities.Count)
                        {
                            MapItemEntities[itemIndex].Destroy();
                            MapItemEntities.RemoveAt(itemIndex);
                        }
                    }

                    break;
                case ServerCommand.CommandType.UpdateMapItem:

                    mapID = (int)command.GetParameter("MapID");
                    if (_gameState.MapEntity.FindComponent<MapComponent>().MapID == mapID)
                    {
                        itemIndex = (int)command.GetParameter("ItemIndex");
                        if (itemIndex < MapItemEntities.Count)
                        {
                            playerID = (int)command.GetParameter("PlayerID");
                            count = (int)command.GetParameter("Count");
                            MapItemEntities[itemIndex].FindComponent<MapItemComponent>().GetMapItem().PlayerID = playerID;
                            MapItemEntities[itemIndex].FindComponent<MapItemComponent>().GetMapItem().Count = count;
                        }
                    }

                    break;
                case ServerCommand.CommandType.ShowShop:

                    int shopID = (int)command.GetParameter("ShopID");
                    ShopData data = ShopData.GetData(shopID);
                    if (data != null)
                    {
                        _gameState.AddControl(new GUI.ShopPanel(_gameState, data));
                    }
                    else
                    {
                        AddClientCommand(new ClientCommand(ClientCommand.CommandType.CloseShop));
                    }

                    break;
                case ServerCommand.CommandType.TradeRequest:
                    playerID = (int)command.GetParameter("PlayerID");
                    playerName = (string)command.GetParameter("PlayerName");
                    MessagePanel.Instance.AddMessage(playerName + " would like to trade.");

                    break;
                case ServerCommand.CommandType.StartTrade:
                    playerID = (int)command.GetParameter("PlayerID");
                    playerName = (string)command.GetParameter("PlayerName");

                    if (InventoryPanel.Instance == null)
                        GameState.Instance.ToggleInventory();

                    _gameState.AddControl(new GUI.TradePanel(_gameState, playerID, playerName));
                    break;
                case ServerCommand.CommandType.AcceptTrade:
                    if (TradePanel.Instance != null)
                    {
                        TradePanel.Instance.TradeRequest.TradeOffer2.Accepted = true;
                    }
                    break;
                case ServerCommand.CommandType.EndTrade:
                    if (TradePanel.Instance != null)
                    {
                        TradePanel.Instance.Close();
                    }
                    break;
                case ServerCommand.CommandType.AddTradeItem:
                    itemID = (int)command.GetParameter("ItemID");
                    count = (int)command.GetParameter("Count");

                    if (TradePanel.Instance != null)
                    {
                        TradePanel.Instance.TradeRequest.TradeOffer2.AddItem(itemID, count);
                        TradePanel.Instance.TradeRequest.TradeOffer1.Accepted = false;
                        TradePanel.Instance.TradeRequest.TradeOffer2.Accepted = false;
                    }

                    break;
                case ServerCommand.CommandType.RemoveTradeItem:
                    itemIndex = (int)command.GetParameter("ItemIndex");
                    count = (int)command.GetParameter("Count");

                    if (TradePanel.Instance != null)
                    {
                        TradePanel.Instance.TradeRequest.TradeOffer2.RemoveItem(itemIndex, count);
                        TradePanel.Instance.TradeRequest.TradeOffer1.Accepted = false;
                        TradePanel.Instance.TradeRequest.TradeOffer2.Accepted = false;
                    }

                    break;
                case ServerCommand.CommandType.CantTrade:
                    if (TradePanel.Instance != null)
                    {
                        TradePanel.Instance.TradeRequest.TradeOffer1.Accepted = false;
                        TradePanel.Instance.TradeRequest.TradeOffer2.Accepted = false;
                    }
                    break;
                case ServerCommand.CommandType.OpenBank:
                    if (InventoryPanel.Instance == null)
                        GameState.Instance.ToggleInventory();

                    _gameState.AddControl(new BankPanel(_gameState));

                    break;
            }
        }

        public void AddClientCommand(ClientCommand command)
        {
            _clientCommands.Add(command);
        }

        private void SendClientCommands()
        {
            int numCommands = _clientCommands.Count;
            byte[] bytes = BitConverter.GetBytes(numCommands);
            _stream.Write(bytes, 0, sizeof(int));
            for (int i = 0; i < numCommands; i++)
            {
                while (_clientCommands[i] == null) ;
                bytes = _clientCommands[i].GetBytes();
                _stream.Write(BitConverter.GetBytes(bytes.Length), 0, sizeof(int));
                _stream.Write(bytes, 0, bytes.Length);
            }
            _clientCommands.RemoveRange(0, numCommands);
            _stream.Flush();
        }

        public void SendMessage(MessagePacket packet)
        {
            _messages.Add(packet);
        }

        private void SendMessagePackets()
        {
            int numMessages = _messages.Count;
            byte[] bytes = BitConverter.GetBytes(numMessages);
            _stream.Write(bytes, 0, sizeof(int));
            for (int i = 0; i < numMessages; i++)
            {
                bytes = _messages[i].GetBytes();
                _stream.Write(BitConverter.GetBytes(bytes.Length), 0, sizeof(int));
                _stream.Write(bytes, 0, bytes.Length);
            }
            _messages.RemoveRange(0, numMessages);
            _stream.Flush();
        }

        private void RecieveMessagePackets()
        {
            byte[] bytes = ReadData(sizeof(int), _stream);
            int numMessages = BitConverter.ToInt32(bytes, 0);

            for (int i = 0; i < numMessages; i++)
            {
                bytes = ReadData(sizeof(int), _stream);
                int messageSize = BitConverter.ToInt32(bytes, 0);
                bytes = ReadData(messageSize, _stream);
                MessagePacket packet = MessagePacket.FromBytes(bytes);
                GUI.MessagePanel.Instance.AddMessage(packet);
            }
        }

        private byte[] ReadData(int size, NetworkStream stream)
        {
            byte[] data = new byte[size];
            int dataRead = 0;

            while (true)
            {
                if (dataRead == size)
                    break;

                dataRead += stream.Read(data, dataRead, size - dataRead);
            }

            return data;
        }

        public bool Connected()
        {
            return _connected && _client.Connected;
        }

        public void Disconnect()
        {
            if (_connected)
            {
                _stream.Close();
                _client.Close();
                _connected = false;
            }
        }
    }
}

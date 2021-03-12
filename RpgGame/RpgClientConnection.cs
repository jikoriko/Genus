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
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace RpgGame
{
    public class RpgClientConnection
    {
        public static RpgClientConnection Instance { get; private set; }
        private XmlDocument _settingsXml;

        private string _ipAddress;
        private int _serverPort;

        private TcpClient _tcpClient;
        private NetworkStream _networkStream;
        private float _sendTimer;
        private bool _sendingPackets;
        private bool _recievingPackets;

        private LoginResult _loginResult;
        private bool _connected = false;

        private Dictionary<int, PlayerPacket> _playerPackets;

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
                XmlElement xElement = _settingsXml.DocumentElement["ExternalIpAddress"];
                _ipAddress = xElement.InnerText;
                xElement = _settingsXml.DocumentElement["ServerPort"];
                _serverPort = int.Parse(xElement.InnerText);

                _tcpClient = new TcpClient();
                _tcpClient.Connect(_ipAddress, _serverPort);
                _networkStream = _tcpClient.GetStream();
                _sendTimer = 0f;
                _sendingPackets = false;
                _recievingPackets = false;

                LoginRequest request = new LoginRequest();
                request.Username = username;
                request.Password = password;

                if (register)
                {
                    SendRegisterRequest(_networkStream, request);
                    RegisterResult result = RecieveRegisterResult(_networkStream);
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
                    SendLoginRequest(_networkStream, request);
                    _loginResult = RecieveLoginResult(_networkStream);

                    if (_loginResult.LoggedIn)
                    {
                        _connected = true;
                        _playerPackets = new Dictionary<int, PlayerPacket>();

                        _messages = new List<MessagePacket>();
                        _clientCommands = new List<ClientCommand>();
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

        public int GetLocalPlayerID()
        {
            if (_loginResult != null && _loginResult.LoggedIn)
            {
                return _loginResult.PlayerID;
            }

            return -1;
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
            byte[] bytes = ReadData(sizeof(int), _networkStream);
            int size = BitConverter.ToInt32(bytes, 0);

            bytes = ReadData(size, _networkStream);
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
            byte[] bytes = ReadData(sizeof(int), _networkStream);
            int size = BitConverter.ToInt32(bytes, 0);

            bytes = ReadData(size, _networkStream);
            LoginResult result = LoginResult.FromBytes(bytes);

            return result;
        }

        public void Update(float deltaTime)
        {
            SendRecievePackets(deltaTime);
        }

        public void SendRecievePackets(float deltaTime)
        {
            if (!Connected())
                return;

            if (_sendTimer <= 0)
            {
                SendPackets();
                _sendTimer = 0.1f;
            }
            else
            {
                _sendTimer -= deltaTime;
            }
            RecievePackets();

        }

        private async void RecievePackets()
        {
            if (!_recievingPackets)
            {
                _recievingPackets = true;
                await Task.Run(() => {                   
                    while (_networkStream.DataAvailable)
                    {
                        try
                        {
                            byte[] bytes = ReadData(sizeof(int), _networkStream);
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
                    _recievingPackets = false;
                });
            }
        }

        private async void SendPackets()
        {
            if (!_sendingPackets)
            {
                _sendingPackets = true;
                await Task.Run(() =>
                {
                    try
                    {
                        List<byte> comunicationBytes = new List<byte>();
                        GetMessagePacketBytes(ref comunicationBytes);
                        GetClientCommandBytes(ref comunicationBytes);

                        if (comunicationBytes.Count > 0)
                        {
                            _networkStream.Write(comunicationBytes.ToArray(), 0, comunicationBytes.Count);
                            _networkStream.Flush();
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        Console.WriteLine(e.StackTrace);
                    }
                    _sendingPackets = false;
                });
            }
        }

        private void RecievePlayerPackets()
        {
            byte[] bytes = ReadData(sizeof(int), _networkStream);
            int numClients = BitConverter.ToInt32(bytes, 0);

            _playerPackets.Clear();

            for (int i = 0; i < numClients; i++)
            {
                bytes = ReadData(sizeof(int), _networkStream);
                int packetSize = BitConverter.ToInt32(bytes, 0);

                bytes = ReadData(packetSize, _networkStream);
                PlayerPacket packet = PlayerPacket.FromBytes(bytes);

                if (_playerPackets.ContainsKey(packet.PlayerID))
                {
                    _playerPackets[packet.PlayerID] = packet;
                }
                else
                {
                    _playerPackets.Add(packet.PlayerID, packet);
                }
            }

            MapComponent.Instance.SetPlayers(_playerPackets);
        }

        private void RecieveMapPackets()
        {
            byte[] bytes = ReadData(sizeof(int), _networkStream);
            int packetSize = BitConverter.ToInt32(bytes, 0);

            bytes = ReadData(packetSize, _networkStream);

            MapPacket packet = MapPacket.FromBytes(bytes);
            MapComponent.Instance.SetMapInstance(packet);
        }

        private void RecieveBankPacket()
        {
            byte[] bytes = ReadData(sizeof(int), _networkStream);
            int packetSize = BitConverter.ToInt32(bytes, 0);
            bytes = ReadData(packetSize, _networkStream);

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
            byte[] bytes = ReadData(sizeof(int), _networkStream);
            int packetSize = BitConverter.ToInt32(bytes, 0);

            bytes = ReadData(packetSize, _networkStream);
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
                    if (_gameState.MapEntity.FindComponent<MapComponent>().GetMapInstance().GetMapPacket().MapID == mapID)
                    {
                        mapX = (int)command.GetParameter("MapX");
                        mapY = (int)command.GetParameter("MapY");
                        bool onBridge = (bool)command.GetParameter("OnBridge");
                        MapEnemy mapEnemy = new MapEnemy(enemyID, mapX, mapY, onBridge);
                        MapComponent.Instance.AddMapEnemy(mapEnemy);
                    }

                    break;
                case ServerCommand.CommandType.UpdateMapEnemy:

                    enemyIndex = (int)command.GetParameter("EnemyIndex");
                    mapID = (int)command.GetParameter("MapID");
                    if ((_gameState.MapEntity.FindComponent<MapComponent>()).GetMapInstance().GetMapPacket().MapID == mapID)
                    {
                        int HP = (int)command.GetParameter("HP");
                        mapX = (int)command.GetParameter("MapX");
                        mapY = (int)command.GetParameter("MapY");
                        realX = (float)command.GetParameter("RealX");
                        realY = (float)command.GetParameter("RealY");
                        direction = (FacingDirection)command.GetParameter("Direction");
                        bool onBridge = (bool)command.GetParameter("OnBridge");
                        bool dead = (bool)command.GetParameter("Dead");
                        MapEnemyComponent enemyComponent = MapComponent.Instance.EnemyEntites[enemyIndex].FindComponent<MapEnemyComponent>();
                        enemyComponent.UpdateMapEnemy(HP, mapX, mapY, realX, realY, direction, onBridge, dead);

                        if (dead)
                        {
                            MapComponent.Instance.EnemyEntites[enemyIndex].Destroy();
                            MapComponent.Instance.EnemyEntites.RemoveAt(enemyIndex);
                            MapComponent.Instance.GetMapInstance().RemoveMapEnemy(enemyIndex);
                        }
                    }

                    break;
                case ServerCommand.CommandType.UpdateMapEvent:

                    eventID = (int)command.GetParameter("EventID");
                    mapID = (int)command.GetParameter("MapID");
                    if (_gameState.MapEntity.FindComponent<MapComponent>().GetMapInstance().GetMapPacket().MapID == mapID)
                    {
                        mapX = (int)command.GetParameter("MapX");
                        mapY = (int)command.GetParameter("MapY");
                        realX = (float)command.GetParameter("RealX");
                        realY = (float)command.GetParameter("RealY");
                        direction = (FacingDirection)command.GetParameter("Direction");
                        bool onBridge = (bool)command.GetParameter("OnBridge");

                        map = _gameState.MapEntity.FindComponent<MapComponent>().GetMapInstance().GetMapData();
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
                    if (_gameState.MapEntity.FindComponent<MapComponent>().GetMapInstance().GetMapPacket().MapID == mapID)
                    {
                        direction = (FacingDirection)command.GetParameter("Direction");

                        map = _gameState.MapEntity.FindComponent<MapComponent>().GetMapInstance().GetMapData();
                        if (map != null)
                        {
                            map.GetMapEvent(eventID).EventDirection = direction;
                        }
                    }

                    break;
                case ServerCommand.CommandType.ChangeMapEventSprite:

                    mapID = (int)command.GetParameter("MapID");
                    if (_gameState.MapEntity.FindComponent<MapComponent>().GetMapInstance().GetMapPacket().MapID == mapID)
                    {
                        eventID = (int)command.GetParameter("EventID");
                        int spriteID = (int)command.GetParameter("SpriteID");
                        _gameState.MapEntity.FindComponent<MapComponent>().ChangeMapEventSprite(eventID, spriteID);
                    }

                    break;
                case ServerCommand.CommandType.ChangeMapEventRenderPriority:

                    mapID = (int)command.GetParameter("MapID");
                    if (_gameState.MapEntity.FindComponent<MapComponent>().GetMapInstance().GetMapPacket().MapID == mapID)
                    {
                        eventID = (int)command.GetParameter("EventID");
                        RenderPriority priority = (RenderPriority)command.GetParameter("RenderPriority");
                        _gameState.MapEntity.FindComponent<MapComponent>().ChangeMapEventRenderPriority(eventID, priority);
                    }

                    break;
                case ServerCommand.CommandType.ChangeMapEventEnabled:

                    mapID = (int)command.GetParameter("MapID");
                    if (_gameState.MapEntity.FindComponent<MapComponent>().GetMapInstance().GetMapPacket().MapID == mapID)
                    {
                        eventID = (int)command.GetParameter("EventID");
                        bool enabled = (bool)command.GetParameter("Enabled");
                        _gameState.MapEntity.FindComponent<MapComponent>().ChangeMapEventEnabled(eventID, enabled);
                    }

                    break;
                case ServerCommand.CommandType.AddProjectile:

                    mapID = (int)command.GetParameter("MapID");
                    if (_gameState.MapEntity.FindComponent<MapComponent>().GetMapInstance().GetMapPacket().MapID == mapID)
                    {
                        {
                            int dataID = (int)command.GetParameter("DataID");
                            realX = (float)command.GetParameter("RealX");
                            realY = (float)command.GetParameter("RealY");
                            direction = (FacingDirection)command.GetParameter("Direction");
                            bool onBridge = (bool)command.GetParameter("OnBridge");

                            MapProjectile projectile = new MapProjectile(dataID, CharacterType.Player, -1, new Vector2(realX, realY), direction);
                            projectile.OnBridge = onBridge;
                            MapComponent.Instance.AddMapProjectile(projectile);
                        }
                    }

                    break;
                case ServerCommand.CommandType.UpdateProjectile:

                    mapID = (int)command.GetParameter("MapID");
                    if (_gameState.MapEntity.FindComponent<MapComponent>().GetMapInstance().GetMapPacket().MapID == mapID)
                    {
                        projectileID = (int)command.GetParameter("ProjectileID");
                        if (projectileID < MapComponent.Instance.ProjectileEntites.Count)
                        {
                            realX = (float)command.GetParameter("RealX");
                            realY = (float)command.GetParameter("RealY");
                            bool onBridge = (bool)command.GetParameter("OnBridge");
                            bool destroyed = (bool)command.GetParameter("Destroyed");
                            ProjectileComponent component = MapComponent.Instance.ProjectileEntites[projectileID].FindComponent<ProjectileComponent>();
                            component.SetRealPosition(realX, realY);
                            component.SetOnBridge(onBridge);
                            if (destroyed)
                            {
                                MapComponent.Instance.ProjectileEntites[projectileID].Destroy();
                                MapComponent.Instance.ProjectileEntites.RemoveAt(projectileID);
                                MapComponent.Instance.GetMapInstance().RemoveMapProjectile(projectileID);
                            }
                        }
                    }

                    break;
                case ServerCommand.CommandType.AddMapItem:

                    mapID = (int)command.GetParameter("MapID");
                    MapComponent mapComponent = _gameState.MapEntity.FindComponent<MapComponent>();
                    if (mapComponent.GetMapInstance().GetMapPacket().MapID == mapID)
                    {
                        itemID = (int)command.GetParameter("ItemID");
                        count = (int)command.GetParameter("Count");
                        mapX = (int)command.GetParameter("MapX");
                        mapY = (int)command.GetParameter("MapY");
                        playerID = (int)command.GetParameter("PlayerID");
                        bool onBridge = (bool)command.GetParameter("OnBridge");
                        MapItem mapItem = new MapItem(itemID, count, mapX, mapY, playerID, onBridge);
                        MapComponent.Instance.AddMapItem(mapItem);
                    }

                    break;
                case ServerCommand.CommandType.RemoveMapItem:

                    mapID = (int)command.GetParameter("MapID");
                    if (_gameState.MapEntity.FindComponent<MapComponent>().GetMapInstance().GetMapPacket().MapID == mapID)
                    {
                        itemIndex = (int)command.GetParameter("ItemIndex");
                        if (itemIndex < MapComponent.Instance.MapItemEntities.Count)
                        {
                            MapComponent.Instance.MapItemEntities[itemIndex].Destroy();
                            MapComponent.Instance.MapItemEntities.RemoveAt(itemIndex);
                            MapComponent.Instance.GetMapInstance().RemoveMapItem(itemIndex);
                        }
                    }

                    break;
                case ServerCommand.CommandType.UpdateMapItem:

                    mapID = (int)command.GetParameter("MapID");
                    if (_gameState.MapEntity.FindComponent<MapComponent>().GetMapInstance().GetMapPacket().MapID == mapID)
                    {
                        itemIndex = (int)command.GetParameter("ItemIndex");
                        if (itemIndex < MapComponent.Instance.MapItemEntities.Count)
                        {
                            playerID = (int)command.GetParameter("PlayerID");
                            count = (int)command.GetParameter("Count");
                            MapComponent.Instance.MapItemEntities[itemIndex].FindComponent<MapItemComponent>().GetMapItem().PlayerID = playerID;
                            MapComponent.Instance.MapItemEntities[itemIndex].FindComponent<MapItemComponent>().GetMapItem().Count = count;
                        }
                    }

                    break;
                case ServerCommand.CommandType.ShowShop:

                    int shopID = (int)command.GetParameter("ShopID");
                    ShopData data = ShopData.GetData(shopID);
                    if (data != null)
                    {
                        if (InventoryPanel.Instance == null)
                            GameState.Instance.ToggleInventory();
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
                case ServerCommand.CommandType.ShowWorkbench:
                    int workbenchID = (int)command.GetParameter("WorkbenchID");

                    if (InventoryPanel.Instance == null)
                        GameState.Instance.ToggleInventory();

                    _gameState.AddControl(new GUI.WorkbenchPanel(_gameState, workbenchID));

                    break;
            }
        }

        public void AddClientCommand(ClientCommand command)
        {
            _clientCommands.Add(command);
        }

        private void GetClientCommandBytes(ref List<byte> oBytes)
        {
            int numCommands = _clientCommands.Count;

            if (numCommands > 0)
            {
                oBytes.AddRange(BitConverter.GetBytes((int)PacketType.ClientCommand));
                oBytes.AddRange(BitConverter.GetBytes(numCommands));

                for (int i = 0; i < numCommands; i++)
                {
                    while (_clientCommands[i] == null) ;
                    byte[] bytes = _clientCommands[i].GetBytes();
                    oBytes.AddRange(BitConverter.GetBytes(bytes.Length));
                    oBytes.AddRange(bytes);
                }
                _clientCommands.RemoveRange(0, numCommands);
            }
        }

        public void SendMessage(MessagePacket packet)
        {
            _messages.Add(packet);
        }

        private void GetMessagePacketBytes(ref List<byte> oBytes)
        {
            int numMessages = _messages.Count;

            if (numMessages > 0)
            {
                oBytes.AddRange(BitConverter.GetBytes((int)PacketType.SendMessagePackets));
                oBytes.AddRange(BitConverter.GetBytes(numMessages));
                for (int i = 0; i < numMessages; i++)
                {
                    byte[] bytes = _messages[i].GetBytes();
                    oBytes.AddRange(BitConverter.GetBytes(bytes.Length));
                    oBytes.AddRange(bytes);
                }
                _messages.RemoveRange(0, numMessages);
            }
        }

        private void RecieveMessagePackets()
        {
            byte[] bytes = ReadData(sizeof(int), _networkStream);
            int numMessages = BitConverter.ToInt32(bytes, 0);

            for (int i = 0; i < numMessages; i++)
            {
                bytes = ReadData(sizeof(int), _networkStream);
                int messageSize = BitConverter.ToInt32(bytes, 0);
                bytes = ReadData(messageSize, _networkStream);
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
            return _connected && _tcpClient.Connected;
        }

        public void Disconnect()
        {
            if (_connected)
            {
                _networkStream.Close();
                _tcpClient.Close();
                _connected = false;
            }
        }
    }
}

using Genus2D.GameData;
using Genus2D.Networking;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Threading;

namespace RpgServer
{
    public class GameClient
    {

        public static void RecieveClient(TcpClient tcpClient)
        {
            Console.WriteLine("Recieved a client...");

            Stream stream = tcpClient.GetStream();
            byte[] bytes = ReadData(sizeof(int), stream);
            PacketType type = (PacketType)BitConverter.ToInt32(bytes, 0);
            if (type == PacketType.RegisterRequest)
            {
                RegisterClient(stream);
                tcpClient.Close();
            }
            else if (type == PacketType.LoginRequest)
            {
                GameClient gameClient;
                if (LoginClient(tcpClient, out gameClient))
                {
                    Server.Instance.ChangeClientsMap(gameClient);
                }
                else
                {
                    tcpClient.Close();
                }
            }
        }

        private static bool RegisterClient(Stream stream)
        {
            LoginRequest loginRequest = RecieveLoginRequest(stream);
            RegisterResult result = new RegisterResult();
            result.Registered = Server.Instance.GetDatabaseConnection().InsertPlayerQuery(loginRequest.Username, loginRequest.Password, out result.Reason);
            SendRegisterResult(stream, result);

            if (result.Registered)
            {
                Console.WriteLine("(" + loginRequest.Username + ") Successful register.");
                return true;
            }
            else
            {
                Console.WriteLine("(" + loginRequest.Username + ") Failed register.");
                return false;
            }
        }

        private static bool LoginClient(TcpClient tcpClient, out GameClient oClient)
        {
            Stream stream = tcpClient.GetStream();
            LoginRequest loginRequest = RecieveLoginRequest(stream);

            int playerID;
            bool loginValid = Server.Instance.GetDatabaseConnection().LoginQuery(loginRequest.Username, loginRequest.Password, out playerID);

            LoginResult result = new LoginResult();
            if (loginValid  && Server.Instance.FindClientByID(playerID) == null)
            {
                Console.WriteLine("(" + playerID + ", " + loginRequest.Username + ") Successful login.");

                result.LoggedIn = true;
                result.PlayerID = playerID;

                GameClient client = new GameClient(tcpClient, result.PlayerID);
                oClient = client;
            }
            else
            {
                Console.WriteLine("(" + playerID + ", " + loginRequest.Username + ") Failed login.");

                result.LoggedIn = false;
                result.PlayerID = -1;
                oClient = null;
            }

            SendLoginResult(stream, result);
            return result.LoggedIn;

        }

        private static LoginRequest RecieveLoginRequest(Stream stream)
        {
            byte[] bytes = ReadData(sizeof(int), stream);
            int size = BitConverter.ToInt32(bytes, 0);

            bytes = ReadData(size, stream);
            LoginRequest loginRequest = LoginRequest.FromBytes(bytes);
            loginRequest.Username = loginRequest.Username.ToLower();
            return loginRequest;
        }

        private static void SendRegisterResult(Stream stream, RegisterResult result)
        {
            byte[] resultBytes = result.GetBytes();
            byte[] sizeBytes = BitConverter.GetBytes(resultBytes.Length);
            stream.Write(sizeBytes, 0, sizeof(int));
            stream.Flush();
            stream.Write(resultBytes, 0, resultBytes.Length);
            stream.Flush();
        }

        private static void SendLoginResult(Stream stream, LoginResult result)
        {
            byte[] resultBytes = result.GetBytes();
            byte[] sizeBytes = BitConverter.GetBytes(resultBytes.Length);
            stream.Write(sizeBytes, 0, sizeof(int));
            stream.Flush();
            stream.Write(resultBytes, 0, resultBytes.Length);
            stream.Flush();
        }

        private TcpClient _tcpClient;
        private NetworkStream _stream;
        private bool _connected;
        private bool _disconnecting;
        private float _disconnectTimer;

        private bool _mapChanged;
        private bool _running;

        private PlayerPacket _playerPacket;
        private BankData _bankData;
        private bool _bankUpdated;
        private MapInstance _mapInstance;

        private List<ServerCommand> _serverCommands;
        private List<MessagePacket> _messagePackets;

        public bool MessageShowing;
        public bool Banking;
        public bool MovementDisabled;
        public int SelectedOption;
        public int ShopID;
        public int TradePlayerID;
        private TradeRequest _tradeRequest;
        private bool _trading;
        private bool _ignoreEvents;

        private float _movementTimer;
        private float _combatTimer;
        private float _attackTimer;
        private float _regenTimer;

        public bool Dead { get; private set; }
        public CharacterType EnemyCharacterType { get; private set; }
        public int EnemyCharacterID { get; private set; }

        private GameClient(TcpClient tcpClient, int playerID)
        {
            Server.Instance.AddGameClient(this);

            _tcpClient = tcpClient;
            _stream = _tcpClient.GetStream();
            _connected = true;
            _disconnecting = false;
            _disconnectTimer = 5f;

            _mapChanged = false;
            _running = false;

            _playerPacket = Server.Instance.GetDatabaseConnection().RetrievePlayerQuery(playerID, out _bankData);
            _bankUpdated = false;
            _mapInstance = null;

            _serverCommands = new List<ServerCommand>();
            _messagePackets = new List<MessagePacket>();

            MessageShowing = false;
            Banking = false;
            MovementDisabled = false;
            SelectedOption = -1;
            ShopID = -1;

            _movementTimer = 0f;
            _combatTimer = 0f;
            _attackTimer = 0f;
            _regenTimer = 0f;

            Dead = false;

            EnemyCharacterType = CharacterType.Player;
            EnemyCharacterID = -1;

            Thread updatePacketsThread = new Thread(new ThreadStart(UpdatePackets));
            updatePacketsThread.Start();

            TradePlayerID = -1;
            _tradeRequest = null;
            _trading = false;
            _ignoreEvents = false;
        }

        private static byte[] ReadData(int size, Stream stream)
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

        private void UpdatePackets()
        {
            long ticks = DateTime.Now.Ticks;
            long prevTicks = ticks;
            double deltaTime = 0.0;
            double updateTimer = 0.0;

            while (true)
            {
                if (Connected() && !_disconnecting)
                {

                    prevTicks = ticks;
                    ticks = DateTime.Now.Ticks;
                    deltaTime = (ticks - prevTicks) / 10000000.0;

                    if (updateTimer <= 0.0)
                    {
                        updateTimer = 0.1;
                        if (_mapInstance != null)
                        {
                            try
                            {
                                SendMapPacket();
                                SendPlayerPackets();
                                SendServerCommands();
                                RecieveClientCommands();
                                RecieveMessagePackets();
                                SendMessagePackets();
                                SendBankPacket();
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine("Client Disconnected");
                                _disconnecting = true;
                            }
                        }
                    }
                    else
                    {
                        updateTimer -= deltaTime;
                    }
                }
                else
                {
                    break;
                }

            }
        }

        public PlayerPacket GetPacket()
        {
            return _playerPacket;
        }

        public void SetMapInstance(MapInstance instance)
        {
            _mapInstance = instance;
            _mapChanged = true;
        }

        public float GetMovementSpeed()
        {
            float speed = 64f;
            if (_running && _playerPacket.Data.Stamina > 0)
                speed *= 2.5f;
            return speed;
        }

        public void AddServerCommand(ServerCommand command)
        {
            if (command != null)
            {
                _serverCommands.Add(command);
            }
        }

        private void SendServerCommands()
        {
            while (_serverCommands.Count > 0)
            {
                ServerCommand command = _serverCommands[0];
                if (command == null) break;

                byte[] bytes = BitConverter.GetBytes((int)PacketType.ServerCommand);
                _stream.Write(bytes, 0, sizeof(int));
                _stream.Flush();

                bytes = command.GetBytes();
                _stream.Write(BitConverter.GetBytes(bytes.Length), 0, sizeof(int));
                _stream.Flush();
                _stream.Write(bytes, 0, bytes.Length);
                _stream.Flush();

                _serverCommands.RemoveAt(0);
            }
        }

        public void RecieveMessage(MessagePacket message)
        {
            if (message.TargetType == MessagePacket.MessageTarget.Public)
            {
                _messagePackets.Add(message);
            }
            else
            {
                if (message.TargetID == _playerPacket.PlayerID)
                    _messagePackets.Add(message);
            }
        }

        private void RecieveMessagePackets()
        {
            byte[] bytes = BitConverter.GetBytes((int)PacketType.SendMessagePackets);
            _stream.Write(bytes, 0, sizeof(int));
            _stream.Flush();

            bytes = ReadData(sizeof(int), _stream);
            int numMessages = BitConverter.ToInt32(bytes, 0);

            for (int i = 0; i < numMessages; i++)
            {
                bytes = ReadData(sizeof(int), _stream);
                int messageSize = BitConverter.ToInt32(bytes, 0);

                bytes = ReadData(messageSize, _stream);
                MessagePacket packet = MessagePacket.FromBytes(bytes);
                packet.Message = _playerPacket.Username + ": " + packet.Message; // the server signs the messages with the username
                packet.PlayerID = _playerPacket.PlayerID;
                Server.Instance.SendMessage(packet);
            }
        }

        private void SendMessagePackets()
        {
            if (_messagePackets.Count > 0)
            {
                byte[] bytes = BitConverter.GetBytes((int)PacketType.RecieveMessagePackets);
                _stream.Write(bytes, 0, sizeof(int));

                _stream.Write(BitConverter.GetBytes(_messagePackets.Count), 0, sizeof(int));

                for (int i = 0; i < _messagePackets.Count; i++)
                {
                    bytes = _messagePackets[i].GetBytes();
                    _stream.Write(BitConverter.GetBytes(bytes.Length), 0, sizeof(int));
                    _stream.Write(bytes, 0, bytes.Length);
                }
                _stream.Flush();
                _messagePackets.Clear();
            }
        }

        private void RecieveClientCommands()
        {
            byte[] bytes = BitConverter.GetBytes((int)PacketType.ClientCommand);
            _stream.Write(bytes, 0, sizeof(int));
            _stream.Flush();

            bytes = ReadData(sizeof(int), _stream);
            int numCommands = BitConverter.ToInt32(bytes, 0);

            int itemIndex;
            Tuple<int, int> itemInfo;
            MapItem mapItem;
            int playerID;
            GameClient other;

            for (int i = 0; i < numCommands; i++)
            {
                bytes = ReadData(sizeof(int), _stream);
                int commandSize = BitConverter.ToInt32(bytes, 0);

                bytes = ReadData(commandSize, _stream);
                ClientCommand command = ClientCommand.FromBytes(bytes);
                
                switch (command.GetCommandType())
                {
                    case ClientCommand.CommandType.CloseMessage:
                        MessageShowing = false;
                        break;
                    case ClientCommand.CommandType.SelectOption:
                        int option = (int)command.GetParameter("Option");
                        SelectedOption = option;
                        MessageShowing = false;
                        break;
                    case ClientCommand.CommandType.MovePlayer:
                        MovementDirection direction = (MovementDirection)command.GetParameter("Direction");
                        Move(direction);
                        break;
                    case ClientCommand.CommandType.ToggleRunning:
                        bool running = (bool)command.GetParameter("Running");
                        _running = running;
                        break;
                    case ClientCommand.CommandType.ActionTrigger:
                        ActionTrigger();
                        break;
                    case ClientCommand.CommandType.SelectItem:
                        itemIndex = (int)command.GetParameter("ItemIndex");
                        itemInfo = _playerPacket.Data.GetInventoryItem(itemIndex);
                        if (itemInfo != null)
                        {
                            ItemData data = ItemData.GetItemData(itemInfo.Item1);
                            switch (data.GetItemType())
                            {
                                case ItemData.ItemType.Tool:

                                    break;
                                case ItemData.ItemType.Consumable:

                                    break;
                                case ItemData.ItemType.Material:

                                    break;
                                case ItemData.ItemType.Equipment:
                                    _playerPacket.Data.EquipItem(itemIndex);
                                    break;
                                case ItemData.ItemType.Ammo:
                                    _playerPacket.Data.EquipAmmo(itemIndex);
                                    break;
                            }
                        }
                        break;
                    case ClientCommand.CommandType.DropItem:
                        itemIndex = (int)command.GetParameter("ItemIndex");
                        int count = (int)command.GetParameter("Count");
                        itemInfo = _playerPacket.Data.GetInventoryItem(itemIndex);

                        if (itemInfo != null)
                        {
                            if (count > itemInfo.Item2)
                                count = itemInfo.Item2;
                            mapItem = new MapItem(itemInfo.Item1, count, _playerPacket.PositionX, _playerPacket.PositionY, _playerPacket.PlayerID, _playerPacket.OnBridge);
                            _playerPacket.Data.RemoveInventoryItemAt(itemIndex, count);
                            _mapInstance.AddMapItem(mapItem);
                        }
                        break;
                    case ClientCommand.CommandType.RemoveEquipment:
                        EquipmentSlot slot = (EquipmentSlot)command.GetParameter("EquipmentIndex");
                        _playerPacket.Data.UnequipItem(slot);
                        break;
                    case ClientCommand.CommandType.RemoveAmmo:
                        _playerPacket.Data.UnequipAmmo();
                        break;
                    case ClientCommand.CommandType.PickupItem:
                        itemIndex = (int)command.GetParameter("ItemIndex");
                        int signature = (int)command.GetParameter("Signature");
                        mapItem = _mapInstance.GetMapItem(itemIndex);
                        if (mapItem != null)
                        {
                            if (!mapItem.PickedUp && (mapItem.PlayerID == -1 || mapItem.PlayerID == _playerPacket.PlayerID))
                            {
                                if (mapItem.GetSignature() == signature && mapItem.OnBridge == _playerPacket.OnBridge)
                                {
                                    int distance = (int)new Vector2(mapItem.MapX - _playerPacket.PositionX, mapItem.MapY - _playerPacket.PositionY).Length;
                                    if (distance <= 1)
                                    {
                                        int added = _playerPacket.Data.AddInventoryItem(mapItem.ItemID, mapItem.Count);
                                        if (added != 0)
                                        {
                                            if (added < mapItem.Count)
                                            {
                                                mapItem.Count = mapItem.Count - added;
                                                _mapInstance.UpdateMapItem(itemIndex);
                                            }
                                            else
                                            {
                                                mapItem.PickedUp = true;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        break;
                    case ClientCommand.CommandType.AttackPlayer:
                        if (_mapInstance.GetMapData().PvpEnabled == false)
                            break;

                        playerID = (int)command.GetParameter("PlayerID");
                        if (EnemyCanAttack(CharacterType.Player, playerID, _mapInstance.GetMapData().MultiCombat))
                        {
                            other = _mapInstance.FindGameClient(playerID);
                            if (other != null && other.EnemyCanAttack(CharacterType.Player, _playerPacket.PlayerID, _mapInstance.GetMapData().MultiCombat))
                            {
                                int pX = _playerPacket.PositionX;
                                int pY = _playerPacket.PositionY;
                                EnemyCharacterType = CharacterType.Player;
                                EnemyCharacterID = playerID;
                                if (other._playerPacket.PositionX < pX && other._playerPacket.PositionY == pY)
                                {
                                    ChangeDirection(FacingDirection.Left);
                                    CombatCheck(pX - 1, pY);
                                }
                                else if (other._playerPacket.PositionX > pX && other._playerPacket.PositionY == pY)
                                {
                                    ChangeDirection(FacingDirection.Right);
                                    CombatCheck(pX + 1, pY);
                                }
                                else if (other._playerPacket.PositionX == pX && other._playerPacket.PositionY < pY)
                                {
                                    ChangeDirection(FacingDirection.Up);
                                    CombatCheck(pX, pY - 1);
                                }
                                else if (other._playerPacket.PositionX == pX && other._playerPacket.PositionY > pY)
                                {
                                    ChangeDirection(FacingDirection.Down);
                                    CombatCheck(pX, pY + 1);
                                }
                            }
                            
                        }
                        break;
                    case ClientCommand.CommandType.AttackEnemy:
                        int enemyID = (int)command.GetParameter("EnemyID");
                        if (EnemyCanAttack(CharacterType.Enemy, enemyID, _mapInstance.GetMapData().MultiCombat))
                        {
                            MapEnemy otherEnemy = _mapInstance.FindMapEnemy(enemyID);
                            bool multiCombat = _mapInstance.GetMapData().MultiCombat;
                            if (otherEnemy != null && otherEnemy.EnemyCanAttack(CharacterType.Player, _playerPacket.PlayerID, multiCombat))
                            {
                                EnemyCharacterType = CharacterType.Enemy;
                                EnemyCharacterID = enemyID;

                                int pX = _playerPacket.PositionX;
                                int pY = _playerPacket.PositionY;
                                if (otherEnemy.MapX < pX && otherEnemy.MapY == pY)
                                {
                                    ChangeDirection(FacingDirection.Left);
                                    CombatCheck(pX - 1, pY);
                                }
                                else if (otherEnemy.MapX > pX && otherEnemy.MapY == pY)
                                {
                                    ChangeDirection(FacingDirection.Right);
                                    CombatCheck(pX + 1, pY);
                                }
                                else if (otherEnemy.MapX == pX && otherEnemy.MapY < pY)
                                {
                                    ChangeDirection(FacingDirection.Up);
                                    CombatCheck(pX, pY - 1);
                                }
                                else if (otherEnemy.MapX == pX && otherEnemy.MapY > pY)
                                {
                                    ChangeDirection(FacingDirection.Down);
                                    CombatCheck(pX, pY + 1);
                                }
                            }

                        }
                        break;
                    case ClientCommand.CommandType.CloseShop:
                        ShopID = -1;
                        break;
                    case ClientCommand.CommandType.BuyShopItem:
                        if (ShopID != -1)
                        {
                            ShopData data = ShopData.GetData(ShopID);
                            if (data != null)
                            {
                                itemIndex = (int)command.GetParameter("ItemIndex");
                                count = (int)command.GetParameter("Count");
                                if (itemIndex >= 0 && itemIndex < data.ShopItems.Count && count > 0)
                                {
                                    ShopData.ShopItem shopItem = data.ShopItems[itemIndex];
                                    if (shopItem.ItemID != -1)
                                    {
                                        int totalCost = shopItem.Cost * count;
                                        if (_playerPacket.Data.Gold >= totalCost)
                                        {
                                            _playerPacket.Data.Gold -= totalCost;
                                            int added = _playerPacket.Data.AddInventoryItem(shopItem.ItemID, count);
                                            int remainder = count - added;
                                            for (int j = 0; j < remainder; j++)
                                            {
                                                _playerPacket.Data.Gold += shopItem.Cost;
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        break;
                    case ClientCommand.CommandType.SellShopItem:
                        itemIndex = (int)command.GetParameter("ItemIndex");
                        count = (int)command.GetParameter("Count");

                        itemInfo = _playerPacket.Data.GetInventoryItem(itemIndex);
                        if (itemInfo != null && count > 0)
                        {
                            ItemData itemData = ItemData.GetItemData(itemInfo.Item1);
                            if (itemData.Sellable)
                            {
                                if (count > itemInfo.Item2)
                                {
                                    count = itemInfo.Item2;
                                }
                                _playerPacket.Data.RemoveInventoryItemAt(itemIndex, count);
                                int sellPrice = count * itemData.SellPrice;
                                _playerPacket.Data.Gold += sellPrice;
                            }
                            else
                            {
                                _messagePackets.Add(new MessagePacket("Item cannot be sold."));
                            }
                        }

                        break;
                    case ClientCommand.CommandType.TradeRequest:
                        playerID = (int)command.GetParameter("PlayerID");
                        other = _mapInstance.FindGameClient(playerID);
                        if (other != null)
                        {
                            Vector2 pos1 = GetMapPosition();
                            Vector2 pos2 = other.GetMapPosition();
                            if ((pos1 - pos2).Length <= 1)
                            {
                                TradePlayerID = playerID;
                                if (other.TradePlayerID != _playerPacket.PlayerID)
                                    RequestTrade(other);
                                else
                                    StartTrade(other);
                            }
                        }

                        break;
                    case ClientCommand.CommandType.AcceptTrade:
                        if (_tradeRequest.TradeOffer1.PlayerID == _playerPacket.PlayerID)
                        {
                            playerID = _tradeRequest.TradeOffer2.PlayerID;
                            _tradeRequest.TradeOffer1.Accepted = true;
                        }
                        else
                        {
                            playerID = _tradeRequest.TradeOffer1.PlayerID;
                            _tradeRequest.TradeOffer2.Accepted = true;
                        }
                        _mapInstance.FindGameClient(playerID).AddServerCommand(new ServerCommand(ServerCommand.CommandType.AcceptTrade));

                        break;
                    case ClientCommand.CommandType.CancelTrade:
                        StopTrading();

                        break;
                    case ClientCommand.CommandType.AddTradeItem:
                        if (_trading)
                        {
                            itemIndex = (int)command.GetParameter("ItemIndex");
                            count = (int)command.GetParameter("Count");
                            AddTradeItem(itemIndex, count);
                        }

                        break;
                    case ClientCommand.CommandType.RemoveTradeItem:
                        if (_trading)
                        {
                            itemIndex = (int)command.GetParameter("ItemIndex");
                            count = (int)command.GetParameter("Count");
                            RemoveTradeItem(itemIndex, count);
                        }

                        break;
                    case ClientCommand.CommandType.CloseBank:

                        StopBanking();

                        break;
                    case ClientCommand.CommandType.AddBankItem:
                        itemIndex = (int)command.GetParameter("ItemIndex");
                        count = (int)command.GetParameter("Count");
                        AddBankItem(itemIndex, count);

                        break;
                    case ClientCommand.CommandType.RemoveBankItem:
                        itemIndex = (int)command.GetParameter("ItemIndex");
                        count = (int)command.GetParameter("Count");
                        RemoveBankItem(itemIndex, count);

                        break;

                }
                
            }
        }

        private void SendPlayerPackets()
        {
            List<PlayerPacket> packets = _mapInstance.GetPlayerPackets();

            PacketType type = PacketType.PlayerPacket;
            byte[] bytes = BitConverter.GetBytes((int)type);
            _stream.Write(bytes, 0, bytes.Length);
            _stream.Flush();

            int numClients = packets.Count;
            bytes = BitConverter.GetBytes(numClients);
            _stream.Write(bytes, 0, sizeof(int));
            _stream.Flush();

            for (int i = 0; i < numClients; i++)
            {
                PlayerPacket packet = packets[i];

                bool isLocalPlayer = packet.PlayerID == _playerPacket.PlayerID;
                bytes = packet.GetBytes(isLocalPlayer);
                byte[] sizeBytes = BitConverter.GetBytes(bytes.Length);
                _stream.Write(sizeBytes, 0, sizeof(int));
                _stream.Flush();

                _stream.Write(bytes, 0, bytes.Length);
                _stream.Flush();
            }

        }

        private void SendMapPacket()
        {
            if (_mapChanged)
            {
                MapPacket mapPacket = _mapInstance.GetMapPacket();

                PacketType type = PacketType.MapPacket;
                byte[] bytes = BitConverter.GetBytes((int)type);
                _stream.Write(bytes, 0, sizeof(int));
                _stream.Flush();

                byte[] mapBytes = mapPacket.GetBytes();
                bytes = BitConverter.GetBytes(mapBytes.Length);
                _stream.Write(bytes, 0, sizeof(int));
                _stream.Flush();
                _stream.Write(mapBytes, 0, mapBytes.Length);
                _stream.Flush();
                _mapChanged = false;
            }
        }

        private void SendBankPacket()
        {
            if (_bankUpdated)
            {
                _bankUpdated = false;

                PacketType type = PacketType.BankPacket;
                byte[] bytes = BitConverter.GetBytes((int)type);
                _stream.Write(bytes, 0, sizeof(int));
                _stream.Flush();
                bytes = _bankData.GetBytes();
                _stream.Write(BitConverter.GetBytes(bytes.Length), 0, sizeof(int));
                _stream.Write(bytes, 0, bytes.Length);
                _stream.Flush();
            }
        }

        public void SetMapID(int id)
        {
            if (_playerPacket.MapID != id)
            {
                _playerPacket.MapID = id;
                _mapInstance.RemoveClient(this);
                _mapInstance.GetServer().ChangeClientsMap(this);
            }
        }

        public int GetMapID()
        {
            return _playerPacket.MapID;
        }

        public bool Moving()
        {
            int posX = _playerPacket.PositionX * 32;
            int posY = _playerPacket.PositionY * 32;
            return (posX != _playerPacket.RealX || posY != _playerPacket.RealY);
        }

        public Hitbox GetHitbox()
        {
            Hitbox hitbox = new Hitbox();

            hitbox.X = _playerPacket.RealX; 
            hitbox.Y = _playerPacket.RealY;
            SpriteData data = SpriteData.GetSpriteData(_playerPacket.SpriteID);
            if (data != null)
            {
                if (_playerPacket.Direction == FacingDirection.Left || _playerPacket.Direction == FacingDirection.Right)
                {
                    hitbox.Width = data.HorizontalBounds.X;
                    hitbox.Height = data.HorizontalBounds.Y;
                }
                else
                {
                    hitbox.Width = data.VerticalBounds.X;
                    hitbox.Height = data.VerticalBounds.Y;
                }
            }

            return hitbox;
        }

        public void ActionTrigger()
        {
            if (CanMove())
            {
                int targetX = _playerPacket.PositionX;
                int targetY = _playerPacket.PositionY;
                switch (_playerPacket.Direction)
                {
                    case FacingDirection.Down:
                        targetY += 1;
                        break;
                    case FacingDirection.Left:
                        targetX -= 1;
                        break;
                    case FacingDirection.Right:
                        targetX += 1;
                        break;
                    case FacingDirection.Up:
                        targetY -= 1;
                        break;
                }

                for (int i = 0; i < 3; i++)
                {
                    Tuple<int, int> tileInfo = _mapInstance.GetMapData().GetTile(i, targetX, targetY);
                    if (tileInfo.Item2 == -1)
                        continue;
                    if (TilesetData.GetTileset(tileInfo.Item2).GetCounterFlag(tileInfo.Item1))
                    {
                        targetX += _playerPacket.Direction == FacingDirection.Left ? -1 : _playerPacket.Direction == FacingDirection.Right ? 1 : 0;
                        targetY += _playerPacket.Direction == FacingDirection.Down ? 1 : _playerPacket.Direction == FacingDirection.Up ? -1 : 0;
                        break;
                    }
                }

                if (!CheckEventTriggers(targetX, targetY, EventTriggerType.Action))
                {
                    if (_mapInstance.GetMapData().MultiCombat)
                        EnemyCharacterID = -1;
                    CombatCheck(targetX, targetY);
                }
            }
        }

        private void RequestTrade(GameClient other)
        {
            ServerCommand command = new ServerCommand(ServerCommand.CommandType.TradeRequest);
            command.SetParameter("PlayerID", _playerPacket.PlayerID);
            command.SetParameter("PlayerName", _playerPacket.Username);
            other.AddServerCommand(command);
        }

        private void StartTrade(GameClient other)
        {
            TradeRequest tradeRequest = new TradeRequest(other._playerPacket.PlayerID, _playerPacket.PlayerID);
            tradeRequest.TradeOffer1.FreeSlots = other._playerPacket.Data.GetFreeInventorySlots();
            tradeRequest.TradeOffer2.FreeSlots = _playerPacket.Data.GetFreeInventorySlots();

            _tradeRequest = tradeRequest;
            other._tradeRequest = tradeRequest;
            _trading = true;
            other._trading = true;
            this.MovementDisabled = true;
            other.MovementDisabled = true;

            ServerCommand sCommand = new ServerCommand(ServerCommand.CommandType.StartTrade);
            sCommand.SetParameter("PlayerID", other._playerPacket.PlayerID);
            sCommand.SetParameter("PlayerName", other._playerPacket.Username);
            AddServerCommand(sCommand);

            sCommand = new ServerCommand(ServerCommand.CommandType.StartTrade);
            sCommand.SetParameter("PlayerID", _playerPacket.PlayerID);
            sCommand.SetParameter("PlayerName", _playerPacket.Username);
            other.AddServerCommand(sCommand);
        }

        public void StopTrading()
        {
            if (_trading)
            {
                TradeRequest.TradeOffer myOffer, othersOffer;
                if (_tradeRequest.TradeOffer1.PlayerID == _playerPacket.PlayerID)
                {
                    myOffer = _tradeRequest.TradeOffer1;
                    othersOffer = _tradeRequest.TradeOffer2;
                }
                else
                {
                    myOffer = _tradeRequest.TradeOffer2;
                    othersOffer = _tradeRequest.TradeOffer1;
                }

                bool accepted = _tradeRequest.Accepted();
                EndTrade(myOffer, othersOffer, accepted);
            }
        }

        private void EndTrade(TradeRequest.TradeOffer myOffer, TradeRequest.TradeOffer othersOffer, bool accepted)
        {
            if (accepted)
            {
                if (myOffer.FreeSlots >= othersOffer.NumItems() && othersOffer.FreeSlots >= myOffer.NumItems())
                {
                    for (int i = 0; i < othersOffer.NumItems(); i++)
                    {
                        Tuple<int, int> itemInfo = othersOffer.GetItem(i);
                        _playerPacket.Data.AddInventoryItem(itemInfo.Item1, itemInfo.Item2);
                    }
                }
                else
                {
                    if (myOffer.FreeSlots < othersOffer.NumItems())
                    {
                        myOffer.Accepted = false;
                        othersOffer.Accepted = false;
                        ServerCommand command = new ServerCommand(ServerCommand.CommandType.CantTrade);
                        this.AddServerCommand(command);
                        GameClient other = _mapInstance.FindGameClient(othersOffer.PlayerID);
                        other.AddServerCommand(command);

                        MessagePacket message = new MessagePacket("You only have " + myOffer.FreeSlots + " inventory slots.");
                        this.RecieveMessage(message);
                        message = new MessagePacket(_playerPacket.Username + " only has " + myOffer.FreeSlots + " inventory slots.");
                        other.RecieveMessage(message);
                    }
                    return;
                }
            }
            else
            {
                for (int i = 0; i < myOffer.NumItems(); i++)
                {
                    Tuple<int, int> itemInfo = myOffer.GetItem(i);
                    _playerPacket.Data.AddInventoryItem(itemInfo.Item1, itemInfo.Item2);
                }
            }

            AddServerCommand(new ServerCommand(ServerCommand.CommandType.EndTrade));
            this.TradePlayerID = -1;
            _tradeRequest = null;
            _trading = false; ;
            MovementDisabled = false;
        }

        private void AddTradeItem(int itemIndex, int count)
        {
            Tuple<int, int> itemInfo = _playerPacket.Data.GetInventoryItem(itemIndex);
            if (itemInfo != null)
            {
                if (count > itemInfo.Item2)
                    count = itemInfo.Item2;

                GameClient other;
                int added = 0;

                if (_tradeRequest.TradeOffer1.PlayerID == _playerPacket.PlayerID)
                {
                    added = _tradeRequest.TradeOffer1.AddItem(itemInfo.Item1, count);
                    _playerPacket.Data.RemoveInventoryItemAt(itemIndex, added);
                    _tradeRequest.TradeOffer1.FreeSlots = _playerPacket.Data.GetFreeInventorySlots();

                    other = _mapInstance.FindGameClient(_tradeRequest.TradeOffer2.PlayerID);
                }
                else
                {
                    added = _tradeRequest.TradeOffer2.AddItem(itemInfo.Item1, count);
                    _playerPacket.Data.RemoveInventoryItemAt(itemIndex, added);
                    _tradeRequest.TradeOffer2.FreeSlots = _playerPacket.Data.GetFreeInventorySlots();

                    other = _mapInstance.FindGameClient(_tradeRequest.TradeOffer1.PlayerID);
                }

                if (other != null && added > 0)
                {
                    ServerCommand command = new ServerCommand(ServerCommand.CommandType.AddTradeItem);
                    command.SetParameter("ItemID", itemInfo.Item1);
                    command.SetParameter("Count", added);
                    other.AddServerCommand(command);

                    _tradeRequest.TradeOffer1.Accepted = false;
                    _tradeRequest.TradeOffer2.Accepted = false;
                }
            }
        }

        private void RemoveTradeItem(int itemIndex, int count)
        {
            Tuple<int, int> itemInfo = null;
            GameClient other = null;
            int removed = 0;

            if (_tradeRequest.TradeOffer1.PlayerID == _playerPacket.PlayerID)
            {
                itemInfo = _tradeRequest.TradeOffer1.GetItem(itemIndex);
                if (itemInfo != null)
                {
                    removed = _tradeRequest.TradeOffer1.RemoveItem(itemIndex, count);
                    if (removed > 0)
                    {
                        _playerPacket.Data.AddInventoryItem(itemInfo.Item1, removed);
                        _tradeRequest.TradeOffer1.FreeSlots = _playerPacket.Data.GetFreeInventorySlots();
                        other = _mapInstance.FindGameClient(_tradeRequest.TradeOffer2.PlayerID);
                        _tradeRequest.TradeOffer1.Accepted = false;
                        _tradeRequest.TradeOffer2.Accepted = false;
                    }
                }

            }
            else
            {
                itemInfo = _tradeRequest.TradeOffer2.GetItem(itemIndex);
                if (itemInfo != null)
                {
                    removed = _tradeRequest.TradeOffer2.RemoveItem(itemIndex, count);
                    if (removed > 0)
                    {
                        _playerPacket.Data.AddInventoryItem(itemInfo.Item1, removed);
                        _tradeRequest.TradeOffer2.FreeSlots = _playerPacket.Data.GetFreeInventorySlots();
                        other = _mapInstance.FindGameClient(_tradeRequest.TradeOffer1.PlayerID);
                        _tradeRequest.TradeOffer1.Accepted = false;
                        _tradeRequest.TradeOffer2.Accepted = false;
                    }
                }
            }

            if (other != null && removed > 0)
            {
                ServerCommand command = new ServerCommand(ServerCommand.CommandType.RemoveTradeItem);
                command.SetParameter("ItemIndex", itemIndex);
                command.SetParameter("Count", removed);
                other.AddServerCommand(command);
            }
        }

        public void StartBanking()
        {
            if (!Banking)
            {
                Banking = true;
                MovementDisabled = true;
                _bankUpdated = true;

                ServerCommand command = new ServerCommand(ServerCommand.CommandType.OpenBank);
                this.AddServerCommand(command);
            }
        }

        public void StopBanking()
        {
            if (Banking)
            {
                Banking = false;
                MovementDisabled = false;
            }
        }

        public void AddBankItem(int index, int count)
        {
            Tuple<int, int> itemInfo = _playerPacket.Data.GetInventoryItem(index);
            if (itemInfo != null)
            {
                if (count >= itemInfo.Item2)
                {
                    count = itemInfo.Item2;
                    _playerPacket.Data.RemoveInventoryItem(index);
                }
                else
                {
                    _playerPacket.Data.RemoveInventoryItemAt(index, count);
                }
                _bankData.AddBankItem(itemInfo.Item1, count);
                _bankUpdated = true;
            }
        }

        public void RemoveBankItem(int index, int count)
        {
            _bankData.RemoveBankItem(index, count, _playerPacket.Data);
            _bankUpdated = true;
        }

        public void Update(float deltaTime)
        {
            if (_disconnecting)
            {
                if (_disconnectTimer > 0)
                    _disconnectTimer -= deltaTime;
                else
                {
                    Disconnect();
                    _disconnecting = false;
                    return;
                }
            }

            //trading
            if (_trading)
            {
                if (_tradeRequest.Accepted())
                {
                    StopTrading();
                }
                else
                {
                    //check for disconnects
                    int playerID;
                    if (_tradeRequest.TradeOffer1.PlayerID == _playerPacket.PlayerID)
                        playerID = _tradeRequest.TradeOffer2.PlayerID;
                    else
                        playerID = _tradeRequest.TradeOffer1.PlayerID;
                    GameClient other = _mapInstance.FindGameClient(playerID);
                    if (other == null || !other._trading || other._disconnecting || _disconnecting)
                    {
                        this.StopTrading();
                    }
                }
            }
            
            //respawn
            if (Dead)
            {
                SpawnPoint spawn = MapInfo.GetSpawnPoint(0);
                MapInstance prevInstance = _mapInstance;
                SetMapID(spawn.MapID);
                SetMapPosition(spawn.MapX, spawn.MapY);
                _playerPacket.Data.HP = _playerPacket.Data.GetMaxHP();
                Dead = false;
                return;
            }

            //update timers
            if (_movementTimer > 0)
                _movementTimer -= deltaTime;

            if (_combatTimer > 0) _combatTimer -= deltaTime;
            else EnemyCharacterID = -1;

            if (_attackTimer > 0)
                _attackTimer -= deltaTime;

            //regen stats
            if (_regenTimer <= 0)
            {
                if (_combatTimer <= 0)
                {
                    _playerPacket.Data.HP++;
                    if (_playerPacket.Data.HP > _playerPacket.Data.GetMaxHP())
                        _playerPacket.Data.HP = _playerPacket.Data.GetMaxHP();

                    _playerPacket.Data.MP++;
                    if (_playerPacket.Data.MP > _playerPacket.Data.GetMaxMP())
                        _playerPacket.Data.MP = _playerPacket.Data.GetMaxMP();
                }

                if (!_running || !Moving())
                {
                    _playerPacket.Data.Stamina++;
                    if (_playerPacket.Data.Stamina > _playerPacket.Data.GetMaxStamina())
                        _playerPacket.Data.Stamina = _playerPacket.Data.GetMaxStamina();
                }
                else
                {
                    if (Moving())
                    {
                        _playerPacket.Data.Stamina--;
                        if (_playerPacket.Data.Stamina < 0)
                            _playerPacket.Data.Stamina = 0;
                    }
                }

                _regenTimer = 1;
            }
            else
            {
                _regenTimer -= deltaTime;
            }

            //movement
            if (!MovementDisabled)
            {
                _playerPacket.MovementSpeed = GetMovementSpeed();

                if (Moving())
                {
                    Vector2 realPos = new Vector2(_playerPacket.RealX, _playerPacket.RealY);
                    Vector2 targetPos = new Vector2(_playerPacket.PositionX * 32, _playerPacket.PositionY * 32);
                    Vector2 dir = targetPos - realPos;
                    Vector2 endPos = realPos + (dir.Normalized() * _playerPacket.MovementSpeed * deltaTime);

                    Vector2 dir2 = targetPos - endPos;

                    if (dir.Length < dir2.Length || dir.Length < 0.5f)
                    {
                        realPos = targetPos;
                        CheckEventTriggers(_playerPacket.PositionX, _playerPacket.PositionY, EventTriggerType.PlayerTouch);

                        if (!_mapInstance.GetBridgeFlag(_playerPacket.PositionX, _playerPacket.PositionY))
                        {
                            _playerPacket.OnBridge = false;
                        }
                        _ignoreEvents = false;
                    }
                    else
                    {
                        realPos = endPos;
                    }

                    _playerPacket.RealX = realPos.X;
                    _playerPacket.RealY = realPos.Y;
                }
            }
        }

        public Vector2 GetMapPosition()
        {
            Vector2 pos = new Vector2(_playerPacket.PositionX, _playerPacket.PositionY);
            return pos;
        }

        public void SetMapPosition(int x, int y)
        {
            _playerPacket.PositionX = x;
            _playerPacket.PositionY = y;
            _playerPacket.RealX = x * 32;
            _playerPacket.RealY = y * 32;
        }

        public bool CanMove()
        {
            if (_movementTimer > 0f) return false;
            if (MovementDisabled) return false;
            if (Moving()) return false;
            if (ShopID != -1) return false;
            return true;
        }

        public void Move(MovementDirection direction, bool ignoreEvents = false)
        {
            if (CanMove())
            {
                _ignoreEvents = ignoreEvents;
                int x = _playerPacket.PositionX;
                int y = _playerPacket.PositionY;
                int targetX = _playerPacket.PositionX;
                int targetY = _playerPacket.PositionY;
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
                if (_playerPacket.Direction != facingDirection)
                {
                    _playerPacket.Direction = facingDirection;
                    _movementTimer = 0.15f;
                }
                else
                {
                    bool bridgeEntry = _mapInstance.MapTilesetPassable(x, y) && _playerPacket.OnBridge;
                    if (_mapInstance.MapTileCharacterPassable(x, y, true, _playerPacket.OnBridge, bridgeEntry, direction) &&
                        _mapInstance.MapTileCharacterPassable(targetX, targetY, true, _playerPacket.OnBridge, bridgeEntry, entryDirection))
                    {
                        _playerPacket.PositionX = targetX;
                        _playerPacket.PositionY = targetY;
                        if (_mapInstance.GetBridgeFlag(targetX, targetY))
                        {
                            if (_mapInstance.MapTilesetPassable(targetX, targetY))
                                _playerPacket.OnBridge = true;
                        }
                    }
                    else
                    {
                        CheckEventTriggers(targetX, targetY, EventTriggerType.PlayerTouch);
                    }
                }
            }
        }

        public void ChangeDirection(FacingDirection direction)
        {
            _playerPacket.Direction = direction;
        }

        public void GainExperience(int experience)
        {
            if (experience <= 0)
                return;

            if (_playerPacket.Data.Level < SystemData.GetData().MaxLvl)
            {
                int xpNeeded = _playerPacket.Data.ExperienceToLevel();
                int xpCurrent = _playerPacket.Data.Experience;
                int addedXp = xpCurrent + experience;

                if (addedXp < xpNeeded)
                {
                _playerPacket.Data.Experience = addedXp;
                }
                else
                {
                    int remainder = addedXp - xpNeeded;
                    _playerPacket.Data.Experience = 0;
                    _playerPacket.Data.Level++;
                    GainExperience(remainder);
                }
            }
        }

        private bool CheckEventTriggers(int x, int y, EventTriggerType triggerType)
        {
            if (_ignoreEvents)
                return false;

            MapData mapData = _mapInstance.GetMapData();
            for (int i = 0; i < mapData.MapEventsCount(); i++)
            {
                MapEvent mapEvent = mapData.GetMapEvent(i);
                if (!mapEvent.Enabled) continue;
                if (mapEvent.MapX == x && mapEvent.MapY == y)
                {
                    if (mapEvent.EventID == -1)
                        return false;
                    if (mapEvent.TriggerType == triggerType)
                    {
                        _mapInstance.GetEventInterpreter().TriggerEventData(this, mapEvent);
                        return true;
                    }
                }
            }
            return false;
        }

        private void CombatCheck(int x, int y)
        {
            if (_attackTimer <= 0)
            {
                int weaponID = _playerPacket.Data.GetEquipedItemID((int)EquipmentSlot.Weapon);
                if (weaponID != -1)
                {
                    ItemData data = ItemData.GetItemData(weaponID);
                    bool multiCombat = _mapInstance.GetMapData().MultiCombat;

                    switch ((AttackStyle)data.GetItemStat("AttackStyle"))
                    {
                        case AttackStyle.Melee:

                            bool attacked = false;

                            if (EnemyCharacterID == -1 || EnemyCharacterType == CharacterType.Player)
                            {
                                if (_mapInstance.GetMapData().PvpEnabled)
                                {
                                    GameClient[] clients = _mapInstance.GetClients();
                                    for (int i = 0; i < clients.Length; i++)
                                    {
                                        GameClient client = clients[i];
                                        if (EnemyCharacterID == -1 || EnemyCharacterID == client.GetPacket().PlayerID)
                                        {
                                            if (client._playerPacket.PositionX == x && client._playerPacket.PositionY == y)
                                            {
                                                if (client.EnemyCanAttack(CharacterType.Player, _playerPacket.PlayerID, multiCombat)
                                                    && EnemyCanAttack(CharacterType.Player, client._playerPacket.PlayerID, multiCombat))
                                                {
                                                    MeleeTrigger(client);
                                                    attacked = true;
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                            if (!attacked)
                            {
                                if (EnemyCharacterID == -1 || EnemyCharacterType == CharacterType.Enemy)
                                {
                                    for (int i = 0; i < _mapInstance.GetMapPacket().Enemies.Count; i++)
                                    {
                                        if (EnemyCharacterID == -1 || EnemyCharacterID == i)
                                        {
                                            MapEnemy other = _mapInstance.GetMapPacket().Enemies[i];
                                            if (other.MapX == x && other.MapY == y)
                                            {
                                                if (other.EnemyCanAttack(CharacterType.Player, _playerPacket.PlayerID, multiCombat)
                                                    && EnemyCanAttack(CharacterType.Enemy, i, multiCombat))
                                                {
                                                    MeleeTrigger(other);
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                            break;
                        case AttackStyle.Ranged:

                            RangeTrigger();

                            break;
                        case AttackStyle.Magic:

                            int mpDrain = (int)data.GetItemStat("MP");
                            if (mpDrain <= _playerPacket.Data.MP)
                            {
                                _playerPacket.Data.MP -= mpDrain;
                                int projectileID = (int)data.GetItemStat("ProjectileID");
                                MagicTrigger(projectileID);
                            }

                            break;
                    }
                }
            }
        }

        private void MeleeTrigger(GameClient other)
        {
            Random rand = new Random();
            CombatStats stats1 = _playerPacket.Data.GetCombinedCombatStats();
            CombatStats stats2 = other._playerPacket.Data.GetCombinedCombatStats();
            int critModifier = rand.Next(1, 6) > 4 ? (int)(stats1.Strength * 0.2f) : 0;
            double maxAccuracy = 1;// Math.Min(stats1.Agility / stats1.Strength, 1.0);
            double accuracy = rand.NextDouble() * maxAccuracy;
            int meleePower = (int)((stats1.Strength + critModifier) * accuracy) - (stats2.MeleeDefence / 2);
            meleePower = Math.Max(meleePower, 0);
            other.TakeDamage(CharacterType.Player, _playerPacket.PlayerID, meleePower, _mapInstance.GetMapData().MultiCombat);
            _attackTimer = Math.Max((1 / stats1.Agility) - 1.0f, 0.1f) * 10;
        }

        private void MeleeTrigger(MapEnemy other)
        {
            Random rand = new Random();
            CombatStats stats1 = _playerPacket.Data.GetCombinedCombatStats();
            CombatStats stats2 = other.GetEnemyData().BaseStats;
            int critModifier = rand.Next(1, 6) > 4 ? (int)(stats1.Strength * 0.2f) : 0;
            double maxAccuracy = 1;// Math.Min(stats1.Agility / stats1.Strength, 1.0);
            double accuracy = rand.NextDouble() * maxAccuracy;
            int meleePower = (int)((stats1.Strength + critModifier) * accuracy) - (stats2.MeleeDefence / 2);
            meleePower = Math.Max(meleePower, 0);
            other.TakeDamage(CharacterType.Player, _playerPacket.PlayerID, meleePower, _mapInstance.GetMapData().MultiCombat);
            _attackTimer = Math.Max((1 / stats1.Agility) - 1.0f, 0.1f) * 10;

            if (other.Dead)
            {
                GainExperience(other.GetEnemyData().Experience);
                _mapInstance.DropItem(other, this);
            }
        }

        private void RangeTrigger()
        {
            int projectileID = _playerPacket.Data.ConsumeAmmo();
            if (projectileID != -1)
            {
                Hitbox hitbox = GetHitbox();
                Vector2 position = new Vector2(hitbox.X, hitbox.Y);
                Projectile projectile = new Projectile(projectileID, CharacterType.Player, _playerPacket.PlayerID, position, _playerPacket.Direction);
                projectile.OnBridge = _playerPacket.OnBridge;
                projectile.Style = AttackStyle.Ranged;
                projectile.TargetType = EnemyCharacterType;
                projectile.TargetID = EnemyCharacterID;

                Random rand = new Random();
                CombatStats stats = _playerPacket.Data.GetCombinedCombatStats();
                int critModifier = rand.Next(1, 6) > 4 ? (int)(stats.Strength * 0.2f) : 0;
                double maxAccuracy = 1;// Math.Min(stats.Agility / stats.Strength, 1.0);
                double accuracy = rand.NextDouble() * maxAccuracy;
                int rangePower = (int)((stats.Strength + critModifier) * accuracy);
                projectile.AttackPower = rangePower;
                //need to add custom player targeting here later

                _mapInstance.AddProjectile(projectile);
                _attackTimer = Math.Max((1 / stats.Agility) - 1.0f, 0.1f) * 10;
            }
        }

        private void MagicTrigger(int projectileID)
        {
            Hitbox hitbox = GetHitbox();
            Vector2 position = new Vector2(hitbox.X, hitbox.Y);
            Projectile projectile = new Projectile(projectileID, CharacterType.Player, _playerPacket.PlayerID, position, _playerPacket.Direction);
            projectile.OnBridge = _playerPacket.OnBridge;
            projectile.Style = AttackStyle.Magic;
            projectile.TargetType = EnemyCharacterType;
            projectile.TargetID = EnemyCharacterID;

            Random rand = new Random();
            CombatStats stats = _playerPacket.Data.GetCombinedCombatStats();
            int critModifier = rand.Next(1, 6) > 4 ? (int)(stats.Strength * 0.2f) : 0;
            double maxAccuracy = 1;// Math.Min(stats.Agility / stats.Inteligence, 1.0);
            double accuracy = rand.NextDouble() * maxAccuracy;
            int magicPower = (int)((stats.Inteligence + critModifier) * accuracy);
            projectile.AttackPower = magicPower;
            //need to add custom player targeting here later

            _mapInstance.AddProjectile(projectile);
            _attackTimer = Math.Max((1 / stats.Agility) - 1.0f, 0.1f) * 10;
        }

        public void TakeDamage(CharacterType enemyType, int enemyID, int damage, bool multiCombat)
        {
            if (EnemyCanAttack(enemyType, enemyID, multiCombat))
            {
                if (enemyID != -1 && !_mapInstance.GetMapData().MultiCombat)
                {
                    EnemyCharacterType = enemyType;
                    EnemyCharacterID = enemyID;
                    _combatTimer = 5f;
                }

                _playerPacket.Data.HP -= damage;

                if (_playerPacket.Data.HP <= 0)
                {
                    Dead = true;
                    _playerPacket.Data.HP = 0;
                }
            }
        }

        public bool EnemyCanAttack(CharacterType enemyType, int enemyID, bool multiCombat)
        {
            if (Dead)
                return false;
            if (EnemyCharacterID == -1)
                return true;
            if (enemyID == -1)
                return true;
            if (multiCombat)
                return true;
            if (EnemyCharacterType == enemyType && EnemyCharacterID == enemyID)
                return true;
            return false;
        }


        public bool TerrainTagCheck(int tag)
        {
            return _mapInstance.TerrainTagCheck(_playerPacket.PositionX, _playerPacket.PositionY, tag);
        }

        public void Disconnect()
        {
            if (_connected)
            {
                StopTrading();
                _serverCommands.Clear();
                try
                {
                    this.AddServerCommand(new ServerCommand(ServerCommand.CommandType.Disconnect));
                    this.SendServerCommands();
                } catch { }
                Server.Instance.GetDatabaseConnection().UpdatePlayerQuery(_playerPacket, _bankData);
                Server.Instance.RemoveGameClient(this);
                _stream.Close();
                _connected = false;
            }
        }

        public bool Connected()
        {
            return _connected;
        }

    }
}

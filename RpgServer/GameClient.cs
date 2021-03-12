using Genus2D.GameData;
using Genus2D.Networking;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

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
            if (loginValid && Server.Instance.FindClientByID(playerID) == null)
            {
                Console.WriteLine("(" + playerID + ", " + loginRequest.Username + ") Successful login.");

                result.LoggedIn = true;
                result.PlayerID = playerID;

                SendLoginResult(stream, result);
                GameClient client = new GameClient(tcpClient, result.PlayerID);
                oClient = client;
            }
            else
            {
                Console.WriteLine("(" + playerID + ", " + loginRequest.Username + ") Failed login.");

                result.LoggedIn = false;
                result.PlayerID = -1;

                SendLoginResult(stream, result);
                oClient = null;
            }

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

        private string _clientIP;
        private TcpClient _tcpClient;
        private NetworkStream _networkStream;
        private float _sendTimer;
        private bool _sendingPackets;
        private bool _recievingPackets;

        private bool _disconnecting;
        private float _disconnectTimer;

        private bool _mapChanged;
        private ServerMap _serverMap;
        private MapPlayer _mapPlayer;

        private List<ServerCommand> _serverCommands;
        private List<MessagePacket> _messagePackets;

        private GameClient(TcpClient tcpClient, int playerID)
        {
            Server.Instance.AddGameClient(this);

            _tcpClient = tcpClient;
            _networkStream = _tcpClient.GetStream();
            _clientIP = ((IPEndPoint)_tcpClient.Client.RemoteEndPoint).Address.ToString();
            _sendTimer = 0f;
            _sendingPackets = false;
            _recievingPackets = false;

            _disconnecting = false;
            _disconnectTimer = 5f;

            _mapChanged = false;
            _serverMap = null;

            _mapPlayer = Server.Instance.GetDatabaseConnection().RetrievePlayerQuery(playerID);
            _mapPlayer.OnEventTrigger += OnEventTrigger;
            _mapPlayer.OnShowMessage += OnShowMessage;
            _mapPlayer.OnChangeMap += OnChangeMap;
            _mapPlayer.OnSendMessage += OnSendMessage;
            _mapPlayer.OnRequestTrade += OnRequestTrade;
            _mapPlayer.OnStartTrade += OnStartTrade;
            _mapPlayer.OnAcceptTrade += OnAcceptTrade;
            _mapPlayer.OnEndTrade += OnEndTrade;
            _mapPlayer.OnAddTradeItem += OnAddTradeItem;
            _mapPlayer.OnRemoveTradeItem += OnRemoveTradeItem;
            _mapPlayer.OnShowShop += OnShowShop;
            _mapPlayer.OnStartBanking += OnStartBanking;
            _mapPlayer.OnShowWorkbench += OnShowWorkbench;

            _serverCommands = new List<ServerCommand>();
            _messagePackets = new List<MessagePacket>();
        }

        public MapPlayer GetMapPlayer()
        {
            return _mapPlayer;
        }

        public string GetClientIP()
        {
            return _clientIP;
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

        private void SendRecievePackets(float deltaTime)
        {
            if (_sendTimer <= 0.0)
            {
                _sendTimer = 0.025f;

                SendPackets();
            }
            else
            {
                _sendTimer -= deltaTime;
            }

            RecievePackets();
        }

        private async void SendPackets()
        {
            if (_disconnecting) return;

            if (!_sendingPackets)
            {
                _sendingPackets = true;

                await Task.Run(() =>
                {
                    try
                    {
                        List<byte> comunicationBytes = new List<byte>();
                        if (_serverMap != null)
                        {
                            GetMapPacketBytes(ref comunicationBytes);
                            GetPlayerPacketBytes(ref comunicationBytes);
                            GetServerCommandBytes(ref comunicationBytes);
                            GetMessagePacketBytes(ref comunicationBytes);
                            GetBankPacketBytes(ref comunicationBytes);

                            if (comunicationBytes.Count > 0)
                            {
                                _networkStream.Write(comunicationBytes.ToArray(), 0, comunicationBytes.Count);
                                _networkStream.Flush();
                            }

                        }
                    }
                    catch
                    {
                        Console.WriteLine("Client disconnected.");
                        _disconnecting = true;
                    }

                    _sendingPackets = false;
                });
            }
        }

        private async void RecievePackets()
        {
            if (_disconnecting) return;

            if (!_recievingPackets)
            {
                _recievingPackets = true;

                await Task.Run(() =>
                {
                    try
                    {
                        while (_networkStream.DataAvailable)
                        {
                            _mapPlayer.AttemptedMove = false;

                            byte[] bytes = ReadData(sizeof(int), _networkStream);
                            PacketType type = (PacketType)BitConverter.ToInt32(bytes, 0);

                            switch (type)
                            {
                                case PacketType.ClientCommand:
                                    RecieveClientCommands();
                                    break;
                                case PacketType.SendMessagePackets:
                                    RecieveMessagePackets();
                                    break;
                            }
                        }
                            
                        
                    }
                    catch
                    {
                        Console.WriteLine("Client disconnected.");
                        _disconnecting = true;
                    }
                    _recievingPackets = false;
                });
            }
        }

        public PlayerPacket GetPacket()
        {
            return _mapPlayer.GetPlayerPacket();
        }

        public void SetServerMap(ServerMap serverMap)
        {
            _serverMap = serverMap;
            _mapChanged = true;
        }

        public void AddServerCommand(ServerCommand command)
        {
            if (command != null)
            {
                _serverCommands.Add(command);
            }
        }

        private void GetServerCommandBytes(ref List<byte> oBytes)
        {
            int numCommands = _serverCommands.Count;
            for (int i = 0; i < numCommands; i++)
            {
                ServerCommand command = _serverCommands[i];
                if (command == null)
                {
                    i--;
                    continue;
                }

                oBytes.AddRange(BitConverter.GetBytes((int)PacketType.ServerCommand));

                byte[] bytes = command.GetBytes();
                oBytes.AddRange(BitConverter.GetBytes(bytes.Length));
                oBytes.AddRange(bytes);
            }

            _serverCommands.RemoveRange(0, numCommands);
        }

        public void RecieveMessage(MessagePacket message)
        {
            if (message.TargetType == MessagePacket.MessageTarget.Public)
            {
                _messagePackets.Add(message);
            }
            else
            {
                if (message.TargetID == _mapPlayer.PlayerID)
                    _messagePackets.Add(message);
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
                packet.Message = _mapPlayer.Username + ": " + packet.Message; // the server signs the messages with the username
                packet.PlayerID = _mapPlayer.PlayerID;
                Server.Instance.SendMessage(packet);
            }
        }

        private void GetMessagePacketBytes(ref List<byte> oBytes)
        {
            if (_messagePackets.Count > 0)
            {
                byte[] bytes;
                oBytes.AddRange(BitConverter.GetBytes((int)PacketType.RecieveMessagePackets));
                oBytes.AddRange(BitConverter.GetBytes(_messagePackets.Count));

                for (int i = 0; i < _messagePackets.Count; i++)
                {
                    bytes = _messagePackets[i].GetBytes();
                    oBytes.AddRange(BitConverter.GetBytes(bytes.Length));
                    oBytes.AddRange(bytes);
                }
                _messagePackets.Clear();
            }
        }

        private void RecieveClientCommands()
        {
            byte[] bytes = ReadData(sizeof(int), _networkStream);
            int numCommands = BitConverter.ToInt32(bytes, 0);

            _mapPlayer.AttemptedMove = false;

            for (int i = 0; i < numCommands; i++)
            {
                bytes = ReadData(sizeof(int), _networkStream);
                int commandSize = BitConverter.ToInt32(bytes, 0);

                bytes = ReadData(commandSize, _networkStream);
                ClientCommand command = ClientCommand.FromBytes(bytes);
                _mapPlayer.RecieveClientCommand(command);
            }
        }

        private void GetPlayerPacketBytes(ref List<byte> oBytes)
        {
            List<PlayerPacket> packets = _serverMap.GetPlayerPackets();

            int numClients = packets.Count;
            if (numClients > 0)
            {
                oBytes.AddRange(BitConverter.GetBytes((int)PacketType.PlayerPacket));
                oBytes.AddRange(BitConverter.GetBytes(numClients));

                for (int i = 0; i < numClients; i++)
                {
                    PlayerPacket packet = packets[i];

                    bool isLocalPlayer = packet.PlayerID == _mapPlayer.PlayerID;
                    byte[] bytes = packet.GetBytes(isLocalPlayer);
                    oBytes.AddRange(BitConverter.GetBytes(bytes.Length));
                    oBytes.AddRange(bytes);
                }
            }

        }

        private void GetMapPacketBytes(ref List<byte> oBytes)
        {
            if (_mapChanged)
            {
                MapPacket mapPacket = _serverMap.GetMapInstance().GetMapPacket();

                oBytes.AddRange(BitConverter.GetBytes((int)PacketType.MapPacket));

                byte[] mapBytes = mapPacket.GetBytes();
                oBytes.AddRange(BitConverter.GetBytes(mapBytes.Length));
                oBytes.AddRange(mapBytes);
                _mapChanged = false;
            }
        }

        private void GetBankPacketBytes(ref List<byte> oBytes)
        {
            if (_mapPlayer.BankUpdated)
            {
                _mapPlayer.BankUpdated = false;

                oBytes.AddRange(BitConverter.GetBytes((int)PacketType.BankPacket));
                byte[] bytes = _mapPlayer.GetBankData().GetBytes();
                oBytes.AddRange(BitConverter.GetBytes(bytes.Length));
                oBytes.AddRange(bytes);
            }
        }

        public void OnEventTrigger(MapPlayer mapPlayer, MapEvent mapEvent)
        {
            _serverMap.GetEventInterpreter().TriggerEventData(mapPlayer, mapEvent);
        }

        public void OnShowMessage(string message, string options)
        {
            ServerCommand serverCommand;
            if (options == null)
            {
                serverCommand = new ServerCommand(ServerCommand.CommandType.ShowMessage);
                serverCommand.SetParameter("Message", message);
                AddServerCommand(serverCommand);
            }
            else
            {
                serverCommand = new ServerCommand(ServerCommand.CommandType.ShowOptions);
                serverCommand.SetParameter("Message", message);
                serverCommand.SetParameter("Options", options);
                AddServerCommand(serverCommand);
            }
        }

        public void OnChangeMap()
        {
            _serverMap.RemoveClient(this);
            Server.Instance.ChangeClientsMap(this);
        }

        public void OnSendMessage(string message)
        {
            this.RecieveMessage(new MessagePacket(message));
        }

        public void OnRequestTrade(int otherID)
        {
            GameClient otherClient = _serverMap.FindGameClient(otherID);
            if (otherClient != null)
            {
                ServerCommand command = new ServerCommand(ServerCommand.CommandType.TradeRequest);
                command.SetParameter("PlayerID", _mapPlayer.GetPlayerPacket().PlayerID);
                command.SetParameter("PlayerName", _mapPlayer.GetPlayerPacket().Username);
                otherClient.AddServerCommand(command);
            }
        }

        public void OnStartTrade(int otherID)
        {
            GameClient other = _serverMap.FindGameClient(otherID);
            if (other != null)
            {
                ServerCommand sCommand = new ServerCommand(ServerCommand.CommandType.StartTrade);
                sCommand.SetParameter("PlayerID", other.GetMapPlayer().PlayerID);
                sCommand.SetParameter("PlayerName", other.GetMapPlayer().Username);
                AddServerCommand(sCommand);

                sCommand = new ServerCommand(ServerCommand.CommandType.StartTrade);
                sCommand.SetParameter("PlayerID", _mapPlayer.PlayerID);
                sCommand.SetParameter("PlayerName", _mapPlayer.Username);
                other.AddServerCommand(sCommand);
            }
        }

        public void OnAcceptTrade(int otherID)
        {
            GameClient other = _serverMap.FindGameClient(otherID);
            if (other != null) other.AddServerCommand(new ServerCommand(ServerCommand.CommandType.AcceptTrade));
        }

        public void OnCantTrade(int otherID, int freeSlots)
        {
            GameClient other = _serverMap.FindGameClient(otherID);

            ServerCommand command = new ServerCommand(ServerCommand.CommandType.CantTrade);
            this.AddServerCommand(command);

            MessagePacket message = new MessagePacket("You only have " + freeSlots + " inventory slots.");
            this.RecieveMessage(message);

            if (other != null)
            {
                other.AddServerCommand(command);
                message = new MessagePacket(other.GetMapPlayer().Username + " only has " + freeSlots + " inventory slots.");
                other.RecieveMessage(message);
            }
        }

        public void OnEndTrade(int otherID)
        {
            AddServerCommand(new ServerCommand(ServerCommand.CommandType.EndTrade));
        }

        public void OnAddTradeItem(int otherID, int itemID, int count)
        {
            GameClient other = _serverMap.FindGameClient(otherID);
            if (other != null)
            {
                ServerCommand command = new ServerCommand(ServerCommand.CommandType.AddTradeItem);
                command.SetParameter("ItemID", itemID);
                command.SetParameter("Count", count);
                other.AddServerCommand(command);
            }
        }

        public void OnRemoveTradeItem(int otherID, int itemID, int count)
        {
            GameClient other = _serverMap.FindGameClient(otherID);
            if (other != null)
            {
                ServerCommand command = new ServerCommand(ServerCommand.CommandType.RemoveTradeItem);
                command.SetParameter("ItemIndex", itemID);
                command.SetParameter("Count", count);
                other.AddServerCommand(command);
            }
        }

        public void OnShowShop(int shopID)
        {
            ServerCommand serverCommand = new ServerCommand(ServerCommand.CommandType.ShowShop);
            serverCommand.SetParameter("ShopID", shopID);
            this.AddServerCommand(serverCommand);
        }

        public void OnStartBanking()
        {
            ServerCommand command = new ServerCommand(ServerCommand.CommandType.OpenBank);
            this.AddServerCommand(command);
        }

        public void OnShowWorkbench(int workbenchID)
        {
            ServerCommand command = new ServerCommand(ServerCommand.CommandType.ShowWorkbench);
            command.SetParameter("WorkbenchID", workbenchID);
            this.AddServerCommand(command);
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

            SendRecievePackets(deltaTime);
            _mapPlayer.Update(deltaTime);
        }

        public void Disconnect()
        {
            if (_mapPlayer.Connected)
            {
                _mapPlayer.StopTrading();
                _serverCommands.Clear();
                try
                {
                    this.AddServerCommand(new ServerCommand(ServerCommand.CommandType.Disconnect));
                } catch { }

                /*
                if (_sendStream != null)
                {
                    _sendStream.Close();
                    _sendClient.Close();
                }

                if (_recieveStream != null)
                {
                    _recieveStream.Close();
                    _recieveClient.Close();
                }*/

                Server.Instance.GetDatabaseConnection().UpdatePlayerQuery(_mapPlayer);
                Server.Instance.RemoveGameClient(this);
                _mapPlayer.Connected = false;
            }
        }

        public bool Connected()
        {
            return _mapPlayer.Connected;
        }

    }
}

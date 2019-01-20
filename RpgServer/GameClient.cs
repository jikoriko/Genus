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
            Console.WriteLine("recieved a client");

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
                    //failed to login character
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

            return false;
        }

        private static bool LoginClient(TcpClient tcpClient, out GameClient oClient)
        {
            Stream stream = tcpClient.GetStream();
            LoginRequest loginRequest = RecieveLoginRequest(stream);

            int playerID;
            bool loginValid = Server.Instance.GetDatabaseConnection().LoginQuery(loginRequest.Username, loginRequest.Password, out playerID);

            LoginResult result = new LoginResult();
            if (loginValid)
            {
                result.LoggedIn = true;
                result.PlayerID = playerID;

                GameClient client = new GameClient(tcpClient, result.PlayerID);
                oClient = client;
            }
            else
            {
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

        private bool _mapChanged;
        private bool _running;

        private PlayerPacket _playerPacket;
        private InputPacket _inputPacket;
        private MapInstance _mapInstance;
        private MapEventData _triggeringEvent;
        private int _triggeringEventCommand;
        private float _triggeringEventWaitTimer;

        private List<ClientCommand> _clientCommands;
        private bool _messageShowing;
        private bool _optionsShowing;
        private int _selectedOption;
        private int _optionsCount;

        private GameClient(TcpClient tcpClient, int playerID)
        {
            _tcpClient = tcpClient;
            _stream = _tcpClient.GetStream();
            _connected = true;

            _mapChanged = false;
            _running = false;

            _playerPacket = Server.Instance.GetDatabaseConnection().RetrievePlayerQuery(playerID);

            _inputPacket = null;
            _mapInstance = null;
            _triggeringEvent = null;
            _triggeringEventCommand = -1;
            _triggeringEventWaitTimer = 0.0f;

            _clientCommands = new List<ClientCommand>();
            _messageShowing = false;
            _optionsShowing = false;
            _selectedOption = -1;
            _optionsCount = 0;

            Thread updatePacketsThread = new Thread(new ThreadStart(UpdatePackets));
            updatePacketsThread.Start();
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
                if (Connected())
                {

                    prevTicks = ticks;
                    ticks = DateTime.Now.Ticks;
                    deltaTime = (ticks - prevTicks) / 10000000.0;

                    if (updateTimer <= 0.0)
                    {
                        updateTimer = 0.05;
                        if (_mapInstance != null)
                        {
                            SendMapPacket();
                            SendPlayerPackets();
                            SendClientCommands();
                            RecieveInputPacket();
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
            if (_running)
                speed *= 2.5f;
            return speed;
        }

        public void AddClientCommand(ClientCommand command)
        {
            if (command != null)
                _clientCommands.Add(command);
        }

        public void SendClientCommands()
        {
            while (_clientCommands.Count > 0)
            {
                ClientCommand command = _clientCommands[0];
                if (command != null)
                {
                    byte[] bytes = BitConverter.GetBytes((int)PacketType.ClientCommand);
                    _stream.Write(bytes, 0, sizeof(int));
                    _stream.Flush();

                    bytes = command.GetBytes();
                    _stream.Write(BitConverter.GetBytes(bytes.Length), 0, sizeof(int));
                    _stream.Flush();
                    _stream.Write(bytes, 0, bytes.Length);
                    _stream.Flush();

                    _clientCommands.RemoveAt(0);
                }
            }
        }

        public void RecieveInputPacket()
        {
            if (_inputPacket == null)
            {
                try
                {
                    byte[] bytes = BitConverter.GetBytes((int)PacketType.InputPacket);
                    _stream.Write(bytes, 0, sizeof(int));
                    _stream.Flush();

                    bytes = ReadData(sizeof(int), _stream);
                    int size = BitConverter.ToInt32(bytes, 0);

                    bytes = ReadData(size, _stream);

                    InputPacket packet = InputPacket.FromBytes(bytes);
                    _inputPacket = packet;
                }
                catch (Exception e)
                {
                    _inputPacket = null;
                    Console.WriteLine(e.Message);
                    Console.WriteLine(e.StackTrace);
                }
            }
        }

        public void SendPlayerPackets()
        {
            try
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

                    bytes = packet.GetBytes();
                    byte[] sizeBytes = BitConverter.GetBytes(bytes.Length);
                    _stream.Write(sizeBytes, 0, sizeof(int));
                    _stream.Flush();

                    _stream.Write(bytes, 0, bytes.Length);
                    _stream.Flush();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to read from client, disconnecting");
                Console.WriteLine(e.Message);
                Disconnect();
            }

        }

        private void SendMapPacket()
        {
            if (_mapChanged)
            {
                try
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
                }
                catch (Exception e)
                {
                    Console.WriteLine("Failed to read from client, disconnecting");
                    Console.WriteLine(e.Message);
                    Disconnect();
                }
                _mapChanged = false;
            }
        }

        public int GetMapID()
        {
            return _playerPacket.MapID;
        }

        private bool Moving()
        {
            int posX = _playerPacket.PositionX * 32;
            int posY = _playerPacket.PositionY * 32;
            return (posX != _playerPacket.RealX || posY != _playerPacket.RealY);
        }

        public bool KeyDown(OpenTK.Input.Key key)
        {
            if (_inputPacket == null)
                return false;

            return _inputPacket.KeysDown.Contains((int)key);
        }

        public void Update(float deltaTime)
        {
            if (_triggeringEvent != null)
            {
                TriggerMapEventData(deltaTime);
            }
            else
            {
                if (_inputPacket != null)
                {
                    if (KeyDown(OpenTK.Input.Key.LShift))
                        _running = true;
                    else
                        _running = false;
                }

                _playerPacket.MovementSpeed = GetMovementSpeed();

                if (!Moving())
                {
                    if (KeyDown(OpenTK.Input.Key.W))
                    {
                        Move(Direction.Up);
                        _playerPacket.Direction = Genus2D.GameData.Direction.Up;
                    }
                    else if (KeyDown(OpenTK.Input.Key.S))
                    {
                        Move(Direction.Down);
                        _playerPacket.Direction = Genus2D.GameData.Direction.Down;
                    }
                    else if (KeyDown(OpenTK.Input.Key.A))
                    {
                        Move(Direction.Left);
                        _playerPacket.Direction = Genus2D.GameData.Direction.Left;
                    }
                    else if (KeyDown(OpenTK.Input.Key.D))
                    {
                        Move(Direction.Right);
                        _playerPacket.Direction = Genus2D.GameData.Direction.Right;
                    }
                    else
                    {
                        if (KeyDown(OpenTK.Input.Key.Space))
                        {
                            int targetX = _playerPacket.PositionX;
                            int targetY = _playerPacket.PositionY;
                            switch (_playerPacket.Direction)
                            {
                                case Direction.Down:
                                    targetY += 1;
                                    break;
                                case Direction.Left:
                                    targetX -= 1;
                                    break;
                                case Direction.Right:
                                    targetX += 1;
                                    break;
                                case Direction.Up:
                                    targetY -= 1;
                                    break;
                            }
                            CheckEventTriggers(targetX, targetY, MapEventData.TriggerType.Action);
                        }
                    }
                }

                if (Moving())
                {
                    Vector2 realPos = new Vector2(_playerPacket.RealX, _playerPacket.RealY);
                    Vector2 dir = new Vector2(_playerPacket.PositionX * 32, _playerPacket.PositionY * 32) - realPos;
                    dir.Normalize();
                    realPos += (dir * _playerPacket.MovementSpeed * deltaTime);

                    dir = new Vector2(_playerPacket.PositionX * 32, _playerPacket.PositionY * 32) - realPos;
                    if (dir.Length <= 0.1f)
                    {
                        realPos = new OpenTK.Vector2(_playerPacket.PositionX * 32, _playerPacket.PositionY * 32);
                        CheckEventTriggers(_playerPacket.PositionX, _playerPacket.PositionY, MapEventData.TriggerType.PlayerTouch);
                    }

                    _playerPacket.RealX = realPos.X;
                    _playerPacket.RealY = realPos.Y;
                }
            }

            _inputPacket = null;
        }

        private void SetMapPosition(int x, int y)
        {
            _playerPacket.PositionX = x;
            _playerPacket.PositionY = y;
            _playerPacket.RealX = x * 32;
            _playerPacket.RealY = y * 32;
        }

        private void Move(Direction direction)
        {
            if (!Moving())
            {
                int targetX = _playerPacket.PositionX;
                int targetY = _playerPacket.PositionY;
                switch (direction)
                {
                    case Direction.Down:
                        targetY += 1;
                        break;
                    case Direction.Left:
                        targetX -= 1;
                        break;
                    case Direction.Right:
                        targetX += 1;
                        break;
                    case Direction.Up:
                        targetY -= 1;
                        break;
                }

                if (_mapInstance.MapTilePassable(targetX, targetY))
                {
                    _playerPacket.PositionX = targetX;
                    _playerPacket.PositionY = targetY;
                }
                else
                {
                    CheckEventTriggers(targetX, targetY, MapEventData.TriggerType.PlayerTouch);
                }
            }
        }

        private void CheckEventTriggers(int x, int y, MapEventData.TriggerType triggerType)
        {
            MapData mapData = _mapInstance.GetMapData();
            for (int i = 0; i < mapData.MapEventsCount(); i++)
            {
                MapEvent mapEvent = mapData.GetMapEvent(i);
                if (mapEvent.GetMapEventData().GetTriggerType() == triggerType)
                {
                    if (mapEvent.MapX == x && mapEvent.MapY == y)
                    {
                        _triggeringEvent = mapEvent.GetMapEventData();
                        break;
                    }
                }
            }
        }

        public void TriggerMapEventData(float deltaTime)
        {
            if (_messageShowing)
            {
                if (KeyDown(OpenTK.Input.Key.Enter))
                {
                    AddClientCommand(new ClientCommand(ClientCommand.CommandType.CloseMessage));
                    _messageShowing = false;
                }

                return;
            }
            else if (_optionsShowing)
            {
                ClientCommand clientCommand;

                if (KeyDown(OpenTK.Input.Key.Space))
                {
                    //get the selected option and trigger event

                    _optionsShowing = false;
                    _selectedOption = -1;
                    _optionsCount = 0;
                    AddClientCommand(new ClientCommand(ClientCommand.CommandType.CloseMessage));
                }
                else if (KeyDown(OpenTK.Input.Key.Down))
                {
                    _selectedOption++;
                    if (_selectedOption >= _optionsCount)
                        _selectedOption = 0;

                    clientCommand = new ClientCommand(ClientCommand.CommandType.UpdateOptions);
                    clientCommand.SetParameter("SelectedOption", _selectedOption.ToString());
                    AddClientCommand(clientCommand);
                }
                if (KeyDown(OpenTK.Input.Key.Up))
                {
                    _selectedOption--;
                    if (_selectedOption < 0)
                        _selectedOption = _optionsCount - 1;

                    clientCommand = new ClientCommand(ClientCommand.CommandType.UpdateOptions);
                    clientCommand.SetParameter("SelectedOption", _selectedOption.ToString());
                    AddClientCommand(clientCommand);
                }

                return;
            }

            if (_triggeringEventWaitTimer > 0)
            {
                _triggeringEventWaitTimer -= deltaTime;
                return;
            }

            if (_triggeringEventCommand < _triggeringEvent.EventCommands.Count - 1)
            {
                _triggeringEventCommand++;
                EventCommand eventCommand = _triggeringEvent.EventCommands[_triggeringEventCommand];

                ClientCommand clientCommand;

                switch (eventCommand.Type)
                {
                    case EventCommand.CommandType.EventWaitTimer:

                        float timer = (float)eventCommand.GetParameter("Time");
                        _triggeringEventWaitTimer = timer;

                        break;
                    case EventCommand.CommandType.MapTransfer:

                        int mapID = (int)eventCommand.GetParameter("MapID");
                        int mapX = (int)eventCommand.GetParameter("MapX");
                        int mapY = (int)eventCommand.GetParameter("MapY");
                        SetMapPosition(mapX, mapY);
                        if (GetMapID() != mapID)
                        {
                            _playerPacket.MapID = mapID;
                            _mapInstance.RemoveClient(this);
                            _mapInstance.GetServer().ChangeClientsMap(this);
                        }

                        break;
                    case EventCommand.CommandType.MovePlayer:

                        Direction direction = (Direction)eventCommand.GetParameter("Direction");
                        Move(direction);

                        break;
                    case EventCommand.CommandType.ShowMessage:

                        clientCommand = new ClientCommand(ClientCommand.CommandType.ShowMessage);
                        clientCommand.SetParameter("Message", (string)eventCommand.GetParameter("Message"));
                        AddClientCommand(clientCommand);
                        _messageShowing = true;

                        break;
                    case EventCommand.CommandType.ShowOptions:

                        List<MessageOption> options = (List<MessageOption>)eventCommand.GetParameter("Options");
                        _selectedOption = 0;
                        _optionsCount = options.Count;

                        string optionStrings = "";
                        for (int i = 0; i < options.Count; i++)
                        {
                            optionStrings += options[i].Option;
                            if (i < options.Count - 1)
                                optionStrings += ",";
                        }

                        clientCommand = new ClientCommand(ClientCommand.CommandType.ShowOptions);
                        clientCommand.SetParameter("Message", (string)eventCommand.GetParameter("Message"));
                        clientCommand.SetParameter("Options", optionStrings);
                        clientCommand.SetParameter("SelectedOption", _selectedOption.ToString());

                        AddClientCommand(clientCommand);
                        _optionsShowing = true;

                        break;
                }
            }
            else
            {
                _triggeringEvent = null;
                _triggeringEventCommand = -1;
            }
        }

        public void Disconnect()
        {
            Server.Instance.GetDatabaseConnection().UpdatePlayerQuery(_playerPacket);
            if (_connected)
            {
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

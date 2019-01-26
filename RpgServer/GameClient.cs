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

        private EventInterpreter _eventInterpreter;
        private List<ClientCommand> _commandsToAdd;
        private List<ClientCommand> _clientCommands;

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

            _eventInterpreter = new EventInterpreter(this);
            _commandsToAdd = new List<ClientCommand>();
            _clientCommands = new List<ClientCommand>();

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
                            try
                            {
                                SendMapPacket();
                                SendPlayerPackets();
                                SendClientCommands();
                                RecieveInputPacket();
                            }
                            catch
                            {
                                Disconnect();
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
            if (_running)
                speed *= 2.5f;
            return speed;
        }

        public void AddClientCommand(ClientCommand command)
        {
            if (command != null)
                _commandsToAdd.Add(command);
        }

        public void SendClientCommands()
        {
            while (_commandsToAdd.Count > 0)
            {
                if (_commandsToAdd[0] != null)
                {
                    _clientCommands.Add(_commandsToAdd[0]);
                    _commandsToAdd.RemoveAt(0);
                }
            }

            while (_clientCommands.Count > 0)
            {
                ClientCommand command = _clientCommands[0];
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

        public void RecieveInputPacket()
        {
            if (_inputPacket == null)
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
        }

        public void SendPlayerPackets()
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

        public bool KeyDown(OpenTK.Input.Key key)
        {
            if (_inputPacket == null)
                return false;

            return _inputPacket.KeysDown.Contains((int)key);
        }

        public void Update(float deltaTime)
        {
            _eventInterpreter.Update(deltaTime);

            if (!_eventInterpreter.EventTriggering())
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

        public void SetMapPosition(int x, int y)
        {
            _playerPacket.PositionX = x;
            _playerPacket.PositionY = y;
            _playerPacket.RealX = x * 32;
            _playerPacket.RealY = y * 32;
        }

        public void Move(Direction direction)
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

        public void ChangeDirection(Direction direction)
        {
            _playerPacket.Direction = direction;
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
                        _eventInterpreter.TriggerEventData(mapEvent.GetMapEventData());
                        break;
                    }
                }
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

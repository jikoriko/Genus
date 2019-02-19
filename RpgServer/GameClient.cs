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

        private List<ClientCommand> _clientCommands;
        private List<MessagePacket> _messagePackets;

        public bool MessageShowing;
        public bool MovementDisabled;

        private GameClient(TcpClient tcpClient, int playerID)
        {
            Server.Instance.AddGameClient(this);

            _tcpClient = tcpClient;
            _stream = _tcpClient.GetStream();
            _connected = true;

            _mapChanged = false;
            _running = false;

            _playerPacket = Server.Instance.GetDatabaseConnection().RetrievePlayerQuery(playerID);

            _inputPacket = null;
            _mapInstance = null;

            _clientCommands = new List<ClientCommand>();
            _messagePackets = new List<MessagePacket>();

            MessageShowing = false;
            MovementDisabled = false;

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
                                RecieveMessagePackets();
                                SendMessagePackets();
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
                _clientCommands.Add(command);
        }

        private void SendClientCommands()
        {
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

        private void RecieveInputPacket()
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
            if (MessageShowing && KeyDown(OpenTK.Input.Key.Space))
            {
                MessageShowing = false;
            }

            if (!MovementDisabled)
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
                    MovementDirection movementDirection;
                    if (KeyDown(OpenTK.Input.Key.W))
                    {
                        if (KeyDown(OpenTK.Input.Key.A))
                            movementDirection = MovementDirection.UpperLeft;
                        else if (KeyDown(OpenTK.Input.Key.D))
                            movementDirection = MovementDirection.UpperRight;
                        else
                            movementDirection = MovementDirection.Up;

                        Move(movementDirection);
                    }
                    else if (KeyDown(OpenTK.Input.Key.S))
                    {
                        if (KeyDown(OpenTK.Input.Key.A))
                            movementDirection = MovementDirection.LowerLeft;
                        else if (KeyDown(OpenTK.Input.Key.D))
                            movementDirection = MovementDirection.LowerRight;
                        else
                            movementDirection = MovementDirection.Down;
                        Move(movementDirection);
                    }
                    else if (KeyDown(OpenTK.Input.Key.A))
                    {
                        Move(MovementDirection.Left);
                    }
                    else if (KeyDown(OpenTK.Input.Key.D))
                    {
                        Move(MovementDirection.Right);
                    }
                    else
                    {
                        if (KeyDown(OpenTK.Input.Key.Space))
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

                            CheckEventTriggers(targetX, targetY, EventTriggerType.Action);
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
                        CheckEventTriggers(_playerPacket.PositionX, _playerPacket.PositionY, EventTriggerType.PlayerTouch);
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

        public void Move(MovementDirection direction)
        {
            if (!Moving())
            {
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
                _playerPacket.Direction = facingDirection;

                if (_mapInstance.MapTilePassable(x, y, direction, -1) && _mapInstance.MapTilePassable(targetX, targetY, entryDirection, -1))
                {
                    _playerPacket.PositionX = targetX;
                    _playerPacket.PositionY = targetY;
                }
                else
                {
                    CheckEventTriggers(targetX, targetY, EventTriggerType.PlayerTouch);
                }
            }
        }

        public void ChangeDirection(FacingDirection direction)
        {
            _playerPacket.Direction = direction;
        }

        private void CheckEventTriggers(int x, int y, EventTriggerType triggerType)
        {
            MapData mapData = _mapInstance.GetMapData();
            for (int i = 0; i < mapData.MapEventsCount(); i++)
            {
                MapEvent mapEvent = mapData.GetMapEvent(i);
                if (mapEvent.MapX == x && mapEvent.MapY == y)
                {
                    if (mapEvent.EventID == -1)
                        return;
                    if (mapEvent.TriggerType == triggerType)
                    {
                        _mapInstance.GetEventInterpreter().TriggerEventData(this, mapEvent);
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

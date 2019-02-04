﻿using Genus2D.Entities;
using Genus2D.GameData;
using Genus2D.Graphics;
using Genus2D.GUI;
using Genus2D.Networking;
using OpenTK;
using RpgGame.EntityComponents;
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
        private Dictionary<int, Entity> _playerEntities;
        private bool _playersUpdated = false;

        private Thread _recievePacketsThread;
        private List<MessagePacket> _messages;

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
                        _playerEntities = new Dictionary<int, Entity>();

                        _recievePacketsThread = new Thread(new ThreadStart(RecievePackets));
                        _recievePacketsThread.Start();
                        _messages = new List<MessagePacket>();
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
                for (int i = 0; i < _playerEntities.Count; i++)
                {
                    int id = _playerEntities.ElementAt(i).Key;
                    if (!_playerPackets.ContainsKey(id))
                    {
                        _playerEntities[id].Destroy();
                        _playerEntities.Remove(id);
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

                    if (_playerEntities.ContainsKey(packet.PlayerID))
                    {
                        Entity clientEntity = _playerEntities[packet.PlayerID];
                        PlayerComponent playerComponent = (PlayerComponent)clientEntity.FindComponent<PlayerComponent>();
                        playerComponent.SetPlayerPacket(packet);
                    }
                    else
                    {
                        Entity clientEntity = Entity.CreateInstance(_gameState.EntityManager);
                        clientEntity.GetTransform().Parent = _gameState.MapEntity.GetTransform();
                        new PlayerComponent(clientEntity, packet);
                        _playerEntities.Add(packet.PlayerID, clientEntity);
                    }

                   
                }
                _playersUpdated = false;
            }

        }

        public void SendInputPacket()
        {
            InputPacket packet = _gameState.GetInputPacket();

            byte[] inputBytes = packet.GetBytes();

            _stream.Write(BitConverter.GetBytes(inputBytes.Length), 0, sizeof(int));
            _stream.Flush();

            _stream.Write(inputBytes, 0, inputBytes.Length);
            _stream.Flush();

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
                        case PacketType.InputPacket:
                            SendInputPacket();
                            break;
                        case PacketType.ClientCommand:
                            ReciveClientCommand();
                            break;
                        case PacketType.SendMessagePackets:
                            SendMessagePackets();
                            break;
                        case PacketType.RecieveMessagePackets:
                            RecieveMessagePackets();
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
            MapComponent.Instance.SetMapData(packet.mapData);
        }

        MessageBox _messageBox = null;

        private void ReciveClientCommand()
        {
            byte[] bytes = ReadData(sizeof(int), _stream);
            int packetSize = BitConverter.ToInt32(bytes, 0);

            bytes = ReadData(packetSize, _stream);
            ClientCommand command = ClientCommand.FromBytes(bytes);

            int eventID;
            int mapX;
            int mapY;
            float realX;
            float realY;
            Direction direction;
            MapData map;

            switch (command.GetCommandType())
            {
                case ClientCommand.CommandType.ShowMessage:
                    if (_messageBox == null)
                    {
                        _messageBox = new MessageBox(command.GetParameter("Message"), _gameState, false);
                        _gameState.AddControl(_messageBox);
                    }
                    break;
                case ClientCommand.CommandType.CloseMessage:
                    if (_messageBox != null)
                    {
                        _messageBox.Close();
                        _messageBox = null;
                    }
                    break;
                case ClientCommand.CommandType.ShowOptions:
                    if (_messageBox == null)
                    {
                        string message = command.GetParameter("Message");
                        Console.WriteLine("showing message: " + message);
                        _messageBox = new MessageBox(message, _gameState, false);
                        string[] options = command.GetParameter("Options").Split(',');
                        for (int i = 0; i < options.Length; i++)
                            _messageBox.AddOption(options[i]);
                        int selectedOption = int.Parse(command.GetParameter("SelectedOption"));
                        _messageBox.SetSelectedOption(selectedOption);
                        _gameState.AddControl(_messageBox);
                    }
                    break;
                case ClientCommand.CommandType.UpdateOptions:
                    if (_messageBox != null)
                    {
                        int selectedOption = int.Parse(command.GetParameter("SelectedOption"));
                        _messageBox.SetSelectedOption(selectedOption);
                    }
                    break;
                case ClientCommand.CommandType.UpdateMapEvent:

                    eventID = int.Parse(command.GetParameter("EventID"));
                    mapX = int.Parse(command.GetParameter("MapX"));
                    mapY = int.Parse(command.GetParameter("MapY"));
                    realX = float.Parse(command.GetParameter("RealX"));
                    realY = float.Parse(command.GetParameter("RealY"));
                    direction = (Direction)int.Parse(command.GetParameter("Direction"));

                    map = ((MapComponent)_gameState.MapEntity.FindComponent<MapComponent>()).GetMapData();
                    if (map != null)
                    {
                        map.GetMapEvent(eventID).MapX = mapX;
                        map.GetMapEvent(eventID).MapY = mapY;
                        map.GetMapEvent(eventID).RealX = realX;
                        map.GetMapEvent(eventID).RealY = realY;
                        map.GetMapEvent(eventID).EventDirection = direction;
                    }

                    break;
                case ClientCommand.CommandType.ChangeMapEventDirection:

                    eventID = int.Parse(command.GetParameter("EventID"));
                    direction = (Direction)int.Parse(command.GetParameter("Direction"));

                    map = ((MapComponent)_gameState.MapEntity.FindComponent<MapComponent>()).GetMapData();
                    if (map != null)
                    {
                        map.GetMapEvent(eventID).EventDirection = direction;
                    }

                    break;

            }
        }

        public void SendMessage(MessagePacket packet)
        {
            _messages.Add(packet);
        }

        private void SendMessagePackets()
        {
            byte[] bytes = BitConverter.GetBytes(_messages.Count);
            _stream.Write(bytes, 0, sizeof(int));
            for (int i = 0; i < _messages.Count; i++)
            {
                bytes = _messages[i].GetBytes();
                _stream.Write(BitConverter.GetBytes(bytes.Length), 0, sizeof(int));
                _stream.Write(bytes, 0, bytes.Length);
            }
            _messages.Clear();
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
            return _connected;
        }

        public void Disconnect()
        {
            if (Connected())
            {
                _stream.Close();
                _client.Close();
                _connected = false;
            }
        }
    }
}

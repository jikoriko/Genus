﻿using Genus2D.GameData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;

namespace Genus2D.Networking
{

    public enum PacketType
    {
        LoginRequest,
        RegisterRequest,
        PlayerPacket,
        MapPacket,
        InputPacket,
        ClientCommand
    }

    public class LoginRequest
    {
        public string Username;
        public string Password;

        public byte[] GetBytes()
        {
            using (MemoryStream stream = new MemoryStream())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(Username);
                stream.Write(BitConverter.GetBytes(bytes.Length), 0, sizeof(int));
                stream.Write(bytes, 0, bytes.Length);

                bytes = Encoding.UTF8.GetBytes(Password);
                stream.Write(BitConverter.GetBytes(bytes.Length), 0, sizeof(int));
                stream.Write(Encoding.UTF8.GetBytes(Password), 0, bytes.Length);

                return stream.ToArray();
            }
        }

        public static LoginRequest FromBytes(byte[] bytes)
        {
            using (MemoryStream stream = new MemoryStream(bytes))
            {
                byte[] tempBytes = new byte[sizeof(int)];
                stream.Read(tempBytes, 0, sizeof(int));
                int usernameSize = BitConverter.ToInt32(tempBytes, 0);

                tempBytes = new byte[usernameSize];
                stream.Read(tempBytes, 0, usernameSize);
                string username = new string(Encoding.UTF8.GetChars(tempBytes));

                tempBytes = new byte[sizeof(int)];
                stream.Read(tempBytes, 0, sizeof(int));
                int passwordSize = BitConverter.ToInt32(tempBytes, 0);

                tempBytes = new byte[passwordSize];
                stream.Read(tempBytes, 0, passwordSize);
                string password = new string(Encoding.UTF8.GetChars(tempBytes));

                LoginRequest request = new LoginRequest();
                request.Username = username;
                request.Password = password;
                return request;
            }
        }
    }

    public class RegisterResult
    {
        public bool Registered;
        public string Reason;

        public byte[] GetBytes()
        {
            using (MemoryStream stream = new MemoryStream())
            {
                stream.Write(BitConverter.GetBytes(Registered), 0, sizeof(bool));
                byte[] stringBytes = Encoding.UTF8.GetBytes(Reason);
                stream.Write(BitConverter.GetBytes(stringBytes.Length), 0, sizeof(int));
                stream.Write(stringBytes, 0, stringBytes.Length);
                return stream.ToArray();
            }
        }

        public static RegisterResult FromBytes(byte[] bytes)
        {
            using (MemoryStream stream = new MemoryStream(bytes))
            {
                byte[] tempBytes = new byte[sizeof(bool)];
                stream.Read(tempBytes, 0, sizeof(bool));
                bool registered = BitConverter.ToBoolean(tempBytes, 0);

                tempBytes = new byte[sizeof(int)];
                stream.Read(tempBytes, 0, sizeof(int));
                int bytesSize = BitConverter.ToInt32(tempBytes, 0);

                tempBytes = new byte[bytesSize];
                stream.Read(tempBytes, 0, bytesSize);
                string reason = new string(Encoding.UTF8.GetChars(tempBytes));

                RegisterResult result = new RegisterResult();
                result.Registered = registered;
                result.Reason = reason;
                return result;
            }
        }
    }


    public class LoginResult
    {
        public bool LoggedIn;
        public int PlayerID;

        public byte[] GetBytes()
        {
            using (MemoryStream stream = new MemoryStream())
            {
                stream.Write(BitConverter.GetBytes(LoggedIn), 0, sizeof(bool));
                stream.Write(BitConverter.GetBytes(PlayerID), 0, sizeof(int));
                return stream.ToArray();
            }
        }

        public static LoginResult FromBytes(byte[] bytes)
        {
            using (MemoryStream stream = new MemoryStream(bytes))
            {
                byte[] tempBytes = new byte[sizeof(bool)];
                stream.Read(tempBytes, 0, sizeof(bool));
                bool loggedIn = BitConverter.ToBoolean(tempBytes, 0);

                tempBytes = new byte[sizeof(int)];
                stream.Read(tempBytes, 0, sizeof(int));
                int playerID = BitConverter.ToInt32(tempBytes, 0);

                LoginResult loginResult = new LoginResult();
                loginResult.LoggedIn = loggedIn;
                loginResult.PlayerID = playerID;
                return loginResult;
            }
        }

    }

    public class PlayerPacket
    {
        public string Username;
        public int PlayerID;
        public int MapID;
        public Direction Direction;
        public int PositionX;
        public int PositionY;
        public float RealX;
        public float RealY;
        public float MovementSpeed;

        public byte[] GetBytes()
        {
            using (MemoryStream stream = new MemoryStream())
            {
                byte[] usernameBytes = Encoding.UTF8.GetBytes(Username);
                stream.Write(BitConverter.GetBytes(usernameBytes.Length), 0, sizeof(int));
                stream.Write(usernameBytes, 0, usernameBytes.Length);
                stream.Write(BitConverter.GetBytes(PlayerID), 0, sizeof(int));
                stream.Write(BitConverter.GetBytes(MapID), 0, sizeof(int));
                stream.Write(BitConverter.GetBytes((int)Direction), 0, sizeof(int));
                stream.Write(BitConverter.GetBytes(PositionX), 0, sizeof(int));
                stream.Write(BitConverter.GetBytes(PositionY), 0, sizeof(int));
                stream.Write(BitConverter.GetBytes(RealX), 0, sizeof(float));
                stream.Write(BitConverter.GetBytes(RealY), 0, sizeof(float));
                stream.Write(BitConverter.GetBytes(MovementSpeed), 0, sizeof(float));

                return stream.ToArray();
            }
        }

        public static PlayerPacket FromBytes(byte[] bytes)
        {
            using (MemoryStream stream = new MemoryStream(bytes))
            {
                byte[] tempBytes = new byte[sizeof(int)];
                stream.Read(tempBytes, 0, sizeof(int));
                int sizeOfUsername = BitConverter.ToInt32(tempBytes, 0);

                tempBytes = new byte[sizeOfUsername];
                stream.Read(tempBytes, 0, sizeOfUsername);
                string username = new string(Encoding.UTF8.GetChars(tempBytes));

                tempBytes = new byte[sizeof(int)];
                stream.Read(tempBytes, 0, sizeof(int));
                int playerID = BitConverter.ToInt32(tempBytes, 0);

                stream.Read(tempBytes, 0, sizeof(int));
                int mapID = BitConverter.ToInt32(tempBytes, 0);

                stream.Read(tempBytes, 0, sizeof(int));
                Direction direction = (Direction)BitConverter.ToInt32(tempBytes, 0);

                stream.Read(tempBytes, 0, sizeof(int));
                int PosX = BitConverter.ToInt32(tempBytes, 0);

                stream.Read(tempBytes, 0, sizeof(int));
                int posY = BitConverter.ToInt32(tempBytes, 0);

                tempBytes = new byte[sizeof(float)];
                stream.Read(tempBytes, 0, sizeof(float));
                float realX = BitConverter.ToSingle(tempBytes, 0);

                stream.Read(tempBytes, 0, sizeof(float));
                float realY = BitConverter.ToSingle(tempBytes, 0);

                stream.Read(tempBytes, 0, sizeof(float));
                float movementSpeed = BitConverter.ToSingle(tempBytes, 0);

                PlayerPacket packet = new PlayerPacket();
                packet.Username = username;
                packet.PlayerID = playerID;
                packet.Username = username;
                packet.MapID = mapID;
                packet.Direction = direction;
                packet.PositionX = PosX;
                packet.PositionY = posY;
                packet.RealX = realX;
                packet.RealY = realY;
                packet.MovementSpeed = movementSpeed;
                return packet;
            }
        }
    }

    public class MapPacket
    {
        public MapData mapData;

        public byte[] GetBytes()
        {
            return mapData.GetBytes();
        }

        public static MapPacket FromBytes(byte[] bytes)
        {
            MapPacket mapPacket = new MapPacket();
            mapPacket.mapData = MapData.FromBytes(bytes);
            return mapPacket;
        }
    }

    public class InputPacket
    {
        public List<int> KeysDown;
        //public OpenTK.Input.MouseState MouseState;

        public InputPacket()
        {
            KeysDown = new List<int>();
        }

        public byte[] GetBytes()
        {
            using (MemoryStream stream = new MemoryStream())
            {
                stream.Write(BitConverter.GetBytes(KeysDown.Count), 0, sizeof(int));

                for (int i = 0; i < KeysDown.Count; i++)
                {
                    stream.Write(BitConverter.GetBytes(KeysDown[i]), 0, sizeof(int));
                }

                return stream.ToArray();
            }
        }

        public static InputPacket FromBytes(byte[] bytes)
        {
            using (MemoryStream stream = new MemoryStream(bytes))
            {
                InputPacket packet = new InputPacket();

                byte[] tempBytes = new byte[sizeof(int)];
                stream.Read(tempBytes, 0, sizeof(int));
                int numKeysDown = BitConverter.ToInt32(tempBytes, 0);

                for (int i = 0; i < numKeysDown; i++)
                {
                    stream.Read(tempBytes, 0, sizeof(int));
                    packet.KeysDown.Add(BitConverter.ToInt32(tempBytes, 0));
                }

                return packet;
            }
        }
    }

}

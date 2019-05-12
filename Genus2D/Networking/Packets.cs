using Genus2D.GameData;
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
        ServerCommand,
        ClientCommand,
        SendMessagePackets,
        RecieveMessagePackets
        
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

    public class MessagePacket
    {

        public enum MessageTarget
        {
            Public,
            Private
        }

        public string Message;
        public int PlayerID;
        public MessageTarget TargetType;
        public int TargetID;

        public MessagePacket(string message)
        {
            Message = message;
            PlayerID = -1;
            TargetType = MessageTarget.Public;
            TargetID = -1;
        }

        public byte[] GetBytes()
        {
            using (MemoryStream stream = new MemoryStream())
            {
                byte[] messageBytes = Encoding.UTF8.GetBytes(Message);
                stream.Write(BitConverter.GetBytes(messageBytes.Length), 0, sizeof(int));
                stream.Write(messageBytes, 0, messageBytes.Length);
                stream.Write(BitConverter.GetBytes(PlayerID), 0, sizeof(int));
                stream.Write(BitConverter.GetBytes((int)TargetType), 0, sizeof(int));
                stream.Write(BitConverter.GetBytes(TargetID), 0, sizeof(int));

                return stream.ToArray();
            }
        }

        public static MessagePacket FromBytes(byte[] bytes)
        {
            using (MemoryStream stream = new MemoryStream(bytes))
            {
                byte[] tempBytes = new byte[sizeof(int)];
                stream.Read(tempBytes, 0, sizeof(int));
                int messageSize = BitConverter.ToInt32(tempBytes, 0);

                tempBytes = new byte[messageSize];
                stream.Read(tempBytes, 0, messageSize);
                string message = new string(Encoding.UTF8.GetChars(tempBytes));

                tempBytes = new byte[sizeof(int)];
                stream.Read(tempBytes, 0, sizeof(int));
                int playerId = BitConverter.ToInt32(tempBytes, 0);

                tempBytes = new byte[sizeof(int)];
                stream.Read(tempBytes, 0, sizeof(int));
                int targetType = BitConverter.ToInt32(tempBytes, 0);

                tempBytes = new byte[sizeof(int)];
                stream.Read(tempBytes, 0, sizeof(int));
                int targetId = BitConverter.ToInt32(tempBytes, 0);

                MessagePacket packet = new MessagePacket(message);
                packet.PlayerID = playerId;
                packet.TargetType = (MessageTarget)targetType;
                packet.TargetID = targetId;
                return packet;
            }
        }
        

    }

    public class PlayerPacket
    {
        public string Username;
        public int PlayerID;
        public int MapID;
        public int SpriteID;
        public FacingDirection Direction;
        public int PositionX;
        public int PositionY;
        public float RealX;
        public float RealY;
        public float MovementSpeed;
        public bool OnBridge;
        public PlayerData Data;

        public byte[] GetBytes(bool isLocalPlayer)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                byte[] usernameBytes = Encoding.UTF8.GetBytes(Username);
                stream.Write(BitConverter.GetBytes(usernameBytes.Length), 0, sizeof(int));
                stream.Write(usernameBytes, 0, usernameBytes.Length);
                stream.Write(BitConverter.GetBytes(PlayerID), 0, sizeof(int));
                stream.Write(BitConverter.GetBytes(MapID), 0, sizeof(int));
                stream.Write(BitConverter.GetBytes(SpriteID), 0, sizeof(int));
                stream.Write(BitConverter.GetBytes((int)Direction), 0, sizeof(int));
                stream.Write(BitConverter.GetBytes(PositionX), 0, sizeof(int));
                stream.Write(BitConverter.GetBytes(PositionY), 0, sizeof(int));
                stream.Write(BitConverter.GetBytes(RealX), 0, sizeof(float));
                stream.Write(BitConverter.GetBytes(RealY), 0, sizeof(float));
                stream.Write(BitConverter.GetBytes(MovementSpeed), 0, sizeof(float));
                stream.Write(BitConverter.GetBytes(OnBridge), 0, sizeof(bool));

                byte[] dataBytes = Data.GetBytes(isLocalPlayer);
                stream.Write(BitConverter.GetBytes(dataBytes.Length), 0, sizeof(int));
                stream.Write(dataBytes, 0, dataBytes.Length);

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
                int spriteID = BitConverter.ToInt32(tempBytes, 0);

                stream.Read(tempBytes, 0, sizeof(int));
                FacingDirection direction = (FacingDirection)BitConverter.ToInt32(tempBytes, 0);

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

                tempBytes = new byte[sizeof(bool)];
                stream.Read(tempBytes, 0, sizeof(bool));
                bool onBridge = BitConverter.ToBoolean(tempBytes, 0);

                tempBytes = new byte[sizeof(int)];
                stream.Read(tempBytes, 0, sizeof(int));
                int dataSize = BitConverter.ToInt32(tempBytes, 0);

                tempBytes = new byte[dataSize];
                stream.Read(tempBytes, 0, dataSize);
                PlayerData data = PlayerData.FromBytes(tempBytes);

                PlayerPacket packet = new PlayerPacket();
                packet.Username = username;
                packet.PlayerID = playerID;
                packet.Username = username;
                packet.MapID = mapID;
                packet.SpriteID = spriteID;
                packet.Direction = direction;
                packet.PositionX = PosX;
                packet.PositionY = posY;
                packet.RealX = realX;
                packet.RealY = realY;
                packet.MovementSpeed = movementSpeed;
                packet.OnBridge = onBridge;
                packet.Data = data;
                return packet;
            }
        }

        public override string ToString()
        {
            string s = ": ";
            s += string.Format("{0}, {1}, {2}, {3}, {4}, {5}, {6}", PlayerID, Username, MapID, SpriteID, Direction.ToString(), PositionX, PositionY);

            return base.ToString() + s;
        }
    }

    public class MapPacket
    {
        public int MapID;
        public MapData mapData;

        public List<MapEnemy> Enemies;
        public List<Projectile> Projectiles;
        public List<MapItem> Items;

        public MapPacket(int id, MapData data)
        {
            MapID = id;
            mapData = data;

            Enemies = new List<MapEnemy>();
            Projectiles = new List<Projectile>();
            Items = new List<MapItem>();
        }

        public byte[] GetBytes()
        {
            using (MemoryStream stream = new MemoryStream())
            {
                stream.Write(BitConverter.GetBytes(MapID), 0, sizeof(int));

                byte[] mapBytes = mapData.GetBytes();
                stream.Write(BitConverter.GetBytes(mapBytes.Length), 0, sizeof(int));
                stream.Write(mapBytes, 0, mapBytes.Length);

                stream.Write(BitConverter.GetBytes(Enemies.Count), 0, sizeof(int));
                for (int i = 0; i < Enemies.Count; i++)
                {
                    byte[] enemyBytes = Enemies[i].GetBytes();
                    stream.Write(BitConverter.GetBytes(enemyBytes.Length), 0, sizeof(int));
                    stream.Write(enemyBytes, 0, enemyBytes.Length);
                }

                stream.Write(BitConverter.GetBytes(Projectiles.Count), 0, sizeof(int));
                for (int i = 0; i < Projectiles.Count; i++)
                {
                    byte[] projectileBytes = Projectiles[i].GetBytes();
                    stream.Write(BitConverter.GetBytes(projectileBytes.Length), 0, sizeof(int));
                    stream.Write(projectileBytes, 0, projectileBytes.Length);
                }

                stream.Write(BitConverter.GetBytes(Items.Count), 0, sizeof(int));
                for (int i = 0; i < Items.Count; i++)
                {
                    byte[] itemBytes = Items[i].GetBytes();
                    stream.Write(BitConverter.GetBytes(itemBytes.Length), 0, sizeof(int));
                    stream.Write(itemBytes, 0, itemBytes.Length);
                }

                return stream.ToArray();
            }
        }

        public static MapPacket FromBytes(byte[] bytes)
        {
            using (MemoryStream stream = new MemoryStream(bytes))
            {
                byte[] tempBytes = new byte[sizeof(int)];
                stream.Read(tempBytes, 0, sizeof(int));
                int mapID = BitConverter.ToInt32(tempBytes, 0);

                stream.Read(tempBytes, 0, sizeof(int));
                int dataSize = BitConverter.ToInt32(tempBytes, 0);

                tempBytes = new byte[dataSize];
                stream.Read(tempBytes, 0, dataSize);

                MapPacket mapPacket = new MapPacket(mapID, MapData.FromBytes(tempBytes));

                tempBytes = new byte[sizeof(int)];
                stream.Read(tempBytes, 0, sizeof(int));
                int enemiesCount = BitConverter.ToInt32(tempBytes, 0);

                for (int i = 0; i < enemiesCount; i++)
                {
                    tempBytes = new byte[sizeof(int)];
                    stream.Read(tempBytes, 0, sizeof(int));
                    int size = BitConverter.ToInt32(tempBytes, 0);

                    tempBytes = new byte[size];
                    stream.Read(tempBytes, 0, size);
                    MapEnemy enemy = MapEnemy.FromBytes(tempBytes);
                    mapPacket.Enemies.Add(enemy);
                }

                tempBytes = new byte[sizeof(int)];
                stream.Read(tempBytes, 0, sizeof(int));
                int projectilesCount = BitConverter.ToInt32(tempBytes, 0);

                for (int i = 0; i < projectilesCount; i++)
                {
                    tempBytes = new byte[sizeof(int)];
                    stream.Read(tempBytes, 0, sizeof(int));
                    int size = BitConverter.ToInt32(tempBytes, 0);

                    tempBytes = new byte[size];
                    stream.Read(tempBytes, 0, size);
                    Projectile projectile = Projectile.FromBytes(tempBytes);
                    mapPacket.Projectiles.Add(projectile);
                }

                tempBytes = new byte[sizeof(int)];
                stream.Read(tempBytes, 0, sizeof(int));
                int itemsCount = BitConverter.ToInt32(tempBytes, 0);

                for (int i = 0; i < itemsCount; i++)
                {
                    tempBytes = new byte[sizeof(int)];
                    stream.Read(tempBytes, 0, sizeof(int));
                    int size = BitConverter.ToInt32(tempBytes, 0);

                    tempBytes = new byte[size];
                    stream.Read(tempBytes, 0, size);
                    MapItem item = MapItem.FromBytes(tempBytes);
                    mapPacket.Items.Add(item);
                }

                return mapPacket;
            }
        }
    }

}

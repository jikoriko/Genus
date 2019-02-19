using OpenTK;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace Genus2D.GameData
{

    [Serializable]
    public class MapEvent
    {
        public string Name;
        public int EventID;
        public int MapX;
        public int MapY;
        public float RealX;
        public float RealY;
        public FacingDirection EventDirection;
        public int SpriteID;
        public EventTriggerType TriggerType;
        public bool Passable;

        public bool Moved = false;
        public bool Locked = false;

        public MapEvent(string name, int id, int x, int y)
        {
            Name = name;
            EventID = id;
            MapX = x;
            MapY = y;
            RealX = x * 32;
            RealY = y * 32;
            EventDirection = FacingDirection.Down;
            SpriteID = -1;
            TriggerType = EventTriggerType.None;
            Passable = false;
        }

        public byte[] GetBytes()
        {
            using (MemoryStream stream = new MemoryStream())
            {
                byte[] nameBytes = Encoding.UTF8.GetBytes(Name);
                stream.Write(BitConverter.GetBytes(nameBytes.Length), 0, sizeof(int));
                stream.Write(nameBytes, 0, nameBytes.Length);
                stream.Write(BitConverter.GetBytes(EventID), 0, sizeof(int));
                stream.Write(BitConverter.GetBytes(MapX), 0, sizeof(int));
                stream.Write(BitConverter.GetBytes(MapY), 0, sizeof(int));
                stream.Write(BitConverter.GetBytes(RealX), 0, sizeof(float));
                stream.Write(BitConverter.GetBytes(RealY), 0, sizeof(float));
                stream.Write(BitConverter.GetBytes((int)EventDirection), 0, sizeof(int));
                stream.Write(BitConverter.GetBytes(SpriteID), 0, sizeof(int));
                stream.Write(BitConverter.GetBytes((int)TriggerType), 0, sizeof(int));
                stream.Write(BitConverter.GetBytes(Passable), 0, sizeof(bool));
                return stream.ToArray();
            }
        }

        public static MapEvent FromBytes(byte[] bytes)
        {
            using (MemoryStream stream = new MemoryStream(bytes))
            {
                byte[] tempBytes = new byte[sizeof(int)];
                stream.Read(tempBytes, 0, sizeof(int));
                int nameLength = BitConverter.ToInt32(tempBytes, 0);

                tempBytes = new byte[nameLength];
                stream.Read(tempBytes, 0, nameLength);
                string name = new string(Encoding.UTF8.GetChars(tempBytes, 0, nameLength));

                tempBytes = new byte[sizeof(int)];
                stream.Read(tempBytes, 0, sizeof(int));
                int eventID = BitConverter.ToInt32(tempBytes, 0);

                stream.Read(tempBytes, 0, sizeof(int));
                int mapX = BitConverter.ToInt32(tempBytes, 0);

                stream.Read(tempBytes, 0, sizeof(int));
                int mapY = BitConverter.ToInt32(tempBytes, 0);

                tempBytes = new byte[sizeof(float)];
                stream.Read(tempBytes, 0, sizeof(float));
                float realX = BitConverter.ToSingle(tempBytes, 0);

                stream.Read(tempBytes, 0, sizeof(float));
                float realY = BitConverter.ToSingle(tempBytes, 0);

                tempBytes = new byte[sizeof(int)];
                stream.Read(tempBytes, 0, sizeof(int));
                FacingDirection direction = (FacingDirection)BitConverter.ToInt32(tempBytes, 0);

                stream.Read(tempBytes, 0, sizeof(int));
                int spriteID = BitConverter.ToInt32(tempBytes, 0);

                stream.Read(tempBytes, 0, sizeof(int));
                EventTriggerType triggerType = (EventTriggerType)BitConverter.ToInt32(tempBytes, 0);

                tempBytes = new byte[sizeof(bool)];
                stream.Read(tempBytes, 0, sizeof(bool));
                bool passable = BitConverter.ToBoolean(tempBytes, 0);

                MapEvent mapEvent = new MapEvent(name, eventID, mapX, mapY);
                mapEvent.RealX = realX;
                mapEvent.RealY = realY;
                mapEvent.EventDirection = direction;
                mapEvent.SpriteID = spriteID;
                mapEvent.TriggerType = triggerType;
                mapEvent.Passable = passable;
                return mapEvent;
            }
        }

        public EventData GetEventData()
        {
            return EventData.GetEventData(EventID);
        }

        public bool Moving()
        {
            if (MapX * 32 != RealX || MapY * 32 != RealY)
                return true;
            return false;
        }

        public void UpdateMovement(float deltaTime)
        {
            if (Moving())
            {
                Vector2 realPos = new Vector2(RealX, RealY);
                Vector2 dir = new Vector2(MapX * 32, MapY * 32) - realPos;
                dir.Normalize();
                realPos += (dir * 64.0f * deltaTime);

                dir = new Vector2(MapX * 32, MapY * 32) - realPos;
                if (dir.Length <= 0.1f)
                {
                    realPos = new OpenTK.Vector2(MapX * 32, MapY * 32);
                }

                RealX = realPos.X;
                RealY = realPos.Y;
                Moved = true;
            }
        }
    }

}

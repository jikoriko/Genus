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
        public int EventID;
        public int MapX;
        public int MapY;
        public float RealX;
        public float RealY;
        public Direction EventDirection;

        public bool Moved = false;

        public MapEvent(int id, int x, int y)
        {
            EventID = id;
            MapX = x;
            MapY = y;
            RealX = x * 32;
            RealY = y * 32;
            EventDirection = Direction.Down;
        }

        public static int SizeOfBytes()
        {
            int size = 0;
            size += sizeof(int) * 4;
            size += sizeof(float) * 2;
            return size;
        }

        public byte[] GetBytes()
        {

            using (MemoryStream stream = new MemoryStream())
            {
                Console.WriteLine("sending map event: " + MapX + "," + MapY);
                stream.Write(BitConverter.GetBytes(EventID), 0, sizeof(int));
                stream.Write(BitConverter.GetBytes(MapX), 0, sizeof(int));
                stream.Write(BitConverter.GetBytes(MapY), 0, sizeof(int));
                stream.Write(BitConverter.GetBytes(RealX), 0, sizeof(float));
                stream.Write(BitConverter.GetBytes(RealY), 0, sizeof(float));
                stream.Write(BitConverter.GetBytes((int)EventDirection), 0, sizeof(int));
                return stream.ToArray();
            }
        }

        public static MapEvent FromBytes(byte[] bytes)
        {
            using (MemoryStream stream = new MemoryStream(bytes))
            {
                byte[] tempBytes = new byte[sizeof(int)];
                stream.Read(tempBytes, 0, sizeof(int));
                int eventID = BitConverter.ToInt32(tempBytes, 0);

                stream.Read(tempBytes, 0, sizeof(int));
                int mapX = BitConverter.ToInt32(tempBytes, 0);

                stream.Read(tempBytes, 0, sizeof(int));
                int mapY = BitConverter.ToInt32(tempBytes, 0);

                stream.Read(tempBytes, 0, sizeof(float));
                float realX = BitConverter.ToSingle(tempBytes, 0);

                stream.Read(tempBytes, 0, sizeof(float));
                float realY = BitConverter.ToSingle(tempBytes, 0);

                stream.Read(tempBytes, 0, sizeof(int));
                Direction direction = (Direction)BitConverter.ToInt32(tempBytes, 0);

                Console.WriteLine("recieved map event: " + mapX + "," + mapY);

                MapEvent mapEvent = new MapEvent(eventID, mapX, mapY);
                mapEvent.RealX = realX;
                mapEvent.RealY = realY;
                mapEvent.EventDirection = direction;
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

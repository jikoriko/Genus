using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genus2D.GameData
{
    public class MapItem
    {

        public int ItemID;
        public int Count;
        public int MapX;
        public int MapY;
        public int PlayerID;
        public bool OnBridge;

        public float PlayerLockTimer;
        public float DespawnTimer;
        public bool PickedUp;

        public bool Changed;

        public MapItem(int itemID, int count, int mapX, int mapY, int playerID, bool onBridge)
        {
            ItemID = itemID;
            Count = count;
            MapX = mapX;
            MapY = mapY;
            OnBridge = onBridge;

            PlayerID = playerID;
            PlayerLockTimer = 60f;
            DespawnTimer = 120f;
            PickedUp = false;

            Changed = false;
        }

        public int GetSignature()
        {
            byte[] combinedBytes = new byte[sizeof(int)];
            byte[] a = BitConverter.GetBytes(ItemID);
            byte[] b = BitConverter.GetBytes(Count);
            byte[] c = BitConverter.GetBytes(MapX);
            byte[] d = BitConverter.GetBytes(MapY);
            for (int i = 0; i < sizeof(int); i++)
            {
                combinedBytes[i] = (byte)(a[i] + b[i] + c[i] + d[i]);
            }
            if (OnBridge) combinedBytes[0]++;

            return BitConverter.ToInt32(combinedBytes, 0);
        }

        public byte[] GetBytes()
        {
            using (MemoryStream stream = new MemoryStream())
            {
                stream.Write(BitConverter.GetBytes(ItemID), 0, sizeof(int));
                stream.Write(BitConverter.GetBytes(Count), 0, sizeof(int));
                stream.Write(BitConverter.GetBytes(MapX), 0, sizeof(int));
                stream.Write(BitConverter.GetBytes(MapY), 0, sizeof(int));
                stream.Write(BitConverter.GetBytes(PlayerID), 0, sizeof(int));
                stream.Write(BitConverter.GetBytes(OnBridge), 0, sizeof(bool));
                return stream.ToArray();
            }
        }

        public static MapItem FromBytes(byte[] bytes)
        {
            using (MemoryStream stream = new MemoryStream(bytes))
            {
                byte[] tempBytes = new byte[sizeof(int)];
                stream.Read(tempBytes, 0, sizeof(int));
                int itemID = BitConverter.ToInt32(tempBytes, 0);

                stream.Read(tempBytes, 0, sizeof(int));
                int count = BitConverter.ToInt32(tempBytes, 0);

                stream.Read(tempBytes, 0, sizeof(int));
                int mapX = BitConverter.ToInt32(tempBytes, 0);

                stream.Read(tempBytes, 0, sizeof(int));
                int mapY = BitConverter.ToInt32(tempBytes, 0);

                stream.Read(tempBytes, 0, sizeof(int));
                int playerID = BitConverter.ToInt32(tempBytes, 0);

                tempBytes = new byte[sizeof(bool)];
                stream.Read(tempBytes, 0, sizeof(bool));
                bool onBridge = BitConverter.ToBoolean(tempBytes, 0);

                MapItem item = new MapItem(itemID, count, mapX, mapY, playerID, onBridge);
                return item;
            }
        }
    }
}

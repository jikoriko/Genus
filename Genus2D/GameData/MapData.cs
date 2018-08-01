using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace Genus2D.GameData
{

    [Serializable]
    public class MapData
    {

        public static readonly int NUM_LAYERS = 3;

        private string _mapName;
        private int _width;
        private int _height;
        private int _tilesetID;
        private int[] _mapData;
        private List<MapEvent> _mapEvents;

        public MapData(string mapName, int width, int height, int tilesetID)
        {
            _mapName = mapName;
            _width = width;
            _height = height;
            _tilesetID = tilesetID;
            _mapData = new int[height * width * NUM_LAYERS];
            _mapEvents = new List<MapEvent>();
        }

        public byte[] GetBytes()
        {
            using (MemoryStream stream = new MemoryStream())
            {
                byte[] nameBytes = Encoding.UTF8.GetBytes(_mapName);
                stream.Write(BitConverter.GetBytes(nameBytes.Length), 0, sizeof(int));
                stream.Write(nameBytes, 0, nameBytes.Length);

                stream.Write(BitConverter.GetBytes(_width), 0, sizeof(int));
                stream.Write(BitConverter.GetBytes(_height), 0, sizeof(int));
                stream.Write(BitConverter.GetBytes(_tilesetID), 0, sizeof(int));

                stream.Write(BitConverter.GetBytes(_mapData.Length), 0, sizeof(int));
                for (int i = 0; i < _mapData.Length; i++)
                {
                    stream.Write(BitConverter.GetBytes(_mapData[i]), 0, sizeof(int));
                }

                stream.Write(BitConverter.GetBytes(_mapEvents.Count), 0, sizeof(int));
                for (int i = 0; i < _mapEvents.Count; i++)
                {
                    byte[] bytes = _mapEvents[i].GetBytes();
                    stream.Write(bytes, 0, bytes.Length);
                }

                return stream.ToArray();
            }
        }

        public static MapData FromBytes(byte[] bytes)
        {
            using (MemoryStream stream = new MemoryStream(bytes))
            {
                byte[] tempBytes = new byte[sizeof(int)];
                stream.Read(tempBytes, 0, sizeof(int));
                int nameSize = BitConverter.ToInt32(tempBytes, 0);

                tempBytes = new byte[nameSize];
                stream.Read(tempBytes, 0, nameSize);
                string mapName = new string(Encoding.UTF8.GetChars(tempBytes));

                tempBytes = new byte[sizeof(int)];
                stream.Read(tempBytes, 0, sizeof(int));
                int width = BitConverter.ToInt32(tempBytes, 0);

                stream.Read(tempBytes, 0, sizeof(int));
                int height = BitConverter.ToInt32(tempBytes, 0);

                stream.Read(tempBytes, 0, sizeof(int));
                int tilesetID = BitConverter.ToInt32(tempBytes, 0);

                stream.Read(tempBytes, 0, sizeof(int));
                int mapDataSize = BitConverter.ToInt32(tempBytes, 0);

                int[] mapData = new int[mapDataSize];

                for (int i = 0; i < mapDataSize; i++)
                {
                    stream.Read(tempBytes, 0, sizeof(int));
                    mapData[i] = BitConverter.ToInt32(tempBytes, 0);
                }

                stream.Read(tempBytes, 0, sizeof(int));
                int mapEventsSize = BitConverter.ToInt32(tempBytes, 0);

                MapData data = new MapData(mapName, width, height, tilesetID);
                data._mapData = mapData;

                tempBytes = new byte[MapEvent.SizeOfBytes()];

                for (int i = 0; i < mapEventsSize; i++)
                {
                    stream.Read(tempBytes, 0, MapEvent.SizeOfBytes());
                    MapEvent mapEvent = MapEvent.FromBytes(tempBytes);
                    data.AddMapEvent(mapEvent);
                }

                return data;
            }
        }

        public string GetMapName()
        {
            return _mapName;
        }

        public int GetWidth()
        {
            return _width;
        }

        public int GetHeight()
        {
            return _height;
        }

        public int GetTilesetID()
        {
            return _tilesetID;
        }

        public void SetTilesetID(int id)
        {
            _tilesetID = id;
        }

        public int[] GetMapData()
        {
            return _mapData;
        }

        public int GetTileID(int layer, int x, int y)
        {
            if (x > -1 && y > -1 && x < _width && y < _height)
                return _mapData[x + _height * (y + _width * layer)];
            return -1;
        }

        public void SetTileID(int layer, int x, int y, int id)
        {
            if (x > -1 && y > -1 && x < _width && y < _height)
                _mapData[x + _height * (y + _width * layer)] = id;
        }

        public void AddMapEvent(MapEvent mapEvent)
        {
            _mapEvents.Add(mapEvent);
        }

        public void RemoveMapEvent(int index)
        {
            if (index >= 0 && index < _mapEvents.Count)
                _mapEvents.RemoveAt(index);
        }

        public MapEvent GetMapEvent(int index)
        {
            if (index >= 0 && index < _mapEvents.Count)
                return _mapEvents[index];
            return null;
        }

        public int MapEventsCount()
        {
            return _mapEvents.Count;
        }

    }
}

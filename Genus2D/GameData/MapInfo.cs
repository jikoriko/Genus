using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace Genus2D.GameData
{
    [Serializable]
    public class MapInfo
    {

        private static List<MapInfo> _mapInfos = LoadMapInfos();
        private static List<MapInfo> LoadMapInfos()
        {
            List<MapInfo> mapInfo;

            if (File.Exists("Data/MapInfo.data"))
            {
                Stream stream = File.OpenRead("Data/MapInfo.data");
                BinaryFormatter formatter = new BinaryFormatter();
                mapInfo = (List<MapInfo>)formatter.Deserialize(stream);
                stream.Close();
            }
            else
            {
                mapInfo = new List<MapInfo>();
            }

            return mapInfo;
        }

        private static void SaveMapInfos()
        {
            if (!Directory.Exists("Data"))
                Directory.CreateDirectory("Data");

            Stream stream = File.Create("Data/MapInfo.data");
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, _mapInfos);
            stream.Close();
        }

        public static bool AddMapInfo(string mapName, int width, int height)
        {
            for (int i = 0; i < _mapInfos.Count; i++)
            {
                if (_mapInfos[i].MapName == mapName)
                    return false;
            }

            _mapInfos.Add(new MapInfo(mapName, width, height));
            SaveMapInfos();
            return true;
        }

        public static MapInfo GetMapInfo(int index)
        {
            if (index >= 0 && index < _mapInfos.Count)
            {
                return _mapInfos[index];
            }
            return null;
        }

        public static List<string> GetMapInfoStrings()
        {
            List<string> strings = new List<string>();

            for (int i = 0; i < _mapInfos.Count; i++)
            {
                strings.Add(_mapInfos[i].MapName);
            }

            return strings;
        }

        public static MapData LoadMap(int index)
        {
            if (index >= 0 && index < _mapInfos.Count)
            {
                string filename = "Data/Maps/" + _mapInfos[index].MapName + ".mapData";

                MapData mapData = null;

                if (File.Exists(filename))
                {
                    FileStream stream = File.Open(filename, FileMode.Open, FileAccess.Read);
                    BinaryFormatter formatter = new BinaryFormatter();
                    mapData = (MapData)formatter.Deserialize(stream);
                    stream.Close();
                }

                return mapData;
            }

            return null;
        }

        public static void SaveMap(MapData mapData)
        {
            if (!Directory.Exists("Data"))
                Directory.CreateDirectory("Data");

            if (!Directory.Exists("Data/Maps"))
                Directory.CreateDirectory("Data/Maps");

            string filename = "Data/Maps/" + mapData.GetMapName() + ".mapData";

            FileStream stream = File.Create(filename);
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, mapData);
            stream.Close();
        }
        

        public string MapName { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        
        public MapInfo(string name, int width, int height)
        {
            MapName = name;
            Width = width;
            Height = height;
        }

    }
}

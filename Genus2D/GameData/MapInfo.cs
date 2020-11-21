using SharpFont;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Genus2D.GameData
{
    [Serializable]
    public class MapInfo : IXmlSerializable
    {
        [Serializable]
        public class MapInfoData
        {
            public List<MapInfo> MapInfos;
            public List<SpawnPoint> SpawnPoints;
        }

        private static MapInfoData _mapInfoData;

        public static void ReloadData()
        {
            _mapInfoData = LoadMapInfoData();
        }

        private static MapInfoData LoadMapInfoData()
        {
            MapInfoData data;

            //if (File.Exists("Data/MapInfo.data"))
            //{
            //    Stream stream = File.OpenRead("Data/MapInfo.data");
            //    BinaryFormatter formatter = new BinaryFormatter();
            //    data.MapInfos = (List<MapInfo>)formatter.Deserialize(stream);
            //    data.SpawnPoints = (List<SpawnPoint>)formatter.Deserialize(stream);
            //    stream.Close();
            //}
            if (File.Exists("Data/MapInfo.xml"))
            {
                FileStream stream = File.Open("Data/MapInfo.xml", FileMode.Open, FileAccess.Read);
                XmlSerializer serializer = new XmlSerializer(typeof(MapInfoData));
                data = (MapInfoData)serializer.Deserialize(stream);
                stream.Close();
            }
            else
            {
                data = new MapInfoData();
                data.MapInfos = new List<MapInfo>();
                data.SpawnPoints = new List<SpawnPoint>();
            }

            return data;
        }

        private static void SaveMapInfos()
        {
            if (!Directory.Exists("Data"))
                Directory.CreateDirectory("Data");

            //Stream stream = File.Create("Data/MapInfo.data");
            //BinaryFormatter formatter = new BinaryFormatter();
            //formatter.Serialize(stream, _mapInfos);
            //formatter.Serialize(stream, _spawnPoints);
            //stream.Close();

            FileStream stream = File.Create("Data/MapInfo.xml");
            XmlSerializer serializer = new XmlSerializer(typeof(MapInfoData));
            serializer.Serialize(stream, _mapInfoData);
            stream.Close();
        }

        public static bool AddMapInfo(string mapName, int width, int height)
        {
            for (int i = 0; i < _mapInfoData.MapInfos.Count; i++)
            {
                if (_mapInfoData.MapInfos[i].MapName == mapName)
                    return false;
            }

            _mapInfoData.MapInfos.Add(new MapInfo(mapName, width, height));
            SaveMapInfos();
            return true;
        }

        public static MapInfo GetMapInfo(int index)
        {
            if (index >= 0 && index < _mapInfoData.MapInfos.Count)
            {
                return _mapInfoData.MapInfos[index];
            }
            return null;
        }

        public static int GetMapID(string name)
        {
            for (int i = 0; i < _mapInfoData.MapInfos.Count; i++)
            {
                if (_mapInfoData.MapInfos[i].MapName == name)
                {
                    return i;
                }
            }
            return -1;
        }

        public static bool RenameMap(int index, string name)
        {
            if (index >= 0 && index < _mapInfoData.MapInfos.Count)
            {
                for (int i = 0; i < _mapInfoData.MapInfos.Count; i++)
                {
                    if (i == index)
                        continue;
                    if (_mapInfoData.MapInfos[i].MapName == name)
                    {
                        return false;
                    }
                }
                File.Delete("Data/Maps/" + _mapInfoData.MapInfos[index].MapName + ".mapData");
                _mapInfoData.MapInfos[index].MapName = name;
                SaveMapInfos();
                return true;
            }
            return false;
        }

        public static void DeleteMap(int index)
        {
            if (index >= 0 && index < _mapInfoData.MapInfos.Count)
            {
                File.Delete("Data/Maps/" + _mapInfoData.MapInfos[index].MapName + ".mapData");
                _mapInfoData.MapInfos.RemoveAt(index);
                SaveMapInfos();
            }
        }

        public static void ResizeMap(int index, int width, int height)
        {
            if (index >= 0 && index < _mapInfoData.MapInfos.Count)
            {
                _mapInfoData.MapInfos[index].Width = width;
                _mapInfoData.MapInfos[index].Height = height;
                SaveMapInfos();
            }
        }

        public static List<string> GetMapInfoStrings()
        {
            List<string> strings = new List<string>();

            for (int i = 0; i < _mapInfoData.MapInfos.Count; i++)
            {
                strings.Add(_mapInfoData.MapInfos[i].MapName);
            }

            return strings;
        }

        public static int NumberMaps()
        {
            return _mapInfoData.MapInfos.Count;
        }

        public static void AddSpawnPoint(SpawnPoint spawn)
        {
            for (int i = 0; i < _mapInfoData.SpawnPoints.Count; i++)
            {
                SpawnPoint point = _mapInfoData.SpawnPoints[i];
                if (point.MapID == spawn.MapID && point.MapX == spawn.MapX && point.MapY == spawn.MapY)
                    return;
            }
            _mapInfoData.SpawnPoints.Add(spawn);
            SaveMapInfos();
        }

        public static void RemoveSpawnPoint(int index)
        {
            if (index >= 0 && index < _mapInfoData.SpawnPoints.Count)
            {
                _mapInfoData.SpawnPoints.RemoveAt(index);
                SaveMapInfos();
            }
        }

        public static SpawnPoint GetSpawnPoint(int index)
        {
            if (index >= 0 && index < _mapInfoData.SpawnPoints.Count)
                return _mapInfoData.SpawnPoints[index];
            return null;
        }

        public static SpawnPoint GetSpawnPoint(string name)
        {
            for (int i = 0; i < _mapInfoData.SpawnPoints.Count; i++)
            {
                if (_mapInfoData.SpawnPoints[i].Label == name)
                    return _mapInfoData.SpawnPoints[i];
            }
            return null;
        }

        public static int NumberSpawnPoints()
        {
            return _mapInfoData.SpawnPoints.Count;
        }

        public static void SetMapEventsCount(int mapID, int count)
        {
            if (mapID >= 0 && mapID < _mapInfoData.MapInfos.Count)
            {
                _mapInfoData.MapInfos[mapID].NumberMapEvents = count;
                SaveMapInfos();
            }
        }

        public static MapData LoadMap(int index)
        {
            if (index >= 0 && index < _mapInfoData.MapInfos.Count)
            {
                //string filename = "Data/Maps/" + _mapInfoData.MapInfos[index].MapName + ".mapData";
                string filename = "Data/Maps/" + _mapInfoData.MapInfos[index].MapName + ".xml";

                MapData data = null;

                /*
                if (File.Exists(filename))
                {
                    FileStream stream = File.Open(filename, FileMode.Open, FileAccess.Read);
                    BinaryFormatter formatter = new BinaryFormatter();
                    data = (MapData)formatter.Deserialize(stream);
                    stream.Close();
                }
                //*/

                //*
                if (File.Exists(filename))
                {
                    FileStream stream = File.Open(filename, FileMode.Open, FileAccess.Read);
                    XmlSerializer serializer = new XmlSerializer(typeof(MapData));
                    data = (MapData)serializer.Deserialize(stream);
                    stream.Close();
                }
                //*/

                return data;
            }

            return null;
        }

        public static void SaveMap(MapData mapData)
        {
            if (!Directory.Exists("Data"))
                Directory.CreateDirectory("Data");

            if (!Directory.Exists("Data/Maps"))
                Directory.CreateDirectory("Data/Maps");

            //string filename = "Data/Maps/" + mapData.GetMapName() + ".mapData";
            string filename = "Data/Maps/" + mapData.GetMapName() + ".xml";

            AddMapInfo(mapData.GetMapName(), mapData.GetWidth(), mapData.GetHeight());

            //FileStream stream = File.Create(filename);
            //BinaryFormatter formatter = new BinaryFormatter();
            //formatter.Serialize(stream, mapData);
            //stream.Close();

            FileStream stream = File.Create(filename);
            XmlSerializer serializer = new XmlSerializer(typeof(MapData));
            serializer.Serialize(stream, mapData);
            stream.Close();
        }


        public string MapName { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }

        public int NumberMapEvents { get; private set; }
        
        public MapInfo()
        {
            MapName = "";
            Width = -1;
            Height = -1;
            NumberMapEvents = 0;
        }
        
        public MapInfo(string name, int width, int height)
        {
            MapName = name;
            Width = width;
            Height = height;
            NumberMapEvents = 0;
        }

        public XmlSchema GetSchema()
        {
            return (null);
        }

        public void ReadXml(XmlReader reader)
        {
            string xml = reader.ReadOuterXml();
            reader = XmlReader.Create(new StringReader(xml));

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (reader.LocalName == "MapName")
                    {
                        reader.Read();
                        MapName = reader.ReadContentAsString();
                    }
                    else if (reader.LocalName == "Width")
                    {
                        reader.Read();
                        Width = reader.ReadContentAsInt();
                    }

                    else if (reader.LocalName == "Height")
                    {
                        reader.Read();
                        Height = reader.ReadContentAsInt();
                    }
                    else if (reader.LocalName == "NumberMapEvents")
                    {
                        reader.Read();
                        NumberMapEvents = reader.ReadContentAsInt();
                    }
                }
            }
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("MapName");
            writer.WriteString(MapName);
            writer.WriteEndElement();

            writer.WriteStartElement("Width");
            writer.WriteString(Width.ToString());
            writer.WriteEndElement();

            writer.WriteStartElement("Height");
            writer.WriteString(Height.ToString());
            writer.WriteEndElement();

            writer.WriteStartElement("NumberMapEvents");
            writer.WriteString(NumberMapEvents.ToString());
            writer.WriteEndElement();
        }

    }
}

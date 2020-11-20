using Genus2D.GUI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Genus2D.GameData
{

    [Serializable]
    public class MapData : IXmlSerializable
    {

        public static readonly int NUM_LAYERS = 3;

        private string _mapName;
        private int _width;
        private int _height;
        private Tuple<int, int>[] _mapData;
        private List<MapEvent> _mapEvents;

        public XmlSchema GetSchema()
        {
            return (null);
        }

        public void ReadXml(XmlReader reader)
        {
            string xml = reader.ReadOuterXml();
            reader = XmlReader.Create(new StringReader(xml));

            XmlSerializer serializer = new XmlSerializer(typeof(List<MapEvent>));

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (reader.LocalName == "MapName")
                    {
                        reader.Read();
                        _mapName = reader.ReadContentAsString();
                    }
                    else if (reader.LocalName == "MapWidth")
                    {
                        reader.Read();
                        _width = reader.ReadContentAsInt();
                    }
                    else if (reader.LocalName == "MapHeight")
                    {
                        reader.Read();
                        _height = reader.ReadContentAsInt();
                    }
                    else if (reader.LocalName == "MapData")
                    {
                        _mapData = new Tuple<int, int>[_height * _width * NUM_LAYERS];
                    }
                    else if (reader.LocalName.StartsWith("MapTile"))
                    {
                        int index = int.Parse(reader.LocalName.Substring(7));

                        reader.Read();
                        reader.Read();
                        int value1 = reader.ReadContentAsInt();
                        reader.Read();
                        reader.Read();
                        int value2 = reader.ReadContentAsInt();

                        _mapData[index] = new Tuple<int, int>(value1, value2);
                    }
                    else if (reader.LocalName == "ArrayOfMapEvent")
                    {
                        xml = reader.ReadOuterXml();
                        Console.WriteLine(xml);
                        XmlReader reader2 = XmlReader.Create(new StringReader(xml));
                        _mapEvents = (List<MapEvent>)serializer.Deserialize(reader2);
                    }
                }
            }
        }

        public void WriteXml(XmlWriter writer)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<MapEvent>));

            writer.WriteStartElement("MapName");
            writer.WriteString(_mapName);
            writer.WriteEndElement();

            writer.WriteStartElement("MapWidth");
            writer.WriteString(_width.ToString());
            writer.WriteEndElement();

            writer.WriteStartElement("MapHeight");
            writer.WriteString(_height.ToString());
            writer.WriteEndElement();

            writer.WriteStartElement("MapData");

            for (int i = 0; i < _mapData.Length; i++)
            {

                writer.WriteStartElement("MapTile" + i);

                writer.WriteStartElement("Value1");
                writer.WriteString(_mapData[i].Item1.ToString());
                writer.WriteEndElement();

                writer.WriteStartElement("Value2");
                writer.WriteString(_mapData[i].Item2.ToString());
                writer.WriteEndElement();

                writer.WriteEndElement();

            }

            writer.WriteEndElement();

            serializer.Serialize(writer, _mapEvents);

        }

        public MapData()
        {
            _mapName = "";
            _width = 0;
            _height = 0;
        }

        public MapData(string mapName, int width, int height)
        {
            _mapName = mapName;
            _width = width;
            _height = height;
            _mapData = new Tuple<int, int>[height * width * NUM_LAYERS];
            for (int i = 0; i < _mapData.Length; i++)
            {
                _mapData[i] = new Tuple<int, int>(0, -1);
            }
            _mapEvents = new List<MapEvent>();
        }

        public void Resize(int width, int height)
        {
            if (width > 0 && height > 0 && (width != _width || height != _height))
            {
                int oldWidth = _width;
                int oldHeight = _height;
                _width = width;
                _height = height;

                Tuple<int, int>[] oldData = _mapData;
                _mapData = new Tuple<int, int>[height * width * NUM_LAYERS];
                for (int i = 0; i < _mapData.Length; i++)
                {
                    _mapData[i] = new Tuple<int, int>(0, -1);
                }

                for (int layer = 0; layer < NUM_LAYERS; layer++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        for (int y = 0; y < height; y++)
                        {
                            if (x >= oldWidth || y >= oldHeight)
                                continue;
                            Tuple<int, int> oldtile = oldData[x + oldWidth * (y + oldHeight * layer)];
                            SetTile(layer, x, y, oldtile.Item1, oldtile.Item2);
                        }
                    }
                }

                for (int i = 0; i < _mapEvents.Count; i++)
                {
                    if (_mapEvents[i].MapX >= width || _mapEvents[i].MapY >= height)
                    {
                        _mapEvents.RemoveAt(i);
                        i--;
                    }
                }

            }
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

                stream.Write(BitConverter.GetBytes(_mapData.Length), 0, sizeof(int));

                for (int i = 0; i < _mapData.Length; i++)
                {
                    stream.Write(BitConverter.GetBytes(_mapData[i].Item1), 0, sizeof(int));
                    stream.Write(BitConverter.GetBytes(_mapData[i].Item2), 0, sizeof(int));
                }

                stream.Write(BitConverter.GetBytes(_mapEvents.Count), 0, sizeof(int));
                for (int i = 0; i < _mapEvents.Count; i++)
                {
                    byte[] bytes = _mapEvents[i].GetBytes();
                    stream.Write(BitConverter.GetBytes(bytes.Length), 0, sizeof(int));
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
                int mapDataSize = BitConverter.ToInt32(tempBytes, 0);

                Tuple<int, int>[] mapData = new Tuple<int, int>[mapDataSize];

                for (int i = 0; i < mapDataSize; i++)
                {
                    stream.Read(tempBytes, 0, sizeof(int));
                    int tileID = BitConverter.ToInt32(tempBytes, 0);
                    stream.Read(tempBytes, 0, sizeof(int));
                    int tilesetID = BitConverter.ToInt32(tempBytes, 0);
                    Tuple<int, int> tuple = new Tuple<int, int>(tileID, tilesetID);
                    mapData[i] = tuple;
                }

                MapData data = new MapData(mapName, width, height);
                data._mapData = mapData;

                stream.Read(tempBytes, 0, sizeof(int));
                int mapEventsSize = BitConverter.ToInt32(tempBytes, 0);

                for (int i = 0; i < mapEventsSize; i++)
                {
                    tempBytes = new byte[sizeof(int)];
                    stream.Read(tempBytes, 0, sizeof(int));
                    int size = BitConverter.ToInt32(tempBytes, 0);

                    tempBytes = new byte[size];
                    stream.Read(tempBytes, 0, size);
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

        public void SetMapName(string name)
        {
            _mapName = name;
        }

        public int GetWidth()
        {
            return _width;
        }

        public int GetHeight()
        {
            return _height;
        }

        public Tuple<int, int>[] GetMapData()
        {
            return _mapData;
        }

        public Tuple<int, int> GetTile(int layer, int x, int y)
        {
            if (x > -1 && y > -1 && x < _width && y < _height)
                return _mapData[x + _width * (y + _height * layer)];
            return null;
        }

        public void SetTile(int layer, int x, int y, int id, int tileset)
        {
            if (x > -1 && y > -1 && x < _width && y < _height)
            {
                int index = x + _width * (y + _height * layer);
                _mapData[index] = new Tuple<int, int>(id, tileset);
            }
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

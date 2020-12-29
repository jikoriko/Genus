using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Genus2D.GameData
{

    [Serializable]
    public class CraftableData : IXmlSerializable
    {

        public string Name;

        public int CraftedItemID;
        public int CraftedItemCount;
        public int WorkbenchID;
        public List<Tuple<int, int>> Materials;

        public CraftableData()
        {
            Initialize("");
        }

        public CraftableData(string name)
        {
            Initialize(name);
        }

        private void Initialize(string name)
        {
            Name = name;
            CraftedItemID = -1;
            CraftedItemCount = 1;
            WorkbenchID = -1;

            Materials = new List<Tuple<int, int>>();
        }

        public XmlSchema GetSchema()
        {
            return (null);
        }

        public void ReadXml(XmlReader reader)
        {
            string xml = reader.ReadOuterXml();
            reader = XmlReader.Create(new StringReader(xml));

            Materials.Clear();

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (reader.LocalName == "Name")
                    {
                        reader.Read();
                        Name = reader.ReadContentAsString();
                    }
                    else if (reader.LocalName == "CraftedItemID")
                    {
                        reader.Read();
                        CraftedItemID = reader.ReadContentAsInt();
                    }
                    else if (reader.LocalName == "CraftedItemCount")
                    {
                        reader.Read();
                        CraftedItemCount = reader.ReadContentAsInt();
                    }
                    else if (reader.LocalName == "WorkbenchID")
                    {
                        reader.Read();
                        WorkbenchID = reader.ReadContentAsInt();
                    }
                    else if (reader.LocalName == "Material")
                    {
                        reader.Read();
                        reader.Read();
                        int item1 = reader.ReadContentAsInt();
                        reader.Read();
                        reader.Read();
                        int item2 = reader.ReadContentAsInt();

                        Materials.Add(new Tuple<int, int>(item1, item2));
                    }
                }
            }
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("Name");
            writer.WriteString(Name);
            writer.WriteEndElement();

            writer.WriteStartElement("CraftedItemID");
            writer.WriteString(CraftedItemID.ToString());
            writer.WriteEndElement();

            writer.WriteStartElement("CraftedItemCount");
            writer.WriteString(CraftedItemCount.ToString());
            writer.WriteEndElement();

            writer.WriteStartElement("WorkbenchID");
            writer.WriteString(WorkbenchID.ToString());
            writer.WriteEndElement();

            writer.WriteStartElement("Materials");

            for (int i = 0; i < Materials.Count; i++)
            {

                writer.WriteStartElement("Material");

                writer.WriteStartElement("ItemID");
                writer.WriteString(Materials[i].Item1.ToString());
                writer.WriteEndElement();

                writer.WriteStartElement("Count");
                writer.WriteString(Materials[i].Item2.ToString());
                writer.WriteEndElement();

                writer.WriteEndElement();

            }

            writer.WriteEndElement();
        }






        private static List<CraftableData> _craftablesData;
        private static List<string> _workBenchesData;

        public static void ReloadData()
        {
            _craftablesData = LoadCraftablesData();
            _workBenchesData = LoadWorkBenchesData();
        }

        private static List<CraftableData> LoadCraftablesData()
        {
            List<CraftableData> data = null;

            /*
            if (File.Exists("Data/CraftablesData.data"))
            {
                FileStream stream = File.Open("Data/CraftablesData.data", FileMode.Open, FileAccess.Read);
                BinaryFormatter formatter = new BinaryFormatter();
                data = (List<CraftableData>)formatter.Deserialize(stream);
                stream.Close();
            }
            //*/
            //*
            if (File.Exists("Data/CraftablesData.xml"))
            {
                FileStream stream = File.Open("Data/CraftablesData.xml", FileMode.Open, FileAccess.Read);
                XmlSerializer serializer = new XmlSerializer(typeof(List<CraftableData>));
                data = (List<CraftableData>)serializer.Deserialize(stream);
                stream.Close();
            }
            //*/
            else
            {
                data = new List<CraftableData>();
            }

            return data;
        }

        public static void SaveCraftablesData()
        {
            if (!Directory.Exists("Data"))
                Directory.CreateDirectory("Data");

            //FileStream stream = File.Create("Data/CraftablesData.data");
            //BinaryFormatter formatter = new BinaryFormatter();
            //formatter.Serialize(stream, _craftablesData);
            //stream.Close();

            FileStream stream = File.Create("Data/CraftablesData.xml");
            XmlSerializer serializer = new XmlSerializer(typeof(List<CraftableData>));
            serializer.Serialize(stream, _craftablesData);
            stream.Close();

        }

        private static List<string> LoadWorkBenchesData()
        {
            List<string> data = null;

            /*
            if (File.Exists("Data/WorkBenchesData.data"))
            {
                FileStream stream = File.Open("Data/WorkBenchesData.data", FileMode.Open, FileAccess.Read);
                BinaryFormatter formatter = new BinaryFormatter();
                data = (List<string>)formatter.Deserialize(stream);
                stream.Close();
            }
            //*/
            //*
            if (File.Exists("Data/WorkBenchesData.xml"))
            {
                FileStream stream = File.Open("Data/WorkBenchesData.xml", FileMode.Open, FileAccess.Read);
                XmlSerializer serializer = new XmlSerializer(typeof(List<string>));
                data = (List<string>)serializer.Deserialize(stream);
                stream.Close();
            }
            //*/
            else
            {
                data = new List<string>();
            }

            return data;
        }

        public static void SaveWorkbenchesData()
        {
            if (!Directory.Exists("Data"))
                Directory.CreateDirectory("Data");

            //FileStream stream = File.Create("Data/WorkBenchesData.data");
            //BinaryFormatter formatter = new BinaryFormatter();
            //formatter.Serialize(stream, _workBenchesData);
            //stream.Close();

            FileStream stream = File.Create("Data/WorkBenchesData.xml");
            XmlSerializer serializer = new XmlSerializer(typeof(List<string>));
            serializer.Serialize(stream, _workBenchesData);
            stream.Close();
        }

        public static void AddCraftableData(CraftableData data)
        {
            _craftablesData.Add(data);
        }

        public static void RemoveCraftableData(int index)
        {
            if (index >= 0 && index < _craftablesData.Count)
            {
                _craftablesData.RemoveAt(index);
            }
        }

        public static CraftableData GetCraftableData(int index)
        {
            if (index >= 0 && index < _craftablesData.Count)
            {
                return _craftablesData[index];
            }
            return null;
        }

        public static int GetCraftableDataCount()
        {
            return _craftablesData.Count;
        }

        public static List<string> GetCraftableNames()
        {
            List<string> names = new List<string>();

            for (int i = 0; i < _craftablesData.Count; i++)
            {
                names.Add(_craftablesData[i].Name);
            }

            return names;
        }

        public static void AddWorkbench(string name)
        {
            if (!_workBenchesData.Contains(name))
            {
                _workBenchesData.Add(name);
            }
        }

        public static void RemoveWorkbench(int index)
        {
            if (index > -1 && index < _workBenchesData.Count)
            {
                _workBenchesData.RemoveAt(index);
            }
        }

        public static string GetWorkbench(int index)
        {
            if (index > -1 && index < _workBenchesData.Count)
            {
                return _workBenchesData[index];
            }
            return null;
        }

        public static int GetWorkbenchDataCount()
        {
            return _workBenchesData.Count;
        }

    }
}

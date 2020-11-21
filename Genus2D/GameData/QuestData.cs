using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Genus2D.GameData
{
    [Serializable]
    public class QuestData
    {
        [Serializable]
        public class QuestObective : IXmlSerializable
        {
            public string Name;
            public string Description;
            public List<Tuple<int, int>> ItemRewards;

            public QuestObective()
            {
                Name = "";
                Description = "";
                ItemRewards = new List<Tuple<int, int>>();
            }

            public QuestObective(string name)
            {
                Name = name;
                Description = "";
                ItemRewards = new List<Tuple<int, int>>();
            }

            public List<string> GetItemRewardNames()
            {
                List<string> names = new List<string>();
                for (int i = 0; i < ItemRewards.Count; i++)
                {
                    int itemID = ItemRewards[i].Item1;
                    ItemData itemData = ItemData.GetItemData(itemID);
                    string itemName;
                    if (itemData == null) itemName = "None";
                    else itemName = itemData.Name;
                    names.Add(itemName + ", " + ItemRewards[i].Item2);
                }
                return names;
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
                        if (reader.LocalName == "Name")
                        {
                            reader.Read();
                            Name = reader.ReadContentAsString();
                        }
                        else if (reader.LocalName == "Description")
                        {
                            reader.Read();
                            Description = reader.ReadContentAsString();
                        }
                        else if (reader.LocalName == "Item")
                        {
                            reader.Read();
                            reader.Read();
                            int id = reader.ReadContentAsInt();
                            reader.Read();
                            reader.Read();
                            int count = reader.ReadContentAsInt();
                            ItemRewards.Add(new Tuple<int, int>(id, count));
                        }
                    }
                }
            }

            public void WriteXml(XmlWriter writer)
            {
                writer.WriteStartElement("Name");
                writer.WriteString(Name);
                writer.WriteEndElement();

                writer.WriteStartElement("Description");
                writer.WriteString(Description);
                writer.WriteEndElement();

                writer.WriteStartElement("ItemRewards");
                
                for (int i = 0; i < ItemRewards.Count; i++)
                {
                    writer.WriteStartElement("Item");

                    writer.WriteStartElement("ID");
                    writer.WriteString(ItemRewards[i].Item1.ToString());
                    writer.WriteEndElement();

                    writer.WriteStartElement("Count");
                    writer.WriteString(ItemRewards[i].Item2.ToString());
                    writer.WriteEndElement();

                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
            }
        }

        public string Name;
        public List<QuestObective> Objectives;

        public QuestData()
        {
            Name = "";
            Objectives = new List<QuestObective>();
        }

        public QuestData(string name)
        {
            Name = name;
            Objectives = new List<QuestObective>();
        }

        public List<string> GetObjectiveNames()
        {
            List<string> names = new List<string>();
            for (int i = 0; i < Objectives.Count; i++)
            {
                names.Add(Objectives[i].Name);
            }
            return names;
        }

        public void AddObjective(string name)
        {
            Objectives.Add(new QuestObective(name));
        }

        public void RemoveObjective(int index)
        {
            if (index >= 0 && index < Objectives.Count)
                Objectives.RemoveAt(index);
        }


        private static List<QuestData> _quests;
        private static List<QuestData> LoadData()
        {
            List<QuestData> data;
            //if (File.Exists("Data/QuestData.data"))
            //{
            //    FileStream stream = File.Open("Data/QuestData.data", FileMode.Open, FileAccess.Read);
            //    BinaryFormatter formatter = new BinaryFormatter();
            //    quests = (List<QuestData>)formatter.Deserialize(stream);
            //    stream.Close();
            //}
            if (File.Exists("Data/ItemData.xml"))
            {
                FileStream stream = File.Open("Data/QuestData.xml", FileMode.Open, FileAccess.Read);
                XmlSerializer serializer = new XmlSerializer(typeof(List<QuestData>));
                data = (List<QuestData>)serializer.Deserialize(stream);
                stream.Close();
            }
            else
            {
                data = new List<QuestData>();
            }
            return data;
        }

        public static void ReloadData()
        {
            _quests = LoadData();
        }

        public static void SaveData()
        {
            if (!Directory.Exists("Data"))
                Directory.CreateDirectory("Data");

            //FileStream stream = File.Create("Data/QuestData.data");
            //BinaryFormatter formatter = new BinaryFormatter();
            //formatter.Serialize(stream, _quests);
            //stream.Close();

            FileStream stream = File.Create("Data/QuestData.xml");
            XmlSerializer serializer = new XmlSerializer(typeof(List<QuestData>));
            serializer.Serialize(stream, _quests);
            stream.Close();
        }

        public static QuestData GetData(int index)
        {
            if (index >= 0 && index < _quests.Count)
                return _quests[index];
            return null;
        }

        public static void AddQuest(string name)
        {
            AddQuest(new QuestData(name));
        }

        public static void AddQuest(QuestData quest)
        {
            if (quest != null)
                _quests.Add(quest);
        }

        public static void RemoveQuest(int index)
        {
            if (index >= 0 && index < _quests.Count)
               _quests.RemoveAt(index);
        }

        public static int DataCount()
        {
            return _quests.Count;
        }

        public static List<string> GetQuestNames()
        {
            List<string> names = new List<string>();
            for (int i = 0; i < _quests.Count; i++)
            {
                names.Add(_quests[i].Name);
            }
            return names;
        }


    }
}

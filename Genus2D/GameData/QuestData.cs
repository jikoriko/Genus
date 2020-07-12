using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Genus2D.GameData
{
    [Serializable]
    public class QuestData
    {
        [Serializable]
        public class QuestObective
        {
            public string Name;
            public string Description;
            public List<Tuple<int, int>> ItemRewards;

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
        }

        public string Name;
        public List<QuestObective> Objectives;

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


        private static List<QuestData> _quests = LoadData();
        private static List<QuestData> LoadData()
        {
            List<QuestData> quests;
            if (File.Exists("Data/QuestData.data"))
            {
                FileStream stream = File.Open("Data/QuestData.data", FileMode.Open, FileAccess.Read);
                BinaryFormatter formatter = new BinaryFormatter();
                quests = (List<QuestData>)formatter.Deserialize(stream);
                stream.Close();
            }
            else
            {
                quests = new List<QuestData>();
            }
            return quests;
        }

        public static void ReloadData()
        {
            _quests = LoadData();
        }

        public static void SaveData()
        {
            if (!Directory.Exists("Data"))
                Directory.CreateDirectory("Data");
            FileStream stream = File.Create("Data/QuestData.data");
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, _quests);
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

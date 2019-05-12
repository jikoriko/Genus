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
    public class DropTableData
    {

        [Serializable]
        public class DropTableItem
        {
            public int ItemID = -1;
            public int ItemCount = 1;
            public int Chance = 1;

            public DropTableItem()
            {
                ItemID = -1;
            }

            public override string ToString()
            {
                string text = "";

                ItemData data = ItemData.GetItemData(ItemID);
                if (data != null)
                    text += "Item: " + data.Name;
                else
                    text += "Item: None";
                text += " {" + ItemCount;
                text += ", " + Chance + "}";

                return text;
            }
        }

        public string Name;
        public List<DropTableItem> TableItems;

        public DropTableData(string name)
        {
            Name = name;
            TableItems = new List<DropTableItem>();
        }





        private static List<DropTableData> _dropTables = LoadData();

        public static int DropTableCount()
        {
            return _dropTables.Count;
        }

        public static DropTableData GetDropTable(int index)
        {
            if (index >= 0 && index < _dropTables.Count)
                return _dropTables[index];
            return null;
        }

        public static void AddDropTable(string name)
        {
            _dropTables.Add(new DropTableData(name));
        }

        public static void AddDropTable(DropTableData dropTable)
        {
            if (dropTable != null)
            {
                _dropTables.Add(dropTable);
            }
        }

        public static void RemoveDropTable(int index)
        {
            if (index >= 0 && index < _dropTables.Count)
            {
                _dropTables.RemoveAt(index);
            }
        }

        public static List<string> GetDropTableNames()
        {
            List<string> names = new List<string>();
            for (int i = 0; i < _dropTables.Count; i++)
            {
                names.Add(_dropTables[i].Name);
            }
            return names;
        }

        public static void ReloadData()
        {
            _dropTables = LoadData();
        }

        private static List<DropTableData> LoadData()
        {
            List<DropTableData> dropTables;
            if (File.Exists("Data/DropTableData.data"))
            {
                FileStream stream = File.Open("Data/DropTableData.data", FileMode.Open, FileAccess.Read);
                BinaryFormatter formatter = new BinaryFormatter();
                dropTables = (List<DropTableData>)formatter.Deserialize(stream);
                stream.Close();
            }
            else
            {
                dropTables = new List<DropTableData>();
            }
            return dropTables;
        }

        public static void SaveData()
        {
            if (!Directory.Exists("Data"))
                Directory.CreateDirectory("Data");
            FileStream stream = File.Create("Data/DropTableData.data");
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, _dropTables);
            stream.Close();
        }

    }
}

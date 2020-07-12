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
    public class ShopData
    {

        [Serializable]
        public class ShopItem
        {
            public int ItemID;
            public int Cost;

            public ShopItem()
            {
                ItemID = -1;
                Cost = 1;
            }

            public override string ToString()
            {
                string name;
                ItemData data = ItemData.GetItemData(ItemID);
                if (data != null)
                    name = data.Name;
                else
                    name = "None";
                name += ", " + Cost;
                return name;
            }
        }

        public string Name;
        public List<ShopItem> ShopItems;

        public ShopData(string name)
        {
            Name = name;
            ShopItems = new List<ShopItem>();
        }

        public void AddItem()
        {
            ShopItems.Add(new ShopItem());
        }

        public void RemoveItem(int index)
        {
            if (index >= 0 && index < ShopItems.Count)
            {
                ShopItems.RemoveAt(index);
            }
        }

        public List<string> GetItemNames()
        {
            List<string> names = new List<string>();
            for (int i = 0; i < ShopItems.Count; i++)
            {
                names.Add(ShopItems[i].ToString());
            }
            return names;
        }


        private static List<ShopData> _shops = LoadData();

        public static int DataCount()
        {
            return _shops.Count;
        }

        public static ShopData GetData(int index)
        {
            if (index >= 0 && index < _shops.Count)
                return _shops[index];
            return null;
        }

        public static void AddShop(string name)
        {
            _shops.Add(new ShopData(name));
        }

        public static void AddShop(ShopData shop)
        {
            if (shop != null)
            {
                _shops.Add(shop);
            }
        }

        public static void RemoveShop(int index)
        {
            if (index >= 0 && index < _shops.Count)
            {
                _shops.RemoveAt(index);
            }
        }

        public static List<string> GetShopNames()
        {
            List<string> names = new List<string>();
            for (int i = 0; i < _shops.Count; i++)
            {
                names.Add(_shops[i].Name);
            }
            return names;
        }

        public static void ReloadData()
        {
            _shops = LoadData();
        }

        private static List<ShopData> LoadData()
        {
            List<ShopData> shops;
            if (File.Exists("Data/ShopData.data"))
            {
                FileStream stream = File.Open("Data/ShopData.data", FileMode.Open, FileAccess.Read);
                BinaryFormatter formatter = new BinaryFormatter();
                shops = (List<ShopData>)formatter.Deserialize(stream);
                stream.Close();
            }
            else
            {
                shops = new List<ShopData>();
            }
            return shops;
        }

        public static void SaveData()
        {
            if (!Directory.Exists("Data"))
                Directory.CreateDirectory("Data");
            FileStream stream = File.Create("Data/ShopData.data");
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, _shops);
            stream.Close();
        }
    }
}

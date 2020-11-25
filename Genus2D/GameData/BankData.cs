using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genus2D.GameData
{
    [Serializable]
    public class BankData
    {

        public int Gold { get; private set; }
        public List<Tuple<int, int>> Items { get; private set; }

        public BankData()
        {
            Gold = 0;
            Items = new List<Tuple<int, int>>();
        }

        public void AddGold(int amount)
        {
            if (amount <= 0) return;
            Gold += amount;
        }

        public void RemoveGold(int amount, PlayerData playerData)
        {
            if (amount <= 0 || playerData == null) return;
            if (amount > Gold) amount = Gold;
            Gold -= amount;
            playerData.Gold += amount;
        }

        public void AddBankItem(int itemID, int count)
        {
            if (count <= 0 || itemID < 0) return;

            bool added = false;
            for (int i = 0; i < Items.Count; i++)
            {
                if (Items[i].Item1 == itemID)
                {
                    Items[i] = new Tuple<int, int>(itemID, Items[i].Item2 + count);
                    added = true;
                    break;
                }
            }

            if (added == false)
            {
                Items.Add(new Tuple<int, int>(itemID, count));
            }

        }

        public void RemoveBankItem(int index, int count, PlayerData playerData)
        {
            if (count <= 0 || playerData == null) return;

            if (index >= 0 && index < Items.Count)
            {
                if (count > Items[index].Item2)
                    count = Items[index].Item2;

                int added = playerData.AddInventoryItem(Items[index].Item1, count);
                if (added < Items[index].Item2)
                {
                    int remainder = Items[index].Item2 - added;
                    Items[index] = new Tuple<int, int>(Items[index].Item1, remainder);
                }   
                else
                {
                    Items.RemoveAt(index);
                }

            }
        }

        public string GetBankString()
        {
            string text = "";

            for (int i = 0; i < Items.Count; i++)
            {
                text += Items[i].Item1 + "," + Items[i].Item2;
                if (i < Items.Count - 1)
                    text += ",";
            }

            return text;
        }

        public void ParseBankString(string text)
        {
            Items.Clear();
            string[] parts = text.Split(',');
            int numItems = parts.Length / 2;
            for (int i = 0; i < numItems; i++)
            {
                int id = int.Parse(parts[i * 2]);
                int count = int.Parse(parts[(i * 2) + 1]);
                Items.Add(new Tuple<int, int>(id, count));
            }
        }

        public byte[] GetBytes()
        {
            using (MemoryStream stream = new MemoryStream())
            {

                stream.Write(BitConverter.GetBytes(Gold), 0, sizeof(int));
                stream.Write(BitConverter.GetBytes(Items.Count), 0, sizeof(int));
                for (int i = 0; i < Items.Count; i++)
                {
                    stream.Write(BitConverter.GetBytes(Items[i].Item1), 0, sizeof(int));
                    stream.Write(BitConverter.GetBytes(Items[i].Item2), 0, sizeof(int));
                }

                return stream.ToArray();
            }
        }

        public static BankData FromBytes(byte[] bytes)
        {
            using (MemoryStream stream = new MemoryStream(bytes))
            {
                BankData data = new BankData();

                byte[] tempBytes = new byte[sizeof(int)];

                stream.Read(tempBytes, 0, sizeof(int));
                data.Gold = BitConverter.ToInt32(tempBytes, 0);

                stream.Read(tempBytes, 0, sizeof(int));
                int numItems = BitConverter.ToInt32(tempBytes, 0);

                for (int i = 0; i < numItems; i++)
                {
                    stream.Read(tempBytes, 0, sizeof(int));
                    int itemID = BitConverter.ToInt32(tempBytes, 0);

                    stream.Read(tempBytes, 0, sizeof(int));
                    int itemCount = BitConverter.ToInt32(tempBytes, 0);

                    data.Items.Add(new Tuple<int, int>(itemID, itemCount));
                }

                return data;
            }
        }
    }
}

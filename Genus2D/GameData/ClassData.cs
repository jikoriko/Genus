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
    public class ClassData
    {

        public string Name;
        public CombatStats BaseStats;

        public ClassData(string name)
        {
            Name = name;
            BaseStats = new CombatStats();
        }

        private static List<ClassData> _classData = LoadData();
        private static List<ClassData> LoadData()
        {
            List<ClassData> data;
            if (File.Exists("Data/ClassData.data"))
            {
                FileStream stream = File.Open("Data/ClassData.data", FileMode.Open, FileAccess.Read);
                BinaryFormatter formatter = new BinaryFormatter();
                data = (List<ClassData>)formatter.Deserialize(stream);
                stream.Close();
            }
            else
            {
                data = new List<ClassData>();
            }
            return data;
        }

        public static void ReloadData()
        {
            _classData = LoadData();
        }

        public static void SaveData()
        {
            if (!Directory.Exists("Data"))
                Directory.CreateDirectory("Data");
            FileStream stream = File.Create("Data/ClassData.data");
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, _classData);
            stream.Close();
        }

        public static ClassData GetClass(int index)
        {
            if (index >= 0 && index < _classData.Count)
                return _classData[index];
            return null;
        }

        public static void AddClass(string name)
        {
            _classData.Add(new ClassData(name));
        }

        public static void AddClass(ClassData data)
        {
            if (data != null)
            {
                _classData.Add(data);
            }
        }

        public static void RemoveClass(int index)
        {
            if (index >= 0 && index < _classData.Count)
            {
                _classData.RemoveAt(index);
            }
        }

        public static List<string> GetClassNames()
        {
            List<string> names = new List<string>();
            for (int i = 0; i < _classData.Count; i++)
            {
                names.Add(_classData[i].Name);
            }
            return names;
        }

        public static int ClassesCount()
        {
            return _classData.Count;
        }
    }
}

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
    public class EnemyData
    {
        public string Name;
        public CombatStats BaseStats;
        public int VisionRage;
        public int AttackRange;
        public int ProjectileID;

        public EnemyData(string name)
        {
            Name = name;
            BaseStats = new CombatStats();
            VisionRage = 1;
            AttackRange = 1;
            ProjectileID = -1;
        }

        private static List<EnemyData> _enemyData = LoadData();
        private static List<EnemyData> LoadData()
        {
            List<EnemyData> data;
            if (File.Exists("Data/EnemyData.data"))
            {
                FileStream stream = File.Open("Data/EnemyData.data", FileMode.Open, FileAccess.Read);
                BinaryFormatter formatter = new BinaryFormatter();
                data = (List<EnemyData>)formatter.Deserialize(stream);
                stream.Close();
            }
            else
            {
                data = new List<EnemyData>();
            }
            return data;
        }

        public static void ReloadData()
        {
            _enemyData = LoadData();
        }

        public static void SaveData()
        {
            if (!Directory.Exists("Data"))
                Directory.CreateDirectory("Data");
            FileStream stream = File.Create("Data/EnemyData.data");
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, _enemyData);
            stream.Close();
        }

        public static EnemyData GetEnemy(int index)
        {
            if (index >= 0 && index < _enemyData.Count)
                return _enemyData[index];
            return null;
        }

        public static void AddEnemy(string name)
        {
            _enemyData.Add(new EnemyData(name));
        }

        public static void AddEnemy(EnemyData data)
        {
            if (data != null)
            {
                _enemyData.Add(data);
            }
        }

        public static void RemoveEnemy(int index)
        {
            if (index >= 0 && index < _enemyData.Count)
            {
                _enemyData.RemoveAt(index);
            }
        }

        public static List<string> GetEnemyNames()
        {
            List<string> names = new List<string>();
            for (int i = 0; i < _enemyData.Count; i++)
            {
                names.Add(_enemyData[i].Name);
            }
            return names;
        }

        public static int EmemiesCount()
        {
            return _enemyData.Count;
        }


    }
}

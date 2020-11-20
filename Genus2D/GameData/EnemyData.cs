using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Genus2D.GameData
{
    [Serializable]
    public class EnemyData
    {
        public string Name;
        public CombatStats BaseStats;
        public int VisionRage;
        public int AttackRange;
        public int WanderRange;
        public int Experience;
        public int AgroLvl;
        public AttackStyle AtkStyle;
        public int ProjectileID;
        public int SpriteID;
        public MovementSpeed Speed;
        public int DropTable;

        public EnemyData()
        {
            Initialize("");
        }

        public EnemyData(string name)
        {
            Initialize(name);
        }

        private void Initialize(string name)
        {
            Name = name;
            BaseStats = new CombatStats();
            AtkStyle = AttackStyle.Melee;
            VisionRage = 1;
            AttackRange = 1;
            WanderRange = 5;
            Experience = 0;
            AgroLvl = 1;
            ProjectileID = -1;
            SpriteID = -1;
            Speed = MovementSpeed.Normal;
            DropTable = -1;
        }

        private static List<EnemyData> _enemyData = LoadData();
        private static List<EnemyData> LoadData()
        {
            List<EnemyData> data;
            //if (File.Exists("Data/EnemyData.data"))
            //{
            //    FileStream stream = File.Open("Data/EnemyData.data", FileMode.Open, FileAccess.Read);
            //    BinaryFormatter formatter = new BinaryFormatter();
            //    data = (List<EnemyData>)formatter.Deserialize(stream);
            //    stream.Close();
            //}
            if (File.Exists("Data/EnemyData.xml"))
            {
                FileStream stream = File.Open("Data/EnemyData.xml", FileMode.Open, FileAccess.Read);
                XmlSerializer serializer = new XmlSerializer(typeof(List<EnemyData>));
                data = (List<EnemyData>)serializer.Deserialize(stream);
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

            //FileStream stream = File.Create("Data/EnemyData.data");
            //BinaryFormatter formatter = new BinaryFormatter();
            //formatter.Serialize(stream, _enemyData);
            //stream.Close();

            FileStream stream = File.Create("Data/EnemyData.xml");
            XmlSerializer serializer = new XmlSerializer(typeof(List<EnemyData>));
            serializer.Serialize(stream, _enemyData);
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

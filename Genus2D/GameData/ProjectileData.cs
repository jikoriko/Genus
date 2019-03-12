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
    public class ProjectileData
    {

        public string Name;
        public string IconSheetImage;
        public int IconID;
        public float Speed;
        public float Lifespan;
        public int AnchorX;
        public int AnchorY;
        public int BoundsWidth;
        public int BoundsHeight;

        public ProjectileData(string name)
        {
            Name = name;
            IconSheetImage = "";
            IconID = 0;
            Speed = 0.1f;
            Lifespan = 0.1f;
            AnchorX = 0;
            AnchorY = 0;
            BoundsWidth = 2;
            BoundsHeight = 2;
        }


        private static List<ProjectileData> _projectileData = LoadData();
        private static List<ProjectileData> LoadData()
        {
            List<ProjectileData> data = null;

            if (File.Exists("Data/ProjectileData.data"))
            {
                FileStream stream = File.Open("Data/ProjectileData.data", FileMode.Open, FileAccess.Read);
                BinaryFormatter formatter = new BinaryFormatter();
                data = (List<ProjectileData>)formatter.Deserialize(stream);
                stream.Close();
            }
            else
            {
                data = new List<ProjectileData>();
            }

            return data;
        }

        public static void ReloadData()
        {
            _projectileData = LoadData();
        }

        public static void SaveItemData()
        {
            if (!Directory.Exists("Data"))
                Directory.CreateDirectory("Data");
            FileStream stream = File.Create("Data/ProjectileData.data");
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, _projectileData);
            stream.Close();
        }

        public static void AddProjectileData(ProjectileData data)
        {
            _projectileData.Add(data);
        }

        public static void RemoveProjectileData(int index)
        {
            if (index >= 0 && index < _projectileData.Count)
            {
                _projectileData.RemoveAt(index);
            }
        }

        public static ProjectileData GetProjectileData(int index)
        {
            if (index >= 0 && index < _projectileData.Count)
            {
                return _projectileData[index];
            }
            return null;
        }

        public static int GetProjectileDataCount()
        {
            return _projectileData.Count;
        }

        public static List<string> GetProjectileNames()
        {
            List<string> names = new List<string>();

            for (int i = 0; i < _projectileData.Count; i++)
            {
                names.Add(_projectileData[i].Name);
            }

            return names;
        }

    }
}

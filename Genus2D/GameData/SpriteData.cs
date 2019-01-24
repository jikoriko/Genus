using OpenTK;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Genus2D.GameData
{
    [Serializable]
    public class SpriteData
    {

        public string Name;
        public string ImagePath;
        public Vector2 VerticalAnchorPoint;
        public Vector2 VerticalBounds;
        public Vector2 HorizontalAnchorPoint;
        public Vector2 HorizontalBounds;

        public SpriteData(string name)
        {
            Name = name;
            ImagePath = "";
            VerticalAnchorPoint = new Vector2();
            VerticalBounds = new Vector2(2, 2);
            HorizontalAnchorPoint = new Vector2();
            HorizontalBounds = new Vector2(2, 2);
        }


        private static List<SpriteData> _spriteData = LoadData();

        private static List<SpriteData> LoadData()
        {
            List<SpriteData> sprites;
            if (File.Exists("Data/SpriteData.data"))
            {
                FileStream stream = File.Open("Data/SpriteData.data", FileMode.Open, FileAccess.Read);
                BinaryFormatter formatter = new BinaryFormatter();
                sprites = (List<SpriteData>)formatter.Deserialize(stream);
                stream.Close();
            }
            else
            {
                sprites = new List<SpriteData>();
            }
            return sprites;
        }

        public static void SaveData()
        {
            if (!Directory.Exists("Data"))
                Directory.CreateDirectory("Data");
            FileStream stream = File.Create("Data/SpriteData.data");
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, _spriteData);
            stream.Close();
        }

        public static void AddSpriteData(string name)
        {
            _spriteData.Add(new SpriteData(name));
            SaveData();
        }

        public static void RemoveSprite(int index)
        {
            if (index >= 0 && index < _spriteData.Count)
            {
                _spriteData.RemoveAt(index);
                SaveData();
            }
        }

        public static SpriteData GetSpriteData(int index)
        {
            if (index > -1 && index < _spriteData.Count)
                return _spriteData[index];
            return null;
        }

        public static List<string> GetSpriteNames()
        {
            List<string> names = new List<string>();
            for (int i = 0; i < _spriteData.Count; i++)
            {
                names.Add(_spriteData[i].Name);
            }
            return names;
        }

        public static int NumSprites()
        {
            return _spriteData.Count;
        }

    }
}

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
    public class SpriteData
    {

        public string TextureName;
        public Vector2 AnchorPoint;
        public Rectangle Collider;

        public SpriteData()
        {
            TextureName = "";
            AnchorPoint = new Vector2();
            Collider = new Rectangle();
        }


        private static List<SpriteData> _spriteData = LoadData();

        private static List<SpriteData> LoadData()
        {
            List<SpriteData> sprites;
            if (File.Exists("Data/TilesetData.data"))
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

        public static SpriteData GetSpriteData(int index)
        {
            if (index > -1 && index < _spriteData.Count)
                return _spriteData[index];
            return null;
        }

        public static int NumSprites()
        {
            return _spriteData.Count;
        }
    }
}

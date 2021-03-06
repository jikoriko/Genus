﻿using OpenTK;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

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

        public SpriteData()
        {
            Name = "";
            ImagePath = "";
            VerticalAnchorPoint = new Vector2();
            VerticalBounds = new Vector2(2, 2);
            HorizontalAnchorPoint = new Vector2();
            HorizontalBounds = new Vector2(2, 2);
        }
        
        public SpriteData(string name)
        {
            Name = name;
            ImagePath = "";
            VerticalAnchorPoint = new Vector2();
            VerticalBounds = new Vector2(2, 2);
            HorizontalAnchorPoint = new Vector2();
            HorizontalBounds = new Vector2(2, 2);
        }


        private static List<SpriteData> _spriteData;

        private static List<SpriteData> LoadData()
        {
            List<SpriteData> data;
            //if (File.Exists("Data/SpriteData.data"))
            //{
            //    FileStream stream = File.Open("Data/SpriteData.data", FileMode.Open, FileAccess.Read);
            //    BinaryFormatter formatter = new BinaryFormatter();
            //    sprites = (List<SpriteData>)formatter.Deserialize(stream);
            //    stream.Close();
            //}
            if (File.Exists("Data/SpriteData.xml"))
            {
                FileStream stream = File.Open("Data/SpriteData.xml", FileMode.Open, FileAccess.Read);
                XmlSerializer serializer = new XmlSerializer(typeof(List<SpriteData>));
                data = (List<SpriteData>)serializer.Deserialize(stream);
                stream.Close();
            }
            else
            {
                data = new List<SpriteData>();
            }
            return data;
        }

        public static void ReloadData()
        {
            _spriteData = LoadData();
        }

        public static void SaveData()
        {
            if (!Directory.Exists("Data"))
                Directory.CreateDirectory("Data");

            // stream = File.Create("Data/SpriteData.data");
            //BinaryFormatter formatter = new BinaryFormatter();
            //formatter.Serialize(stream, _spriteData);
            //stream.Close();

            FileStream stream = File.Create("Data/SpriteData.xml");
            XmlSerializer serializer = new XmlSerializer(typeof(List<SpriteData>));
            serializer.Serialize(stream, _spriteData);
            stream.Close();
        }

        public static void AddSpriteData(string name)
        {
            _spriteData.Add(new SpriteData(name));
        }

        public static void RemoveSprite(int index)
        {
            if (index >= 0 && index < _spriteData.Count)
            {
                _spriteData.RemoveAt(index);
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

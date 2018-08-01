using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;

using Genus2D.Graphics;
using Genus2D.Utililities;

namespace Genus2D.GameData
{
    public class TilesetData
    {

        [Serializable]
        public class Tileset
        {
            public string Name;
            public string TexturePath { get; private set; }
            public bool[,] Pasabilities { get; private set; }
            public int[,] Priorities { get; private set; }

            public Tileset(string name)
            {
                Name = name;
                TexturePath = "";
                Pasabilities = null;
                Priorities = null;
            }

            public void SetTexturePath(string filepath)
            {
                if (TexturePath != filepath)
                {
                    Texture texture = Assets.GetTexture(filepath);
                    if (texture != null)
                    {
                        TexturePath = filepath;
                        Pasabilities = new bool[texture.GetWidth() / 32, texture.GetHeight() / 32];
                        Priorities = new int[texture.GetWidth() / 32, texture.GetHeight() / 32];
                    }
                }
            }

            public bool GetPassable(int tileID)
            {
                if (Pasabilities != null)
                    return GetPassable(tileID % Pasabilities.GetLength(0), tileID / Pasabilities.GetLength(0));
                return false;
            }

            public bool GetPassable(int x, int y)
            {
                if (Pasabilities != null)
                {
                    if (x >= 0 && x < Pasabilities.GetLength(0) && y >= 0 && y < Pasabilities.GetLength(1))
                    {
                        return Pasabilities[x, y];
                    }
                }

                return false;
            }

            public void SetPassable(int x, int y, bool passable)
            {
                if (Pasabilities != null)
                {
                    if (x >= 0 && x < Pasabilities.GetLength(0) && y >= 0 && y < Pasabilities.GetLength(1))
                    {
                        Pasabilities[x, y] = passable;
                    }
                }
            }

            public int GetTilePriority(int tileID)
            {
                if (Priorities != null)
                    return GetTilePriority(tileID % Priorities.GetLength(0), tileID / Priorities.GetLength(0));
                return -1;
            }

            public int GetTilePriority(int x, int y)
            {
                if (Priorities != null)
                {
                    if (x >= 0 && x < Priorities.GetLength(0) && y >= 0 && y < Priorities.GetLength(1))
                    {
                        return Priorities[x, y];
                    }
                }

                return -1;
            }

            public void SetPriority(int x, int y, int priority)
            {
                if (Priorities != null)
                {
                    if (x >= 0 && x < Priorities.GetLength(0) && y >= 0 && y < Priorities.GetLength(1))
                    {
                        Priorities[x, y] = priority;
                    }
                }
            }

            public bool TextureLoaded()
            {
                return TexturePath != "";
            }
        }

        private static List<Tileset> _tilesets = LoadData();

        public static int TilesetCount()
        {
            return _tilesets.Count;
        }

        public static Tileset GetTileset(int index)
        {
            if (index >= 0 && index < _tilesets.Count)
                return _tilesets[index];
            return null;
        }

        public static void AddTileset(string name)
        {
            _tilesets.Add(new Tileset(name));
            SaveData();
        }

        public static void AddTileset(Tileset tileset)
        {
            if (tileset != null)
            {
                _tilesets.Add(tileset);
                SaveData();
            }
        }

        public static void RemoveTileset(int index)
        {
            if (index >= 0 && index < _tilesets.Count)
            {
                _tilesets.RemoveAt(index);
                SaveData();
            }
        }

        public static List<string> GetTilesetNames()
        {
            List<string> names = new List<string>();
            for (int i = 0; i < _tilesets.Count; i++)
            {
                names.Add(_tilesets[i].Name);
            }
            return names;
        }

        public static List<Tileset> LoadData()
        {
            List<Tileset> tilesets;
            if (File.Exists("Data/TilesetData.data"))
            {
                FileStream stream = File.Open("Data/TilesetData.data", FileMode.Open, FileAccess.Read);
                BinaryFormatter formatter = new BinaryFormatter();
                tilesets = (List<Tileset>)formatter.Deserialize(stream);
                stream.Close();
            }
            else
            {
                tilesets = new List<Tileset>();
            }
            return tilesets;
        }

        public static void SaveData()
        {
            if (!Directory.Exists("Data"))
                Directory.CreateDirectory("Data");
            FileStream stream = File.Create("Data/TilesetData.data");
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, _tilesets);
            stream.Close();
        }
    }
}

using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;

using Genus2D.Graphics;
using Genus2D.Utililities;
using System.Drawing;

namespace Genus2D.GameData
{
    public class TilesetData
    {

        [Serializable]
        public class Tileset
        {
            public string Name;

            public string ImagePath { get; private set; }
            private bool[,,] Pasabilities;
            private int[,] Priorities;
            private int[,] TerrainTags;
            private bool[,] BushFlags;
            private bool[,] CounterFlags;

            private string[] AutoTiles;
            private float[] AutoTileTimers;

            public static int[,] AutoTileIndex = {
                { 27, 28, 33, 34 }, { 5, 28, 33, 34 }, { 27, 6, 33, 34 }, { 5, 6, 33, 34 },
                { 27, 28, 33, 12 }, { 5, 28, 33, 12 }, { 27, 6, 33, 12 }, { 5, 6, 33, 12 },
                { 27, 28, 11, 34 }, { 5, 28, 11, 34 }, { 27, 6, 11, 34 }, { 5, 6, 11, 34 },
                { 27, 28, 11, 12 }, { 5, 28, 11, 12 }, { 27, 6, 11, 12 }, { 5, 6, 11, 12 },

                { 25, 26, 31, 32 }, { 25, 6, 31, 32 }, { 25, 26, 31, 12 }, { 25, 6, 31, 12 },
                { 15, 16, 21, 22 }, { 15, 16, 21, 12 }, { 15, 16, 11, 22 }, { 15, 16, 11, 12 },
                { 29, 30, 35, 36 }, { 29, 30, 11, 36 }, { 5, 30, 35, 36 }, { 5, 30, 11, 36 },
                { 39, 40, 45, 46 }, { 5, 40, 45, 46 }, { 39, 6, 45, 46 }, { 5, 6, 45, 46 },

                { 25, 30, 31, 36 }, { 15, 16, 45, 46 }, { 13, 14, 19, 20 }, { 13, 14, 19, 12 },
                { 17, 18, 23, 24 }, { 17, 18, 11, 24 }, { 41, 42, 47, 48 }, { 5, 42, 47, 48 },
                { 37, 38, 43, 44 }, { 37, 6, 43, 44 }, { 13, 18, 19, 24 }, { 13, 14, 43, 44 },
                { 37, 42, 43, 48 }, { 17, 18, 47, 48 }, { 13, 18, 43, 48 }, { 1, 2, 7, 8 }
            };

            public static Rectangle[] GetAutoTileSources(int hashcode)
            {
                Rectangle[] sources = new Rectangle[4];

                int tileID = AutoHashToID(hashcode);

                for (int i = 0; i < 4; i++)
                {
                    int miniTile = AutoTileIndex[tileID, i] - 1;
                    sources[i] = new Rectangle(miniTile % 6 * 16, miniTile / 6 * 16, 16, 16);
                }

                return sources;
            }

            private static int AutoHashToID(int hash)
            {
                switch (hash)
                {
                    #region hash code mapping
                    case 1:
                    case 3:
                    case 9:
                    case 11:
                    case 33:
                    case 35:
                    case 41:
                    case 43:
                    case 129:
                    case 131:
                    case 137:
                    case 139:
                    case 161:
                    case 163:
                    case 169:
                    case 171:
                        return 44;

                    case 4:
                    case 6:
                    case 12:
                    case 14:
                    case 36:
                    case 38:
                    case 44:
                    case 46:
                    case 132:
                    case 134:
                    case 140:
                    case 142:
                    case 164:
                    case 166:
                    case 172:
                    case 174:
                        return 43;

                    case 5:
                    case 13:
                    case 37:
                    case 45:
                    case 133:
                    case 141:
                    case 165:
                    case 173:
                        return 41;

                    case 7:
                    case 15:
                    case 39:
                    case 47:
                    case 135:
                    case 143:
                    case 167:
                    case 175:
                        return 40;

                    case 16:
                    case 18:
                    case 24:
                    case 26:
                    case 48:
                    case 50:
                    case 56:
                    case 58:
                    case 144:
                    case 146:
                    case 152:
                    case 154:
                    case 176:
                    case 178:
                    case 184:
                    case 186:
                        return 42;

                    case 17:
                    case 19:
                    case 25:
                    case 27:
                    case 49:
                    case 51:
                    case 57:
                    case 59:
                    case 145:
                    case 147:
                    case 153:
                    case 155:
                    case 177:
                    case 179:
                    case 185:
                    case 187:
                        return 32;

                    case 20:
                    case 22:
                    case 52:
                    case 54:
                    case 148:
                    case 150:
                    case 180:
                    case 182:
                        return 35;

                    case 21:
                    case 53:
                    case 149:
                    case 181:
                        return 19;

                    case 23:
                    case 55:
                    case 151:
                    case 183:
                        return 18;

                    case 28:
                    case 30:
                    case 60:
                    case 62:
                    case 156:
                    case 158:
                    case 188:
                    case 190:
                        return 34;

                    case 29:
                    case 61:
                    case 157:
                    case 189:
                        return 17;

                    case 31:
                    case 63:
                    case 159:
                    case 191:
                        return 16;

                    case 64:
                    case 66:
                    case 72:
                    case 74:
                    case 96:
                    case 98:
                    case 104:
                    case 106:
                    case 192:
                    case 194:
                    case 200:
                    case 202:
                    case 224:
                    case 226:
                    case 232:
                    case 234:
                        return 45;

                    case 65:
                    case 67:
                    case 73:
                    case 75:
                    case 97:
                    case 99:
                    case 105:
                    case 107:
                        return 39;

                    case 68:
                    case 70:
                    case 76:
                    case 78:
                    case 100:
                    case 102:
                    case 108:
                    case 110:
                    case 196:
                    case 198:
                    case 204:
                    case 206:
                    case 228:
                    case 230:
                    case 236:
                    case 238:
                        return 33;

                    case 69:
                    case 77:
                    case 101:
                    case 109:
                        return 31;

                    case 71:
                    case 79:
                    case 103:
                    case 111:
                        return 29;

                    case 80:
                    case 82:
                    case 88:
                    case 90:
                    case 208:
                    case 210:
                    case 216:
                    case 218:
                        return 37;

                    case 81:
                    case 83:
                    case 89:
                    case 91:
                        return 27;

                    case 92:
                    case 94:
                    case 220:
                    case 222:
                        return 22;

                    case 84:
                    case 86:
                    case 212:
                    case 214:
                        return 23;

                    case 112:
                    case 114:
                    case 120:
                    case 122:
                    case 240:
                    case 242:
                    case 248:
                    case 250:
                        return 36;

                    case 113:
                    case 115:
                    case 121:
                    case 123:
                        return 26;

                    case 116:
                    case 118:
                    case 244:
                    case 246:
                        return 21;

                    case 124:
                    case 126:
                    case 252:
                    case 254:
                        return 20;

                    case 193:
                    case 195:
                    case 201:
                    case 203:
                    case 225:
                    case 227:
                    case 233:
                    case 235:
                        return 38;

                    case 197:
                    case 205:
                    case 229:
                    case 237:
                        return 30;

                    case 199:
                    case 207:
                    case 231:
                    case 239:
                        return 28;

                    case 209:
                    case 211:
                    case 217:
                    case 219:
                        return 25;

                    case 241:
                    case 243:
                    case 249:
                    case 251:
                        return 24;



                    case 253:
                        return 2;
                    case 247:
                        return 4;
                    case 245:
                        return 6;
                    case 223:
                        return 8;
                    case 221:
                        return 10;
                    case 215:
                        return 12;
                    case 213:
                        return 14;
                    case 127:
                        return 1;
                    case 125:
                        return 3;
                    case 119:
                        return 5;
                    case 117:
                        return 7;
                    case 95:
                        return 9;
                    case 93:
                        return 11;
                    case 87:
                        return 13;
                    case 85:
                        return 15;
                    case 255:
                        return 0;
                    case 0:
                        return 46;


                    default:
                        return 46;
                        #endregion
                }
            }



            public Tileset(string name)
            {
                Name = name;
                ImagePath = "";
                Pasabilities = new bool[8, 1, 8];
                SetPassable(0, 0, true);
                Priorities = new int[8, 1];
                TerrainTags = new int[8, 1];
                BushFlags = new bool[8, 1];
                CounterFlags = new bool[8, 1];

                AutoTiles = new string[7];
                AutoTileTimers = new float[7];

                for (int i = 0; i < 7; i++)
                    AutoTiles[i] = "";
            }



            public void SetImagePath(string filepath)
            {
                if (ImagePath != filepath)
                {
                    try
                    {
                        Bitmap image = new Bitmap("Assets/Textures/Tilesets/" + filepath);
                        ImagePath = filepath;

                        bool[,,] oldPasabilites = Pasabilities;
                        int[,] oldPriorities = Priorities;
                        int[,] oldTerrainTags = TerrainTags;
                        bool[,] oldBushFlags = BushFlags;
                        bool[,] oldCounterFlags = CounterFlags;

                        Pasabilities = new bool[8, (image.Height / 32) + 1, 8];
                        Priorities = new int[8, (image.Height / 32) + 1];
                        TerrainTags = new int[8, (image.Height / 32) + 1];
                        BushFlags = new bool[8, (image.Height / 32) + 1];
                        CounterFlags = new bool[8, (image.Height / 32) + 1];

                        for (int x = 0; x < Pasabilities.GetLength(0); x++)
                        {
                            for (int y = 0; y < Pasabilities.GetLength(1); y++)
                            {
                                if (y < oldPasabilites.GetLength(1))
                                {
                                    for (int dir = 0; dir < 8; dir++)
                                    {
                                        Pasabilities[x, y, dir] = oldPasabilites[x, y, dir];
                                    }
                                    Priorities[x, y] = oldPriorities[x, y];
                                    TerrainTags[x, y] = oldTerrainTags[x, y];
                                    BushFlags[x, y] = oldBushFlags[x, y];
                                    CounterFlags[x, y] = oldCounterFlags[x, y];
                                }
                            }
                        }

                        image.Dispose();
                    }
                    catch { }
                }
            }

            public bool GetPassable(int tileID)
            {
                bool passable = false;
                for (int i = 0; i < 8; i++)
                {
                    if (GetPassable(tileID, (MovementDirection)i))
                    {
                        passable = true;
                        break;
                    }
                }
                return passable;
            }

            public bool GetPassable(int tileID, MovementDirection direction)
            {
                return GetPassable(tileID % Pasabilities.GetLength(0), tileID / Pasabilities.GetLength(0), direction);
            }

            public bool GetPassable(int x, int y)
            {
                bool passable = false;
                for (int i = 0; i < 8; i++)
                {
                    if (GetPassable(x, y, (MovementDirection)i))
                    {
                        passable = true;
                        break;
                    }
                }
                return passable;
            }

            public bool GetPassable(int x, int y, MovementDirection direction)
            {
                if (x >= 0 && x < Pasabilities.GetLength(0) && y >= 0 && y < Pasabilities.GetLength(1))
                {
                    return Pasabilities[x, y, (int)direction];
                }

                return false;
            }

            public void SetPassable(int x, int y, bool passable)
            {
                for (int dir = 0; dir < 8; dir++)
                {
                    SetPassable(x, y, (MovementDirection)dir, passable);
                }
            }

            public void SetPassable(int x, int y, MovementDirection direction, bool passable)
            {
                if (x >= 0 && x < Pasabilities.GetLength(0) && y >= 0 && y < Pasabilities.GetLength(1))
                {
                    Pasabilities[x, y, (int)direction] = passable;
                }
            }

            public int GetTilePriority(int tileID)
            {
                return GetTilePriority(tileID % Priorities.GetLength(0), tileID / Priorities.GetLength(0));
            }

            public int GetTilePriority(int x, int y)
            {
                if (x >= 0 && x < Priorities.GetLength(0) && y >= 0 && y < Priorities.GetLength(1))
                {
                    return Priorities[x, y];
                }

                return -1;
            }

            public void SetPriority(int x, int y, int priority)
            {
                if (x >= 0 && x < Priorities.GetLength(0) && y >= 0 && y < Priorities.GetLength(1))
                {
                    Priorities[x, y] = priority;
                }
            }

            public int GetTerrainTag(int tileID)
            {
                return GetTerrainTag(tileID % TerrainTags.GetLength(0), tileID / TerrainTags.GetLength(0));
            }

            public int GetTerrainTag(int x, int y)
            {
                if (x >= 0 && x < TerrainTags.GetLength(0) && y >= 0 && y < TerrainTags.GetLength(1))
                {
                    return TerrainTags[x, y];
                }
                return -1;
            }

            public void SetTerrainTag(int x, int y, int tag)
            {
                if (x >= 0 && x < TerrainTags.GetLength(0) && y >= 0 && y < TerrainTags.GetLength(1))
                {
                    TerrainTags[x, y] = tag;
                }
            }

            public bool GetBushFlag(int tileID)
            {
                return GetBushFlag(tileID % BushFlags.GetLength(0), tileID / BushFlags.GetLength(0));
            }

            public bool GetBushFlag(int x, int y)
            {
                if (x >= 0 && x < BushFlags.GetLength(0) && y >= 0 && y < BushFlags.GetLength(1))
                {
                    return BushFlags[x, y];
                }
                return false;
            }

            public void SetBushFlag(int x, int y, bool flag)
            {
                if (x >= 0 && x < BushFlags.GetLength(0) && y >= 0 && y < BushFlags.GetLength(1))
                {
                    BushFlags[x, y] = flag;
                }
            }

            public bool GetCounterFlag(int tileID)
            {
                return GetCounterFlag(tileID % CounterFlags.GetLength(0), tileID / CounterFlags.GetLength(0));
            }

            public bool GetCounterFlag(int x, int y)
            {
                if (x >= 0 && x < CounterFlags.GetLength(0) && y >= 0 && y < CounterFlags.GetLength(1))
                {
                    return CounterFlags[x, y];
                }
                return false;
            }

            public void SetCounterFlag(int x, int y, bool flag)
            {
                if (x >= 0 && x < CounterFlags.GetLength(0) && y >= 0 && y < CounterFlags.GetLength(1))
                {
                    CounterFlags[x, y] = flag;
                }
            }

            public string GetAutoTile(int index)
            {
                if (index >= 0 && index < 7)
                    return AutoTiles[index];
                return "";
            }

            public void SetAutoTile(int index, string autoTile)
            {
                if (index >= 0 && index < 7)
                    AutoTiles[index] = autoTile;
            }

            public float GetAutoTileTimer(int index)
            {
                if (index >= 0 && index < 7)
                    return AutoTileTimers[index];
                return 0f;
            }

            public void SetAutoTileTimer(int index, float time)
            {
                if (index >= 0 && index < 7)
                    AutoTileTimers[index] = time;
            }

            public bool TextureLoaded()
            {
                return ImagePath != "";
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
        }

        public static void AddTileset(Tileset tileset)
        {
            if (tileset != null)
            {
                _tilesets.Add(tileset);
            }
        }

        public static void RemoveTileset(int index)
        {
            if (index >= 0 && index < _tilesets.Count)
            {
                _tilesets.RemoveAt(index);
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

        public static void ReloadData()
        {
            _tilesets = LoadData();
        }

        private static List<Tileset> LoadData()
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

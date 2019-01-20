using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genus2D.GameData
{
    [Serializable]
    public class SpawnPoint
    {
        public int MapID;
        public int MapX, MapY;
        public string Label;

        public SpawnPoint(int mapID, int mapX, int mapY, string label)
        {
            MapID = mapID;
            MapX = mapX;
            MapY = mapY;
            Label = label;
        }
    }
}

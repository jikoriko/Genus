using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genus2D.GameData
{
    public class Preloader
    {

        public static void PreLoadData()
        {
            ClassData.ReloadData();
            DropTableData.ReloadData();
            EnemyData.ReloadData();
            EventData.ReloadData();
            ItemData.ReloadData();
            MapInfo.ReloadData();
            ProjectileData.ReloadData();
            CraftableData.ReloadData();
            ParticleEmitterData.ReloadData();
            QuestData.ReloadData();
            ShopData.ReloadData();
            SpriteData.ReloadData();
            SystemData.ReloadData();
            SystemVariable.ReloadData();
            TilesetData.ReloadData();
        }
    }
}

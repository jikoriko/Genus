using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genus2D.GameData
{
    [Serializable]
    public class CombatStats
    {
        public int Vitality;
        public int Inteligence;
        public int Strength;
        public int Agility;
        public int MeleeDefence;
        public int RangeDefence;
        public int MagicDefence; //break into elements: fire, water, earth, air?

        public CombatStats()
        {
            Vitality = 0;
            Inteligence = 0;
            Strength = 0;
            Agility = 0;
            MeleeDefence = 0;
            RangeDefence = 0;
            MagicDefence = 0;
        }

        public CombatStats(CombatStats other)
        {
            Vitality = other.Vitality;
            Inteligence = other.Inteligence;
            Strength = other.Strength;
            Agility = other.Agility;
            MeleeDefence = other.MeleeDefence;
            RangeDefence = other.RangeDefence;
            MagicDefence = other.MagicDefence;
        }
    }
}

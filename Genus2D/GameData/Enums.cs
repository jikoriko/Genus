using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Genus2D.GameData
{
    
    [Serializable]
    public enum Direction
    {
        Down,
        Left,
        Right,
        Up
    }

    [Serializable]
    public enum EventTriggerType
    {
        None,
        Action,
        PlayerTouch,
        EventTouch,
        Autorun
    }

    [Serializable]
    public enum EquipmentSlot
    {
        Weapon,
        Shield,
        Head,
        Body,
        Legs,
        Feet,
        Necklace,
        Bracelet,
        Ring
    }

    [Serializable]
    public enum ToolType
    {
        Axe,
        Pickaxe,
        Shovel
    }

    [Serializable]
    public enum ItemStatType
    {
        Integer,
        ToolType,
        EquipmentSlot
    }



}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Genus2D.GameData
{
    [Serializable]
    public enum CharacterType
    {
        Player,
        Enemy
    }

    [Serializable]
    public enum FacingDirection
    {
        Down,
        Left,
        Right,
        Up
    }

    [Serializable]
    public enum MovementDirection
    {
        Down,
        Left,
        Right,
        Up,
        UpperLeft,
        UpperRight,
        LowerLeft,
        LowerRight
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
    public enum ChangeMapEventProperty
    {
        Teleport,
        Move,
        Direction,
        Sprite,
        RenderPriority,
        MovementSpeed,
        MovementFrequency,
        Passable,
        RandomMovement,
        Enabled
    }

    [Serializable]
    public enum RenderPriority
    {
        BelowPlayer,
        AbovePlayer,
        OnTop
    }

    [Serializable]
    public enum MovementSpeed
    {
        ExtraFast,
        Fast,
        Normal,
        Slow,
        ExtraSlow
    }

    [Serializable]
    public enum MovementFrequency
    {
        Instant,
        High,
        Normal,
        Low,
        VeryLow
    }

    [Serializable]
    public enum VariableType
    {
        Integer,
        Float,
        Bool,
        Text
    }

    [Serializable]
    public enum ConditionalBranchType
    {
        PlayerPosition,
        MapEventPosition,
        ItemEquipped,
        ItemInInventory,
        SystemVariable,
        QuestStatus,
        SelectedOption,
        TerrainTag,
        PlayerDirection,
        PlayerGold
    }

    [Serializable]
    public enum ConditionValueCheck
    {
        Equal,
        NotEqual,
        Greater,
        GreaterOrEqual,
        Lower,
        LowerOrEqual
    }

    [Serializable]
    public enum ConditionalTextCheck
    {
        Equal,
        NotEqual,
        Includes
    }

    [Serializable]
    public enum QuestStatusCheck
    {
        Started,
        Complete,
        Progression
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
    public enum AttackStyle
    {
        None,
        Melee,
        Ranged,
        Magic
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
        EquipmentSlot,
        AttackStyle
    }



}

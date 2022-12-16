namespace ClaraMundi
{
    public enum EffectType
    {
        Physical,
        Magical
    }
    public enum StatType
    {
        Strength,
        Dexterity,
        Vitality,
        Agility,
        Intelligence,
        Mind,
        Charisma
    }
    public enum AttributeType
    {
        PhysicalAttack,
        PhysicalAccuracy,
        PhysicalDefense,
        PhysicalEvasion,

        MagicalAttack,
        MagicalAccuracy,
        MagicalDefense,
        MagicalEvasion,

        // Speed of physical/magical skill cast and recast times
        PhysicalSpeed,
        MagicalSpeed,
        Healing,
    }
    public enum ElementType
    {
        Neutral,
        Light,
        Dark,
        Earth,
        Water,
        Wind,
        Fire,
        Ice,
        Lightning
    }
}
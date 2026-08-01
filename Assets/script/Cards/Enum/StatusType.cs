using System;

[Serializable]
public class StatusEffect
{
    public StatusType Type;

    public int Value;
}

public enum StatusType
{
    None,
    Strength,
    Weak,
    Vulnerable,
    Stun,
    Counter,
    Immortal,
    Bleed,
    Regen,
    Lifesteal,
    Poison,
}
using System;

[Serializable]
public class StatusEffect
{
    public StatusType Type;

    public int Value;
}

public enum StatusType
{
    Strength,
    Weak,
    Vulnerable,
}
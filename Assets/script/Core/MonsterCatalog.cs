using System.Collections.Generic;

public enum MonsterCategory
{
    Normal,
    MiniBoss,
    Boss
}

public class MonsterCatalogEntry
{
    public EnemyData data;
    public MonsterCategory category;
    public readonly List<int> maps = new List<int>();
}

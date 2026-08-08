using System.Collections.Generic;
using UnityEngine;

public class RelicManager : MonoBehaviour
{
    private static RelicManager _instance;

    public static RelicManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = new GameObject("RelicManager");
                _instance = go.AddComponent<RelicManager>();
                DontDestroyOnLoad(go);
            }

            return _instance;
        }
    }

    public static event System.Action<RelicData> RelicAdded;

    public static bool Exists => _instance != null;

    public static bool Owns(string relicName)
    {
        return _instance != null && _instance.HasRelic(relicName);
    }

    public static void EmitBattleStart()
    {
        Instance.OnBattleStart();
    }

    public static void EmitBattleEnd()
    {
        Instance.OnBattleEnd();
    }

    public static void EmitPlayerTurnStart()
    {
        Instance.OnPlayerTurnStart();
    }

    public static void EmitPlayerTurnEnd()
    {
        Instance.OnPlayerTurnEnd();
    }

    public static void EmitRestSite()
    {
        Instance.OnRestSite();
    }

    public static void EmitObtainCard(CardData card)
    {
        Instance.OnObtainCard(card);
    }

    [Header("All Relics")]
    public List<RelicData> allRelics = new();

    [Header("Owned Relics")]
    [SerializeField] private List<RelicData> ownedRelics = new();

    // Trạng thái nội bộ của relic
    private int giryaUses;
    private bool teaSetPending;
    private int happyFlowerCounter;
    private bool redSkullActive;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);

        if (allRelics == null || allRelics.Count == 0)
        {
            allRelics = new List<RelicData>(Resources.LoadAll<RelicData>("Relics"));
            Debug.Log($"[RelicManager] Loaded {allRelics.Count} relics from Resources/Relics");
        }

        if (GetComponent<RelicBarUI>() == null)
            gameObject.AddComponent<RelicBarUI>();
    }

    #region Relic

    public void AddRelic(RelicData relic)
    {
        if (relic == null)
            return;

        if (!relic.stackable && HasRelic(relic.relicName))
            return;

        ownedRelics.Add(relic);

        ApplyInstantEffect(relic);

        RelicAdded?.Invoke(relic);

        Debug.Log($"[Relic] Obtained: {relic.relicName}");
    }

    public void RemoveRelic(RelicData relic)
    {
        if (ownedRelics.Contains(relic))
            ownedRelics.Remove(relic);
    }

    public void ClearRelics()
    {
        ownedRelics.Clear();

        giryaUses = 0;
        teaSetPending = false;
        happyFlowerCounter = 0;
        redSkullActive = false;

        if (RelicBarUI.Instance != null)
            RelicBarUI.Instance.Refresh();
    }

    public bool HasRelic(string relicName)
    {
        foreach (RelicData relic in ownedRelics)
        {
            if (relic.relicName == relicName)
                return true;
        }

        return false;
    }

    public List<RelicData> GetOwnedRelics()
    {
        return ownedRelics;
    }

    // ==========================
    // Random Relic for Chest
    // ==========================
    public RelicData GetRandomChestRelic()
    {
        List<RelicData> availableRelics = new List<RelicData>();

        foreach (RelicData relic in allRelics)
        {
            // Boss relics do not appear in Chest
            if (relic.rarity == RelicRarity.Boss)
                continue;

            // Skip relics the player already owns
            if (HasRelic(relic.relicName))
                continue;

            availableRelics.Add(relic);
        }

        if (availableRelics.Count == 0)
        {
            Debug.LogWarning("No relic left to receive.");
            return null;
        }

        int randomIndex = Random.Range(0, availableRelics.Count);
        return availableRelics[randomIndex];
    }

    private RelicData GetRelic(string relicName)
    {
        foreach (RelicData relic in ownedRelics)
        {
            if (relic.relicName == relicName)
                return relic;
        }

        return null;
    }

    #endregion

    #region Instant Effect

    // Hiệu ứng tức thời khi nhận relic:
    // - MaxHealth (Mango, Strawberry, Ancient Core): +Max HP ngay lập tức
    private void ApplyInstantEffect(RelicData relic)
    {
        switch (relic.relicType)
        {
            case RelicType.MaxHealth:
                if (relic.value > 0)
                {
                    RunSession.PlayerMaxHealth += relic.value;
                    RunSession.PlayerCurrentHealth += relic.value;
                    Debug.Log($"[Relic] {relic.relicName}: Max HP +{relic.value}");
                }
                break;
        }
    }

    #endregion

    #region Battle

    public void OnBattleStart()
    {
        happyFlowerCounter = 0;
        redSkullActive = false;

        PlayerStatus status = FindAnyObjectByType<PlayerStatus>();
        PlayerHealth hp = FindAnyObjectByType<PlayerHealth>();
        PlayerBlock block = FindAnyObjectByType<PlayerBlock>();

        // Dragon Soul, Vajra, Ancient Core, Girya: Strength vĩnh viễn mỗi trận
        int strength = GetPermanentStrength();
        if (strength > 0 && status != null)
        {
            status.AddStatus(StatusType.Strength, strength, 99);
            Debug.Log($"[Relic] Battle start: +{strength} Strength");
        }

        // Anchor: +Block đầu trận
        RelicData anchor = GetRelic("Anchor");
        if (anchor != null && block != null)
        {
            block.AddBlock(anchor.value);
            Debug.Log($"[Relic] Anchor: +{anchor.value} Block");
        }

        // Blood Vial: hồi máu đầu trận
        RelicData bloodVial = GetRelic("Blood Vial");
        if (bloodVial != null && hp != null)
        {
            hp.Heal(bloodVial.value);
            Debug.Log($"[Relic] Blood Vial: Heal +{bloodVial.value}");
        }

        // Bag of Mables: áp dụng Vulnerable cho từng enemy khi spawn (BattleManager gọi ApplyBagOfMables)

        // Red Skull: kiểm tra HP < 50%
        UpdateRedSkull(status, hp);
    }

    public void ApplyBagOfMables(EnemyStatus enemyStatus)
    {
        if (enemyStatus == null)
            return;

        RelicData mables = GetRelic("Bag of Mables");
        if (mables == null)
            return;

        enemyStatus.AddStatus(StatusType.Vulnerable, mables.value, 99);
    }

    // Energy đầu trận (Coffee Dripper, Ectoplasm, Tea Set).
    // Gọi sau khi ResetEnergy để bonus không bị xóa.
    public static int GetBattleStartEnergyBonus()
    {
        if (_instance == null)
            return 0;

        return _instance.ComputeBattleStartEnergyBonus();
    }

    private int ComputeBattleStartEnergyBonus()
    {
        int bonus = 0;

        RelicData coffee = GetRelic("Coffee Dripper");
        if (coffee != null)
            bonus += coffee.value;

        RelicData ecto = GetRelic("Ectoplasm");
        if (ecto != null)
            bonus += ecto.value;

        if (teaSetPending)
        {
            RelicData tea = GetRelic("Tea Set");
            if (tea != null)
                bonus += tea.value;

            teaSetPending = false;
        }

        if (bonus > 0)
            Debug.Log($"[Relic] Battle start: +{bonus} Energy bonus");

        return bonus;
    }

    public void OnBattleEnd()
    {
        // Burning Blood: hồi máu sau mỗi trận thắng
        RelicData burning = GetRelic("Burning Blood");
        if (burning != null)
        {
            PlayerHealth hp = FindAnyObjectByType<PlayerHealth>();
            hp?.Heal(burning.value);
            Debug.Log($"[Relic] Burning Blood: Heal +{burning.value}");
        }
    }

    private int GetPermanentStrength()
    {
        int strength = 0;

        foreach (RelicData relic in ownedRelics)
        {
            switch (relic.relicName)
            {
                case "Dragon Soul":
                case "Vajra":
                    strength += relic.value;
                    break;

                case "Ancient Core":
                    strength += relic.secondValue;
                    break;

                case "Girya":
                    strength += giryaUses * relic.value;
                    break;
            }
        }

        return strength;
    }

    private void UpdateRedSkull(PlayerStatus status, PlayerHealth hp)
    {
        RelicData skull = GetRelic("Red Skull");
        if (skull == null || status == null || hp == null)
            return;

        bool below = hp.CurrentHealth < hp.MaxHealth * skull.secondValue / 100f;

        if (below && !redSkullActive)
        {
            status.AddStatus(StatusType.Strength, skull.value, 99);
            redSkullActive = true;
            Debug.Log($"[Relic] Red Skull: HP < {skull.secondValue}% -> +{skull.value} Strength");
        }
        else if (!below && redSkullActive)
        {
            status.AddStatus(StatusType.Strength, -skull.value);
            redSkullActive = false;
            Debug.Log($"[Relic] Red Skull: HP restored -> -{skull.value} Strength");
        }
    }

    #endregion

    #region Turn

    public void OnPlayerTurnStart()
    {
        PlayerStatus status = FindAnyObjectByType<PlayerStatus>();
        PlayerHealth hp = FindAnyObjectByType<PlayerHealth>();

        // Happy Flower: cứ mỗi secondValue lượt người chơi +value năng lượng
        RelicData flower = GetRelic("Happy Flower");
        if (flower != null)
        {
            happyFlowerCounter++;
            Debug.Log($"[Relic] Happy Flower - turn counter {happyFlowerCounter}/{flower.secondValue}");

            if (happyFlowerCounter >= flower.secondValue)
            {
                happyFlowerCounter = 0;

                EnergyManager energy = FindAnyObjectByType<EnergyManager>();
                energy?.GainEnergy(flower.value);
                Debug.Log($"[Relic] Happy Flower: +{flower.value} Energy");
            }
        }

        // Red Skull: HP dưới 50% thì +3 Strength
        UpdateRedSkull(status, hp);
    }

    public void OnPlayerTurnEnd()
    {
        // Orichalcum: kết thúc lượt không có Block thì +value Block
        RelicData orichalcum = GetRelic("Orichalcum");
        if (orichalcum != null)
        {
            PlayerBlock block = FindAnyObjectByType<PlayerBlock>();

            if (block != null && block.CurrentBlock == 0)
            {
                block.AddBlock(orichalcum.value);
                Debug.Log($"[Relic] Orichalcum: +{orichalcum.value} Block");
            }
        }
    }

    public bool ShouldRetainEnergy()
    {
        return HasRelic("Ice Cream");
    }

    #endregion

    #region Events

    public int OnGainGold(int amount)
    {
        if (amount <= 0)
            return 0;

        // Ectoplasm: cannot gain Gold
        if (HasRelic("Ectoplasm"))
        {
            Debug.Log("[Relic] Ectoplasm: gold gain blocked");
            return 0;
        }

        RunSession.Gold += amount;
        return amount;
    }

    public void OnObtainCard(CardData card)
    {
        // Ceramic Fish: nhận thẻ là +value vàng
        RelicData fish = GetRelic("Ceramic Fish");
        if (fish != null)
        {
            RunSession.Gold += fish.value;
            Debug.Log($"[Relic] Ceramic Fish: +{fish.value} Gold");
        }
    }

    public void OnRestSite()
    {
        // Girya: mỗi lần nghỉ +1 Strength, tối đa secondValue lần
        RelicData girya = GetRelic("Girya");
        if (girya != null && giryaUses < girya.secondValue)
        {
            giryaUses++;
            Debug.Log($"[Relic] Girya: rest {giryaUses}/{girya.secondValue} (+{girya.value} Strength per rest)");
        }

        // Tea Set: +1 năng lượng ở battle kế tiếp
        if (GetRelic("Tea Set") != null)
        {
            teaSetPending = true;
            Debug.Log("[Relic] Tea Set: next battle +1 Energy pending");
        }
    }

    #endregion

    #region Save

    public void SaveRelics()
    {
        // TODO
    }

    public void LoadRelics()
    {
        // TODO
    }

    #endregion
}

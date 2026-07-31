using UnityEngine;

public class CardEffectResolver : MonoBehaviour
{
    [SerializeField] private PlayerCombat playerCombat;
    [SerializeField] private PlayerBlock playerBlock;
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private PlayerStatus playerStatus;
    [SerializeField] private HandManager handManager;
    [SerializeField] private DeckManager deckManager;
    [SerializeField] private EnergyManager energyManager;

    private HandManager GetHand() => handManager ??= FindAnyObjectByType<HandManager>();
    private DeckManager GetDeck() => deckManager ??= FindAnyObjectByType<DeckManager>();
    private EnergyManager GetEnergy() => energyManager ??= FindAnyObjectByType<EnergyManager>();
    private PlayerStatus GetStatus() => playerStatus ??= FindAnyObjectByType<PlayerStatus>();
    private PlayerHealth GetHealth() => playerHealth ??= FindAnyObjectByType<PlayerHealth>();
    private PlayerCombat GetCombat() => playerCombat ??= FindAnyObjectByType<PlayerCombat>();
    private PlayerBlock GetBlock() => playerBlock ??= FindAnyObjectByType<PlayerBlock>();

    public void Resolve(CardData card, EnemyHealth target)
    {
        switch (card.cardName.Trim())
        {
            case "Strike":          Strike(target); break;
            case "Defend":          Defend(card); break;
            case "Bash":            Bash(target, card); break;
            case "HeavyBlade":      HeavyBlade(target, card); break;
            case "Combo":           Combo(target, card); break;
            case "Chain Hit":       ChainHit(target, card); break;
            case "Counter Stance":  CounterStance(target, card); break;
            case "Last Stand":      LastStand(target, card); break;
            case "Sacrifice":       Sacrifice(target, card); break;
            case "Bloodthirst":     Bloodthirst(target, card); break;
            case "Executioner":     Executioner(target); break;
            case "Blade Storm":     BladeStorm(target, card); break;
            case "Guardian":        Guardian(card); break;
            case "Blood Barrier":   BloodBarrier(card); break;
            case "Steel Skin":      SteelSkin(target, card); break;
            case "Undying Will":    UndyingWill(card); break;
            case "Second Wind":     SecondWind(card); break;
            case "Blood Feast":     BloodFeast(card); break;
            case "Rejuvenating Aura": RejuvenatingAura(card); break;
            case "Enrage":          Enrage(); break;
            case "Shockwave":       Shockwave(target); break;
            case "Shatter Armor":   ShatterArmor(target); break;
            case "Intimidate":      Intimidate(target); break;
            case "Hemorrhage":      Hemorrhage(target); break;
            case "Refresh":         Refresh(); break;
            case "Risky Gambit":    RiskyGambit(); break;
        }
    }

    private void Strike(EnemyHealth target)
    {
        GetCombat().Attack(target, 6);
    }

    private void Defend(CardData card)
    {
        GetBlock().AddBlock(card.block);
    }

    private void Bash(EnemyHealth target, CardData card)
    {
        AttackAllEnemies(card.damage);
    }

    private void AttackAllEnemies(int damage)
    {
        foreach (EnemyHealth e in FindObjectsByType<EnemyHealth>(FindObjectsSortMode.None))
            GetCombat().Attack(e, damage);
    }

    private void HeavyBlade(EnemyHealth target, CardData card)
    {
        GetCombat().Attack(target, card.damage);
    }

    private void Combo(EnemyHealth target, CardData card)
    {
        GetCombat().Attack(target, card.damage);
        GetCombat().Attack(target, card.damage);
    }

    private void ChainHit(EnemyHealth target, CardData card)
    {
        GetCombat().Attack(target, card.damage);
        HandManager h = GetHand();
        if (h == null) return;
        foreach (CardDisplay c in h.GetCardsInHand())
            if (c.CardData != null)
                c.CardData.energyCost = Mathf.Max(0, c.CardData.energyCost - 1);
    }

    private void CounterStance(EnemyHealth target, CardData card)
    {
        GetCombat().Attack(target, card.damage);
        GetStatus()?.AddStatus(StatusType.Counter, 2);
    }

    private void LastStand(EnemyHealth target, CardData card)
    {
        int dmg = card.damage;
        PlayerHealth hp = GetHealth();
        if (hp != null && hp.CurrentHealth < hp.MaxHealth * 0.5f)
            dmg = 20;
        GetCombat().Attack(target, dmg);
    }

    private void Sacrifice(EnemyHealth target, CardData card)
    {
        GetCombat().Attack(target, 20);
        GetHealth()?.TakeDamage(10, false);
    }

    private void Bloodthirst(EnemyHealth target, CardData card)
    {
        int prev = target.CurrentHealth;
        GetCombat().Attack(target, card.damage);
        int dealt = prev - target.CurrentHealth;
        if (dealt > 0)
            GetHealth()?.Heal(Mathf.RoundToInt(dealt * 0.5f));
    }

    private void Executioner(EnemyHealth target)
    {
        if (target == null || target.MaxHealth <= 0) return;
        if (target.IsBoss) return;
        if (target.CurrentHealth <= target.MaxHealth * 0.2f)
            target.TakeDamage(9999);
    }

    private void BladeStorm(EnemyHealth target, CardData card)
    {
        int kills = 0;
        foreach (EnemyHealth e in FindObjectsByType<EnemyHealth>(FindObjectsSortMode.None))
        {
            int prev = e.CurrentHealth;
            GetCombat().Attack(e, card.damage);
            if (prev > 0 && e.CurrentHealth <= 0)
                kills++;
        }
        if (kills >= 2)
        {
            CardDatabase db = FindAnyObjectByType<CardDatabase>();
            CardData c = db?.GetRandomCard();
            if (c != null)
            {
                GetDeck()?.AddCardToDeck(c);
                HandManager h = GetHand();
                if (h != null && !h.IsFull)
                    h.AddCard(c);
            }
        }
    }

    private void Guardian(CardData card)
    {
        GetBlock().AddBlock(card.block);
    }

    private void BloodBarrier(CardData card)
    {
        int bonus = 0;
        PlayerHealth hp = GetHealth();
        if (hp != null)
            bonus = Mathf.RoundToInt((hp.MaxHealth - hp.CurrentHealth) * 0.1f);
        GetBlock().AddBlock(card.block + bonus);
    }

    private void SteelSkin(EnemyHealth target, CardData card)
    {
        GetBlock().AddBlock(card.block);
        target?.GetComponent<EnemyStatus>()?.AddStatus(StatusType.Weak, 3, 1);
    }

    private void UndyingWill(CardData card)
    {
        GetBlock().AddBlock(card.block);
        GetStatus()?.AddStatus(StatusType.Immortal, 1);
    }

    private void SecondWind(CardData card)
    {
        int healAmt = 10;
        PlayerHealth hp = GetHealth();
        if (hp != null && hp.CurrentHealth < hp.MaxHealth * 0.3f)
            healAmt = Mathf.RoundToInt(healAmt * 1.3f);
        hp?.Heal(healAmt);
    }

    private void BloodFeast(CardData card)
    {
        GetHealth()?.Heal(8);
        GetStatus()?.AddStatus(StatusType.Lifesteal, 1);
    }

    private void RejuvenatingAura(CardData card)
    {
        GetHealth()?.Heal(30);
        GetStatus()?.AddStatus(StatusType.Regen, 8);
    }

    private void Enrage()
    {
        GetStatus()?.AddStatus(StatusType.Strength, 3, 99);
    }

    private void Shockwave(EnemyHealth target)
    {
        target?.GetComponent<EnemyStatus>()?.AddStatus(StatusType.Weak, 3, 1);
    }

    private void ShatterArmor(EnemyHealth target)
    {
        target?.GetComponent<EnemyStatus>()?.AddStatus(StatusType.Vulnerable, 2, 1);
    }

    private void Intimidate(EnemyHealth target)
    {
        if (target == null) return;
        if (target.IsBoss)
            target.GetComponent<EnemyStatus>()?.AddStatus(StatusType.Weak, 5, 1);
        else
            target.GetComponent<EnemyStatus>()?.AddStatus(StatusType.Stun, 1);
    }

    private void Hemorrhage(EnemyHealth target)
    {
        target?.GetComponent<EnemyStatus>()?.AddStatus(StatusType.Bleed, 6);
    }

    private void Refresh()
    {
        HandManager h = GetHand();
        DeckManager d = GetDeck();
        if (h == null || d == null) return;
        foreach (CardDisplay c in h.GetCardsInHand())
        {
            d.AddToDiscard(c.CardData);
            h.RemoveCard(c);
            break;
        }
        CardData drawn = d.DrawCard();
        if (drawn != null && !h.IsFull)
            h.AddCard(drawn);
    }

    private void RiskyGambit()
    {
        GetHealth()?.TakeDamage(5, false);
        HandManager h = GetHand();
        DeckManager d = GetDeck();
        if (h == null || d == null) return;
        for (int i = 0; i < 2; i++)
        {
            CardData drawn = d.DrawCard();
            if (drawn != null && !h.IsFull)
                h.AddCard(drawn);
        }
    }
}

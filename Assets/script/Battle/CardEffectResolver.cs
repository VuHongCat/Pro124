using System.Collections.Generic;
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
            case "Strike":          Strike(target, card); break;
            case "Defend":          Defend(card); break;
            case "Bash":            Bash(target, card); break;
            case "HeavyBlade":      HeavyBlade(target, card); break;
            case "Combo":           Combo(target, card); break;
            case "Chain Hit":       ChainHit(target, card); break;
            case "Counter Stance":  CounterStance(target, card); break;
            case "Last Stand":      LastStand(target, card); break;
            case "Sacrifice":       Sacrifice(target, card); break;
            case "Bloodthirst":     Bloodthirst(target, card); break;
            case "Executioner":     Executioner(target, card); break;
            case "Blade Storm":     BladeStorm(target, card); break;
            case "Guardian":        Guardian(card); break;
            case "Blood Barrier":   BloodBarrier(card); break;
            case "Steel Skin":      SteelSkin(target, card); break;
            case "Undying Will":    UndyingWill(card); break;
            case "Second Wind":     SecondWind(card); break;
            case "Blood Feast":     BloodFeast(card); break;
            case "Rejuvenating Aura": RejuvenatingAura(card); break;
            case "Enrage":          Enrage(card); break;
            case "Shockwave":       Shockwave(target, card); break;
            case "Shatter Armor":   ShatterArmor(target, card); break;
            case "Intimidate":      Intimidate(target, card); break;
            case "Hemorrhage":      Hemorrhage(target, card); break;
            case "Refresh":         Refresh(card); break;
            case "Risky Gambit":    RiskyGambit(card); break;
            case "Double Edge":     DoubleEdge(target, card); break;
            case "Whirlwind":       Whirlwind(target, card); break;
            case "Puncture":        Puncture(target, card); break;
            case "Shield Bash":     ShieldBash(target, card); break;
            case "Vampiric Strike": VampiricStrike(target, card); break;
            case "Crushing Blow":   CrushingBlow(target, card); break;
            case "Assassinate":     Assassinate(target, card); break;
            case "Poison Dagger":   PoisonDagger(target, card); break;
            case "Blood Boil":      BloodBoil(target, card); break;
            case "Fury":            Fury(target, card); break;
            case "Iron Wall":       IronWall(card); break;
            case "Brace":           Brace(card); break;
            case "Reposition":      Reposition(card); break;
            case "Mirror Shield":   MirrorShield(card); break;
            case "Fortify":         Fortify(card); break;
            case "Stoneskin":       Stoneskin(card); break;
            case "Aegis":           Aegis(card); break;
            case "Bandage":         Bandage(card); break;
            case "Leech":           Leech(target, card); break;
            case "Life Spring":     LifeSpring(card); break;
            case "Greater Heal":    GreaterHeal(card); break;
            case "Absorb":          Absorb(card); break;
            case "Vampiric Aura":   VampiricAura(card); break;
            case "Weaken":          Weaken(target, card); break;
            case "Mark Target":     MarkTarget(target, card); break;
            case "Bleed Out":       BleedOut(target, card); break;
            case "Venom":           Venom(target, card); break;
            case "Debilitate":      Debilitate(target, card); break;
            case "Adrenaline":      Adrenaline(card); break;
            case "Second Chance":   SecondChance(card); break;
        }
    }

    private void Strike(EnemyHealth target, CardData card)
    {
        GetCombat().Attack(target, card.damage);
    }

    private void Defend(CardData card)
    {
        GetBlock().AddBlock(card.block);
    }

    private void Bash(EnemyHealth target, CardData card)
    {
        AttackAllEnemies(card.damage);
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
        GetStatus()?.AddStatus(StatusType.Counter, card.statusAmount, card.statusDuration);
    }

    private void LastStand(EnemyHealth target, CardData card)
    {
        int dmg = card.damage;
        PlayerHealth hp = GetHealth();
        if (hp != null && hp.CurrentHealth < hp.MaxHealth * 0.5f)
            dmg = card.damage * 2;
        GetCombat().Attack(target, dmg);
    }

    private void Sacrifice(EnemyHealth target, CardData card)
    {
        GetCombat().Attack(target, card.damage);
        GetHealth()?.TakeDamage(card.damage / 2, false);
    }

    private void Bloodthirst(EnemyHealth target, CardData card)
    {
        int prev = target.CurrentHealth;
        GetCombat().Attack(target, card.damage);
        int dealt = prev - target.CurrentHealth;
        if (dealt > 0)
        {
            int pct = card.statusAmount > 0 ? card.statusAmount : 50;
            GetHealth()?.Heal(Mathf.RoundToInt(dealt * (pct / 100f)));
        }
    }

    private void Executioner(EnemyHealth target, CardData card)
    {
        if (target == null || target.MaxHealth <= 0) return;
        if (target.IsBoss) return;
        int thresholdPct = card.statusAmount > 0 ? card.statusAmount : 20;
        if (target.CurrentHealth <= target.MaxHealth * (thresholdPct / 100f))
            target.TakeDamage(9999, false);
    }

    private void BladeStorm(EnemyHealth target, CardData card)
    {
        int kills = 0;
        foreach (EnemyHealth e in GetAliveEnemies())
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

    private void AttackAllEnemies(int damage)
    {
        foreach (EnemyHealth e in GetAliveEnemies())
            GetCombat().Attack(e, damage);
    }

    private System.Collections.Generic.List<EnemyHealth> GetAliveEnemies()
    {
        System.Collections.Generic.List<EnemyHealth> result = new();
        foreach (EnemyHealth e in FindObjectsByType<EnemyHealth>(FindObjectsSortMode.None))
        {
            if (e != null && e.CurrentHealth > 0)
                result.Add(e);
        }
        return result;
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
        target?.GetComponent<EnemyStatus>()?.AddStatus(StatusType.Weak, card.statusAmount, card.statusDuration);
    }

    private void UndyingWill(CardData card)
    {
        GetBlock().AddBlock(card.block);
        GetStatus()?.AddStatus(StatusType.Immortal, card.statusAmount > 0 ? card.statusAmount : 1, card.statusDuration > 0 ? card.statusDuration : 1);
    }

    private void SecondWind(CardData card)
    {
        int healAmt = card.heal;
        PlayerHealth hp = GetHealth();
        if (hp != null && hp.CurrentHealth < hp.MaxHealth * 0.3f)
            healAmt = Mathf.RoundToInt(healAmt * 1.3f);
        hp?.Heal(healAmt);
    }

    private void BloodFeast(CardData card)
    {
        GetHealth()?.Heal(card.heal);
        GetStatus()?.AddStatus(StatusType.Lifesteal, card.statusAmount > 0 ? card.statusAmount : 1, card.statusDuration > 0 ? card.statusDuration : 1);
    }

    private void RejuvenatingAura(CardData card)
    {
        GetHealth()?.Heal(card.heal);
        GetStatus()?.AddStatus(StatusType.Regen, card.statusAmount, card.statusDuration);
    }

    private void Enrage(CardData card)
    {
        GetStatus()?.AddStatus(StatusType.Strength, card.statusAmount, card.statusDuration);
    }

    private void Shockwave(EnemyHealth target, CardData card)
    {
        target?.GetComponent<EnemyStatus>()?.AddStatus(StatusType.Weak, card.statusAmount, card.statusDuration);
    }

    private void ShatterArmor(EnemyHealth target, CardData card)
    {
        target?.GetComponent<EnemyStatus>()?.AddStatus(StatusType.Vulnerable, card.statusAmount, card.statusDuration);
    }

    private void Intimidate(EnemyHealth target, CardData card)
    {
        if (target == null) return;
        EnemyStatus es = target.GetComponent<EnemyStatus>();
        if (es == null) return;
        if (target.IsBoss)
            es.AddStatus(StatusType.Weak, card.statusAmount, card.statusDuration);
        else
            es.AddStatus(StatusType.Stun, 1, 1);
    }

    private void Hemorrhage(EnemyHealth target, CardData card)
    {
        target?.GetComponent<EnemyStatus>()?.AddStatus(StatusType.Bleed, card.statusAmount, card.statusDuration);
    }

    private void Refresh(CardData card)
    {
        HandManager h = GetHand();
        DeckManager d = GetDeck();
        if (h == null || d == null) return;
        List<CardDisplay> hand = h.GetCardsInHand();
        if (hand.Count == 0) return;
        CardDisplay toDiscard = hand[0];
        int index = h.GetIndex(toDiscard);
        d.AddToDiscard(toDiscard.CardData);
        h.RemoveCard(toDiscard);
        int amount = card.statusAmount > 0 ? card.statusAmount : 1;
        for (int i = 0; i < amount; i++)
        {
            CardData drawn = d.DrawCard();
            if (drawn == null || h.IsFull) break;
            h.AddCard(drawn, index);
        }
    }

    private void RiskyGambit(CardData card)
    {
        GetHealth()?.TakeDamage(card.damage, false);
        HandManager h = GetHand();
        DeckManager d = GetDeck();
        if (h == null || d == null) return;
        int amount = card.statusAmount > 0 ? card.statusAmount : 2;
        for (int i = 0; i < amount; i++)
        {
            CardData drawn = d.DrawCard();
            if (drawn != null && !h.IsFull)
                h.AddCard(drawn);
        }
    }

    private void DoubleEdge(EnemyHealth target, CardData card)
    {
        GetCombat().Attack(target, card.damage);
        GetCombat().Attack(target, card.damage);
    }

    private void Whirlwind(EnemyHealth target, CardData card)
    {
        AttackAllEnemies(card.damage);
    }

    private void Puncture(EnemyHealth target, CardData card)
    {
        GetCombat().Attack(target, card.damage);
        target?.GetComponent<EnemyStatus>()?.AddStatus(StatusType.Bleed, card.statusAmount, card.statusDuration);
    }

    private void ShieldBash(EnemyHealth target, CardData card)
    {
        GetBlock().AddBlock(card.block);
        GetCombat().Attack(target, card.damage);
    }

    private void VampiricStrike(EnemyHealth target, CardData card)
    {
        if (target == null) return;
        int prev = target.CurrentHealth;
        GetCombat().Attack(target, card.damage);
        int dealt = prev - target.CurrentHealth;
        if (dealt > 0)
            GetHealth()?.Heal(dealt);
    }

    private void CrushingBlow(EnemyHealth target, CardData card)
    {
        int dmg = card.damage;
        EnemyStatus es = target?.GetComponent<EnemyStatus>();
        if (es != null && es.GetStatus(StatusType.Vulnerable) > 0)
            dmg *= 2;
        GetCombat().Attack(target, dmg);
    }

    private void Assassinate(EnemyHealth target, CardData card)
    {
        int dmg = card.damage;
        EnemyStatus es = target?.GetComponent<EnemyStatus>();
        if (es != null && es.GetStatus(StatusType.Bleed) > 0)
            dmg = card.damage * 3;
        GetCombat().Attack(target, dmg);
    }

    private void PoisonDagger(EnemyHealth target, CardData card)
    {
        GetCombat().Attack(target, card.damage);
        target?.GetComponent<EnemyStatus>()?.AddStatus(StatusType.Poison, card.statusAmount, card.statusDuration);
    }

    private void BloodBoil(EnemyHealth target, CardData card)
    {
        GetHealth()?.TakeDamage(card.statusAmount, false);
        GetCombat().Attack(target, card.damage);
    }

    private void Fury(EnemyHealth target, CardData card)
    {
        GetCombat().Attack(target, card.damage);
        GetStatus()?.AddStatus(StatusType.Strength, card.statusAmount, card.statusDuration);
    }

    private void IronWall(CardData card)
    {
        GetBlock().AddBlock(card.block);
    }

    private void Brace(CardData card)
    {
        GetBlock().AddBlock(card.block);
        HandManager h = GetHand();
        DeckManager d = GetDeck();
        if (h == null || d == null) return;
        CardData drawn = d.DrawCard();
        if (drawn != null && !h.IsFull)
            h.AddCard(drawn);
    }

    private void Reposition(CardData card)
    {
        GetBlock().AddBlock(card.block);
    }

    private void MirrorShield(CardData card)
    {
        GetBlock().AddBlock(card.block);
        GetStatus()?.AddStatus(StatusType.Counter, card.statusAmount, card.statusDuration);
    }

    private void Fortify(CardData card)
    {
        int blk = card.block;
        PlayerHealth hp = GetHealth();
        if (hp != null && hp.CurrentHealth < hp.MaxHealth * 0.5f)
            blk *= 2;
        GetBlock().AddBlock(blk);
    }

    private void Stoneskin(CardData card)
    {
        PlayerStatus ps = GetStatus();
        int str = ps != null ? ps.GetStatus(StatusType.Strength) : 0;
        int extra = str * card.statusAmount;
        GetBlock().AddBlock(card.block + extra);
    }

    private void Aegis(CardData card)
    {
        GetBlock().AddBlock(card.block);
        GetStatus()?.AddStatus(StatusType.Regen, card.statusAmount, card.statusDuration);
    }

    private void Bandage(CardData card)
    {
        GetHealth()?.Heal(card.heal);
        GetBlock().AddBlock(card.block);
    }

    private void Leech(EnemyHealth target, CardData card)
    {
        if (target == null) return;
        int prev = target.CurrentHealth;
        GetCombat().Attack(target, card.damage);
        int dealt = prev - target.CurrentHealth;
        if (dealt > 0)
            GetHealth()?.Heal(dealt);
    }

    private void LifeSpring(CardData card)
    {
        GetHealth()?.Heal(card.heal);
        GetStatus()?.AddStatus(StatusType.Regen, card.statusAmount, card.statusDuration);
    }

    private void GreaterHeal(CardData card)
    {
        GetHealth()?.Heal(card.heal);
    }

    private void Absorb(CardData card)
    {
        GetHealth()?.Heal(card.heal);
        GetBlock().AddBlock(card.block);
    }

    private void VampiricAura(CardData card)
    {
        GetStatus()?.AddStatus(StatusType.Lifesteal, card.statusAmount, card.statusDuration);
    }

    private void Weaken(EnemyHealth target, CardData card)
    {
        target?.GetComponent<EnemyStatus>()?.AddStatus(StatusType.Weak, card.statusAmount, card.statusDuration);
    }

    private void MarkTarget(EnemyHealth target, CardData card)
    {
        target?.GetComponent<EnemyStatus>()?.AddStatus(StatusType.Vulnerable, card.statusAmount, card.statusDuration);
    }

    private void BleedOut(EnemyHealth target, CardData card)
    {
        target?.GetComponent<EnemyStatus>()?.AddStatus(StatusType.Bleed, card.statusAmount, card.statusDuration);
    }

    private void Venom(EnemyHealth target, CardData card)
    {
        target?.GetComponent<EnemyStatus>()?.AddStatus(StatusType.Poison, card.statusAmount, card.statusDuration);
    }

    private void Debilitate(EnemyHealth target, CardData card)
    {
        EnemyStatus es = target?.GetComponent<EnemyStatus>();
        if (es == null) return;
        es.AddStatus(StatusType.Weak, card.statusAmount, card.statusDuration);
        es.AddStatus(StatusType.Vulnerable, card.statusAmount, card.statusDuration);
    }

    private void Adrenaline(CardData card)
    {
        GetHealth()?.TakeDamage(card.damage, false);
        HandManager h = GetHand();
        DeckManager d = GetDeck();
        if (h == null || d == null) return;
        int amount = card.statusAmount > 0 ? card.statusAmount : 3;
        for (int i = 0; i < amount; i++)
        {
            CardData drawn = d.DrawCard();
            if (drawn != null && !h.IsFull)
                h.AddCard(drawn);
        }
    }

    private void SecondChance(CardData card)
    {
        GetStatus()?.AddStatus(StatusType.Immortal, card.statusAmount, card.statusDuration);
    }
}

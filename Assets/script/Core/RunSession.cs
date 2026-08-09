using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class RunSession
{
    public const int MaxDeckSize = 30;

    public static bool IsDeckFull => Deck != null && Deck.Count >= MaxDeckSize;

    public static int LastBuiltDeckCount = 0;

    public static bool HasNewCards => Deck != null && Deck.Count > LastBuiltDeckCount;

    public static void EnsureDeckReady()
    {
        if (!RunActive) return;
        if (Deck != null && Deck.Count > 0) return;

        CardDatabase db = Object.FindAnyObjectByType<CardDatabase>();
        if (db != null)
            Deck = db.GetStarterDeck();
        else
            Deck = RuntimeCardLibrary.GetStarterDeck();

        LastBuiltDeckCount = Deck != null ? Deck.Count : 0;
    }

    public static bool RunActive = false;
    public static string MapSceneName = "MapLevel1";
    public static int MapLevel = 1;
    public static List<CardData> Deck = new List<CardData>();

    public static List<CardData> BattleDeck = null;
    public static int PlayerMaxHealth = 80;
    public static int PlayerCurrentHealth = 80;
    public static int Gold = 100;

    public static bool IsBossBattle = false;
    public static bool IsFinalBoss = false;
    public static List<EnemyData> BossSequence = null;

    public static void StartNewRun()
    {
        RunActive = true;
        MapSceneName = "MapLevel1";
        MapLevel = 1;
        Deck = new List<CardData>();
        BattleDeck = null;
        LastBuiltDeckCount = 0;
        PlayerMaxHealth = 80;
        PlayerCurrentHealth = 80;
        Gold = 100;
        IsBossBattle = false;
        IsFinalBoss = false;
        BossSequence = null;

        MapManager.ClearProgress();
        RelicManager.Instance.ClearRelics();

        CloudSave.Delete();
    }

    public static void AdvanceToNextMap()
    {
        IsBossBattle = false;
        IsFinalBoss = false;
        BossSequence = null;

        if (MapLevel < 4)
        {
            MapLevel++;
            MapSceneName = "MapLevel" + MapLevel;
            MapManager.ClearProgress();
            CloudSave.Save();
            SceneLoader.TransitionTo(MapSceneName);
            return;
        }

        // Map 4 done -> victory, start a new run
        StartNewRun();
        SceneLoader.TransitionTo("MainMenu");
    }

    public static void ClearDeck()
    {
        if (Deck == null) Deck = new List<CardData>();
        else Deck.Clear();
    }

    public static void UpgradeCards(string cardName)
    {
        if (Deck == null) return;

        foreach (CardData card in Deck)
        {
            if (card == null) continue;
            if (card.cardName != cardName) continue;
            if (card.isUpgraded) continue;
            card.Upgrade();
        }
    }

    public static void ReturnToMap()
    {
        BattleDeck = null;
        if (string.IsNullOrEmpty(MapSceneName))
            MapSceneName = "MainMenu";

        CloudSave.Save();

        SceneLoader.TransitionTo(MapSceneName);
    }
}

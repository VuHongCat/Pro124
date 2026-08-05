using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class RunSession
{
    public static bool RunActive = false;
    public static string MapSceneName = "MapLevel1";
    public static List<CardData> Deck = new List<CardData>();
    public static int PlayerMaxHealth = 80;
    public static int PlayerCurrentHealth = 80;
    public static int Gold = 100;

    public static bool IsBossBattle = false;
    public static List<EnemyData> BossSequence = null;

    public static void StartNewRun()
    {
        RunActive = true;
        MapSceneName = "MapLevel1";
        Deck = new List<CardData>();
        PlayerMaxHealth = 80;
        PlayerCurrentHealth = 80;
        Gold = 100;
        IsBossBattle = false;
        BossSequence = null;

        MapManager.ClearProgress();
        RelicManager.Instance.ClearRelics();
    }

    public static void ClearDeck()
    {
        if (Deck == null) Deck = new List<CardData>();
        else Deck.Clear();
    }

    public static void ReturnToMap()
    {
        if (string.IsNullOrEmpty(MapSceneName))
            MapSceneName = "MainMenu";
        SceneManager.LoadScene(MapSceneName);
    }
}

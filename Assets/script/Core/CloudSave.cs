using System;
using System.Collections.Generic;
using Firebase.Auth;
using Firebase.Extensions;
using Firebase.Firestore;
using UnityEngine;

[Serializable]
public class CloudSaveData
{
    public bool runActive;
    public string mapSceneName = "MapLevel1";
    public int mapLevel = 1;
    public int playerMaxHealth = 80;
    public int playerCurrentHealth = 80;
    public int gold = 100;
    public List<string> deck = new();
    public List<string> relics = new();
    public List<string> completedNodes = new();
    public string battleNode = "";
    public int unlockedIsland = 1;
}

public static class CloudSave
{
    public const string SaveCollection = "players";
    public const string SaveField = "save";
    private const char CardSeparator = '|';

    // =========================================================
    // HELPERS
    // =========================================================

    private static bool CanSave()
    {
        if (FirebaseManager.Instance == null)
            return false;
        if (!FirebaseManager.Instance.IsFirebaseReady)
            return false;
        return FirebaseManager.Instance.CurrentUser != null;
    }

    private static string GetUserId()
    {
        return FirebaseManager.Instance.CurrentUser.UserId;
    }

    // =========================================================
    // SAVE
    // =========================================================

    public static void Save()
    {
        if (!CanSave())
            return;

        CloudSaveData data = Capture();
        string json = JsonUtility.ToJson(data);

        FirebaseManager.Instance.Firestore
            .Collection(SaveCollection)
            .Document(GetUserId())
            .SetAsync(new Dictionary<string, object> { { SaveField, json } })
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted || task.IsCanceled)
                    Debug.LogWarning($"[CloudSave] Save failed: {task.Exception}");
                else
                    Debug.Log("[CloudSave] Save OK");
            });
    }

    private static CloudSaveData Capture()
    {
        CloudSaveData data = new CloudSaveData();

        data.runActive = RunSession.RunActive;
        data.mapSceneName = string.IsNullOrEmpty(RunSession.MapSceneName)
            ? "MapLevel1"
            : RunSession.MapSceneName;
        data.mapLevel = Mathf.Clamp(RunSession.MapLevel, 1, 4);
        data.playerMaxHealth = Mathf.Max(1, RunSession.PlayerMaxHealth);
        data.playerCurrentHealth = Mathf.Clamp(
            RunSession.PlayerCurrentHealth,
            0,
            data.playerMaxHealth
        );
        data.gold = Mathf.Max(0, RunSession.Gold);

        if (RunSession.Deck != null)
        {
            foreach (CardData card in RunSession.Deck)
            {
                if (card == null) continue;
                data.deck.Add(
                    card.cardName + CardSeparator + (card.isUpgraded ? "1" : "0")
                );
            }
        }

        if (RelicManager.Instance != null)
        {
            foreach (RelicData relic in RelicManager.Instance.GetOwnedRelics())
            {
                if (relic != null && !string.IsNullOrEmpty(relic.relicName))
                    data.relics.Add(relic.relicName);
            }
        }

        data.completedNodes = MapManager.GetCompletedNodeNames();
        data.battleNode = PlayerPrefs.GetString(MapManager.BattleNodeKey, "");
        data.unlockedIsland = Mathf.Max(1, PlayerPrefs.GetInt("UnlockedIsland", 1));

        return data;
    }

    // =========================================================
    // LOAD
    // =========================================================

    public static void Load(Action<bool> onDone = null)
    {
        if (!CanSave())
        {
            onDone?.Invoke(false);
            return;
        }

        FirebaseManager.Instance.Firestore
            .Collection(SaveCollection)
            .Document(GetUserId())
            .GetSnapshotAsync()
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted || task.IsCanceled)
                {
                    Debug.LogWarning($"[CloudSave] Load failed: {task.Exception}");
                    onDone?.Invoke(false);
                    return;
                }

                DocumentSnapshot snapshot = task.Result;
                if (snapshot == null || !snapshot.Exists)
                {
                    onDone?.Invoke(false);
                    return;
                }

                if (!snapshot.TryGetValue<string>(SaveField, out string json) ||
                    string.IsNullOrEmpty(json))
                {
                    onDone?.Invoke(false);
                    return;
                }

                CloudSaveData data = JsonUtility.FromJson<CloudSaveData>(json);
                if (data == null)
                {
                    onDone?.Invoke(false);
                    return;
                }

                Apply(data);
                onDone?.Invoke(true);
            });
    }

    private static void Apply(CloudSaveData data)
    {
        // ---- PlayerPrefs (MapManager / WorldMap read at Start) ----

        if (data.completedNodes == null || data.completedNodes.Count == 0)
            PlayerPrefs.DeleteKey(MapManager.CompletedNodeKey);
        else
            PlayerPrefs.SetString(
                MapManager.CompletedNodeKey,
                string.Join(";", data.completedNodes)
            );

        if (string.IsNullOrEmpty(data.battleNode))
            PlayerPrefs.DeleteKey(MapManager.BattleNodeKey);
        else
            PlayerPrefs.SetString(MapManager.BattleNodeKey, data.battleNode);

        PlayerPrefs.SetInt("UnlockedIsland", Mathf.Max(1, data.unlockedIsland));
        PlayerPrefs.Save();

        // ---- RunSession ----

        RunSession.RunActive = data.runActive;
        RunSession.MapSceneName = string.IsNullOrEmpty(data.mapSceneName)
            ? "MapLevel1"
            : data.mapSceneName;
        RunSession.MapLevel = Mathf.Clamp(data.mapLevel, 1, 4);
        RunSession.PlayerMaxHealth = Mathf.Max(1, data.playerMaxHealth);
        RunSession.PlayerCurrentHealth = Mathf.Clamp(
            data.playerCurrentHealth,
            0,
            RunSession.PlayerMaxHealth
        );
        RunSession.Gold = Mathf.Max(0, data.gold);
        RunSession.IsBossBattle = false;
        RunSession.IsFinalBoss = false;
        RunSession.BossSequence = null;

        // ---- Deck ----

        RunSession.Deck = new List<CardData>();
        if (data.deck != null)
        {
            foreach (string entry in data.deck)
            {
                CardData card = RebuildCard(entry);
                if (card != null)
                    RunSession.Deck.Add(card);
            }
        }

        RunSession.LastBuiltDeckCount = RunSession.Deck.Count;

        // ---- Relics ----

        RelicManager.PendingRelicNames = data.relics;
        if (RelicManager.Instance != null)
            RelicManager.Instance.LoadRelics();
    }

    private static CardData RebuildCard(string entry)
    {
        if (string.IsNullOrEmpty(entry))
            return null;

        string[] parts = entry.Split(CardSeparator);
        string name = parts[0];
        bool upgraded = parts.Length > 1 && parts[1] == "1";

        if (string.IsNullOrEmpty(name))
            return null;

        CardData card = null;

        CardDatabase db = UnityEngine.Object.FindAnyObjectByType<CardDatabase>();
        if (db != null)
            card = db.GetCard(name);

        if (card == null)
            card = RuntimeCardLibrary.GetCardByName(name);

        if (card == null)
        {
            Debug.LogWarning($"[CloudSave] Card not found: {name}");
            return null;
        }

        CardData instance = UnityEngine.Object.Instantiate(card);
        if (upgraded)
            instance.Upgrade();

        return instance;
    }

    // =========================================================
    // DELETE
    // =========================================================

    public static void Delete()
    {
        if (!CanSave())
            return;

        FirebaseManager.Instance.Firestore
            .Collection(SaveCollection)
            .Document(GetUserId())
            .DeleteAsync()
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted || task.IsCanceled)
                    Debug.LogWarning($"[CloudSave] Delete failed: {task.Exception}");
                else
                    Debug.Log("[CloudSave] Save deleted");
            });
    }
}

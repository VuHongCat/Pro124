using System.Collections.Generic;
using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
    private static readonly string[] BasicTracks =
    {
        "Music/Bassic",
        "Music/Basic1",
        "Music/Basic2",
        "Music/Basic3"
    };

    private static readonly string[] BossTracks =
    {
        "Music/Boss",
        "Music/Boss1",
        "Music/Boss2",
        "Music/Boss3"
    };

    private static readonly List<string> usedBossTracks = new List<string>();
    private static readonly List<string> usedBasicTracks = new List<string>();
    private static string currentBasicTrack;
    private static int currentBasicLevel = -1;

    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource sfxSource;

    public static AudioManager EnsureInstance()
    {
        if (Instance != null) return Instance;
        GameObject go = new GameObject("AudioManager");
        return go.AddComponent<AudioManager>();
    }

    public static void ResetBossTracks()
    {
        usedBossTracks.Clear();
        usedBasicTracks.Clear();
        currentBasicTrack = null;
        currentBasicLevel = -1;
    }

    public static void PlayMapMusic(int mapLevel)
    {
        PlayTrack(PickBasicTrack(mapLevel));
    }

    public static void PlayBossMusic()
    {
        PlayTrack(PickBossTrack());
    }

    private static void PlayTrack(string path)
    {
        AudioClip clip = Resources.Load<AudioClip>(path);
        if (clip == null)
        {
            Debug.LogWarning("[AudioManager] Khong tim thay track: " + path);
            return;
        }

        EnsureInstance().PlayMusic(clip);
    }

    private static string PickBasicTrack(int mapLevel)
    {
        if (mapLevel == currentBasicLevel && !string.IsNullOrEmpty(currentBasicTrack))
            return currentBasicTrack;

        List<string> available = new List<string>();
        foreach (string track in BasicTracks)
        {
            if (!usedBasicTracks.Contains(track))
                available.Add(track);
        }

        if (available.Count == 0)
        {
            usedBasicTracks.Clear();
            available.AddRange(BasicTracks);
        }

        string chosen = available[Random.Range(0, available.Count)];
        usedBasicTracks.Add(chosen);
        currentBasicTrack = chosen;
        currentBasicLevel = mapLevel;
        return chosen;
    }

    private static string PickBossTrack()
    {
        List<string> available = new List<string>();
        foreach (string track in BossTracks)
        {
            if (!usedBossTracks.Contains(track))
                available.Add(track);
        }

        if (available.Count == 0)
        {
            usedBossTracks.Clear();
            available.AddRange(BossTracks);
        }

        string chosen = available[Random.Range(0, available.Count)];
        usedBossTracks.Add(chosen);
        return chosen;
    }

    protected override void Awake()
    {
        base.Awake();

        if (musicSource == null)
            musicSource = gameObject.AddComponent<AudioSource>();
        if (sfxSource == null)
            sfxSource = gameObject.AddComponent<AudioSource>();

        musicSource.loop = true;
        musicSource.playOnAwake = false;
        sfxSource.playOnAwake = false;
    }

    public void PlayMusic(AudioClip clip)
    {
        if (musicSource.clip == clip && musicSource.isPlaying)
            return;

        musicSource.clip = clip;
        musicSource.Play();
    }

    public void PlaySFX(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }
}

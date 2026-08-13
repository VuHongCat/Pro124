using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameSettingsUI : MonoBehaviour
{
    private static GameSettingsUI instance;
    private GameObject buttonCanvas;
    private GameObject settingsPanel;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Init()
    {
        if (instance != null) return;
        GameObject go = new GameObject("GameSettingsUI");
        instance = go.AddComponent<GameSettingsUI>();
        DontDestroyOnLoad(go);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        RefreshButton();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        RefreshButton();
    }

    private void RefreshButton()
    {
        if (buttonCanvas != null)
        {
            Destroy(buttonCanvas);
            buttonCanvas = null;
        }
        if (settingsPanel != null)
        {
            Destroy(settingsPanel);
            settingsPanel = null;
        }

        string scene = SceneManager.GetActiveScene().name;
        if (scene == "Login" || scene == "MainMenu")
            return;

        CreateButton();
    }

    private void CreateButton()
    {
        Canvas canvas = RuntimeUi.CreateCanvas("SettingsButtonCanvas");
        buttonCanvas = canvas.gameObject;
        Button btn = RuntimeUi.CreateButton(canvas.transform, "Settings", new Vector2(0, 0), new Vector2(150, 50), OnSettingsClick);
        RectTransform rt = btn.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.one;
        rt.anchorMax = Vector2.one;
        rt.pivot = Vector2.one;
        rt.anchoredPosition = new Vector2(-20, -20);
    }

    private void OnSettingsClick()
    {
        if (settingsPanel != null) return;
        AudioManager.EnsureInstance();

        Canvas canvas = RuntimeUi.CreateCanvas("SettingsCanvas");
        settingsPanel = RuntimeUi.CreatePanel(canvas.transform, new Color(0, 0, 0, 0.85f));

        RuntimeUi.CreateText(settingsPanel.transform, "Settings", 30, TextAnchor.MiddleCenter,
            new Vector2(0, 0.8f), new Vector2(1, 0.92f));

        RuntimeUi.CreateText(settingsPanel.transform, "Music Volume", 18, TextAnchor.MiddleLeft,
            new Vector2(0.06f, 0.62f), new Vector2(0.44f, 0.7f));
        CreateVolumeSlider(settingsPanel.transform,
            new Vector2(0.5f, 0.62f), new Vector2(0.94f, 0.7f),
            AudioManager.MusicVolume, v => AudioManager.MusicVolume = v);

        RuntimeUi.CreateText(settingsPanel.transform, "SFX Volume", 18, TextAnchor.MiddleLeft,
            new Vector2(0.06f, 0.44f), new Vector2(0.44f, 0.52f));
        CreateVolumeSlider(settingsPanel.transform,
            new Vector2(0.5f, 0.44f), new Vector2(0.94f, 0.52f),
            AudioManager.SfxVolume, v => AudioManager.SfxVolume = v);

        RuntimeUi.CreateButton(settingsPanel.transform, "Close", new Vector2(0, -320), new Vector2(200, 50), () =>
        {
            Destroy(settingsPanel);
            settingsPanel = null;
        });
    }

    private Slider CreateVolumeSlider(Transform parent, Vector2 anchorMin, Vector2 anchorMax, float value, Action<float> onChange)
    {
        GameObject go = new GameObject("VolumeSlider", typeof(RectTransform), typeof(Image));
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.SetParent(parent, false);
        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;

        Image bgImg = go.GetComponent<Image>();
        bgImg.color = new Color(0.1f, 0.1f, 0.12f, 0.9f);

        GameObject fill = new GameObject("Fill", typeof(RectTransform), typeof(Image));
        RectTransform fillRt = fill.GetComponent<RectTransform>();
        fillRt.SetParent(rt, false);
        fillRt.anchorMin = Vector2.zero;
        fillRt.anchorMax = Vector2.one;
        fillRt.offsetMin = Vector2.zero;
        fillRt.offsetMax = Vector2.zero;
        fill.GetComponent<Image>().color = new Color(0.35f, 0.75f, 0.4f, 1f);

        Slider slider = go.AddComponent<Slider>();
        slider.targetGraphic = bgImg;
        slider.fillRect = fillRt;
        slider.minValue = 0f;
        slider.maxValue = 1f;
        slider.value = value;
        slider.onValueChanged.AddListener(v => onChange(v));
        return slider;
    }
}

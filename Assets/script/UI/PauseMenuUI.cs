using UnityEngine;
using UnityEngine.UI;

public class PauseMenuUI : MonoBehaviour
{
    public static PauseMenuUI Instance;

    private GameObject panelRoot;
    private bool isPaused;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        BuildUI();
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Time.timeScale = 1f;
            Instance = null;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            Toggle();
    }

    private void BuildUI()
    {
        Canvas canvas = RuntimeUi.CreateCanvas("PauseMenuCanvas");
        canvas.sortingOrder = 170;

        GameObject btnGo = new GameObject("PauseButton", typeof(RectTransform), typeof(Image), typeof(Button));
        RectTransform btnRt = btnGo.GetComponent<RectTransform>();
        btnRt.SetParent(canvas.transform, false);
        btnRt.anchorMin = new Vector2(1, 1);
        btnRt.anchorMax = new Vector2(1, 1);
        btnRt.pivot = new Vector2(1, 1);
        btnRt.anchoredPosition = new Vector2(-10, -10);
        btnRt.sizeDelta = new Vector2(120, 42);

        Image img = btnGo.GetComponent<Image>();
        img.color = new Color(0.15f, 0.15f, 0.2f, 0.9f);
        Button btn = btnGo.GetComponent<Button>();
        btn.targetGraphic = img;
        btn.onClick.AddListener(Toggle);
        RuntimeUi.CreateText(btnRt, "Pause", 16, TextAnchor.MiddleCenter, Vector2.zero, Vector2.one);

        panelRoot = RuntimeUi.CreatePanel(canvas.transform, new Color(0, 0, 0, 0.78f));
        panelRoot.SetActive(false);

        RuntimeUi.CreateText(panelRoot.transform, "Paused", 40, TextAnchor.MiddleCenter,
            new Vector2(0, 0.55f), new Vector2(1, 0.75f));

        RuntimeUi.CreateButton(panelRoot.transform, "Resume", new Vector2(0, -60), new Vector2(240, 60), Resume);
    }

    public void Toggle()
    {
        if (isPaused)
            Resume();
        else
            Pause();
    }

    private void Pause()
    {
        isPaused = true;
        Time.timeScale = 0f;
        panelRoot.SetActive(true);
    }

    private void Resume()
    {
        isPaused = false;
        Time.timeScale = 1f;
        panelRoot.SetActive(false);
    }
}

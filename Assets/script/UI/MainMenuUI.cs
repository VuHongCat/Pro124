using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Button playButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private float hoverScale = 1.15f;
    [SerializeField] private float zoomDuration = 0.25f;

    private bool playHovered;
    private bool quitHovered;

    private void Start()
    {
        if (playButton == null) playButton = FindButton("Play");
        if (quitButton == null) quitButton = FindButton("Quit");

        if (playButton != null)
        {
            playButton.onClick.RemoveAllListeners();
            playButton.onClick.AddListener(OnPlayClick);
        }

        if (quitButton != null)
        {
            quitButton.onClick.RemoveAllListeners();
            quitButton.onClick.AddListener(OnQuitClick);
        }
    }

    private void Update()
    {
        bool nowPlayHovered = IsPointerOver(playButton);
        bool nowQuitHovered = IsPointerOver(quitButton);

        if (nowPlayHovered != playHovered)
        {
            playHovered = nowPlayHovered;
            ScaleTo(playButton.transform, nowPlayHovered ? hoverScale : 1f);
        }

        if (nowQuitHovered != quitHovered)
        {
            quitHovered = nowQuitHovered;
            ScaleTo(quitButton.transform, nowQuitHovered ? hoverScale : 1f);
        }
    }

    private bool IsPointerOver(Button button)
    {
        if (button == null) return false;
        RectTransform rt = (RectTransform)button.transform;
        return RectTransformUtility.RectangleContainsScreenPoint(rt, Input.mousePosition);
    }

    private void ScaleTo(Transform target, float scale)
    {
        StartCoroutine(ScaleRoutine(target, scale));
    }

    private IEnumerator ScaleRoutine(Transform target, float scale)
    {
        Vector3 start = target.localScale;
        Vector3 goal = new Vector3(scale, scale, 1f);

        float elapsed = 0f;
        while (elapsed < zoomDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / zoomDuration);
            target.localScale = Vector3.Lerp(start, goal, EaseOutBack(t));
            yield return null;
        }

        target.localScale = goal;
    }

    private Button FindButton(string name)
    {
        foreach (Button b in Object.FindObjectsByType<Button>(FindObjectsSortMode.None))
        {
            if (b.gameObject.name == name) return b;
        }
        return null;
    }

    private void OnPlayClick()
    {
        PlayGame();
    }

    private void OnQuitClick()
    {
        QuitGame();
    }

    private static float EaseOutBack(float t)
    {
        const float c1 = 1.70158f;
        const float c3 = c1 + 1f;
        return 1f + c3 * Mathf.Pow(t - 1f, 3f) + c1 * Mathf.Pow(t - 1f, 2f);
    }

    public void PlayGame()
    {
        SceneLoader.TransitionTo("WorldMap");
    }

    public void ContinueGame()
    {
        if (RunSession.RunActive &&
            !string.IsNullOrEmpty(RunSession.MapSceneName))
        {
            SceneLoader.TransitionTo(RunSession.MapSceneName);
        }
        else
        {
            SceneLoader.TransitionTo("WorldMap");
        }
    }

    public void NewRun()
    {
        RunSession.StartNewRun();
        SceneLoader.TransitionTo("WorldMap");
    }

    public void QuitGame()
    {
        SceneLoader.Instance.QuitGame();
    }
}

using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : Singleton<SceneLoader>
{
    [SerializeField] private float fadeDuration = 0.3f;
    [SerializeField] private Color fadeColor = Color.black;

    private Image fadeImage;
    private Coroutine transitionRoutine;

    protected override void Awake()
    {
        base.Awake();
        if (Instance != this) return;
        EnsureFadeOverlay();
    }

    public static void TransitionTo(string sceneName)
    {
        GetOrCreate().LoadScene(sceneName);
    }

    public void LoadScene(string sceneName)
    {
        if (transitionRoutine != null)
            StopCoroutine(transitionRoutine);
        transitionRoutine = StartCoroutine(Transition(sceneName));
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    private static SceneLoader GetOrCreate()
    {
        if (Instance == null)
        {
            GameObject go = new GameObject("SceneLoader");
            go.AddComponent<SceneLoader>();
        }
        return Instance;
    }

    private IEnumerator Transition(string sceneName)
    {
        yield return StartCoroutine(Fade(0f, 1f, fadeDuration));
        SceneManager.LoadScene(sceneName);
        yield return StartCoroutine(Fade(1f, 0f, fadeDuration));
    }

    private IEnumerator Fade(float from, float to, float duration)
    {
        EnsureFadeOverlay();
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(from, to, t / duration);
            SetFadeAlpha(alpha);
            yield return null;
        }
        SetFadeAlpha(to);
    }

    private void SetFadeAlpha(float alpha)
    {
        if (fadeImage == null) return;
        Color c = fadeImage.color;
        c.a = alpha;
        fadeImage.color = c;
    }

    private void EnsureFadeOverlay()
    {
        if (fadeImage != null) return;

        GameObject canvasGo = new GameObject(
            "SceneTransitionCanvas",
            typeof(Canvas),
            typeof(CanvasScaler),
            typeof(GraphicRaycaster)
        );
        Canvas canvas = canvasGo.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 1000;
        DontDestroyOnLoad(canvasGo);

        GameObject imageGo = new GameObject(
            "Fade",
            typeof(RectTransform),
            typeof(Image)
        );
        imageGo.transform.SetParent(canvasGo.transform, false);

        Image img = imageGo.GetComponent<Image>();
        img.color = fadeColor;
        img.raycastTarget = false;

        RectTransform rt = img.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;

        fadeImage = img;
        SetFadeAlpha(0f);
    }
}

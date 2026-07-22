using UnityEngine;

public class UIManager : MonoBehaviour
{
    [Header("Panel")]

    [SerializeField] GameObject pausePanel;
    [SerializeField] GameObject loadingPanel;

    public void ShowPause()
    {
        pausePanel.SetActive(true);
    }

    public void HidePause()
    {
        pausePanel.SetActive(false);
    }

    public void ShowLoading(bool show)
    {
        loadingPanel.SetActive(show);
    }
}

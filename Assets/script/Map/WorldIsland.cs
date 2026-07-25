using UnityEngine;
using UnityEngine.SceneManagement;

public class WorldIsland : MonoBehaviour
{
    public string sceneName;

    private bool unlocked;

    private SpriteRenderer sr;
    private Collider2D col;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
    }

    public void SetUnlocked(bool value)
    {
        unlocked = value;

        sr.color = unlocked ? Color.white : Color.gray;

        if (col != null)
            col.enabled = unlocked;
    }

    private void OnMouseDown()
    {
        if (!unlocked)
            return;

        SceneManager.LoadScene(sceneName);
    }
}
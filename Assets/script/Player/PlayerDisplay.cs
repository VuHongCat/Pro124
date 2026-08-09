using UnityEngine;
using TMPro;
using System.Collections;

public class PlayerDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text hpText;
    [SerializeField] private TMP_Text statusText;
    private int currentBlock;

    [SerializeField] private float punchScale = 1.15f;
    [SerializeField] private float punchDuration = 0.18f;

    private Coroutine punchRoutine;

    private PlayerHealth playerHealth;
    private PlayerBlock playerBlock;
    private PlayerStatus playerStatus;

    private void Awake()
    {
        playerHealth = GetComponent<PlayerHealth>();
        playerBlock = GetComponent<PlayerBlock>();
        playerStatus = GetComponent<PlayerStatus>();
    }

    private void OnEnable()
    {
        playerHealth.OnHealthChanged += UpdateHealthUI;
        playerBlock.OnBlockChanged += UpdateBlockUI;
        if (playerStatus != null) playerStatus.OnStatusChanged += UpdateStatusUI;
    }

    private void OnDisable()
    {
        playerHealth.OnHealthChanged -= UpdateHealthUI;
        playerBlock.OnBlockChanged -= UpdateBlockUI;
        if (playerStatus != null) playerStatus.OnStatusChanged -= UpdateStatusUI;
    }
    private void UpdateHealthUI(int current, int max)
    {
        RefreshUI();
    }
    private void RefreshUI()
    {
        if (currentBlock > 0)
        {
            hpText.text = $"{playerHealth.CurrentHealth}/{playerHealth.MaxHealth} ({currentBlock})";
        }
        else
        {
            hpText.text = $"{playerHealth.CurrentHealth}/{playerHealth.MaxHealth}";
        }
    }
    private void UpdateBlockUI(int block)
    {
        currentBlock = block;
        RefreshUI();
    }

    private void UpdateStatusUI()
    {
        if (statusText == null || playerStatus == null) return;
        System.Text.StringBuilder sb = new();
        int s = playerStatus.GetStatus(StatusType.Strength);
        int w = playerStatus.GetStatus(StatusType.Weak);
        int v = playerStatus.GetStatus(StatusType.Vulnerable);
        if (s > 0) sb.Append($"STR+{s} ");
        if (w > 0) sb.Append($"Weak({w}) ");
        if (v > 0) sb.Append($"Vuln({v}) ");
        statusText.text = sb.ToString();
    }

    public void Punch()
    {
        if (punchRoutine != null) StopCoroutine(punchRoutine);
        punchRoutine = StartCoroutine(PunchRoutine());
    }

    private IEnumerator PunchRoutine()
    {
        RectTransform rt = GetComponent<RectTransform>();
        Vector3 baseScale = rt.localScale;
        float elapsed = 0f;

        while (elapsed < punchDuration)
        {
            float t = elapsed / punchDuration;
            float factor = 1f + (punchScale - 1f) * Mathf.Sin(t * Mathf.PI);
            rt.localScale = new Vector3(baseScale.x * factor, baseScale.y * factor, baseScale.z);
            elapsed += Time.deltaTime;
            yield return null;
        }

        rt.localScale = baseScale;
    }
}

using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemyDisplay : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image artworkImage;

    [SerializeField] private TMP_Text enemyNameText;

    [SerializeField] private TMP_Text hpText;

    [SerializeField] private TMP_Text intentText;

    [SerializeField] private TMP_Text statusText;

    private EnemyHealth enemyHealth;
    private EnemyIntent enemyIntent;
    private EnemyBlock enemyBlock;
    private EnemyStatus enemyStatus;
    private int currentBlock;
    public EnemyData EnemyData { get; private set; }
    private void Awake()
    {
        enemyHealth = GetComponent<EnemyHealth>();
        enemyIntent = GetComponent<EnemyIntent>();
        enemyBlock = GetComponent<EnemyBlock>();
        enemyStatus = GetComponent<EnemyStatus>();
    }
    public void Setup(EnemyData data)
    {
        EnemyData = data;

        artworkImage.sprite = data.artwork;

        enemyNameText.text = data.enemyName;
        RefreshUI();
    }

    private void OnEnable()
    {
        enemyHealth.OnHealthChanged += UpdateHealthUI;
        enemyIntent.OnIntentChanged += UpdateIntentUI;
        enemyBlock.OnBlockChanged += UpdateBlockUI;
        if (enemyStatus != null) enemyStatus.OnStatusChanged += UpdateStatusUI;
    }
    private void OnDisable()
    {
        enemyHealth.OnHealthChanged -= UpdateHealthUI;
        enemyIntent.OnIntentChanged -= UpdateIntentUI;
        enemyBlock.OnBlockChanged -= UpdateBlockUI;
        if (enemyStatus != null) enemyStatus.OnStatusChanged -= UpdateStatusUI;
    }
    private void UpdateHealthUI(int current, int max)
    {
        RefreshUI();
    }
    private void UpdateIntentUI(EnemyIntentType type, int value)
    {
        switch (type)
        {
            case EnemyIntentType.Attack:
                intentText.text = $"ATK {value}";
                break;

            case EnemyIntentType.Block:
                intentText.text = $"BLK {value}";
                break;

            case EnemyIntentType.Buff:
                intentText.text = "BUF";
                break;

            case EnemyIntentType.Debuff:
                intentText.text = "DEB";
                break;

            default:
                intentText.text = "";
                break;
        }
    }
    private void RefreshUI()
    {
        Debug.Log(enemyHealth.CurrentHealth);
        Debug.Log(enemyHealth.MaxHealth);
        if (currentBlock > 0)
        {
            hpText.text =
                $"{enemyHealth.CurrentHealth}/{enemyHealth.MaxHealth} ({currentBlock})";
        }
        else
        {
            hpText.text =
                $"{enemyHealth.CurrentHealth}/{enemyHealth.MaxHealth}";
        }
    }
    private void UpdateBlockUI(int block)
    {
        currentBlock = block;
        RefreshUI();
    }

    private void UpdateStatusUI()
    {
        if (statusText == null || enemyStatus == null) return;
        System.Text.StringBuilder sb = new();
        int s = enemyStatus.GetStatus(StatusType.Strength);
        int w = enemyStatus.GetStatus(StatusType.Weak);
        int v = enemyStatus.GetStatus(StatusType.Vulnerable);
        if (s > 0) sb.Append($"STR+{s} ");
        if (w > 0) sb.Append($"Weak({w}) ");
        if (v > 0) sb.Append($"Vuln({v}) ");
        statusText.text = sb.ToString();
    }
}
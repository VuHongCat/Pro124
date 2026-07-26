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

    private EnemyHealth enemyHealth;
    private EnemyIntent enemyIntent;
    public EnemyData EnemyData { get; private set; }
    private void Awake()
    {
        enemyHealth = GetComponent<EnemyHealth>();
        enemyIntent = GetComponent<EnemyIntent>();
    }
    public void Setup(EnemyData data)
    {
        EnemyData = data;

        artworkImage.sprite = data.artwork;

        enemyNameText.text = data.enemyName;

    }

    private void OnEnable()
    {
        enemyHealth.OnHealthChanged += UpdateHealthUI;
        enemyIntent.OnIntentChanged += UpdateIntentUI;
    }
    private void OnDisable()
    {
        enemyHealth.OnHealthChanged -= UpdateHealthUI;
        enemyIntent.OnIntentChanged -= UpdateIntentUI;
    }
    private void UpdateHealthUI(int current, int max)
    {
        hpText.text = $"{current}/{max}";
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
}
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemyDisplay : MonoBehaviour
{
    [Header("Enemy Image")]
    [SerializeField] private Image artworkImage;


    [Header("Animator")]
    [SerializeField] private Animator animator;


    [Header("UI")]
    [SerializeField] private TMP_Text enemyNameText;
    [SerializeField] private TMP_Text hpText;
    [SerializeField] private TMP_Text intentText;
    [SerializeField] private TMP_Text statusText;



    private EnemyHealth enemyHealth;
    private EnemyIntent enemyIntent;
    private EnemyBlock enemyBlock;
    private EnemyStatus enemyStatus;


    private int currentBlock;


    [Header("Punch Effect")]
    [SerializeField] private float punchScale = 1.15f;
    [SerializeField] private float punchDuration = 0.18f;

    private Coroutine punchRoutine;


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


        // ==========================
        // SET ANIMATOR
        // ==========================

        if (animator != null &&
           data.animatorController != null)
        {
            animator.runtimeAnimatorController =
                data.animatorController;
        }



        // ==========================
        // SET SPRITE
        // ==========================

        if (artworkImage != null)
        {
            artworkImage.sprite = data.artwork;


            // Flip hướng enemy
            if (data.flipX)
            {
                artworkImage.rectTransform.localScale =
                    new Vector3(-1, 1, 1);
            }
            else
            {
                artworkImage.rectTransform.localScale =
                    new Vector3(1, 1, 1);
            }
        }



        // ==========================
        // SET NAME
        // ==========================

        if (enemyNameText != null)
        {
            enemyNameText.text =
                data.enemyName;
        }


        RefreshUI();
    }





    private void OnEnable()
    {
        if (enemyHealth != null)
            enemyHealth.OnHealthChanged += UpdateHealthUI;


        if (enemyIntent != null)
            enemyIntent.OnIntentChanged += UpdateIntentUI;


        if (enemyBlock != null)
            enemyBlock.OnBlockChanged += UpdateBlockUI;


        if (enemyStatus != null)
            enemyStatus.OnStatusChanged += UpdateStatusUI;
    }





    private void OnDisable()
    {
        if (enemyHealth != null)
            enemyHealth.OnHealthChanged -= UpdateHealthUI;


        if (enemyIntent != null)
            enemyIntent.OnIntentChanged -= UpdateIntentUI;


        if (enemyBlock != null)
            enemyBlock.OnBlockChanged -= UpdateBlockUI;


        if (enemyStatus != null)
            enemyStatus.OnStatusChanged -= UpdateStatusUI;
    }





    private void UpdateHealthUI(int current, int max)
    {
        RefreshUI();
    }





    private void UpdateIntentUI(
        EnemyIntentType type,
        int value)
    {
        if (intentText == null)
            return;


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


            case EnemyIntentType.Stun:
                intentText.text = "STUN";
                break;


            case EnemyIntentType.Poison:
                intentText.text = $"PSN {value}";
                break;


            case EnemyIntentType.Heal:
                intentText.text = $"HEAL {value}";
                break;


            case EnemyIntentType.LifestealAttack:
                intentText.text = $"ATK {value}";
                break;


            default:
                intentText.text = "";
                break;
        }
    }





    public void Punch()
    {
        if (artworkImage == null) return;
        if (punchRoutine != null) StopCoroutine(punchRoutine);
        punchRoutine = StartCoroutine(PunchRoutine());
    }

    private IEnumerator PunchRoutine()
    {
        RectTransform rt = artworkImage.rectTransform;
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




    private void RefreshUI()
    {
        if (enemyHealth == null ||
           hpText == null)
            return;


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
        if (statusText == null ||
           enemyStatus == null)
            return;


        System.Text.StringBuilder sb = new();



        int strength =
            enemyStatus.GetStatus(StatusType.Strength);

        int weak =
            enemyStatus.GetStatus(StatusType.Weak);

        int vulnerable =
            enemyStatus.GetStatus(StatusType.Vulnerable);



        if (strength > 0)
            sb.Append($"STR+{strength} ");


        if (weak > 0)
            sb.Append($"Weak({weak}) ");


        if (vulnerable > 0)
            sb.Append($"Vuln({vulnerable}) ");



        statusText.text =
            sb.ToString();
    }
}
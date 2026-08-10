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
    [SerializeField] private Image intentIcon;
    [SerializeField] private Sprite attackIntentSprite;
    [SerializeField] private Sprite shieldIntentSprite;
    [SerializeField] private Sprite poisonIntentSprite;
    [SerializeField] private Sprite healIntentSprite;
    [SerializeField] private Sprite buffIntentSprite;



    private EnemyHealth enemyHealth;
    private EnemyIntent enemyIntent;
    private EnemyBlock enemyBlock;
    private EnemyStatus enemyStatus;


    private int currentBlock;


    [Header("Punch Effect")]
    [SerializeField] private float punchScale = 1.15f;
    [SerializeField] private float punchDuration = 0.18f;

    private Coroutine punchRoutine;

    [Header("Lunge Effect")]
    [SerializeField] private float lungeDistance = 220f;
    [SerializeField] private float lungeDuration = 0.3f;

    private Coroutine lungeRoutine;
    private Vector2 baseAnchoredPosition;


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

        Sprite icon = null;
        string label = value.ToString();

        switch (type)
        {
            case EnemyIntentType.Attack:
            case EnemyIntentType.LifestealAttack:
                icon = attackIntentSprite;
                break;

            case EnemyIntentType.Block:
                icon = shieldIntentSprite;
                break;

            case EnemyIntentType.Poison:
                icon = poisonIntentSprite;
                break;

            case EnemyIntentType.Heal:
                icon = healIntentSprite;
                label = $"HEAL {value}";
                break;

            case EnemyIntentType.Buff:
                icon = buffIntentSprite;
                label = "BUF";
                break;

            case EnemyIntentType.Debuff:
                label = "DEB";
                break;

            case EnemyIntentType.Stun:
                label = "STUN";
                break;
        }

        intentText.text = label;

        if (intentIcon != null)
        {
            intentIcon.gameObject.SetActive(icon != null);
            if (icon != null)
                intentIcon.sprite = icon;
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

    public void Lunge(System.Action onReachEnd = null, System.Action onComplete = null)
    {
        RectTransform rt = GetComponent<RectTransform>();
        if (rt == null) return;
        if (lungeRoutine != null) StopCoroutine(lungeRoutine);
        lungeRoutine = StartCoroutine(LungeRoutine(rt, onReachEnd, onComplete));
    }

    private IEnumerator LungeRoutine(RectTransform rt, System.Action onReachEnd, System.Action onComplete)
    {
        baseAnchoredPosition = rt.anchoredPosition;

        int prevStateHash = 0;
        if (animator != null)
            prevStateHash = animator.GetCurrentAnimatorStateInfo(0).shortNameHash;

        string attackState = EnemyData != null ? EnemyData.attackStateName : null;
        if (animator != null && !string.IsNullOrEmpty(attackState))
            animator.Play(attackState, 0, 0f);

        float elapsed = 0f;
        bool endFired = false;

        while (elapsed < lungeDuration)
        {
            float t = elapsed / lungeDuration;
            float factor = Mathf.Sin(t * Mathf.PI);
            rt.anchoredPosition = baseAnchoredPosition + new Vector2(-lungeDistance * factor, 0f);

            if (!endFired && t >= 0.5f)
            {
                endFired = true;
                onReachEnd?.Invoke();
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        rt.anchoredPosition = baseAnchoredPosition;

        onComplete?.Invoke();

        if (animator != null)
        {
            if (enemyHealth != null && enemyHealth.currentHealth > 0)
                animator.Play(prevStateHash, 0, 0f);
        }

        lungeRoutine = null;
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
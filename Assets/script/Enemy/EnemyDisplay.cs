using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemyDisplay : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image artworkImage;

    [SerializeField] private TMP_Text enemyNameText;

    [SerializeField] private TMP_Text hpText;

    public EnemyData EnemyData { get; private set; }

    public void Setup(EnemyData data)
    {
        EnemyData = data;

        artworkImage.sprite = data.artwork;

        enemyNameText.text = data.enemyName;

        hpText.text = $"{data.maxHealth}/{data.maxHealth}";
    }
}
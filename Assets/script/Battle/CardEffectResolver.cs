using UnityEngine;

public class CardEffectResolver : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private PlayerCombat playerCombat;
    [SerializeField] private PlayerBlock playerBlock;
    [SerializeField] private PlayerHealth playerHealth;

    public void Resolve(CardData card, EnemyHealth target)
    {
        if (card == null)
            return;

        switch (card.cardType)
        {
            case CardType.Attack:
                ResolveAttack(card, target);
                break;

            case CardType.Block:
                ResolveBlock(card);
                break;

            case CardType.Heal:
                ResolveHeal(card);
                break;

            default:
                Debug.LogWarning("Card type chưa được xử lý!");
                break;
        }
    }

    private void ResolveAttack(CardData card, EnemyHealth target)
    {
        if (target == null)
        {
            Debug.Log("Chưa chọn enemy!");
            return;
        }

        if (!target.IsAlive())
        {
            Debug.Log("Enemy đã chết!");
            return;
        }

        playerCombat.Attack(
            target,
            card.damage
        );
    }

    private void ResolveBlock(CardData card)
    {
        if (playerBlock == null)
        {
            Debug.LogError("PlayerBlock chưa được gán!");
            return;
        }

        playerBlock.AddBlock(card.block);
    }

    private void ResolveHeal(CardData card)
    {
        if (playerHealth == null)
        {
            Debug.LogError("PlayerHealth chưa được gán!");
            return;
        }

        // Nếu CardData chưa có heal thì đổi dòng này
        // thành giá trị cố định hoặc thêm int heal vào CardData.
        playerHealth.Heal(card.heal);
    }
}
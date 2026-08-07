using UnityEngine;

public class EnemyHitVFX : MonoBehaviour
{
    [SerializeField] private GameObject hitEffect;
    [SerializeField] private Animator animator;

    public void Play()
    {
        if (hitEffect == null) return;

        hitEffect.SetActive(true);

        if (animator != null)
            animator.Play("Hit");

        Invoke(nameof(Hide), 0.3f);
    }

    private void Hide()
    {
        hitEffect.SetActive(false);
    }
}
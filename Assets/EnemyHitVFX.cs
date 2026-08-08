using System.Collections;
using UnityEngine;

public class EnemyHitVFX : MonoBehaviour
{
    [SerializeField] private GameObject hitEffect;
    [SerializeField] private Animator animator;

    public void Play()
    {
        if (hitEffect == null) return;

        CancelInvoke(nameof(Hide));
        hitEffect.SetActive(true);
        StartCoroutine(PlayDelayed());
        Invoke(nameof(Hide), 0.3f);
    }

    private IEnumerator PlayDelayed()
    {
        yield return null;
        if (animator != null)
            animator.Play("Hit", 0, 0f);
    }

    private void Hide()
    {
        hitEffect.SetActive(false);
    }
}
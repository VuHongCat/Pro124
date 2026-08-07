using UnityEngine;

public class DestroyAfterAnimation : MonoBehaviour
{
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (animator != null && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
            Destroy(gameObject);
    }
}

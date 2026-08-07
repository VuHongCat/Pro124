using UnityEngine;

public class SlashEffect : MonoBehaviour
{
    public Animator animator;

    public void Play()
    {
        gameObject.SetActive(true);
        animator.Play("Slash_Attack");
    }

    public void Disable()
    {
        gameObject.SetActive(false);
    }
}
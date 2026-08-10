using UnityEngine;

public class SlashEffect : MonoBehaviour
{
    public Animator animator;

    public void Play()
    {
        gameObject.SetActive(true);
        animator.Play("Slash_Attack");
        AudioManager.PlaySlash();
    }

    public void Disable()
    {
        gameObject.SetActive(false);
    }
}
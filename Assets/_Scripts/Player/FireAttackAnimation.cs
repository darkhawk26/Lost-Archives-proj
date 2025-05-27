using UnityEngine;

public class FireAttackAnimation : MonoBehaviour
{
    public Animator animator;
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void TriggerFireVFX()
    {
        animator.SetTrigger("Cast");
    }
}

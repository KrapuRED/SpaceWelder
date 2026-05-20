using UnityEngine;

public class CaptianRhea : MonoBehaviour
{
    public Animator animator;

    public void TalkingAnimation()
    {
        animator.SetBool("IsTalking", true);
    }

    public void IdleAnimation()
    {
        animator.SetBool("IsTalking", false);

    }

}

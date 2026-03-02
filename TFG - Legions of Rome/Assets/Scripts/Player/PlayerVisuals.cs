using UnityEngine;

public class PlayerVisuals : MonoBehaviour
{
    public Animator[] animators;

    public void UpdateVisuals(Vector2 input)
    {  
        bool isMoving = input.sqrMagnitude > 0.01f;

        foreach (Animator anim in animators)
        {
            if (isMoving)
            {
                anim.SetFloat("inputX", input.x);
                anim.SetFloat("inputY", input.y);
            }

            anim.SetBool("isMoving", isMoving);
        }
    }

    public void TriggerAttack()
    {
        foreach (Animator anim in animators)
        {
            anim.SetTrigger("Attack");
        }
    }
}

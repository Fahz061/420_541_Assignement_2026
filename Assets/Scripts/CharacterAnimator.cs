using UnityEngine;

public class CharacterAnimator : MonoBehaviour
{
    private Animator animator;
    private Rigidbody rb;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        Vector3 velocity = rb.linearVelocity;
        velocity.y = 0f;

        animator.SetFloat("CharacterSpeed",
            velocity.magnitude, 0.1f, Time.deltaTime);
    }
}

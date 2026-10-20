using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    public float walkSpeed = 4f;
    public float runSpeed = 8f;
    public float minJumpForce = 4f;
    public float maxJumpForce = 10f;
    public float doubleJumpForce = 7f;

    public bool canDoubleJump = false;

    private Rigidbody rb;
    private Collider playerCollider;
    private Vector3 movement;

    private bool isGrounded;
    private bool isRunning;
    private bool chargingJump;
    private bool jumpRequested;
    private bool doubleJumpUsed;

    private float jumpStartTime;
    private float pendingJumpForce;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerCollider = GetComponent<Collider>();

        rb.freezeRotation = true;
    }

    void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        // Camera-relative movement
        if (Camera.main != null)
        {
            Transform cam = Camera.main.transform;

            Vector3 forward = cam.forward;
            Vector3 right = cam.right;

            forward.y = 0f;
            right.y = 0f;

            forward.Normalize();
            right.Normalize();

            movement = (forward * vertical + right * horizontal).normalized;
        }
        else
        {
            movement = new Vector3(horizontal, 0, vertical).normalized;
        }

        isRunning = Input.GetKey(KeyCode.LeftShift);

        isGrounded = Physics.Raycast(
            playerCollider.bounds.center,
            Vector3.down,
            playerCollider.bounds.extents.y + 0.2f,
            Physics.DefaultRaycastLayers,
            QueryTriggerInteraction.Ignore
        );

        if (isGrounded)
        {
            doubleJumpUsed = false;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isGrounded)
            {
                chargingJump = true;
                jumpStartTime = Time.time;
            }
            else if (canDoubleJump && !doubleJumpUsed)
            {
                doubleJumpUsed = true;
                pendingJumpForce = doubleJumpForce;
                jumpRequested = true;
            }
        }

        if (chargingJump)
        {
            float holdTime = Time.time - jumpStartTime;

            if (Input.GetKeyUp(KeyCode.Space) || holdTime >= 3f)
            {
                float charge = Mathf.Clamp01(holdTime / 3f);

                pendingJumpForce = Mathf.Lerp(
                    minJumpForce,
                    maxJumpForce,
                    charge
                );

                jumpRequested = true;
                chargingJump = false;
            }
        }
    }

    void FixedUpdate()
    {
        float speed = isRunning ? runSpeed : walkSpeed;

        Vector3 velocity = rb.linearVelocity;
        velocity.x = movement.x * speed;
        velocity.z = movement.z * speed;

        rb.linearVelocity = velocity;

        if (jumpRequested)
        {
            Vector3 currentVelocity = rb.linearVelocity;
            currentVelocity.y = 0f;
            rb.linearVelocity = currentVelocity;

            rb.AddForce(
                Vector3.up * pendingJumpForce,
                ForceMode.Impulse
            );

            jumpRequested = false;
        }
    }
}

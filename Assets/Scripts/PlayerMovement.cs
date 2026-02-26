using UnityEngine;
using UnityEngine.InputSystem;   // New Input System support

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float maxSpeed = 10f;
    public float acceleration = 20f;
    public float airControl = 0.5f;      // How much control in air (if ever needed)

    [Header("Dash")]
    public float dashSpeed = 20f;        // Speed while dashing
    public float dashDuration = 0.2f;    // How long dash lasts
    public float dashCooldown = 1f;      // Cooldown before next dash
    private bool canDash = true;
    private bool isDashing = false;
    private float dashTimeLeft;
    private Vector3 dashDirection;       // Stores which direction we dash in

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    [Header("Animation")]
    public Animator animator;

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;

    // Input state
    private Vector2 moveInput;
    private bool dashPressed;
    private bool usingNewInput;

    // Camera ref for aiming with mouse
    private Camera mainCamera;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        if (animator == null)
            animator = GetComponent<Animator>();
        mainCamera = Camera.main;

        // Detect if new Input System is being used
        usingNewInput = Keyboard.current != null || Gamepad.current != null;
    }

    void Update()
    {
        // ---------- Input ----------
        if (usingNewInput)
        {
            var keyboard = Keyboard.current;
            var gamepad = Gamepad.current;

            moveInput = Vector2.zero;
            if (keyboard != null)
            {
                if (keyboard.aKey.isPressed) moveInput.x = -1f;
                if (keyboard.dKey.isPressed) moveInput.x = 1f;
                if (keyboard.wKey.isPressed) moveInput.y = 1f;
                if (keyboard.sKey.isPressed) moveInput.y = -1f;
            }
            if (gamepad != null)
            {
                Vector2 stick = gamepad.leftStick.ReadValue();
                if (stick.magnitude > 0.1f)
                    moveInput = stick;
            }

            dashPressed = (keyboard != null && keyboard.leftShiftKey.wasPressedThisFrame) ||
                          (gamepad != null && gamepad.rightShoulder.wasPressedThisFrame);
        }
        else
        {
            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");
            moveInput = new Vector2(x, z);
            dashPressed = Input.GetKeyDown(KeyCode.LeftShift);
        }

        // ---------- Ground Check ----------
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        // ---------- Dash Logic ----------
        if (dashPressed && canDash && !isDashing)
        {
            StartDash();
        }

        if (isDashing)
        {
            dashTimeLeft -= Time.deltaTime;
            if (dashTimeLeft <= 0f)
            {
                EndDash();
            }
        }

        // ---------- Rotation (mouse aim) ----------
        RotateToMouse();

        // ---------- Movement ----------
        Vector3 moveDirection = (transform.right * moveInput.x + transform.forward * moveInput.y).normalized;
        Vector3 targetVelocity = moveDirection * maxSpeed;

        if (!isDashing)
        {
            // Handle regular movement acceleration
            float currentAccel = isGrounded ? acceleration : acceleration * airControl;
            Vector3 horizontalVel = new Vector3(velocity.x, 0f, velocity.z);
            horizontalVel = Vector3.MoveTowards(horizontalVel, targetVelocity, currentAccel * Time.deltaTime);
            velocity.x = horizontalVel.x;
            velocity.z = horizontalVel.z;
        }
        else
        {
            // While dashing, lock velocity in chosen direction
            velocity.x = dashDirection.x * dashSpeed;
            velocity.z = dashDirection.z * dashSpeed;
        }

        // ---------- Gravity ----------
        // if (isGrounded && velocity.y < 0f)
        //     velocity.y = -2f;   // Keeps grounded without bouncing
        // velocity.y += Physics.gravity.y * Time.deltaTime;

        // ---------- Apply Movement ----------
        controller.Move(velocity * Time.deltaTime);

        // ---------- Animation ----------
        if (animator != null)
        {
            float speed = new Vector3(velocity.x, 0f, velocity.z).magnitude;
            animator.SetFloat("Speed", speed);
            // animator.SetBool("IsDashing", isDashing); // uncomment if needed
        }
    }

    void RotateToMouse()
    {
        // Raycast from camera to mouse position
        Ray ray = mainCamera.ScreenPointToRay(Mouse.current != null ? Mouse.current.position.ReadValue() : Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, new Vector3(0f, transform.position.y, 0f));

        float rayDistance;
        if (groundPlane.Raycast(ray, out rayDistance))
        {
            Vector3 point = ray.GetPoint(rayDistance);
            Vector3 lookDirection = point - transform.position;
            lookDirection.y = 0f;

            if (lookDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
                transform.rotation = targetRotation;
            }
        }
    }

    void StartDash()
    {
        isDashing = true;
        canDash = false;
        dashTimeLeft = dashDuration;

        // Figure out which direction to dash in
        Vector3 moveDir = (transform.right * moveInput.x + transform.forward * moveInput.y).normalized;

        // If we’re not moving, dash forward
        if (moveDir.magnitude < 0.1f)
            dashDirection = transform.forward;
        else
            dashDirection = moveDir;

        dashDirection.Normalize();
        // Could add dash VFX or sound here
    }

    void EndDash()
    {
        isDashing = false;
        Invoke(nameof(ResetDash), dashCooldown);
    }

    void ResetDash()
    {
        canDash = true;
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(groundCheck.position, groundDistance);
        }
    }
}
using UnityEngine;
using UnityEngine.InputSystem;   // If using new Input System, otherwise comment out

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float maxSpeed = 10f;
    public float acceleration = 20f;
    public float airControl = 0.5f;      // Still useful if you ever fall

    [Header("Dash")]
    public float dashSpeed = 20f;         // Speed during dash
    public float dashDuration = 0.2f;     // How long the dash lasts
    public float dashCooldown = 1f;       // Time before you can dash again
    private bool canDash = true;
    private bool isDashing = false;
    private float dashTimeLeft;

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
    private bool dashPressed;     // For new Input System polling
    private bool usingNewInput;   // Flag to decide which input method to use

    // Camera reference for mouse aiming
    private Camera mainCamera;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        if (animator == null)
            animator = GetComponent<Animator>();
        mainCamera = Camera.main;   // Assumes your camera is tagged "MainCamera"

        // Detect if new Input System is active (optional)
        usingNewInput = Keyboard.current != null || Gamepad.current != null;
    }

    void Update()
    {
        // ---------- Input Handling ----------
        if (usingNewInput)
        {
            // New Input System (keyboard + gamepad)
            var keyboard = Keyboard.current;
            var gamepad = Gamepad.current;

            // Movement
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

            // Dash input (using Left Shift or gamepad button)
            dashPressed = (keyboard != null && keyboard.leftShiftKey.wasPressedThisFrame) ||
                          (gamepad != null && gamepad.rightShoulder.wasPressedThisFrame);  // Example: RB
        }
        else
        {
            // Legacy Input (if Active Input Handling = Both)
            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");
            moveInput = new Vector2(x, z);
            dashPressed = Input.GetKeyDown(KeyCode.LeftShift);   // Change to KeyCode.Space if you prefer
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

        // ---------- Mouse Rotation (Top‑Down Aim) ----------
        RotateToMouse();

        // ---------- Horizontal Movement (accelerate / decelerate) ----------
        Vector3 moveDirection = (transform.right * moveInput.x + transform.forward * moveInput.y).normalized;
        Vector3 targetVelocity = moveDirection * maxSpeed;

        // During dash we override velocity with dash speed in facing direction
        if (isDashing)
        {
            // Dash forward (the direction the player is facing)
            targetVelocity = transform.forward * dashSpeed;
            // Ignore input during dash – we set velocity directly later
        }

        // Apply acceleration only if not dashing (dash uses instant speed)
        if (!isDashing)
        {
            float currentAccel = isGrounded ? acceleration : acceleration * airControl;
            Vector3 horizontalVel = new Vector3(velocity.x, 0f, velocity.z);
            horizontalVel = Vector3.MoveTowards(horizontalVel, targetVelocity, currentAccel * Time.deltaTime);
            velocity.x = horizontalVel.x;
            velocity.z = horizontalVel.z;
        }
        else
        {
            // During dash, set velocity directly to dash speed forward
            velocity.x = transform.forward.x * dashSpeed;
            velocity.z = transform.forward.z * dashSpeed;
        }

        // ---------- Apply Gravity (optional if you want the player to stay on ground) ----------
        // Since we're top‑down, you might not need gravity at all.
        // But if you have slopes or want the player to stick to ground, keep it.
        if (isGrounded && velocity.y < 0f)
            velocity.y = -2f;
        // If you want NO vertical movement, set velocity.y = 0 and skip gravity.
        velocity.y += Physics.gravity.y * Time.deltaTime;   // Uses default gravity

        // ---------- Move the Character ----------
        controller.Move(velocity * Time.deltaTime);

        // ---------- Animation ----------
        if (animator != null)
        {
            float speed = new Vector3(velocity.x, 0f, velocity.z).magnitude;
            animator.SetFloat("Speed", speed);
            // Optionally set a "Dashing" bool if you have a dashing animation
            // animator.SetBool("IsDashing", isDashing);
        }
    }

    void RotateToMouse()
    {
        // Create a ray from the camera through the mouse cursor
        Ray ray = mainCamera.ScreenPointToRay(Mouse.current != null ? Mouse.current.position.ReadValue() : Input.mousePosition);
        // Define a plane at the player's height (y = 0, or use transform.position.y)
        Plane groundPlane = new Plane(Vector3.up, new Vector3(0f, transform.position.y, 0f));

        float rayDistance;
        if (groundPlane.Raycast(ray, out rayDistance))
        {
            Vector3 point = ray.GetPoint(rayDistance);
            // Look at the point on the ground
            Vector3 lookDirection = point - transform.position;
            lookDirection.y = 0f;   // Keep rotation only around Y axis

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
        // Optional: play a dash effect or sound
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
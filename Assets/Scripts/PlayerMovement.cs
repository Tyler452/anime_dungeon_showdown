using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float maxSpeed = 10f;
    public float acceleration = 20f;
    public float airControl = 0.5f;

    [Header("Dash")]
    public float dashSpeed = 20f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;
    public AudioClip dashSFX;
    public GameObject dashVFX;

    [Header("Knockback")]
    public float knockbackDecay = 20f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    [Header("Gravity & Jump")]
    public float gravity = -20f;
    public float jumpHeight = 2f;

    [Header("Animation")]
    public Animator animator;

    private CharacterController controller;
    private AudioSource audioSource;
    private Camera mainCamera;

    private Vector3 velocity;
    private Vector3 externalVelocity;

    private bool isGrounded;
    private bool canDash = true;
    private bool isDashing = false;
    private float dashTimeLeft;
    private Vector3 dashDirection;

    private Vector2 moveInput;
    private bool dashPressed;
    private bool usingNewInput;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        controller.stepOffset = 0.1f;

        if (animator == null)
            animator = GetComponent<Animator>();

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        mainCamera = Camera.main;
        usingNewInput = Keyboard.current != null || Gamepad.current != null;
    }

    void Update()
    {
        ReadInput();
        CheckGrounded();
        HandleDash();
        RotateToMouse();
        HandleMovement();
        ApplyGravity();
        ApplyFinalMovement();
        UpdateAnimation();
    }

    void CheckGrounded()
    {
        isGrounded = groundCheck != null &&
                     Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;
    }

    void HandleDash()
    {
        if (dashPressed && canDash && !isDashing)
            StartDash();

        if (isDashing)
        {
            dashTimeLeft -= Time.deltaTime;
            if (dashTimeLeft <= 0f)
                EndDash();
        }
    }

    void HandleMovement()
    {
        if (isDashing) return;

        Vector3 moveDirection = (transform.right * moveInput.x + transform.forward * moveInput.y).normalized;
        Vector3 targetVelocity = moveDirection * maxSpeed;

        float currentAccel = isGrounded ? acceleration : acceleration * airControl;
        Vector3 horizontalVel = new Vector3(velocity.x, 0f, velocity.z);
        horizontalVel = Vector3.MoveTowards(horizontalVel, targetVelocity, currentAccel * Time.deltaTime);
        velocity.x = horizontalVel.x;
        velocity.z = horizontalVel.z;
    }

    void ApplyGravity()
    {
        if (!isDashing)
        {
            velocity.y += gravity * Time.deltaTime;
        }
    }

    void ApplyFinalMovement()
    {
        externalVelocity = Vector3.MoveTowards(externalVelocity, Vector3.zero, knockbackDecay * Time.deltaTime);
        Vector3 finalVelocity = velocity + externalVelocity;
        controller.Move(finalVelocity * Time.deltaTime);
    }

    void UpdateAnimation()
    {
        if (animator != null)
        {
            float speed = new Vector3(velocity.x, 0f, velocity.z).magnitude;
            animator.SetFloat("Speed", speed);
            animator.SetBool("IsGrounded", isGrounded);
        }
    }

    void ReadInput()
    {
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
            float x = Input.GetAxisRaw("Horizontal");
            float z = Input.GetAxisRaw("Vertical");
            moveInput = new Vector2(x, z);
            dashPressed = Input.GetKeyDown(KeyCode.LeftShift);
        }
    }

    void RotateToMouse()
    {
        Vector3 lookDirection = GetAimDirection();
        if (lookDirection.sqrMagnitude > 0.0001f)
        {
            transform.rotation = Quaternion.LookRotation(lookDirection);
        }
    }

    public Vector3 GetAimDirection()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        if (mainCamera == null)
            return transform.forward;

        Vector2 mousePosition = Mouse.current != null
            ? Mouse.current.position.ReadValue()
            : (Vector2)Input.mousePosition;

        Ray ray = mainCamera.ScreenPointToRay(mousePosition);
        Plane groundPlane = new Plane(Vector3.up, new Vector3(0f, transform.position.y, 0f));

        if (groundPlane.Raycast(ray, out float rayDistance))
        {
            Vector3 point = ray.GetPoint(rayDistance);
            Vector3 lookDirection = point - transform.position;
            lookDirection.y = 0f;

            if (lookDirection.sqrMagnitude > 0.0001f)
                return lookDirection.normalized;
        }

        return transform.forward;
    }

    void StartDash()
    {
        isDashing = true;
        canDash = false;
        dashTimeLeft = dashDuration;

        Vector3 moveDir = (transform.right * moveInput.x + transform.forward * moveInput.y).normalized;
        dashDirection = moveDir.sqrMagnitude < 0.01f ? transform.forward : moveDir.normalized;

        if (dashSFX != null)
            audioSource.PlayOneShot(dashSFX);

        if (dashVFX != null)
        {
            GameObject vfx = Instantiate(dashVFX, transform.position, transform.rotation);
            Destroy(vfx, 2f);
        }
    }

    void EndDash()
    {
        isDashing = false;
        Invoke(nameof(ResetDash), dashCooldown);
    }

    void ResetDash() => canDash = true;

    public void AddExternalForce(Vector3 force) => externalVelocity += force;

    public bool IsDashing() => isDashing;

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(groundCheck.position, groundDistance);
        }
    }
}
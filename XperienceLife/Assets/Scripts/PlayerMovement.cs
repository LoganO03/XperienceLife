using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D playerRB;

    [SerializeField] private int maxSpeed = 5;

    Vector2 moveInput;
    Vector2 lastMoveInput = Vector2.right;

    InputSystem_Actions controls;

    [Header("Aim Indicator")]
    [SerializeField] Transform aimIndicator;
    [SerializeField] float aimDistance = 0.7f;

    [SerializeField] private PlayerMeleeAttack meleeAttack;


    public Vector2 AimDirection => lastMoveInput.normalized;

    private void Awake()
    {
        playerRB = GetComponent<Rigidbody2D>();
        controls = new InputSystem_Actions();

        if (meleeAttack == null)
        meleeAttack = GetComponent<PlayerMeleeAttack>();
    }

    void OnEnable()
    {
        // Enable the actions and subscribe to the Move action callbacks
        controls.Enable();
        controls.Player.Move.performed += OnMove;
        controls.Player.Move.canceled += OnMove;
        controls.Player.Attack.performed += OnAttack;
    }

    void OnDisable()
    {
        // Unsubscribe and disable to avoid memory leaks
        controls.Player.Move.performed -= OnMove;
        controls.Player.Move.canceled -= OnMove;
        controls.Player.Attack.performed -= OnAttack;
        controls.Disable();
    }


    private void OnMove(InputAction.CallbackContext ctx)
    {
        moveInput = ctx.ReadValue<Vector2>();

        if (moveInput.sqrMagnitude > 0.001f)
        {
            lastMoveInput = moveInput;
        }
    }

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();

        if (moveInput.sqrMagnitude > 0.001f)
        {
            lastMoveInput = moveInput;
        }
    }

    private void OnAttack(InputAction.CallbackContext ctx)
    {
        if (meleeAttack != null)
            meleeAttack.PerformAttack();
    }


    private void FixedUpdate()
    {
        // KEEPING LINEAR VELOCITY AS YOU REQUESTED
        playerRB.linearVelocity = moveInput * maxSpeed;
    }

    private void Update()
    {
        UpdateAimIndicator();
    }

    private void UpdateAimIndicator()
    {
        if (aimIndicator == null || lastMoveInput.sqrMagnitude < 0.001f)
            return;

        Vector2 dir = lastMoveInput.normalized;

        // Position the triangle *around* the player
        aimIndicator.localPosition = dir * aimDistance;

        // Rotate the triangle to face movement direction
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        aimIndicator.localRotation = Quaternion.Euler(0f, 0f, angle - 90f);
    }
}

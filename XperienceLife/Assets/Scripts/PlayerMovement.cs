using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    Rigidbody2D playerRB;

    [SerializeField] int maxSpeed = 5;

    // Stores current movement from input system (X = horizontal, Y = vertical)
    Vector2 moveInput;

    // Generated input actions class (auto-generated from InputSystem_Actions.inputactions)
    InputSystem_Actions controls;

    void Awake()
    {
        playerRB = GetComponent<Rigidbody2D>();
        controls = new InputSystem_Actions();
    }

    void OnEnable()
    {
        // Enable the actions and subscribe to the Move action callbacks
        controls.Enable();
        controls.Player.Move.performed += OnMove;
        controls.Player.Move.canceled += OnMove;
    }

    void OnDisable()
    {
        // Unsubscribe and disable to avoid memory leaks
        controls.Player.Move.performed -= OnMove;
        controls.Player.Move.canceled -= OnMove;
        controls.Disable();
    }

    // Called by the generated InputSystem_Actions when Move is performed or canceled
    private void OnMove(InputAction.CallbackContext ctx)
    {
        moveInput = ctx.ReadValue<Vector2>();
    }

    // Also support PlayerInput "Send Messages" / Unity Events pattern by providing this method.
    // If you add a PlayerInput component and set Behavior to "Send Messages", it will call OnMove with an InputValue.
    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    private void FixedUpdate()
    {
        // Apply movement to the Rigidbody2D. Keep original property name in case the project expects it.
        // If your project uses `velocity` instead of `linearVelocity` change this line accordingly.
        playerRB.linearVelocity = moveInput * maxSpeed;
    }
}
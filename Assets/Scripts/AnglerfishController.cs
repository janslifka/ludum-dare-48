using UnityEngine;
using UnityEngine.InputSystem;

public class AnglerfishController : MonoBehaviour
{
    [Header("Movement")] [SerializeField] float movementSpeed;
    [SerializeField] float lerpFastConst;
    [SerializeField] float lerpSlowConst;
    [Header("Dash")] [SerializeField] float dashSpeed;
    [SerializeField] float dashDuration;

    InputActions _inputActions;
    Rigidbody2D _rigidbody;
    Vector2 _movement;
    bool _isDashing;

    void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();

        _inputActions = new InputActions();
        _inputActions.Enable();
        _inputActions.Player.Move.performed += Move;
        _inputActions.Player.Move.canceled += Move;
        _inputActions.Player.Dash.performed += Dash;
        _inputActions.Player.Light.performed += LightOn;
        _inputActions.Player.Light.canceled += LightOff;
    }

    void Dash(InputAction.CallbackContext ctx)
    {
        var worldPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        var direction = (worldPos - transform.position).normalized;

        _rigidbody.velocity = direction * dashSpeed;
        _isDashing = true;

        Invoke(nameof(StopDash), dashDuration);
    }

    void StopDash()
    {
        _rigidbody.velocity = _rigidbody.velocity.normalized * movementSpeed;
        _isDashing = false;
    }

    void LightOn(InputAction.CallbackContext ctx)
    {
        Debug.Log("light on");
    }

    void LightOff(InputAction.CallbackContext ctx)
    {
        Debug.Log("light off");
    }

    void Move(InputAction.CallbackContext ctx)
    {
        _movement = ctx.ReadValue<Vector2>() * movementSpeed;
    }

    void FixedUpdate()
    {
        if (_isDashing) return;

        var x = LerpMove(_rigidbody.velocity.x, _movement.x);
        var y = LerpMove(_rigidbody.velocity.y, _movement.y);

        _rigidbody.velocity = new Vector2(x, y);
    }

    float LerpMove(float oldMove, float newMove)
    {
        return Mathf.Approximately(newMove, 0)
            ? LerpSlow(oldMove, newMove)
            : LerpFast(oldMove, newMove);
    }

    float LerpFast(float a, float b)
    {
        return Mathf.Lerp(a, b, lerpFastConst);
    }

    float LerpSlow(float a, float b)
    {
        return Mathf.Lerp(a, b, lerpSlowConst);
    }
}
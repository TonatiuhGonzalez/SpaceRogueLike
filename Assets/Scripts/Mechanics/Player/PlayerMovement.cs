using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private InputReader _inputReader;

    private Rigidbody2D _rb;
    private float _moveSpeed;
    private bool _movementEnabled = true;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    public void Initialize(float moveSpeed)
    {
        _moveSpeed = moveSpeed;
    }

    public void SetMovementEnabled(bool enabled)
    {
        _movementEnabled = enabled;
        if (!enabled && _rb != null)
            _rb.linearVelocity = Vector2.zero;
    }

    private void FixedUpdate()
    {
        if (!_movementEnabled) return;
        _rb.linearVelocity = _inputReader.MoveInput * _moveSpeed;
    }
}

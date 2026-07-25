using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private InputReader _inputReader;
    [SerializeField] private BoxCollider2D _mapBounds;

    private Rigidbody2D _rb;
    private Collider2D _collider;
    private float _moveSpeed;
    private bool _movementEnabled = true;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _collider = GetComponent<Collider2D>();
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

        Vector2 velocity = _inputReader.MoveInput * _moveSpeed;

        if (_mapBounds != null)
        {
            Vector2 nextPosition = _rb.position + velocity * Time.fixedDeltaTime;
            Vector2 clampedPosition = ClampToBounds(nextPosition);
            if (clampedPosition != nextPosition)
                velocity = (clampedPosition - _rb.position) / Time.fixedDeltaTime;
        }

        _rb.linearVelocity = velocity;
    }

    private Vector2 ClampToBounds(Vector2 position)
    {
        Bounds bounds = _mapBounds.bounds;
        Vector2 extents = _collider != null ? (Vector2)_collider.bounds.extents : Vector2.zero;

        return new Vector2(
            Mathf.Clamp(position.x, bounds.min.x + extents.x, bounds.max.x - extents.x),
            Mathf.Clamp(position.y, bounds.min.y + extents.y, bounds.max.y - extents.y));
    }
}

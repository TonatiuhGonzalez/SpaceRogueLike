using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyMovement : MonoBehaviour
{
    private Rigidbody2D _rb;
    private float _moveSpeed;
    private BoxCollider2D _mapBounds;

    public Vector2 Position => _rb.position;
    public float FormationAngle { get; set; }

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    public void Initialize(float moveSpeed, BoxCollider2D mapBounds = null)
    {
        _moveSpeed = moveSpeed;
        _mapBounds = mapBounds;
    }

    public void SetVelocity(Vector2 velocity)
    {
        _rb.linearVelocity = velocity;
    }

    public void MoveToward(Vector2 target)
    {
        if (_mapBounds != null)
            target = ClampToBounds(target);

        Vector2 direction = (target - _rb.position).normalized;
        _rb.linearVelocity = direction * _moveSpeed;
    }

    private Vector2 ClampToBounds(Vector2 position)
    {
        Bounds bounds = _mapBounds.bounds;
        return new Vector2(
            Mathf.Clamp(position.x, bounds.min.x, bounds.max.x),
            Mathf.Clamp(position.y, bounds.min.y, bounds.max.y));
    }

    public void Stop()
    {
        _rb.linearVelocity = Vector2.zero;
    }

    public void TeleportTo(Vector2 position)
    {
        _rb.position = position;
    }
}

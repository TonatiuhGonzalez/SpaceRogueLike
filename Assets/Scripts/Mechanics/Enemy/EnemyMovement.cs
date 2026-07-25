using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyMovement : MonoBehaviour
{
    private Rigidbody2D _rb;
    private float _moveSpeed;

    public Vector2 Position => _rb.position;
    public float FormationAngle { get; set; }

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    public void Initialize(float moveSpeed)
    {
        _moveSpeed = moveSpeed;
    }

    public void SetVelocity(Vector2 velocity)
    {
        _rb.linearVelocity = velocity;
    }

    public void MoveToward(Vector2 target)
    {
        Vector2 direction = (target - _rb.position).normalized;
        _rb.linearVelocity = direction * _moveSpeed;
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

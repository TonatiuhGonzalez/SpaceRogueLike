using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private InputReader _inputReader;

    private Rigidbody2D _rb;
    private float _moveSpeed;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    public void Initialize(float moveSpeed)
    {
        _moveSpeed = moveSpeed;
    }

    private void FixedUpdate()
    {
        _rb.linearVelocity = _inputReader.MoveInput * _moveSpeed;
    }
}

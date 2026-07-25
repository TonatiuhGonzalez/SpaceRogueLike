using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class InputReader : MonoBehaviour
{
    public Vector2 MoveInput { get; private set; }
    public Vector2 AimInput { get; private set; }

    public event Action<Vector2> OnMoveChanged;
    public event Action<Vector2> OnAimChanged;

    private void OnMove(InputValue value)
    {
        MoveInput = value.Get<Vector2>();
        OnMoveChanged?.Invoke(MoveInput);
    }

    private void OnAim(InputValue value)
    {
        AimInput = value.Get<Vector2>();
        OnAimChanged?.Invoke(AimInput);
    }

    public void ResetInput()
    {
        MoveInput = Vector2.zero;
        AimInput = Vector2.zero;
    }
}

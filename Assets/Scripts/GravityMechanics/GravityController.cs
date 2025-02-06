using System;
using UnityEngine;

public enum GravityDirection
{
    Up = 0,
    Right = 1,
    Down = 2,
    Left = 3
}

public class GravityController : MonoBehaviour
{
    public static GravityController Instance { get; private set; }

    public event Action<GravityDirection> OnGravityChangeStarted;
    public event Action<GravityDirection> OnGravityChangeFinished;

    public bool IsActiveRotate { get; private set; }

    public GravityDirection CurrentGravity => _currentGravity;

    [SerializeField]
    private GravityRotator _gravityRotator;

    [SerializeField]
    private BoulderMover _boulderMover;

    private GravityDirection _currentGravity = GravityDirection.Down;

    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        _gravityRotator.OnRotationCompleted += OnGravityChangeComplete;
    }

    private void OnGravityChangeComplete()
    {
        OnGravityChangeFinished?.Invoke(_currentGravity);
        IsActiveRotate = false;
    }

    public void RotateLeft()
    {
        SetNewGravity((GravityDirection)(((int)_currentGravity + 3) % 4));
    }

    public void RotateRight()
    {
        SetNewGravity((GravityDirection)(((int)_currentGravity + 1) % 4));
    }

    public void Rotate180()
    {
        SetNewGravity((GravityDirection)(((int)_currentGravity + 2) % 4));
    }

    private void SetNewGravity(GravityDirection newGravity)
    {
        if (IsActiveRotate)
            return;

        if (_boulderMover)
        {
            if (_boulderMover.BoulderIsFalling)
                return;
        }

        IsActiveRotate = true;
        _currentGravity = newGravity;
        _gravityRotator.RotateWorld(newGravity);
        OnGravityChangeStarted?.Invoke(_currentGravity);
    }

    private void OnDisable()
    {
        _gravityRotator.OnRotationCompleted -= OnGravityChangeComplete;
    }
}

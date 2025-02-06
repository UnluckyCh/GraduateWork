using System;
using UnityEngine;

public class RotatorUnifier : MonoBehaviour
{
    public event Action OnStartDisappear;
    public event Action OnStartAppear;

    [SerializeField]
    private BoulderMover _boulderMover;

    private void Start()
    {
        if (!_boulderMover)
            Debug.LogError($"Не указан объект BoulderMover у {name}");

        _boulderMover.OnBoulderFalled += StartAppear;
    }

    public void StartDisappear()
    {
        OnStartDisappear?.Invoke();
    }

    public void StartAppear()
    {
        OnStartAppear?.Invoke();
    }

    private void OnDisable()
    {
        _boulderMover.OnBoulderFalled -= StartAppear;
    }
}

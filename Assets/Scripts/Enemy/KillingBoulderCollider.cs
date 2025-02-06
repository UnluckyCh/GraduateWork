using System.Collections.Generic;
using UnityEngine;

public class KillingBoulderCollider : MonoBehaviour
{
    [SerializeField]
    private PlayerMovement _playerMovement;

    [SerializeField]
    private GravityDirection _colliderDirection;

    private BoulderMover _boulderMover;
    private readonly HashSet<SimplePlayerController> _player = new();
    private bool _isActiveCollider = false;

    private void Start()
    {
        GravityController.Instance.OnGravityChangeFinished += OnGravityChangeFinished;
        _boulderMover = GetComponentInParent<BoulderMover>();
        _boulderMover.OnBoulderFalled += OnBoulderFalled;
    }

    private void OnGravityChangeFinished(GravityDirection currentGravity)
    {
        _isActiveCollider = (currentGravity == _colliderDirection);
    }

    private void OnBoulderFalled()
    {
        _isActiveCollider = false;
    }

    private void Update()
    {
        if (_player.Count <= 0)
            return;

        if (!_playerMovement.IsGrounded)
            return;

        KillPlayer();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!_isActiveCollider) return;

        collision.TryGetComponent<SimplePlayerController>(out var player);

        if (!player) return;

        _player.Add(player);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!_isActiveCollider) return;

        collision.TryGetComponent<SimplePlayerController>(out var player);

        if (!player) return;

        _player.Remove(player);
    }

    private void KillPlayer()
    {
        foreach (var player in _player)
        {
            player.TakeDamage(1000);
            player.Rb.simulated = false;
        }
    }

    private void OnDisable()
    {
        GravityController.Instance.OnGravityChangeFinished -= OnGravityChangeFinished;
    }
}

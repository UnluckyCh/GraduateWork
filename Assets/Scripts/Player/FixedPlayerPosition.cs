using UnityEngine;

public class FixedPlayerPosition : MonoBehaviour
{
    [SerializeField] private Transform _player;
    [SerializeField] private Transform _location;
    [SerializeField] private float _verticalOffset = 1.75f;
    [SerializeField] private Rigidbody2D _rigidbodyPlayer;

    private Vector3 _localAnchorPosition;

    private bool _isRotating = false;

    private void Start()
    {
        GravityController.Instance.OnGravityChangeStarted += HandleGravityChangeStarted;
        GravityController.Instance.OnGravityChangeFinished += HandleGravityChangeFinished;
    }

    private void OnDisable()
    {
        if (GravityController.Instance != null)
        {
            GravityController.Instance.OnGravityChangeStarted -= HandleGravityChangeStarted;
            GravityController.Instance.OnGravityChangeFinished -= HandleGravityChangeFinished;
        }
    }

    private void HandleGravityChangeStarted(GravityDirection newGravity)
    {
        Vector3 anchorWorldPosition = _player.position + Vector3.up * _verticalOffset;
        _localAnchorPosition = _location.InverseTransformPoint(anchorWorldPosition);
        _isRotating = true;
    }

    private void HandleGravityChangeFinished(GravityDirection newGravity)
    {
        _isRotating = false;
        Vector3 newAnchorWorldPosition = _location.TransformPoint(_localAnchorPosition);
        _rigidbodyPlayer.velocity = Vector2.zero;
        _player.position = newAnchorWorldPosition - Vector3.up * _verticalOffset;
    }

    private void LateUpdate()
    {
        if (_isRotating)
        {
            Vector3 newAnchorWorldPosition = _location.TransformPoint(_localAnchorPosition);
            _player.position = newAnchorWorldPosition - Vector3.up * _verticalOffset;
        }
    }
}

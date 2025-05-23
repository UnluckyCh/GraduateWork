using UnityEngine;

public class TriggerColliderByGravity : MonoBehaviour
{
    [SerializeField] private GravityDirection[] _allowedGravities;
    [SerializeField] private Collider2D _collider;

    public GravityDirection ActiveGravity { get; private set; }

    private void Start()
    {
        GravityController.Instance.OnGravityChangeFinished += HandleGravityChanged;
        UpdateColliderState(GravityController.Instance.CurrentGravity);
    }

    private void OnDisable()
    {
        if (GravityController.Instance != null)
        {
            GravityController.Instance.OnGravityChangeFinished -= HandleGravityChanged;
        }
    }

    private void HandleGravityChanged(GravityDirection newGravity)
    {
        UpdateColliderState(newGravity);
    }

    private void UpdateColliderState(GravityDirection currentGravity)
    {
        ActiveGravity = currentGravity;
        _collider.enabled = System.Array.Exists(_allowedGravities, dir => dir == currentGravity);
    }
}

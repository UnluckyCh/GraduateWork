using UnityEngine;

public class CameraFollowDisabler : MonoBehaviour
{
    [SerializeField]
    private CameraFreeFollow _cameraFollow;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (_cameraFollow != null)
            {
                _cameraFollow.StopFollowingPlayer();
            }
        }
    }
}

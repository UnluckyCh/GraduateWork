using UnityEngine;

public class CameraFollowDisabler : MonoBehaviour
{
    [SerializeField]
    private CameraFreeFollow _cameraFollow;

    [SerializeField]
    private SimplePlayerController _player;

    private void Start()
    {
        if (!_cameraFollow)
        {
            Debug.LogWarning("������ �� ������, ����������� �����...");
            _cameraFollow = FindObjectOfType<CameraFreeFollow>();
            if (!_cameraFollow)
            {
                Debug.LogError("CameraFreeFollow �� ������ �� �����!");
            }
        }

        if (!_player)
        {
            Debug.LogWarning("����� �� �����, ����������� �����...");
            _player = FindObjectOfType<SimplePlayerController>();
            if (!_player)
            {
                Debug.LogError("SimplePlayerController �� ������ �� �����!");
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (_cameraFollow && _player)
            {
                _cameraFollow.StopFollowingPlayer();
                _player.TakeDamage(100);
            }
        }
    }
}

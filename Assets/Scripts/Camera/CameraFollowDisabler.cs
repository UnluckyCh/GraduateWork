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
            Debug.LogWarning("Камера не задана, выполняется поиск...");
            _cameraFollow = FindObjectOfType<CameraFreeFollow>();
            if (!_cameraFollow)
            {
                Debug.LogError("CameraFreeFollow не найден на сцене!");
            }
        }

        if (!_player)
        {
            Debug.LogWarning("Игрок не задан, выполняется поиск...");
            _player = FindObjectOfType<SimplePlayerController>();
            if (!_player)
            {
                Debug.LogError("SimplePlayerController не найден на сцене!");
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

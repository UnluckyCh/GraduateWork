using UnityEngine;

public class SoundRotatorPlayer : MonoBehaviour
{
    [SerializeField] private RotatorUnifier _rotatorUnifier;
    [SerializeField] private float _minPitch = 0.8f;
    [SerializeField] private float _maxPitch = 1.15f;

    private AudioSource _audioSource;
    private float _originalPitch;

    private void Awake()
    {
        if (!_rotatorUnifier)
        {
            Debug.LogError($"RotatorUnifier не назначен в {name}");
        }

        if (!TryGetComponent(out _audioSource))
        {
            Debug.LogError($"AudioSource не найден на {name}");
            return;
        }

        _originalPitch = _audioSource.pitch;

        _rotatorUnifier.OnStartDisappear += PlaySoundWithRandomPitch;
    }

    private void OnDestroy()
    {
        if (_rotatorUnifier)
        {
            _rotatorUnifier.OnStartDisappear -= PlaySoundWithRandomPitch;
        }
    }

    private void PlaySoundWithRandomPitch()
    {
        if (!_audioSource) return;

        _audioSource.pitch = _originalPitch * Random.Range(_minPitch, _maxPitch);
        _audioSource.Play();
    }
}

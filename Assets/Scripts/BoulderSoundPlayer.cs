using UnityEngine;
using System;

[RequireComponent(typeof(AudioSource))]
public class BoulderSoundPlayer : MonoBehaviour
{
    [SerializeField] private BoulderMover _boulderMover;

    [Header("Диапазон дистанции")]
    [SerializeField] private float _minDistance = 0.01f;
    [SerializeField] private float _maxDistance = 40f;

    [Header("Множитель громкости")]
    [SerializeField] private float _minVolumeMultiplier = 0.7f;
    [SerializeField] private float _maxVolumeMultiplier = 1.1f;

    [Header("Питч")]
    [SerializeField] private float _minPitch = 0.95f;
    [SerializeField] private float _maxPitch = 1.05f;

    private AudioSource _audioSource;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        if (_boulderMover != null)
        {
            _boulderMover.OnBoulderFlewDistance += PlayScaledSound;
        }
    }

    private void OnDisable()
    {
        if (_boulderMover != null)
        {
            _boulderMover.OnBoulderFlewDistance -= PlayScaledSound;
        }
    }

    private void PlayScaledSound(float distance)
    {
        float clampedDistance = Mathf.Clamp(distance, _minDistance, _maxDistance);
        float t = Mathf.InverseLerp(_minDistance, _maxDistance, clampedDistance);
        float volumeMultiplier = Mathf.Lerp(_minVolumeMultiplier, _maxVolumeMultiplier, t);

        float originalVolume = _audioSource.volume;
        float randomPitch = UnityEngine.Random.Range(_minPitch, _maxPitch);

        StartCoroutine(PlayWithTemporarySettings(originalVolume, volumeMultiplier, randomPitch));
    }

    private System.Collections.IEnumerator PlayWithTemporarySettings(float originalVolume, float volumeMultiplier, float pitch)
    {
        _audioSource.volume = originalVolume * volumeMultiplier;
        _audioSource.pitch = pitch;
        _audioSource.Play();

        yield return new WaitForSeconds(_audioSource.clip.length);

        _audioSource.volume = originalVolume;
        _audioSource.pitch = 1f;
    }
}

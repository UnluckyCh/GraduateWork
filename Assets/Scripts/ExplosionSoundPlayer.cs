using UnityEngine;

public class ExplosionSoundPlayer : MonoBehaviour
{
    public static ExplosionSoundPlayer Instance { get; private set; }

    [SerializeField] private AudioClip _explosionClip;
    [SerializeField] private float _volume = 1f;

    private const string SfxVolumeKey = "SfxVolume_0_50"; // עמע זו ךכ‏ק, קעמ ג UI

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void PlayExplosion(Vector3 position)
    {
        GameObject tempGO = new GameObject("TempExplosionSound");
        tempGO.transform.position = position;

        AudioSource audioSource = tempGO.AddComponent<AudioSource>();
        audioSource.clip = _explosionClip;

        int savedVolume = PlayerPrefs.GetInt(SfxVolumeKey, 25);
        float userVolume = savedVolume / 50f;
        audioSource.volume = _volume * userVolume;

        audioSource.Play();
        Destroy(tempGO, _explosionClip.length);
    }

}

using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class RandomPitchPerSourceButton : MonoBehaviour
{
    [System.Serializable]
    public class SoundData
    {
        public AudioSource source;
        public float minPitch = 1.4f;
        public float maxPitch = 1.8f;
    }

    [SerializeField] private SoundData[] _sounds;

    private void Awake()
    {
        if (_sounds == null || _sounds.Length == 0)
        {
            Debug.LogError("Не заданы звуки!", this);
        }

        GetComponent<Button>().onClick.AddListener(PlayRandomSound);
    }

    private void PlayRandomSound()
    {
        if (_sounds == null || _sounds.Length == 0) { return; }

        SoundData selected = _sounds[Random.Range(0, _sounds.Length)];
        if (!selected.source) { return; }

        float pitch = Random.Range(selected.minPitch, selected.maxPitch);
        selected.source.pitch = pitch;
        selected.source.Play();
    }
}

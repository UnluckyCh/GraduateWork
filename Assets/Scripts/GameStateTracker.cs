using UnityEngine;

public class GameStateTracker : MonoBehaviour
{
    public static GameStateTracker Instance { get; private set; }

    [Header("��������� ������")]
    [SerializeField] private AudioVolumeController _audioVolumeController;

    public bool IsGameRunning { get; private set; }

    private void Awake()
    {
        if (Instance && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        StartGame();
    }

    public void StartGame()
    {
        IsGameRunning = true;
    }

    public void StopGame()
    {
        IsGameRunning = false;
        if (_audioVolumeController)
        {
            _audioVolumeController.StopCurrentMusic(0.8f);
        }
    }
}

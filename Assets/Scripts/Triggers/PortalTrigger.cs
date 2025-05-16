using UnityEngine;

public class PortalTrigger : MonoBehaviour
{
    public SimplePlayerController playerController;
    public GameObject complitedGameObject;
    public GameObject completedObject;

    public Vector2 targetOffset;
    public float shrinkDuration = 1f;

    private LevelProgressSaver _levelProgressSaver;

    private bool _triggered = false;
    private Vector2 _startPosition;
    private Vector2 _targetPosition;
    private float _shrinkProgress = 0f;
    private bool _gameCompleted = false;

    private void Start()
    {
        TryGetComponent(out _levelProgressSaver);

        if (!_levelProgressSaver)
        {
            Debug.Log("�� ������ LevelProgressSaver �� �������");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !_triggered && playerController.Alive)
        {
            _triggered = true;
            if (playerController)
            {
                playerController.BlockDamage();
                playerController.StopPlayer();
                playerController.LockPlayerActions();
                playerController.PlayJumpAnimation();
                playerController.GetComponent<Rigidbody2D>().simulated = false;
            }

            if (GameStateTracker.Instance)
            {
                GameStateTracker.Instance.StopGame();
            }

            if (completedObject)
            {
                completedObject.SetActive(true);
            }

            _startPosition = playerController.transform.position;
            _targetPosition = (Vector2)transform.position + targetOffset;
        }
    }

    private void Update()
    {
        if (!_triggered) { return; }

        _shrinkProgress += Time.deltaTime / shrinkDuration;
        _shrinkProgress = Mathf.Clamp01(_shrinkProgress);

        // ������� ��������� �� ������ �� �������� �����
        Vector2 newPosition = Vector2.Lerp(_startPosition, _targetPosition, _shrinkProgress);
        playerController.transform.position = newPosition;

        // ��������� ������ �� ��������
        float newSize = Mathf.Lerp(0.35f, 0f, _shrinkProgress * _shrinkProgress);
        float scaleSign = Mathf.Sign(playerController.transform.localScale.x);
        playerController.transform.localScale = new Vector3(newSize * scaleSign, newSize, 1f);

        if (_shrinkProgress >= 1f)
        {
            CompletedGame();
        }
    }

    private void CompletedGame()
    {
        if (_gameCompleted) return;

        if (_levelProgressSaver)
        {
            _levelProgressSaver.SaveCurrentLevelProgress();
        }

        complitedGameObject.GetComponent<CompletedScript>().CompletedGame();

        _gameCompleted = true;
    }
}

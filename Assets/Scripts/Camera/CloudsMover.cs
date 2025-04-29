using UnityEngine;

public class CloudsMover : MonoBehaviour
{
    [SerializeField] private float _minMoveSpeed = 1f;
    [SerializeField] private float _maxMoveSpeed = 3f;
    [SerializeField] private float _resetXPosition = -40f;
    [SerializeField] private float _maxXPosition = 40f;
    [SerializeField] private Vector2 _randomYRange = new Vector2(-12f, 12f);

    private Cloud[] _clouds;

    private void Awake()
    {
        _clouds = new Cloud[transform.childCount];

        for (int i = 0; i < transform.childCount; i++)
        {
            Transform cloudTransform = transform.GetChild(i);
            float randomSpeed = Random.Range(_minMoveSpeed, _maxMoveSpeed);

            _clouds[i] = new Cloud(cloudTransform, randomSpeed);
        }
    }

    private void Update()
    {
        MoveClouds();
    }

    private void MoveClouds()
    {
        foreach (var cloud in _clouds)
        {
            cloud.Transform.localPosition += Vector3.right * cloud.MoveSpeed * Time.deltaTime;

            if (cloud.Transform.localPosition.x >= _maxXPosition)
            {
                Vector3 newPosition = cloud.Transform.localPosition;
                newPosition.x = _resetXPosition;
                newPosition.y = Random.Range(_randomYRange.x, _randomYRange.y);
                cloud.Transform.localPosition = newPosition;
            }
        }
    }

    private class Cloud
    {
        public Transform Transform { get; }
        public float MoveSpeed { get; }

        public Cloud(Transform transform, float moveSpeed)
        {
            Transform = transform;
            MoveSpeed = moveSpeed;
        }
    }
}

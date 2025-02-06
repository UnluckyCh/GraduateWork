using UnityEngine;

public class TrajectoryController : MonoBehaviour
{
    private TrajectoryBuilder _trajectoryBuilder;

    private void Start()
    {
        TryGetComponent(out _trajectoryBuilder);
        if (!_trajectoryBuilder)
            Debug.LogError($"Не найден компонент TrajectoryBuilder у {name}");
    }

    public void DrawTrajectory()
    {
        Vector2 playerPos = transform.position;

        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        Vector2 direction = mousePos - playerPos;

        float angleDegrees = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        _trajectoryBuilder.DrawTrajectory(playerPos, angleDegrees);
    }

    public void ClearTrajectory()
    {
        _trajectoryBuilder.ClearTrajectory();
    }
}

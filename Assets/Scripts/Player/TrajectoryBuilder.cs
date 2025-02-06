using UnityEngine;

public class TrajectoryBuilder : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public float initialSpeed = 10f;
    public float gravity = 6f;
    public float maxTime = 1f;
    public int pointCount = 35;

    public void DrawTrajectory(Vector2 startPos, float angleDegrees)
    {
        ClearTrajectory();

        float angleRad = angleDegrees * Mathf.Deg2Rad;
        Vector2 velocity = new(
            initialSpeed * Mathf.Cos(angleRad),
            initialSpeed * Mathf.Sin(angleRad)
        );

        Vector3[] points = new Vector3[pointCount];
        float timeStep = maxTime / pointCount;
        int actualPointCount = pointCount;

        for (int i = 0; i < pointCount; i++)
        {
            float t = i * timeStep;
            float x = velocity.x * t;
            float y = velocity.y * t - 0.5f * gravity * t * t;
            Vector3 newPoint = startPos + new Vector2(x, y);

            if (i > 0)
            {
                // Проверка столкновения между предыдущей точкой и новой
                RaycastHit2D hit = Physics2D.Linecast(points[i - 1], newPoint);
                if (hit.collider != null && (!hit.collider.isTrigger
                                           || hit.collider.GetComponent<RotatorTrigger>())
                                           && !hit.collider.CompareTag("Player"))
                {
                    // Если столкновение обнаружено, устанавливаем точку столкновения и прекращаем вычисление траектории
                    newPoint = hit.point;
                    points[i] = newPoint;
                    actualPointCount = i + 1;
                    break;
                }
            }
            points[i] = newPoint;
        }

        lineRenderer.positionCount = actualPointCount;
        lineRenderer.SetPositions(points);
    }

    public void ClearTrajectory()
    {
        lineRenderer.enabled = false;
        lineRenderer.positionCount = 0;
        lineRenderer.enabled = true;
    }
}

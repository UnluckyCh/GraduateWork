using UnityEngine;

public class ProjectileLauncher : MonoBehaviour
{
    [SerializeField]
    private GameObject projectilePrefab; // Префаб магически синего снаряда

    [SerializeField]
    private Transform _parentTransform; // Префаб магически синего снаряда

    [SerializeField]
    private float projectileInitialSpeed = 10f; // Начальная скорость снаряда

    [SerializeField]
    private float projectileGravity = 6f; // Гравитация для расчёта траектории снаряда

    /// <summary>
    /// Запускает снаряд. Вычисляет направление от позиции лаунчера к целевой позиции.
    /// </summary>
    /// <param name="targetPosition">Целевая позиция (например, положение мыши в мире).</param>
    public void LaunchProjectile(Vector2 targetPosition)
    {
        // Используем позицию самого лаунчера как точку спауна
        Vector2 spawnPosition = transform.position;

        // Вычисляем направление от спауна к цели
        Vector2 direction = (targetPosition - spawnPosition).normalized;
        Vector2 initialVelocity = direction * projectileInitialSpeed;

        // Создаём снаряд с начальной ротацией, которая не наследуется от лаунчера
        GameObject projectileInstance = Instantiate(projectilePrefab, spawnPosition, Quaternion.identity, _parentTransform);
        ProjectileController projectileController = projectileInstance.GetComponent<ProjectileController>();
        if (projectileController != null)
        {
            projectileController.Initialize(initialVelocity, projectileGravity);
        }
        else
        {
            Debug.LogError("Не найден компонент ProjectileController на префабе снаряда!");
        }
    }
}

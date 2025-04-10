using UnityEngine;

public class ProjectileLauncher : MonoBehaviour
{
    [SerializeField]
    private GameObject projectilePrefab; // ������ ��������� ������ �������

    [SerializeField]
    private Transform _parentTransform; // ������ ��������� ������ �������

    [SerializeField]
    private float projectileInitialSpeed = 10f; // ��������� �������� �������

    [SerializeField]
    private float projectileGravity = 6f; // ���������� ��� ������� ���������� �������

    /// <summary>
    /// ��������� ������. ��������� ����������� �� ������� �������� � ������� �������.
    /// </summary>
    /// <param name="targetPosition">������� ������� (��������, ��������� ���� � ����).</param>
    public void LaunchProjectile(Vector2 targetPosition)
    {
        // ���������� ������� ������ �������� ��� ����� ������
        Vector2 spawnPosition = transform.position;

        // ��������� ����������� �� ������ � ����
        Vector2 direction = (targetPosition - spawnPosition).normalized;
        Vector2 initialVelocity = direction * projectileInitialSpeed;

        // ������ ������ � ��������� ��������, ������� �� ����������� �� ��������
        GameObject projectileInstance = Instantiate(projectilePrefab, spawnPosition, Quaternion.identity, _parentTransform);
        ProjectileController projectileController = projectileInstance.GetComponent<ProjectileController>();
        if (projectileController != null)
        {
            projectileController.Initialize(initialVelocity, projectileGravity);
        }
        else
        {
            Debug.LogError("�� ������ ��������� ProjectileController �� ������� �������!");
        }
    }
}

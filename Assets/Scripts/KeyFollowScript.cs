using UnityEngine;

public class KeyFollowScript : MonoBehaviour
{
    public Transform target; // ������� ������, �� ������� ����� ��������� ����
    public Vector2 followOffset; // ��������� ������ ��� ����� �������������
    public float followSpeed = 5f; // �������� ����������

    private bool isFollowing = false;

    // ����� ��� ������ ����������
    public void StartFollowing()
    {
        isFollowing = true;
    }

    // ����� ��� ��������� ����������
    public void StopFollowing()
    {
        isFollowing = false;
    }

    void Update()
    {
        if (isFollowing && target != null)
        {
            // �������� ������� ���� � ��������� ������������
            Vector2 targetPosition = target.position;

            // ��������� ��������� ������ � ������� ����
            targetPosition += followOffset;

            // ��������� ����� ������� ��� ����� � �������������� ������������
            Vector2 newPosition = Vector2.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
            transform.position = new Vector3(newPosition.x, newPosition.y, transform.position.z);
        }
    }
}

using UnityEngine;

public class CameraFreeFollow : MonoBehaviour
{
    public Transform Player;
    public float cameraMoveSpeed = 0.2f; // ��� SmoothDamp (���� �����)
    public float smoothTimeElse = 0.3f;  // �����, �� ������� ������ ������� ������ � ����� else
    public float yOffset = 2.5f;
    private Vector3 cameraVelocity = Vector3.zero;

    void LateUpdate()
    {
        Vector3 targetPosition;

        if (!GravityController.Instance.IsActiveRotate)
        {
            // ������� �������� �� ���� X � Y � �������������� SmoothDamp
            targetPosition = transform.position;
            targetPosition.y = Mathf.SmoothDamp(transform.position.y, Player.position.y, ref cameraVelocity.y, cameraMoveSpeed);
            targetPosition.x = Mathf.SmoothDamp(transform.position.x, Player.position.x, ref cameraVelocity.x, cameraMoveSpeed);
        }
        else
        {
            targetPosition = Vector3.Lerp(transform.position, Player.position, Time.deltaTime / smoothTimeElse);
        }

        // ��������� ������� ������, �������� Z ����������
        transform.position = new Vector3(targetPosition.x, targetPosition.y, transform.position.z);
    }
}

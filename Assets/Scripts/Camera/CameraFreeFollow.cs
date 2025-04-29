using UnityEngine;

public class CameraFreeFollow : MonoBehaviour
{
    public Transform Player;
    public float cameraMoveSpeed = 0.2f; // ��� SmoothDamp (���� �����)
    public float smoothTimeElse = 0.3f;  // �����, �� ������� ������ ������� ������ � ����� else
    public float yOffset = 2.5f;

    private Vector3 _cameraVelocity = Vector3.zero;
    private bool _isFollowingPlayer = true;
    private Vector3 _fixedTargetPosition;

    void LateUpdate()
    {
        Vector3 targetPosition;
        if (_isFollowingPlayer)
        {
            if (!GravityController.Instance.IsActiveRotate)
            {
                // ������� �������� �� ���� X � Y � �������������� SmoothDamp
                targetPosition = transform.position;
                targetPosition.y = Mathf.SmoothDamp(transform.position.y, Player.position.y, ref _cameraVelocity.y, cameraMoveSpeed);
                targetPosition.x = Mathf.SmoothDamp(transform.position.x, Player.position.x, ref _cameraVelocity.x, cameraMoveSpeed);
            }
            else
            {
                targetPosition = Vector3.Lerp(transform.position, Player.position, Time.deltaTime / smoothTimeElse);
            }
        }
        else
        {
            // ���� ������������ ��������� � ������ ���������� ��������� � ��������������� �����
            targetPosition = Vector3.Lerp(transform.position, new Vector3(_fixedTargetPosition.x, _fixedTargetPosition.y + yOffset, transform.position.z), Time.deltaTime / smoothTimeElse);
        }

        transform.position = new Vector3(targetPosition.x, targetPosition.y, transform.position.z);
    }

    public void StopFollowingPlayer()
    {
        if (_isFollowingPlayer)
        {
            _isFollowingPlayer = false;
            _fixedTargetPosition = Player.position;
        }
    }
}

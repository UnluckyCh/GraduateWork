using UnityEngine;

public class CameraFreeFollow : MonoBehaviour
{
    public Transform Player;
    public float cameraMoveSpeed = 0.2f; // дл€ SmoothDamp (если нужно)
    public float smoothTimeElse = 0.3f;  // врем€, за которое камера догонит игрока в ветке else
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
                // ѕлавное движение по ос€м X и Y с использованием SmoothDamp
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
            // ≈сли отслеживание отключено Ч камера продолжает двигатьс€ к зафиксированной точке
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

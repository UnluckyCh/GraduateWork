using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform Player;
    public float cameraMoveSpeed = 0.2f;
    public float yOffset = 2.5f;
    private Vector3 cameraVelocity = Vector3.zero;

    void LateUpdate()
    {
        Vector3 targetPosition = transform.position;

        targetPosition.y = Mathf.SmoothDamp(transform.position.y, Player.position.y + yOffset, ref cameraVelocity.y, cameraMoveSpeed);

        transform.position = new Vector3(transform.position.x, targetPosition.y, transform.position.z);
    }
}

using UnityEngine;

public class CameraFollowInClamp : MonoBehaviour
{
    public Transform Player;
    public float cameraMoveSpeed = 0.2f;
    public float minY = 0f;
    public float maxY = 20f;
    public float yOffset = 2.5f;
    private Vector3 cameraVelocity = Vector3.zero;

    void Update()
    {
        Vector3 targetPosition = transform.position;

        targetPosition.y = Mathf.SmoothDamp(transform.position.y, Player.position.y + yOffset, ref cameraVelocity.y, cameraMoveSpeed);
        targetPosition.y = Mathf.Clamp(targetPosition.y, minY, maxY);

        transform.position = new Vector3(transform.position.x, targetPosition.y, transform.position.z);
    }
}

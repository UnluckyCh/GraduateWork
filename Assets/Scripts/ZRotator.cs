using UnityEngine;

public class ZRotator : MonoBehaviour
{
    [SerializeField] private float _rotationSpeed = 90f; // градусов в секунду

    private void Update()
    {
        transform.Rotate(0f, 0f, _rotationSpeed * Time.deltaTime);
    }
}

using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    // Ссылка на камеру. Если не назначена, используется основная камера.
    [SerializeField] private Transform cameraTransform;

    // Коэффициент параллакса для данного слоя
    [SerializeField] private float parallaxCoefficient = 0.5f;

    // Предыдущая позиция камеры в локальных координатах локации
    private Vector3 previousCameraLocalPosition;

    // Родительский объект (локация), относительно которого вычисляем позицию камеры
    private Transform parentTransform;

    private void Start()
    {
        if (cameraTransform == null)
        {
            cameraTransform = Camera.main.transform;
        }

        // Предполагается, что фон является дочерним элементом локации
        parentTransform = transform.parent;

        // Начальная позиция камеры в локальной системе координат родителя (локации)
        previousCameraLocalPosition = parentTransform.InverseTransformPoint(cameraTransform.position);
    }

    private void LateUpdate()
    {
        // Получаем текущую позицию камеры в локальных координатах локации
        Vector3 currentCameraLocalPosition = parentTransform.InverseTransformPoint(cameraTransform.position);

        // Разница смещения камеры в локальной системе координат
        Vector3 deltaMovement = currentCameraLocalPosition - previousCameraLocalPosition;

        // Смещаем фон в локальной системе координат с учетом коэффициента параллакса
        transform.localPosition += new Vector3(deltaMovement.x * parallaxCoefficient, deltaMovement.y * parallaxCoefficient, 0);

        // Обновляем предыдущую позицию для следующего кадра
        previousCameraLocalPosition = currentCameraLocalPosition;
    }
}

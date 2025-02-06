using UnityEngine;
using System.Collections;

public class CameraZoomController : MonoBehaviour
{
    private Camera cam;
    private float originalSize;
    private float targetSize;
    private float originalY;
    private float targetY;

    // Можно задать величину изменения и длительность перехода
    public float zoomDelta = 2f;
    public float transitionTime = 1f;

    // Хранение текущей запущенной корутины для остановки при переключении
    private Coroutine currentCoroutine;

    void Start()
    {
        cam = GetComponent<Camera>();
        // Сохраняем исходные значения
        originalSize = cam.orthographicSize;
        targetSize = originalSize + zoomDelta;

        originalY = transform.localPosition.y;
        targetY = originalY + zoomDelta;
    }

    void Update()
    {
        // При нажатии правой кнопки мыши запускаем увеличение
        if (Input.GetMouseButtonDown(1))
        {
            if (currentCoroutine != null)
                StopCoroutine(currentCoroutine);
            // Запускаем корутину, начиная с текущих значений, стремясь к увеличенным
            currentCoroutine = StartCoroutine(ZoomCamera(
                cam.orthographicSize, targetSize,
                transform.localPosition.y, targetY,
                transitionTime));
        }
        // При отпускании кнопки запускаем уменьшение
        else if (Input.GetMouseButtonUp(1))
        {
            if (currentCoroutine != null)
                StopCoroutine(currentCoroutine);
            // Запускаем корутину, начиная с текущих значений, возвращаясь к исходным
            currentCoroutine = StartCoroutine(ZoomCamera(
                cam.orthographicSize, originalSize,
                transform.localPosition.y, originalY,
                transitionTime));
        }
    }

    IEnumerator ZoomCamera(float startSize, float endSize, float startY, float endY, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            float t = elapsed / duration;
            // Интерполяция размера камеры
            cam.orthographicSize = Mathf.Lerp(startSize, endSize, t);
            // Интерполяция локальной позиции по Y
            Vector3 pos = transform.localPosition;
            pos.y = Mathf.Lerp(startY, endY, t);
            transform.localPosition = pos;

            elapsed += Time.deltaTime;
            yield return null;
        }
        // Гарантируем установку конечных значений
        cam.orthographicSize = endSize;
        Vector3 finalPos = transform.localPosition;
        finalPos.y = endY;
        transform.localPosition = finalPos;

        currentCoroutine = null;
    }
}

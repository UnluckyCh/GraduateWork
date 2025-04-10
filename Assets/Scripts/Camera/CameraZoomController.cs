using UnityEngine;
using System.Collections;

public class CameraZoomController : MonoBehaviour
{
    private Camera cam;
    private float originalSize;
    private float targetSize;
    private float originalY;
    private float targetY;

    // ����� ������ �������� ��������� � ������������ ��������
    public float zoomDelta = 2f;
    public float transitionTime = 1f;

    // �������� ������� ���������� �������� ��� ��������� ��� ������������
    private Coroutine currentCoroutine;

    void Start()
    {
        cam = GetComponent<Camera>();
        // ��������� �������� ��������
        originalSize = cam.orthographicSize;
        targetSize = originalSize + zoomDelta;

        originalY = transform.localPosition.y;
        targetY = originalY + zoomDelta;
    }

    void Update()
    {
        // ��� ������� ������ ������ ���� ��������� ����������
        if (Input.GetMouseButtonDown(1))
        {
            if (currentCoroutine != null)
                StopCoroutine(currentCoroutine);
            // ��������� ��������, ������� � ������� ��������, �������� � �����������
            currentCoroutine = StartCoroutine(ZoomCamera(
                cam.orthographicSize, targetSize,
                transform.localPosition.y, targetY,
                transitionTime));
        }
        // ��� ���������� ������ ��������� ����������
        else if (Input.GetMouseButtonUp(1))
        {
            if (currentCoroutine != null)
                StopCoroutine(currentCoroutine);
            // ��������� ��������, ������� � ������� ��������, ����������� � ��������
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
            // ������������ ������� ������
            cam.orthographicSize = Mathf.Lerp(startSize, endSize, t);
            // ������������ ��������� ������� �� Y
            Vector3 pos = transform.localPosition;
            pos.y = Mathf.Lerp(startY, endY, t);
            transform.localPosition = pos;

            elapsed += Time.deltaTime;
            yield return null;
        }
        // ����������� ��������� �������� ��������
        cam.orthographicSize = endSize;
        Vector3 finalPos = transform.localPosition;
        finalPos.y = endY;
        transform.localPosition = finalPos;

        currentCoroutine = null;
    }
}

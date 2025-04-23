using UnityEngine;
using System.Collections;

public class CameraZoomControllerWithoutYDelta : MonoBehaviour
{
    [SerializeField] private float _zoomDelta = 3f;
    [SerializeField] private float _transitionTime = 1f;

    private Camera _cam;
    private float _originalSize;
    private float _targetSize;

    private Coroutine _currentCoroutine;

    private void Start()
    {
        _cam = GetComponent<Camera>();
        _originalSize = _cam.orthographicSize;
        _targetSize = _originalSize + _zoomDelta;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            StartZoomCoroutine(_cam.orthographicSize, _targetSize);
        }
        else if (Input.GetMouseButtonUp(1))
        {
            StartZoomCoroutine(_cam.orthographicSize, _originalSize);
        }
    }

    private void StartZoomCoroutine(float fromSize, float toSize)
    {
        if (_currentCoroutine != null)
            StopCoroutine(_currentCoroutine);

        _currentCoroutine = StartCoroutine(ZoomCamera(fromSize, toSize, _transitionTime));
    }

    private IEnumerator ZoomCamera(float startSize, float endSize, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            float t = elapsed / duration;
            _cam.orthographicSize = Mathf.Lerp(startSize, endSize, t);

            elapsed += Time.deltaTime;
            yield return null;
        }

        _cam.orthographicSize = endSize;
        _currentCoroutine = null;
    }
}

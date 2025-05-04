using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScreenController : MonoBehaviour
{
    [SerializeField] private GameObject _loadingObject;
    [SerializeField] private RectTransform _outerSquare;
    [SerializeField] private RectTransform _innerSquare;
    [SerializeField] private Slider _slider;

    [SerializeField] private float _rotationDuration = 0.5f;
    [SerializeField] private float _moveDuration = 0.25f;

    private Vector2 _startInnerPosition;

    private void Awake()
    {
        _startInnerPosition = _innerSquare.anchoredPosition;
    }

    public void StartLoading(int index)
    {
        _loadingObject.SetActive(true);
        StartCoroutine(LoadingAnimationCoroutine(index));
    }

    private IEnumerator InfinityLoadingAnimationCoroutine(int index)
    {
        float fakeProgress = 0f;
        int step = 0;

        while (true)
        {
            // Фейковый прогресс слайдера
            fakeProgress = Mathf.Min(fakeProgress + Time.deltaTime * 2f, 1f);
            _slider.value = fakeProgress;

            // Анимация квадрата
            yield return StartCoroutine(AnimateStep(step));
            step = (step + 1) % 4;
        }
    }


    private IEnumerator LoadingAnimationCoroutine(int index)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(index);
        operation.allowSceneActivation = false;

        int step = 0;

        while (!operation.isDone)
        {
            _slider.value = Mathf.Clamp01(operation.progress / 0.9f);

            if (operation.progress >= 0.9f)
            {
                // Не запускаем анимацию, сразу активируем сцену
                operation.allowSceneActivation = true;
                yield break; // Прерываем корутину
            }

            // Анимация квадрата
            yield return StartCoroutine(AnimateStep(step));
            step = (step + 1) % 4;
        }
    }

    private IEnumerator AnimateStep(int step)
    {
        Quaternion startRot = _outerSquare.rotation;
        Quaternion endRot = startRot * Quaternion.Euler(0, 0, -90f);

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / _rotationDuration;
            _outerSquare.rotation = Quaternion.Lerp(startRot, endRot, t);
            yield return null;
        }

        yield return new WaitForSeconds(0.05f);

        Vector2 direction = step switch
        {
            0 => Vector2.right,   // +X
            1 => Vector2.up,      // +Y
            2 => Vector2.left,    // -X
            3 => Vector2.down,    // -Y
            _ => Vector2.zero
        };

        Vector2 startPos = _innerSquare.anchoredPosition;
        Vector2 endPos = startPos + direction * 70f;

        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / _moveDuration;
            _innerSquare.anchoredPosition = Vector2.Lerp(startPos, endPos, t);
            yield return null;
        }

        yield return new WaitForSeconds(0.3f);
    }


    private void OnDisable()
    {
        _innerSquare.anchoredPosition = _startInnerPosition;
        _outerSquare.rotation = Quaternion.identity;
        _slider.value = 0;
    }
}

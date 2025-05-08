using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class LevelStarter : MonoBehaviour
{
    [System.Serializable]
    public class DifficultyVisualGroup
    {
        public TMP_Text text1;
        public TMP_Text text2;
        public Image image;
        public HoverActivator hoverActivator;
        public Transform focusTarget;
    }

    public DifficultyVisualGroup[] difficultyGroups; // Индекс соответствует int уровню сложности - 1

    [Header("Fade и затемнение")]
    public ScreenFader _screenFader; // UI Image на весь экран (черный)
    public float fadeDuration = 1.5f;

    public Image _imageBG;

    [Header("Настройки камеры")]
    public Camera mainCamera;
    public float transitionDuration = 2f;
    public float zoomSize = 3f; // Конечный размер ortho камеры

    [Header("Канвасы для переключения")]
    public Canvas[] _canvases;

    [Header("Экран загрузки")]
    [SerializeField] private LoadingScreenController _loadingScreen;

    private int _targetLevelBuildIndex = 1;

    public void StartDifficulty(int level)
    {
        PlayerPrefs.SetInt("Difficulty", level);
        StartCoroutine(PlayTransition(level - 1));
    }

    private IEnumerator PlayTransition(int index)
    {
        DifficultyVisualGroup group = difficultyGroups[index];

        foreach (var canvas in _canvases)
        {
            canvas.renderMode = RenderMode.WorldSpace;
        }

        // Запоминаем начальные параметры камеры
        mainCamera.transform.GetPositionAndRotation(out Vector3 startPosition, out Quaternion startRotation);
        float startSize = mainCamera.orthographicSize;

        // Целевые параметры
        Vector3 targetPosition = new(group.focusTarget.position.x, group.focusTarget.position.y, startPosition.z);
        Quaternion targetRotation = GetTargetRotation(index);
        float targetSize = zoomSize;

        // Альфы начальные
        Color t1Start = group.text1.color;
        Color t2Start = group.text2.color;
        Color imgStart = group.image.color;

        group.hoverActivator.Block = true;

        _screenFader.FadeIn(fadeDuration * 0.9f);

        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            float t = elapsed / fadeDuration;
            float tSmooth = Mathf.SmoothStep(0f, 1f, t);

            // Fade UI группы (затухание)
            group.text1.color = new Color(t1Start.r, t1Start.g, t1Start.b, Mathf.Lerp(t1Start.a, 0f, tSmooth));
            group.text2.color = new Color(t2Start.r, t2Start.g, t2Start.b, Mathf.Lerp(t2Start.a, 0f, tSmooth));
            group.image.color = new Color(imgStart.r, imgStart.g, imgStart.b, Mathf.Lerp(imgStart.a, 0f, tSmooth));

            if (_imageBG)
            {
                _imageBG.transform.localScale = Vector3.Lerp(Vector3.one, Vector3.one * 4f, tSmooth);
            }

            // Поворот камеры
            mainCamera.transform.SetPositionAndRotation(Vector3.Lerp(startPosition, targetPosition, tSmooth),
                                                        Quaternion.Slerp(startRotation, targetRotation, tSmooth));
            mainCamera.orthographicSize = Mathf.Lerp(startSize, targetSize, tSmooth);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Финальные значения
        group.text1.color = new Color(t1Start.r, t1Start.g, t1Start.b, 0f);
        group.text2.color = new Color(t2Start.r, t2Start.g, t2Start.b, 0f);
        group.image.color = new Color(imgStart.r, imgStart.g, imgStart.b, 0f);

        mainCamera.transform.SetPositionAndRotation(targetPosition, targetRotation);
        mainCamera.orthographicSize = targetSize;

        yield return null;

        _loadingScreen.StartLoading(SceneManager.GetActiveScene().buildIndex + _targetLevelBuildIndex);
    }

    public void SetTargetLevel(int buildIndex)
    {
        _targetLevelBuildIndex = buildIndex;
    }


    private Quaternion GetTargetRotation(int index)
    {
        return index switch
        {
            0 => Quaternion.Euler(0, 0, 90),// Против часовой
            1 => Quaternion.Euler(0, 0, -90),// По часовой
            2 => Quaternion.Euler(0, 0, 180),// На 180
            _ => Quaternion.identity,
        };
    }
}

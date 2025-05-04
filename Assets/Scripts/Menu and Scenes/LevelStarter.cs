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

    public DifficultyVisualGroup[] difficultyGroups; // ������ ������������� int ������ ��������� - 1

    [Header("Fade � ����������")]
    public ScreenFader _screenFader; // UI Image �� ���� ����� (������)
    public float fadeDuration = 1.5f;

    public Image _imageBG;

    [Header("��������� ������")]
    public Camera mainCamera;
    public float transitionDuration = 2f;
    public float zoomSize = 3f; // �������� ������ ortho ������

    [Header("������� ��� ������������")]
    public Canvas[] _canvases;

    [Header("����� ��������")]
    [SerializeField] private LoadingScreenController _loadingScreen;

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

        // ���������� ��������� ��������� ������
        mainCamera.transform.GetPositionAndRotation(out Vector3 startPosition, out Quaternion startRotation);
        float startSize = mainCamera.orthographicSize;

        // ������� ���������
        Vector3 targetPosition = new(group.focusTarget.position.x, group.focusTarget.position.y, startPosition.z);
        Quaternion targetRotation = GetTargetRotation(index);
        float targetSize = zoomSize;

        // ����� ���������
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

            // Fade UI ������ (���������)
            group.text1.color = new Color(t1Start.r, t1Start.g, t1Start.b, Mathf.Lerp(t1Start.a, 0f, tSmooth));
            group.text2.color = new Color(t2Start.r, t2Start.g, t2Start.b, Mathf.Lerp(t2Start.a, 0f, tSmooth));
            group.image.color = new Color(imgStart.r, imgStart.g, imgStart.b, Mathf.Lerp(imgStart.a, 0f, tSmooth));

            if (_imageBG)
            {
                _imageBG.transform.localScale = Vector3.Lerp(Vector3.one, Vector3.one * 4f, tSmooth);
            }

            // ������� ������
            mainCamera.transform.SetPositionAndRotation(Vector3.Lerp(startPosition, targetPosition, tSmooth),
                                                        Quaternion.Slerp(startRotation, targetRotation, tSmooth));
            mainCamera.orthographicSize = Mathf.Lerp(startSize, targetSize, tSmooth);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // ��������� ��������
        group.text1.color = new Color(t1Start.r, t1Start.g, t1Start.b, 0f);
        group.text2.color = new Color(t2Start.r, t2Start.g, t2Start.b, 0f);
        group.image.color = new Color(imgStart.r, imgStart.g, imgStart.b, 0f);

        mainCamera.transform.SetPositionAndRotation(targetPosition, targetRotation);
        mainCamera.orthographicSize = targetSize;

        yield return null;

        _loadingScreen.StartLoading(SceneManager.GetActiveScene().buildIndex + 1);
    }

    private Quaternion GetTargetRotation(int index)
    {
        return index switch
        {
            0 => Quaternion.Euler(0, 0, 90),// ������ �������
            1 => Quaternion.Euler(0, 0, -90),// �� �������
            2 => Quaternion.Euler(0, 0, 180),// �� 180
            _ => Quaternion.identity,
        };
    }
}

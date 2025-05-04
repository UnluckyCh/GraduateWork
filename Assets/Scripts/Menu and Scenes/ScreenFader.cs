using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ScreenFader : MonoBehaviour
{
    [SerializeField] private float _defaultFadeDuration = 1f;
    [SerializeField] private Image _fadeImage;
    [SerializeField] private bool _fadeOnStart;
    [SerializeField] private LoadingScreenController _loadingScreen;

    private void Start()
    {
        if (_fadeOnStart)
        {
            _fadeImage.gameObject.SetActive(true);
            FadeOut(_defaultFadeDuration);
        }
    }

    public Coroutine FadeIn(float duration = -1f)
    {
        if (!_fadeImage) { return null; }
        _fadeImage.gameObject.SetActive(true);
        return StartCoroutine(FadeRoutine(0f, 1f, duration < 0 ? _defaultFadeDuration : duration));
    }

    public Coroutine FadeOut(float duration = -1f)
    {
        if (!_fadeImage) { return null; }
        return StartCoroutine(FadeRoutine(1f, 0f, duration < 0 ? _defaultFadeDuration : duration));
    }

    private IEnumerator FadeRoutine(float from, float to, float duration)
    {
        Color startColor = _fadeImage.color;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            float alpha = Mathf.Lerp(from, to, Mathf.SmoothStep(0f, 1f, t));
            _fadeImage.color = new Color(startColor.r, startColor.g, startColor.b, alpha);

            elapsed += Time.deltaTime;
            yield return null;
        }

        _fadeImage.color = new Color(startColor.r, startColor.g, startColor.b, to);
        if (to == 0f)
        {
            _fadeImage.gameObject.SetActive(false);
        }
    }

    public void FadeInAndLoadNextScene(int index, float duration = -0.3f)
    {
        float usedDuration = duration < 0 ? _defaultFadeDuration : duration;
        if (_fadeImage)
        {
            _fadeImage.gameObject.SetActive(true);
        }

        StartCoroutine(FadeAndLoadRoutine(usedDuration, index));
    }

    private IEnumerator FadeAndLoadRoutine(float duration, int index)
    {
        yield return FadeRoutine(0f, 1f, duration);
        if (_loadingScreen)
        {
            _loadingScreen.StartLoading(index);
        }
        else
        {
            Debug.LogWarning("ScreenFader: _loadingScreen не задан!");
        }
    }
}

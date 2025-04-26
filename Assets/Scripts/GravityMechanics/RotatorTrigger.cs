using System.Collections;
using UnityEngine;

public class RotatorTrigger : MonoBehaviour
{
    private enum TypeRotator
    {
        Yellow,
        Blue,
        Red
    }

    [Header("��������� ������������")]
    [Tooltip("����� ����� ������������ (���)")]
    public float _effectDuration = 0.5f;
    [Tooltip("�� ������� ��� �������� ������������� ������� (��������, 1.2 = +20%)")]
    public float _maxScaleMultiplier = 1.2f;

    [Header("��������� ���������")]
    [Tooltip("����� ��������� (���)")]
    public float _appearDuration = 0.5f;

    [SerializeField]
    private TypeRotator _typeRotator;

    [SerializeField]
    private bool _isOneTimeRotator = false;

    private ParticleSystem _childParticleSystem;
    private Color _originalParticleStartColor;

    private SpriteRenderer _spriteRenderer;
    private Vector3 _originalScale;
    private Color _originalSpriteColor;

    private RotatorUnifier _rotatorUnifier;

    private bool _hasActivated = false;
    private bool _isActive = true;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _originalScale = transform.localScale;
        _originalSpriteColor = _spriteRenderer.color;

        _childParticleSystem = GetComponentInChildren<ParticleSystem>();
        if (_childParticleSystem)
        {
            _originalParticleStartColor = _childParticleSystem.main.startColor.color;
        }
    }

    private void Start()
    {
        _rotatorUnifier = GetComponentInParent<RotatorUnifier>();

        if (!_rotatorUnifier)
            Debug.LogError($"�� ������ RotatorUnifier ����� ������������ ������� � {name}!");
        
        _rotatorUnifier.OnStartDisappear += OnStartDisappear;
        _rotatorUnifier.OnStartAppear += OnStartAppear;
    }

    private void OnStartDisappear()
    {
        StartCoroutine(DisappearCoroutine());
    }

    private void OnStartAppear()
    {
        if (_isOneTimeRotator && _hasActivated)
        {
            gameObject.SetActive(false);
        }
        else
        {
            StartCoroutine(AppearCoroutine());
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!_isActive)
            return;

        if (!collision.CompareTag("Bullet")) return;

        Destroy(collision.gameObject);

        _isActive = false;
        _hasActivated = true;

        switch (_typeRotator)
        {
            case TypeRotator.Yellow:
                GravityController.Instance.RotateRight();
                break;
            case TypeRotator.Blue:
                GravityController.Instance.RotateLeft();
                break;
            case TypeRotator.Red:
                GravityController.Instance.Rotate180();
                break;
            default:
                break;
        }

        _rotatorUnifier.StartDisappear();
    }

    private IEnumerator DisappearCoroutine()
    {
        float firstDuration = _effectDuration / 3f;
        Color originalColor = _spriteRenderer.color;
        Vector3 scaleUp = _originalScale * _maxScaleMultiplier;

        float timer = 0f;
        while (timer < firstDuration)
        {
            timer += Time.deltaTime;
            float t = timer / firstDuration;
            transform.localScale = Vector3.Lerp(_originalScale, scaleUp, t);
            yield return null;
        }

        timer = 0f;
        Vector3 startScale = transform.localScale;
        while (timer < (_effectDuration - firstDuration))
        {
            timer += Time.deltaTime;
            float t = timer / (_effectDuration - firstDuration);
            transform.localScale = Vector3.Lerp(startScale, Vector3.zero, t);

            Color newColor = _spriteRenderer.color;
            newColor.a = Mathf.Lerp(originalColor.a, 0f, t);
            _spriteRenderer.color = newColor;

            if (_childParticleSystem)
            {
                float time = Mathf.Clamp(timer / firstDuration, 0 , 1f);
                var mainModule = _childParticleSystem.main;
                Color newParticleColor = mainModule.startColor.color;
                newParticleColor.a = Mathf.Lerp(_originalParticleStartColor.a, 0f, time);
                mainModule.startColor = newParticleColor;
            }

            yield return null;
        }
    }

    public IEnumerator AppearCoroutine()
    {
        float timer = 0f;
        // ���������� ���������� �������� ����
        Color targetColor = _originalSpriteColor;
        float targetAlpha = targetColor.a;
        Vector3 targetScale = _originalScale;

        transform.localScale = Vector3.zero;
        Color startColor = targetColor;
        startColor.a = 0f;
        _spriteRenderer.color = startColor;

        while (timer < _appearDuration)
        {
            timer += Time.deltaTime;
            float t = timer / _appearDuration;
            transform.localScale = Vector3.Lerp(Vector3.zero, targetScale, t);

            Color newColor = _spriteRenderer.color;
            newColor.a = Mathf.Lerp(0f, targetAlpha, t);
            _spriteRenderer.color = newColor;

            if (_childParticleSystem)
            {
                float time = timer / _appearDuration;
                var mainModule = _childParticleSystem.main;
                Color newParticleColor = mainModule.startColor.color;
                newParticleColor.a = Mathf.Lerp(0f, _originalParticleStartColor.a, time);
                mainModule.startColor = newParticleColor;
            }

            yield return null;
        }

        transform.localScale = targetScale;
        targetColor.a = targetAlpha;
        _spriteRenderer.color = targetColor;
        _isActive = true;
    }

    private void OnDisable()
    {
        _rotatorUnifier.OnStartDisappear -= OnStartDisappear;
        _rotatorUnifier.OnStartAppear -= OnStartAppear;
    }
}

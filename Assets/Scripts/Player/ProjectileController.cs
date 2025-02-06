using UnityEngine;
using System.Collections;

public class ProjectileController : MonoBehaviour
{
    private Vector2 _initialVelocity;
    private float _gravity = 6f;
    private float _timer = 0f;
    private Vector2 _startPos;
    private bool _isExploding = false;

    // --- Переменные для фриза ---
    private bool _isFrozen = false;
    private Vector3 _localAnchor;             // Локальное смещение от родителя до пули
    private Quaternion _freezeParentRotation; // Ротация родителя в момент фриза
    private Vector2 _frozenGlobalVelocity;      // Глобальная скорость пули в момент фриза

    private Rigidbody2D _rb;
    private SpriteRenderer _spriteRenderer;

    // Спрайт для полёта
    [SerializeField]
    private Sprite flightSprite;
    // Массив спрайтов для анимации взрыва
    [SerializeField]
    private Sprite[] explosionSprites;
    // Задержка между кадрами анимации взрыва
    [SerializeField]
    private float explosionFrameDelay = 0.1f;

    /// <summary>
    /// Инициализирует снаряд начальными параметрами
    /// </summary>
    public void Initialize(Vector2 initialVelocity, float gravity)
    {
        _initialVelocity = initialVelocity;
        _gravity = gravity;
        _startPos = transform.position;
    }

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        if (_spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer не найден на снаряде!");
        }
        if (flightSprite != null)
            _spriteRenderer.sprite = flightSprite;
        TryGetComponent(out _rb);
    }

    private void OnEnable()
    {
        if (GravityController.Instance != null)
        {
            GravityController.Instance.OnGravityChangeStarted += Freeze;
            GravityController.Instance.OnGravityChangeFinished += Unfreeze;
        }
    }

    private void OnDisable()
    {
        if (GravityController.Instance != null)
        {
            GravityController.Instance.OnGravityChangeStarted -= Freeze;
            GravityController.Instance.OnGravityChangeFinished -= Unfreeze;
        }
    }

    /// <summary>
    /// Вызывается при начале поворота мира.
    /// Сохраняем локальное смещение от родителя, ротацию родителя и текущую глобальную скорость пули.
    /// </summary>
    private void Freeze(GravityDirection newGravity)
    {
        if (_isFrozen)
            return;

        if (transform.parent != null)
        {
            // Сохраняем локальную позицию пули относительно родителя
            _localAnchor = transform.parent.InverseTransformPoint(transform.position);
            // Сохраняем ротацию родителя в момент фриза
            _freezeParentRotation = transform.parent.rotation;
        }
        else
        {
            _localAnchor = transform.position;
            _freezeParentRotation = Quaternion.identity;
        }

        // Текущая глобальная скорость: v = v0 + (0,-gravity)*t
        Vector2 currentVelocity = _initialVelocity + new Vector2(0, -_gravity) * _timer;
        _frozenGlobalVelocity = currentVelocity;

        _isFrozen = true;
    }

    /// <summary>
    /// Вызывается после окончания поворота мира.
    /// Вычисляем, насколько повернулся родитель, и поворачиваем сохранённую скорость,
    /// затем обновляем стартовую точку и сбрасываем таймер.
    /// </summary>
    private void Unfreeze(GravityDirection newGravity)
    {
        if (!_isFrozen)
            return;

        // Получаем текущую глобальную позицию пули (относительно родителя)
        Vector3 currentGlobalPosition = transform.parent != null ? transform.parent.TransformPoint(_localAnchor) : _localAnchor;
        transform.position = currentGlobalPosition;

        // Вычисляем дельту ротации: на сколько изменилось направление родителя
        Quaternion currentParentRotation = transform.parent != null ? transform.parent.rotation : Quaternion.identity;
        Quaternion rotationDelta = currentParentRotation * Quaternion.Inverse(_freezeParentRotation);

        // Применяем ту же дельту к сохранённой скорости
        Vector2 newVelocity = rotationDelta * _frozenGlobalVelocity;

        // Обновляем стартовую позицию и скорость для продолжения симуляции
        _startPos = currentGlobalPosition;
        _initialVelocity = newVelocity;
        _timer = 0f;

        _isFrozen = false;
    }

    private void Update()
    {
        if (_isExploding)
            return;

        if (_isFrozen)
        {
            // Пока мир поворачивается, фиксируем позицию пули относительно родителя.
            if (transform.parent != null)
                transform.position = transform.parent.TransformPoint(_localAnchor);
            else
                transform.position = _localAnchor;
            return;
        }

        // Если мир не вращается, увеличиваем таймер
        if (!GravityController.Instance.IsActiveRotate)
            _timer += Time.deltaTime;

        // Расчёт новой позиции по формуле движения с ускорением (гравитация действует строго вниз)
        Vector2 newPos = _startPos + _initialVelocity * _timer + 0.5f * new Vector2(0, -_gravity) * _timer * _timer;
        transform.position = newPos;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_isExploding)
            return;

        if (collision.isTrigger)
            return;

        // Не обрабатываем столкновение с игроком
        if (collision.CompareTag("Player"))
            return;

        // При столкновении запускаем анимацию взрыва
        StartCoroutine(ExplosionAnimation());
    }

    private IEnumerator ExplosionAnimation()
    {
        _isExploding = true;
        _rb.simulated = false;
        // Отключаем коллайдер, чтобы избежать повторных столкновений
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
            col.enabled = false;

        // Проигрываем анимацию взрыва с увеличением масштаба
        for (int i = 0; i < explosionSprites.Length; i++)
        {
            _spriteRenderer.sprite = explosionSprites[i];
            transform.localScale *= 1.35f;
            yield return new WaitForSeconds(explosionFrameDelay);
        }
        // После завершения анимации уничтожаем снаряд
        Destroy(gameObject);
    }
}

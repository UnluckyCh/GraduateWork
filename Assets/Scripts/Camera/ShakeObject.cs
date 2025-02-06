using UnityEngine;
using System.Collections;

public class ShakeObject : MonoBehaviour
{
    // Длительность тряски (в секундах)
    public float shakeDuration = 0.5f; 
    // Базовая амплитуда смещения
    public float shakeMagnitude = 0.2f; 

    [SerializeField]
    private BoulderMover _boulderMover;

    // Исходная позиция объекта
    private Vector3 initialPosition;

    private void Awake()
    {
        initialPosition = transform.localPosition;
    }

    private void Start()
    {
        if (_boulderMover)
            _boulderMover.OnBoulderFlewDistance += Shake;
    }

    // Метод для вызова тряски с заданной интенсивностью
    public void Shake(float intensity)
    {
        StartCoroutine(ShakeCoroutine(intensity));
    }

    // Coroutine, реализующая плавную тряску с постепенным затуханием и сглаживанием смещения.
    private IEnumerator ShakeCoroutine(float intensity)
    {
        float elapsed = 0f;
        Vector3 velocity = Vector3.zero;

        while (elapsed < shakeDuration + (intensity / 50f))
        {
            // Коэффициент демпфирования для плавного затухания тряски
            float damper = 1.0f - (elapsed / (shakeDuration + (intensity / 50f)));
            
            // Вычисляем случайное смещение, умноженное на базовую амплитуду, intensity и коэффициент затухания
            Vector3 randomOffset = damper * (0.6f * intensity) * shakeMagnitude * UnityEngine.Random.insideUnitCircle;
            
            // Плавно перемещаем объект к новому положению
            transform.localPosition = Vector3.SmoothDamp(transform.localPosition, initialPosition + randomOffset, ref velocity, 0.02f);
            
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Возвращаем объект в исходное положение
        transform.localPosition = initialPosition;
    }

    private void OnDisable()
    {
        if (_boulderMover)
            _boulderMover.OnBoulderFlewDistance -= Shake;
    }
}

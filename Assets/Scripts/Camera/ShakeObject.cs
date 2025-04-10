using UnityEngine;
using System.Collections;

public class ShakeObject : MonoBehaviour
{
    // ������������ ������ (� ��������)
    public float shakeDuration = 0.5f; 
    // ������� ��������� ��������
    public float shakeMagnitude = 0.2f; 

    [SerializeField]
    private BoulderMover _boulderMover;

    // �������� ������� �������
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

    // ����� ��� ������ ������ � �������� ��������������
    public void Shake(float intensity)
    {
        StartCoroutine(ShakeCoroutine(intensity));
    }

    // Coroutine, ����������� ������� ������ � ����������� ���������� � ������������ ��������.
    private IEnumerator ShakeCoroutine(float intensity)
    {
        float elapsed = 0f;
        Vector3 velocity = Vector3.zero;

        while (elapsed < shakeDuration + (intensity / 50f))
        {
            // ����������� ������������� ��� �������� ��������� ������
            float damper = 1.0f - (elapsed / (shakeDuration + (intensity / 50f)));
            
            // ��������� ��������� ��������, ���������� �� ������� ���������, intensity � ����������� ���������
            Vector3 randomOffset = damper * (0.6f * intensity) * shakeMagnitude * UnityEngine.Random.insideUnitCircle;
            
            // ������ ���������� ������ � ������ ���������
            transform.localPosition = Vector3.SmoothDamp(transform.localPosition, initialPosition + randomOffset, ref velocity, 0.02f);
            
            elapsed += Time.deltaTime;
            yield return null;
        }

        // ���������� ������ � �������� ���������
        transform.localPosition = initialPosition;
    }

    private void OnDisable()
    {
        if (_boulderMover)
            _boulderMover.OnBoulderFlewDistance -= Shake;
    }
}

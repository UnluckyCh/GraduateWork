using System;
using System.Collections;
using UnityEngine;

public class GravityRotator : MonoBehaviour
{
    public event Action OnRotationCompleted;

    [SerializeField] private float _rotationSpeed = 100f;

    public void RotateWorld(GravityDirection newGravity)
    {
        float targetAngle = GetTargetRotation(newGravity);
        //StopAllCoroutines();
        StartCoroutine(RotateToAngle(targetAngle));
    }

    private float GetTargetRotation(GravityDirection gravityDirection)
    {
        return gravityDirection switch
        {
            GravityDirection.Up => 180f,
            GravityDirection.Right => -90f,
            GravityDirection.Down => 0f,
            GravityDirection.Left => 90f,
            _ => 0f,
        };
    }

    private IEnumerator RotateToAngle(float targetAngle)
    {
        Quaternion startRotation = transform.rotation;
        Quaternion endRotation = Quaternion.Euler(0f, 0f, targetAngle);
        float elapsed = 0f;

        while (elapsed < 1f)
        {
            transform.rotation = Quaternion.Lerp(startRotation, endRotation, elapsed);
            elapsed += Time.deltaTime * (_rotationSpeed / 90f);
            yield return null;
        }

        transform.rotation = endRotation;

        OnRotationCompleted?.Invoke();
    }
}

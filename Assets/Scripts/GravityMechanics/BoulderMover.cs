using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoulderMover : MonoBehaviour
{
    public event Action OnBoulderFalled;
    public event Action<float> OnBoulderFlewDistance;

    public bool BoulderIsFalling {  get; private set; }

    [SerializeField]
    private float _gravityAcceleration = 12.8f;

    [SerializeField]
    private GravityNode _startNode;

    private GravityNode _currentNode;

    private void Start()
    {
        GravityController.Instance.OnGravityChangeFinished += ChangeGravity;

        if (_startNode != null)
        {
            Initialize(_startNode);
        }
        else
        {
            Debug.LogError("Start node is not assigned!");
        }
    }

    private void OnDestroy()
    {
        GravityController.Instance.OnGravityChangeFinished -= ChangeGravity;
    }

    public void Initialize(GravityNode startNode)
    {
        _currentNode = startNode;
        transform.position = _currentNode.transform.position;

        List<int> ints = new();
        foreach (int i in ints)
        {
            Debug.Log(i);
        }

        for (int i = 0; i < ints.Count; i++)
        {
            Debug.Log(i);
        }

    }

    private void ChangeGravity(GravityDirection newGravity)
    {
        GravityNode nextNode = _currentNode.GetNextNode(newGravity);

        if (nextNode != null)
        {
            StopAllCoroutines();
            StartCoroutine(FallToNode(nextNode));
        }
    }

    private IEnumerator FallToNode(GravityNode targetNode)
    {
        BoulderIsFalling = true;
        _currentNode = targetNode;
        Vector3 start = transform.position;
        Vector3 end = targetNode.transform.position;

        float distance = Vector3.Distance(start, end);
        float velocity = 0f;
        float positionOffset = 0f;
        float elapsed = 0f;

        while (positionOffset < distance)
        {
            velocity += _gravityAcceleration * Time.deltaTime;
            positionOffset += velocity * Time.deltaTime;

            float progress = positionOffset / distance;
            transform.position = Vector3.Lerp(start, end, Mathf.Clamp01(progress));

            elapsed += Time.deltaTime;
            yield return null;
        }

        if (distance > 0.01f)
            OnBoulderFlewDistance.Invoke(distance);

        transform.position = end;
        BoulderIsFalling = false;
        OnBoulderFalled?.Invoke();
    }
}

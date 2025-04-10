using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnomalysManager : MonoBehaviour
{
    public SimplePlayerController _playerController;
    public List<GameObject> magFunnels; // ������ ��� �������� ���� �������
    public GameObject player; // ��������� ���� ��� �������� ������ �� �������� ���������

    public float minXOffset = 0.5f;
    public float maxXOffset = 2f;
    public float minYOffset = 0.5f;
    public float maxYOffset = 2f;

    private readonly Dictionary<GameObject, Vector3> originalPositions = new();

    void Start()
    {
        // ��������� ��� ������� ��� ������
        DisableAllMagFunnels();

        _playerController.OnPlayerDead += DisableEvent;
    }

    public void EnableEvent()
    {
        StartCoroutine(StartMagFunnelsNearPlayer());
    }

    public void DisableEvent()
    {
        StopAllCoroutines();
    }

    private IEnumerator StartMagFunnelsNearPlayer()
    {
        while (true) // ��������� ����������� ����, ������� ����� ����� ��������� ���������� �������
        {
            // �������� ������ ���������� �������
            List<GameObject> inactiveMagFunnels = GetInactiveMagFunnels();

            if (inactiveMagFunnels.Count > 0)
            {
                // ��������� ���������� ������� �� ���������� �� ������
                inactiveMagFunnels.Sort((a, b) =>
                    Vector3.Distance(a.transform.position, player.transform.position).CompareTo(
                    Vector3.Distance(b.transform.position, player.transform.position)));

                // ����� ������ (���������) ���������� ������� �� ������
                GameObject nearestMagFunnel = inactiveMagFunnels[0];

                // �������� �������� ������� ������� ����� � ����������
                Vector3 originalPosition;
                if (originalPositions.ContainsKey(nearestMagFunnel))
                {
                    originalPosition = originalPositions[nearestMagFunnel];
                }
                else
                {
                    originalPosition = nearestMagFunnel.transform.position;
                    originalPositions[nearestMagFunnel] = originalPosition;
                }

                // ���������� ��������� �������� �� x � y
                float randomXOffset = Random.Range(minXOffset, maxXOffset);
                float randomYOffset = Random.Range(minYOffset, maxYOffset);

                // ������������ ����� ������� � ������ ���������� �������� �� �������� �������
                Vector3 offset = new(randomXOffset, randomYOffset, 0f);
                Vector3 newPosition = originalPosition + offset;

                // ������������� ����� ������� � �������� ��������� ���������� �������
                nearestMagFunnel.transform.position = newPosition;
                nearestMagFunnel.SetActive(true);
            }

            yield return new WaitForSeconds(0.9f);
        }
    }

    private List<GameObject> GetInactiveMagFunnels()
    {
        List<GameObject> inactiveFunnels = new List<GameObject>();

        // ���������� ��� ������� � ��������� ���������� � ������
        foreach (GameObject magFunnel in magFunnels)
        {
            if (magFunnel != null && !magFunnel.activeSelf)
            {
                inactiveFunnels.Add(magFunnel);
            }
        }

        return inactiveFunnels;
    }

    private void DisableAllMagFunnels()
    {
        // ��������� ��� ������� � ������ � ��������� �������� ������� ������
        foreach (GameObject magFunnel in magFunnels)
        {
            if (magFunnel != null)
            {
                // ��������� �������� ������� ����� ������������
                originalPositions[magFunnel] = magFunnel.transform.position;
                magFunnel.SetActive(false);
            }
        }
    }

    private void OnDisable()
    {
        _playerController.OnPlayerDead -= DisableEvent;
    }
}

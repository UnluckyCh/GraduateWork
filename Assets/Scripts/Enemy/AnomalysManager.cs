using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnomalysManager : MonoBehaviour
{
    public SimplePlayerController _playerController;
    public List<GameObject> magFunnels; // Список для хранения всех воронок
    public GameObject player; // Публичное поле для хранения ссылки на игрового персонажа

    public float minXOffset = 0.5f;
    public float maxXOffset = 2f;
    public float minYOffset = 0.5f;
    public float maxYOffset = 2f;

    private readonly Dictionary<GameObject, Vector3> originalPositions = new();

    void Start()
    {
        // Выключаем все воронки при старте
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
        while (true) // Запускаем бесконечный цикл, который будет брать ближайшие неактивные воронки
        {
            // Получаем список неактивных воронок
            List<GameObject> inactiveMagFunnels = GetInactiveMagFunnels();

            if (inactiveMagFunnels.Count > 0)
            {
                // Сортируем неактивные воронки по расстоянию до игрока
                inactiveMagFunnels.Sort((a, b) =>
                    Vector3.Distance(a.transform.position, player.transform.position).CompareTo(
                    Vector3.Distance(b.transform.position, player.transform.position)));

                // Берем первую (ближайшую) неактивную воронку из списка
                GameObject nearestMagFunnel = inactiveMagFunnels[0];

                // Получаем исходную позицию воронки перед её активацией
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

                // Генерируем случайное смещение по x и y
                float randomXOffset = Random.Range(minXOffset, maxXOffset);
                float randomYOffset = Random.Range(minYOffset, maxYOffset);

                // Рассчитываем новую позицию с учетом случайного смещения от исходной позиции
                Vector3 offset = new(randomXOffset, randomYOffset, 0f);
                Vector3 newPosition = originalPosition + offset;

                // Устанавливаем новую позицию и включаем выбранную неактивную воронку
                nearestMagFunnel.transform.position = newPosition;
                nearestMagFunnel.SetActive(true);
            }

            yield return new WaitForSeconds(0.9f);
        }
    }

    private List<GameObject> GetInactiveMagFunnels()
    {
        List<GameObject> inactiveFunnels = new List<GameObject>();

        // Перебираем все воронки и добавляем неактивные в список
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
        // Выключаем все воронки в списке и сохраняем исходную позицию каждой
        foreach (GameObject magFunnel in magFunnels)
        {
            if (magFunnel != null)
            {
                // Сохраняем исходную позицию перед деактивацией
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

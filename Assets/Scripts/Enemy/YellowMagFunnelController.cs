using System.Collections;
using UnityEngine;

public class YellowMagFunnelController : MonoBehaviour
{
    public float fadeInDuration = 1f; // Продолжительность плавного проявления воронки
    public float projectileFadeDuration = 0.5f;
    public float projectileSpeed = 5f; // Скорость снарядов
    public Vector2 projectileOffset = new Vector2(-0.01f, 0.06f); // Оффсет для снарядов внутри воронки
    public GameObject[] projectiles; // Массив для хранения снарядов
    public GameObject player; // Ссылка на объект Player

    private SpriteRenderer[] spriteRenderers;

    private void Awake()
    {
        // Получаем все SpriteRenderer для воронки
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();

        // Начинаем с нулевой прозрачности
        SetAlpha(0f);

        // Включаем Rigidbody2D для каждого снаряда, если его еще нет
        foreach (GameObject projectile in projectiles)
        {
            if (projectile != null && projectile.GetComponent<Rigidbody2D>() == null)
            {
                projectile.AddComponent<Rigidbody2D>();
            }
        }

        // Выключаем все снаряды
        SetProjectilesActive(false);
    }

    private void OnEnable()
    {
        // Начинаем процесс плавного проявления воронки
        StartCoroutine(FadeIn());
    }

    private void OnDisable()
    {
        // Останавливаем все корутины при отключении воронки
        StopAllCoroutines();
    }

    private IEnumerator FadeIn()
    {
        float timer = 0f;
        while (timer < fadeInDuration)
        {
            float alpha = Mathf.Lerp(0f, 1f, timer / fadeInDuration);
            SetAlpha(alpha);
            timer += Time.deltaTime;
            yield return null;
        }

        // Задержка перед появлением снарядов
        yield return new WaitForSeconds(fadeInDuration);

        // Плавное появление снарядов и перемещение внутрь воронки
        StartCoroutine(FadeInProjectiles());

        // Задержка перед началом плавного исчезновения воронки
        yield return new WaitForSeconds(4f);

        // Начинаем процесс плавного исчезновения воронки
        StartCoroutine(FadeOut());
    }

    private IEnumerator FadeInProjectiles()
    {
        // Проходим по массиву снарядов поочередно
        for (int i = 0; i < projectiles.Length; i++)
        {
            GameObject projectile = projectiles[i];
            if (projectile != null)
            {
                // Получаем Rigidbody2D снаряда
                Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    // Сбрасываем позицию и скорость снаряда перед включением
                    rb.velocity = Vector2.zero; // Сбрасываем скорость
                    projectile.transform.position = transform.position; // Помещаем воронку

                    // Включаем снаряд
                    projectile.SetActive(true);

                    // Перемещаем снаряд внутрь воронки с заданным оффсетом
                    projectile.transform.Translate(projectileOffset);

                    // Задаем направление снаряда в сторону игрока
                    Vector2 directionToPlayer = ((Vector2)player.transform.position + new Vector2(0f, 0.8f) - (Vector2)transform.position).normalized;
                    rb.velocity = directionToPlayer * projectileSpeed;

                    // Плавно изменяем прозрачность снаряда
                    StartCoroutine(FadeInProjectile(projectile));

                    // Ждем перед активацией следующего снаряда
                    if (i < projectiles.Length - 1) // Если это не последний снаряд
                    {
                        yield return new WaitForSeconds(0.5f); // Задержка между активациями снарядов (примерно 0.5 секунды)
                    }
                }
            }
        }
    }

    private IEnumerator FadeInProjectile(GameObject projectile)
    {
        SpriteRenderer renderer = projectile.GetComponent<SpriteRenderer>();
        if (renderer != null)
        {
            float timer = 0f;
            while (timer < projectileFadeDuration)
            {
                float alpha = Mathf.Lerp(0f, 1f, timer / projectileFadeDuration);
                Color color = renderer.color;
                color.a = alpha;
                renderer.color = color;
                timer += Time.deltaTime;
                yield return null;
            }
        }
    }

    private IEnumerator FadeOut()
    {
        float timer = 0f;
        while (timer < fadeInDuration)
        {
            float alpha = Mathf.Lerp(1f, 0f, timer / fadeInDuration);
            SetAlpha(alpha);
            timer += Time.deltaTime;
            yield return null;
        }

        SetProjectilesActive(false);
        // По завершении плавного исчезновения воронки отключаем ее
        gameObject.SetActive(false);
    }

    private void SetAlpha(float alpha)
    {
        // Устанавливаем прозрачность для всех SpriteRenderer воронки
        foreach (SpriteRenderer renderer in spriteRenderers)
        {
            Color color = renderer.color;
            color.a = alpha;
            renderer.color = color;
        }
    }

    private void SetProjectilesActive(bool active)
    {
        // Включаем или выключаем все снаряды
        foreach (GameObject projectile in projectiles)
        {
            if (projectile != null)
            {
                projectile.SetActive(active);
            }
        }
    }
}

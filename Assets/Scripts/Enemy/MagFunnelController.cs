using System.Collections;
using UnityEngine;

public class MagFunnelController : MonoBehaviour
{
    public float fadeInDuration = 1f; // Продолжительность плавного проявления воронки
    public float projectileFadeDuration = 0.3f;
    public float projectileSpeed = 5f; // Скорость снарядов
    public Vector2 projectileOffset = new Vector2(-0.01f, 0.06f); // Оффсет для снарядов внутри воронки
    public GameObject[] projectiles; // Массив для хранения снарядов

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
        foreach (GameObject projectile in projectiles)
        {
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

                    // Задаем случайное направление с постоянной скоростью
                    Vector2 randomDirection = Random.insideUnitCircle.normalized;
                    rb.velocity = randomDirection * projectileSpeed;

                    // Плавно изменяем прозрачность снаряда
                    float timer = 0f;
                    while (timer < projectileFadeDuration)
                    {
                        float alpha = Mathf.Lerp(0f, 1f, timer / projectileFadeDuration);
                        SetProjectileAlpha(projectile, alpha);
                        timer += Time.deltaTime;
                        yield return null;
                    }
                }
            }
        }
    }

    private void SetProjectileAlpha(GameObject projectile, float alpha)
    {
        // Получаем SpriteRenderer снаряда и устанавливаем прозрачность
        SpriteRenderer renderer = projectile.GetComponent<SpriteRenderer>();
        if (renderer != null)
        {
            Color color = renderer.color;
            color.a = alpha;
            renderer.color = color;
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

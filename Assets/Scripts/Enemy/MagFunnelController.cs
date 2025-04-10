using System.Collections;
using UnityEngine;

public class MagFunnelController : MonoBehaviour
{
    public float fadeInDuration = 1f; // ����������������� �������� ���������� �������
    public float projectileFadeDuration = 0.3f;
    public float projectileSpeed = 5f; // �������� ��������
    public Vector2 projectileOffset = new Vector2(-0.01f, 0.06f); // ������ ��� �������� ������ �������
    public GameObject[] projectiles; // ������ ��� �������� ��������

    private SpriteRenderer[] spriteRenderers;

    private void Awake()
    {
        // �������� ��� SpriteRenderer ��� �������
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();

        // �������� � ������� ������������
        SetAlpha(0f);

        // �������� Rigidbody2D ��� ������� �������, ���� ��� ��� ���
        foreach (GameObject projectile in projectiles)
        {
            if (projectile != null && projectile.GetComponent<Rigidbody2D>() == null)
            {
                projectile.AddComponent<Rigidbody2D>();
            }
        }

        // ��������� ��� �������
        SetProjectilesActive(false);
    }

    private void OnEnable()
    {
        // �������� ������� �������� ���������� �������
        StartCoroutine(FadeIn());
    }

    private void OnDisable()
    {
        // ������������� ��� �������� ��� ���������� �������
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

        // �������� ����� ���������� ��������
        yield return new WaitForSeconds(fadeInDuration);

        // ������� ��������� �������� � ����������� ������ �������
        StartCoroutine(FadeInProjectiles());

        // �������� ����� ������� �������� ������������ �������
        yield return new WaitForSeconds(4f);

        // �������� ������� �������� ������������ �������
        StartCoroutine(FadeOut());
    }

    private IEnumerator FadeInProjectiles()
    {
        foreach (GameObject projectile in projectiles)
        {
            if (projectile != null)
            {
                // �������� Rigidbody2D �������
                Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    // ���������� ������� � �������� ������� ����� ����������
                    rb.velocity = Vector2.zero; // ���������� ��������
                    projectile.transform.position = transform.position; // �������� �������

                    // �������� ������
                    projectile.SetActive(true);

                    // ���������� ������ ������ ������� � �������� ��������
                    projectile.transform.Translate(projectileOffset);

                    // ������ ��������� ����������� � ���������� ���������
                    Vector2 randomDirection = Random.insideUnitCircle.normalized;
                    rb.velocity = randomDirection * projectileSpeed;

                    // ������ �������� ������������ �������
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
        // �������� SpriteRenderer ������� � ������������� ������������
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
        // �� ���������� �������� ������������ ������� ��������� ��
        gameObject.SetActive(false);
    }

    private void SetAlpha(float alpha)
    {
        // ������������� ������������ ��� ���� SpriteRenderer �������
        foreach (SpriteRenderer renderer in spriteRenderers)
        {
            Color color = renderer.color;
            color.a = alpha;
            renderer.color = color;
        }
    }

    private void SetProjectilesActive(bool active)
    {
        // �������� ��� ��������� ��� �������
        foreach (GameObject projectile in projectiles)
        {
            if (projectile != null)
            {
                projectile.SetActive(active);
            }
        }
    }
}

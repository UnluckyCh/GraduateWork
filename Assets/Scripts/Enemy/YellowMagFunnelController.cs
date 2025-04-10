using System.Collections;
using UnityEngine;

public class YellowMagFunnelController : MonoBehaviour
{
    public float fadeInDuration = 1f; // ����������������� �������� ���������� �������
    public float projectileFadeDuration = 0.5f;
    public float projectileSpeed = 5f; // �������� ��������
    public Vector2 projectileOffset = new Vector2(-0.01f, 0.06f); // ������ ��� �������� ������ �������
    public GameObject[] projectiles; // ������ ��� �������� ��������
    public GameObject player; // ������ �� ������ Player

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
        // �������� �� ������� �������� ����������
        for (int i = 0; i < projectiles.Length; i++)
        {
            GameObject projectile = projectiles[i];
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

                    // ������ ����������� ������� � ������� ������
                    Vector2 directionToPlayer = ((Vector2)player.transform.position + new Vector2(0f, 0.8f) - (Vector2)transform.position).normalized;
                    rb.velocity = directionToPlayer * projectileSpeed;

                    // ������ �������� ������������ �������
                    StartCoroutine(FadeInProjectile(projectile));

                    // ���� ����� ���������� ���������� �������
                    if (i < projectiles.Length - 1) // ���� ��� �� ��������� ������
                    {
                        yield return new WaitForSeconds(0.5f); // �������� ����� ����������� �������� (�������� 0.5 �������)
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

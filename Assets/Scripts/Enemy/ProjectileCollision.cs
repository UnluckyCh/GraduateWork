using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public Sprite[] explosionSprites; // ������ �������� ��� ������
    public float explosionScaleMultiplier = 1.5f;
    private SpriteRenderer spriteRenderer;
    private Sprite initialSprite;
    private Vector3 initialScale;
    private Collider2D projectileCollider;
    public AudioSource blastSound;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        initialSprite = spriteRenderer.sprite;
        initialScale = new Vector3(3.9f, 3.9f, 3.9f);
        transform.localScale = initialScale;

        projectileCollider = GetComponent<Collider2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // ���������, ��� ����������� � ���������� �����������
        if (!collision.collider.isTrigger)
        {
            // ������������� �������� �������
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = Vector2.zero;
            }

            if (collision.collider.CompareTag("Player"))
            {
                // �������� ��������� SimplePlayerController � �������� ����� TakeDamage(1)
                SimplePlayerController playerController = collision.collider.GetComponent<SimplePlayerController>();
                if (playerController != null)
                {
                    playerController.TakeDamage(1);
                }
            }

            // ��������� ��������� �������
            DisableCollider();

            // ������������� ������� ������
            if (explosionSprites.Length > 0)
            {
                StartCoroutine(ExplodeAndDisable());
            }
        }
    }

    private IEnumerator ExplodeAndDisable()
    {
        // ����������� ������ ������� ��� �������� ������
        float explosionDuration = 0.5f;
        float timer = 0f;

        // �������� ������� ��������� ��� ������������ �����
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        Vector3 playerPos = playerObject.transform.position;
        Vector3 soundPosition = new Vector3(playerPos.x + 1f, playerPos.y, playerPos.z); // ������� ����� ������ �� ���������

        while (timer < explosionDuration)
        {
            float scale = Mathf.Lerp(1f, explosionScaleMultiplier, timer / explosionDuration);
            transform.localScale = initialScale * scale;
            timer += Time.deltaTime;
            yield return null;
        }


        blastSound.transform.position = soundPosition;
        blastSound.Play();

        // ������������� �������� �������
        for (int i = 0; i < explosionSprites.Length; i++)
        {
            spriteRenderer.sprite = explosionSprites[i];
            yield return new WaitForSeconds(0.1f); // �������� ����� ������ ��������
        }

        // ������������ � ������������ ������� ��� ��������� �����
        transform.localScale = initialScale;

        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        // ��� ��������� ������� ���������� ������ �� ��������� ��������� ������
        spriteRenderer.sprite = initialSprite;

        // �������� ��������� �������
        EnableCollider();
    }

    private void OnDisable()
    {
        // ��������� ��������� ������� ��� ��� �����������
        DisableCollider();
    }

    private void EnableCollider()
    {
        if (projectileCollider != null)
        {
            projectileCollider.enabled = true;
        }
    }

    private void DisableCollider()
    {
        if (projectileCollider != null)
        {
            projectileCollider.enabled = false;
        }
    }
}

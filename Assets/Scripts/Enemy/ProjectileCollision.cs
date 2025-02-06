using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public Sprite[] explosionSprites; // Массив спрайтов для взрыва
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
        // Проверяем, что столкнулись с физическим коллайдером
        if (!collision.collider.isTrigger)
        {
            // Останавливаем движение снаряда
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = Vector2.zero;
            }

            if (collision.collider.CompareTag("Player"))
            {
                // Получаем компонент SimplePlayerController и вызываем метод TakeDamage(1)
                SimplePlayerController playerController = collision.collider.GetComponent<SimplePlayerController>();
                if (playerController != null)
                {
                    playerController.TakeDamage(1);
                }
            }

            // Отключаем коллайдер снаряда
            DisableCollider();

            // Устанавливаем спрайты взрыва
            if (explosionSprites.Length > 0)
            {
                StartCoroutine(ExplodeAndDisable());
            }
        }
    }

    private IEnumerator ExplodeAndDisable()
    {
        // Увеличиваем размер спрайта для анимации взрыва
        float explosionDuration = 0.5f;
        float timer = 0f;

        // Получаем позицию персонажа для расположения звука
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        Vector3 playerPos = playerObject.transform.position;
        Vector3 soundPosition = new Vector3(playerPos.x + 1f, playerPos.y, playerPos.z); // Позиция звука справа от персонажа

        while (timer < explosionDuration)
        {
            float scale = Mathf.Lerp(1f, explosionScaleMultiplier, timer / explosionDuration);
            transform.localScale = initialScale * scale;
            timer += Time.deltaTime;
            yield return null;
        }


        blastSound.transform.position = soundPosition;
        blastSound.Play();

        // Устанавливаем взрывные спрайты
        for (int i = 0; i < explosionSprites.Length; i++)
        {
            spriteRenderer.sprite = explosionSprites[i];
            yield return new WaitForSeconds(0.1f); // Задержка между сменой спрайтов
        }

        // Возвращаемся к изначальному размеру при появлении вновь
        transform.localScale = initialScale;

        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        // При включении снаряда сбрасываем спрайт на начальный статичный спрайт
        spriteRenderer.sprite = initialSprite;

        // Включаем коллайдер снаряда
        EnableCollider();
    }

    private void OnDisable()
    {
        // Отключаем коллайдер снаряда при его деактивации
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

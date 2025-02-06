using UnityEngine;
using System.Collections;

public class EnemySpriteAnimation : MonoBehaviour
{
    public Sprite[] idleSprites;
    public Sprite[] attackSprites;
    public float idleFrameRate = 0.25f;
    public float attackFrameRate = 0.15f;
    public float attackDuration = 0.4f;
    public SimplePlayerController playerController;
    public AudioSource biteSound;

    private float timer = 0f;
    private int currentSpriteIndex = 0;
    private SpriteRenderer spriteRenderer;
    private bool isAttacking = false;
    private bool isInAttackRange = false;
    private Collider2D playerCollider;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer component not found on GameObject.");
        }

        playerController.OnPlayerDead += DisableAttack;
    }

    void Update()
    {
        if (!isAttacking)
        {
            timer += Time.deltaTime;

            float currentFrameRate = (isAttacking) ? attackFrameRate : idleFrameRate;

            if (timer >= currentFrameRate)
            {
                timer = 0f;
                currentSpriteIndex = (currentSpriteIndex + 1) % idleSprites.Length;
                spriteRenderer.sprite = idleSprites[currentSpriteIndex];
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (!isAttacking)
            {
                playerCollider = other;
                isAttacking = true;
                StartCoroutine(AttackSequence());
            }
            biteSound.Play();
            isInAttackRange = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other == playerCollider)
            isInAttackRange = false;
    }

    IEnumerator AttackSequence()
    {
        for (int i = 0; i < attackSprites.Length; i++)
        {
            spriteRenderer.sprite = attackSprites[i];
            yield return new WaitForSeconds(attackFrameRate);
        }

        if (isInAttackRange && playerCollider != null && playerCollider.gameObject.CompareTag("Player"))
        {
            playerController.TakeDamage(1);
        }

        yield return new WaitForSeconds(attackDuration);

        if (isInAttackRange && playerCollider != null && playerCollider.gameObject.CompareTag("Player"))
        {
            biteSound.Play();
            StartCoroutine(AttackSequence());
        }
        else
        {
            isAttacking = false;
        }
    }

    public void DisableAttack()
    {
        isInAttackRange = false;
    }

    private void OnDisable()
    {
        playerController.OnPlayerDead -= DisableAttack;
    }
}

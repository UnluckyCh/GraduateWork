using UnityEngine;
using System;

public class SimplePlayerController : MonoBehaviour
{
    public event Action OnPlayerDead;
    public Rigidbody2D Rb => rb;
    public bool Alive => alive;

    public bool ActionLocked => moveLocked || isRotating;

    public GameObject blueGemObject;
    public GameObject shieldAuraObject;
    public AudioSource deathSound;
    public AudioSource damage1Sound;
    public AudioSource damage2Sound;
    public AudioSource lavaSound;
    public GameObject deathObject;

    public float lavaDamageCooldown = 1f;
    public float spikeDamageCooldown = 0.5f;
    public float shieldEffectDuration = 15f;
    public bool moveLocked = false;

    private Rigidbody2D rb;
    private Animator anim;
    private HealthUIUpdater healthUIUpdater;
    private PlayerMovement playerMovement;
    private AttackController _attackController;

    private readonly int direction = 1;
    private float lastLavaDamageTime = 0f;
    private float lastSpikeDamageTime = 0f;
    private float shieldEffectTimer = 0f;
    private bool alive = true;
    private bool inLava = false;
    private bool tookLavaDamage = false;
    private bool inSpike = false;
    private bool damageBlocked = false;
    private bool hasShieldEffect = false;
    private bool damageSwap = false;
    private bool isRotating = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
        healthUIUpdater = FindObjectOfType<HealthUIUpdater>();
        if (healthUIUpdater == null)
        {
            Debug.LogError("HealthUIUpdater not found in the scene!");
        }
        healthUIUpdater.CurrentHealth = healthUIUpdater.MaxHealth;
        shieldAuraObject.SetActive(false);
        if (GravityController.Instance)
        {
            GravityController.Instance.OnGravityChangeStarted += OnGravityChangeStarted;
            GravityController.Instance.OnGravityChangeFinished += OnGravityChangeFinished;
        }

        _attackController = GetComponentInChildren<AttackController>();
        if (!_attackController) Debug.LogError($"Не найден компонент AttackController среди дочерних объектов у {name}");
    }

    private void Update()
    {
        if (hasShieldEffect)
        {
            shieldEffectTimer -= Time.deltaTime;

            if (shieldEffectTimer <= shieldEffectDuration / 4f)
            {
                blueGemObject.SetActive(true);
            }

            if (shieldEffectTimer <= 0f)
            {
                DisableShield();
            }
        }

        if (!alive) return;

        if (!ActionLocked)
        {
            if (GravityController.Instance)
            {
                if (!GravityController.Instance.IsActiveRotate)
                {
                    HandlePlayerActions();
                }
            }
            else
            {
                HandlePlayerActions();
            }
        }

        _attackController.Attack(ActionLocked);
    }

    private void HandlePlayerActions()
    {
        Hurt();
        LookUp();
        playerMovement.Run();
        playerMovement.Jump();

        CheckEnvironmentDamage();
    }

    private void CheckEnvironmentDamage()
    {
        if (inLava && Time.time - lastLavaDamageTime >= lavaDamageCooldown)
        {
            TakeLavaDamage();
        }

        if (inSpike && Time.time - lastSpikeDamageTime >= spikeDamageCooldown)
        {
            TakeSpikeDamage();
        }
    }

    private void OnGravityChangeStarted(GravityDirection newGravity)
    {
        isRotating = true;
        PlayJumpAnimation();
    }

    private void OnGravityChangeFinished(GravityDirection newGravity)
    {
        isRotating = false;
        PlayIdleUpAnimation();
    }

    private void TakeLavaDamage()
    {
        TakeDamage(1);
        tookLavaDamage = true;
        lastLavaDamageTime = Time.time;
    }

    private void TakeSpikeDamage()
    {
        TakeDamage(1);
        lastSpikeDamageTime = Time.time;
    }

    private void DamageSound()
    {
        if (damageSwap)
        {
            damageSwap = false;
            damage1Sound.Play();
        }
        else
        {
            damageSwap = true;
            damage2Sound.Play();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Lava"))
        {
            if (!tookLavaDamage || Time.time - lastLavaDamageTime >= lavaDamageCooldown)
            {
                lavaSound.Play();
                TakeLavaDamage();
            }
            inLava = true;
        }

        if (other.CompareTag("Spike"))
        {
            inSpike = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Lava"))
        {
            inLava = false;
            tookLavaDamage = false;
        }

        if (other.CompareTag("Spike"))
        {
            inSpike = false;
        }
    }

    void Hurt()
    {
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            anim.SetTrigger("hurt");
            if (direction == 1)
                rb.AddForce(new Vector2(-5f, 1f), ForceMode2D.Impulse);
            else
                rb.AddForce(new Vector2(5f, 1f), ForceMode2D.Impulse);
        }
    }

    public void TakeDamage(int amount)
    {
        if (alive && !damageBlocked)
        {
            DamageSound();
            anim.SetTrigger("hurt");
            int currentHealth = healthUIUpdater.CurrentHealth;
            currentHealth -= amount;
            healthUIUpdater.CurrentHealth = currentHealth;
            if (currentHealth <= 0)
            {
                Die();
            }
        }
        else if (alive && damageBlocked)
        {
            damageBlocked = false;
            shieldAuraObject.SetActive(false);
        }
    }

    void Die()
    {
        alive = false;
        deathSound.Play();
        anim.SetTrigger("die");
        OnPlayerDead?.Invoke();
        if (deathObject)
        {
            deathObject.SetActive(true);
            deathObject.GetComponent<DeathMenu>().DeathGame();
        }
    }

    void LookUp()
    {
        anim.SetBool("isLookUp", Input.GetKey(KeyCode.Alpha4));
    }

    public void StopPlayer()
    {
        rb.velocity = Vector2.zero;
        anim.SetBool("isRun", false);
    }

    public void BlockDamage()
    {
        damageBlocked = true;
    }

    public void UnblockDamage()
    {
        damageBlocked = false;
    }

    public void LockPlayerActions()
    {
        moveLocked = true;
    }

    public void UnlockPlayerActions()
    {
        moveLocked = false;
    }

    public void PlayLookUpAnimation()
    {
        anim.SetBool("isRun", false);
        anim.SetBool("isJump", false);
        anim.SetBool("isLookUp", true);
    }

    public void PlayJumpAnimation()
    {
        anim.SetBool("isLookUp", false);
        anim.SetBool("isRun", false);
        anim.SetBool("isJump", true);
    }

    public void PlayIdleUpAnimation()
    {
        anim.SetBool("isRun", false);
        anim.SetBool("isJump", false);
        anim.SetBool("isLookUp", false);
    }

    public void EnableShield()
    {
        damageBlocked = true;
        hasShieldEffect = true;
        shieldEffectTimer = shieldEffectDuration;
        shieldAuraObject.SetActive(true);
    }

    public void DisableShield()
    {
        damageBlocked = false;
        hasShieldEffect = false;
        shieldEffectTimer = 0f;
        shieldAuraObject.SetActive(false);
    }

    private void OnDisable()
    {
        if (GravityController.Instance)
        {
            GravityController.Instance.OnGravityChangeStarted -= OnGravityChangeStarted;
            GravityController.Instance.OnGravityChangeFinished -= OnGravityChangeFinished;
        }
    }
}

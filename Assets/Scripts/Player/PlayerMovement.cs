using System.Linq;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public enum AirControlMode           // тип «сложности»
    {
        Always,              // 1) как сейчас: можно двигаться в воздухе всегда
        LockAfterGravity     // 2) после смены гравитации – до приземления нельзя
    }

    [Header("Movement mode")]
    [SerializeField] private AirControlMode airControlMode = AirControlMode.Always;

    public bool IsGrounded => _isGrounded;

    public float movePower = 6.5f;
    public float jumpPower = 22f;
    public float doubleJumpEffectDuration = 15f;
    public float coyoteTime = 0.1f;
    public GameObject gemObject;
    public GameObject auraObject;
    public AudioSource landingSound;

    private SimplePlayerController _playerController;
    private Rigidbody2D rb;
    private Animator anim;
    private AttackController _attackController;
    private Vector3 originalScale;
    private int direction = 1;
    private float doubleJumpEffectTimer = 0f;
    private float coyoteTimeCounter = 0f;
    private bool isJump = false;
    private bool isFall = true;
    private bool hasDoubleJumpEffect = false;
    private bool _isGrounded = false;

    private bool airControlLocked = false;        // сейчас ли запрещено верх/низ
    private bool fallStartedAfterGravity = false; // чтобы не отпустить сразу же, когда v=0 на первом кадре

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x);
        originalScale = scale;
        auraObject.SetActive(false);
        TryGetComponent(out _playerController);
        _attackController = GetComponentInChildren<AttackController>();
        _playerController.OnPlayerDead += OnPlayerDead;
        GravityController.Instance.OnGravityChangeFinished += OnGravityChangeFinished;
    }

    void Update()
    {
        if (hasDoubleJumpEffect)
        {
            doubleJumpEffectTimer -= Time.deltaTime;
            if (doubleJumpEffectTimer <= 0f)
            {
                DisableDoubleJumpEffect();
            }
        }

        if (airControlLocked)
        {
            // ждём, пока скорость станет не-нулевой (начало реального падения)
            if (!fallStartedAfterGravity && Mathf.Abs(rb.velocity.y) > 0.05f)
                fallStartedAfterGravity = true;

            // а потом, когда после начала падения скорость снова 0, тоже разблокируем
            if (fallStartedAfterGravity && Mathf.Abs(rb.velocity.y) < 0.05f)
                airControlLocked = false;
        }
    }

    public void Run()
    {
        if (GravityController.Instance.IsActiveRotate ||
            (airControlMode == AirControlMode.LockAfterGravity && airControlLocked))
            return;

        Vector3 moveVelocity = Vector3.zero;
        anim.SetBool("isRun", false);

        if (Input.GetAxisRaw("Horizontal") < 0)
        {
            direction = -1;
            moveVelocity = Vector3.left;
            if(!_attackController.IsAimingOrAttackLocked)
                transform.localScale = new Vector3(direction * originalScale.x, originalScale.y, originalScale.z);
            if (!anim.GetBool("isJump"))
                anim.SetBool("isRun", true);
        }
        if (Input.GetAxisRaw("Horizontal") > 0)
        {
            direction = 1;
            moveVelocity = Vector3.right;
            if (!_attackController.IsAimingOrAttackLocked)
                transform.localScale = new Vector3(direction * originalScale.x, originalScale.y, originalScale.z);
            if (!anim.GetBool("isJump"))
                anim.SetBool("isRun", true);
        }
        transform.position += movePower * Time.deltaTime * moveVelocity;
    }

    public void Jump()
    {
        // Визуализация рейкаста: рисуем красную линию от центра объекта вниз на 0.4 единиц
        Debug.DrawRay(transform.position, Vector2.down * 0.6f, Color.red);

        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, Vector2.down, 0.6f);
        _isGrounded = hits.Any(hit => hit.collider != null && !hit.collider.isTrigger);

        // Если игрок на земле, то обновляем счетчик "койот тайма"
        if (_isGrounded && !isJump)
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        // Если нажата кнопка прыжка и игрок всё ещё находится в пределах "койот тайма"
        if ((Input.GetButtonDown("Jump") || Input.GetKeyDown(KeyCode.W)) && coyoteTimeCounter > 0f)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpPower);
            anim.SetBool("isJump", true);
            isJump = true;
            isFall = false;
        }
        else if (hasDoubleJumpEffect && isJump && (Input.GetButtonDown("Jump") || Input.GetKeyDown(KeyCode.W)))
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpPower);
            anim.SetBool("isJump", true);
            isJump = false;
        }

        if (hasDoubleJumpEffect && !_isGrounded && isFall && (Input.GetButtonDown("Jump") || Input.GetKeyDown(KeyCode.W)))
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpPower);
            anim.SetBool("isJump", true);
            isFall = false;
        }

        if (rb.velocity.y <= 0 && _isGrounded)
        {
            anim.SetBool("isJump", false);
            if (isJump || !isFall)
            {
                landingSound.Play();
            }
            isJump = false;
            isFall = true;
        }
    }

    private void OnGravityChangeFinished(GravityDirection _)
    {
        if (airControlMode == AirControlMode.LockAfterGravity)
        {
            airControlLocked = true;          // запрещаем Run()
            fallStartedAfterGravity = false;  // ждём, пока реально начнёт падать
        }

        anim.SetBool("isJump", true);
        isJump = true;
        isFall = false;
    }

    public void EnableDoubleJumpEffect()
    {
        hasDoubleJumpEffect = true;
        doubleJumpEffectTimer = doubleJumpEffectDuration;
        auraObject.SetActive(true);
    }

    public void DisableDoubleJumpEffect()
    {
        hasDoubleJumpEffect = false;
        doubleJumpEffectTimer = 0f;
        gemObject.SetActive(true);
        auraObject.SetActive(false);
    }

    private void OnPlayerDead() => DisableDoubleJumpEffect();

    private void OnDisable()
    {
        _playerController.OnPlayerDead -= OnPlayerDead;
    }
}

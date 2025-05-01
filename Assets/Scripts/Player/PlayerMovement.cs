using System;
using System.Linq;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public enum AirControlMode
    {
        Always,
        LockAfterGravity
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

    private bool airControlLocked = false;
    private bool fallStartedAfterGravity = false;

    private float _jumpClickBlockTimer = 0f;
    private const float JUMPCLICKBLOCKDURATION = 0.1f;

    private bool _onlyFallingBoulderUnderFoot = false;

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
        DetectGround();

        if (hasDoubleJumpEffect)
        {
            doubleJumpEffectTimer -= Time.deltaTime;
            if (doubleJumpEffectTimer <= 0f)
            {
                DisableDoubleJumpEffect();
            }
        }

        if (_jumpClickBlockTimer > 0f)
        {
            _jumpClickBlockTimer -= Time.deltaTime;
        }

        if (airControlLocked)
        {
            if (_onlyFallingBoulderUnderFoot)
                return;

            if (_isGrounded)
            {
                airControlLocked = false;
            }
            else
            {
                if (!fallStartedAfterGravity && Mathf.Abs(rb.velocity.y) > 0.05f)
                    fallStartedAfterGravity = true;

                if (fallStartedAfterGravity && Mathf.Abs(rb.velocity.y) < 0.05f)
                    airControlLocked = false;
            }
        }
    }

    private void DetectGround()
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, Vector2.down, 0.6f);

        bool anyStableSurface = false;
        bool anyFallingBoulder = false;

        foreach (var hit in hits)
        {
            if (hit.collider == null || hit.collider.isTrigger) continue;

            if (hit.collider.TryGetComponent<BoulderMover>(out var boulder))
            {
                if (boulder.BoulderIsFalling)
                    anyFallingBoulder = true;
                else
                    anyStableSurface = true;
            }
            else
            {
                anyStableSurface = true;
            }
        }

        _isGrounded = anyStableSurface;
        _onlyFallingBoulderUnderFoot = !anyStableSurface && anyFallingBoulder;
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
        Debug.DrawRay(transform.position, Vector2.down * 0.6f, Color.red);

        //RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, Vector2.down, 0.6f);
        //_isGrounded = hits.Any(IsSolidGround);

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
        if ((Input.GetButtonDown("Jump") || Input.GetKeyDown(KeyCode.W)) && coyoteTimeCounter > 0f && _jumpClickBlockTimer <= 0f && !_onlyFallingBoulderUnderFoot)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpPower);
            anim.SetBool("isJump", true);
            isJump = true;
            isFall = false;
            _jumpClickBlockTimer = JUMPCLICKBLOCKDURATION;
        }
        else if (hasDoubleJumpEffect && isJump && (Input.GetButtonDown("Jump") || Input.GetKeyDown(KeyCode.W)) && _jumpClickBlockTimer <= 0f && !_onlyFallingBoulderUnderFoot)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpPower);
            anim.SetBool("isJump", true);
            isJump = false;
            _jumpClickBlockTimer = JUMPCLICKBLOCKDURATION;
        }

        if (hasDoubleJumpEffect && !_isGrounded && isFall && (Input.GetButtonDown("Jump") || Input.GetKeyDown(KeyCode.W)) && _jumpClickBlockTimer <= 0f && !_onlyFallingBoulderUnderFoot)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpPower);
            anim.SetBool("isJump", true);
            isFall = false;
            _jumpClickBlockTimer = JUMPCLICKBLOCKDURATION;
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

    private static bool IsSolidGround(RaycastHit2D hit)
    {
        if (hit.collider == null || hit.collider.isTrigger) return false;

        var boulder = hit.collider.GetComponent<BoulderMover>();
        return boulder == null || !boulder.BoulderIsFalling;
    }

    private void OnGravityChangeFinished(GravityDirection _)
    {
        if (airControlMode == AirControlMode.LockAfterGravity)
        {
            airControlLocked = true;          // запрещаем Run()
            fallStartedAfterGravity = false;  // ждём, пока реально начнёт падать
        }

        _isGrounded = false;
        coyoteTimeCounter = 0f;

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

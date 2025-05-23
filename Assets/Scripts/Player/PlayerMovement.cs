using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public enum AirControlMode
    {
        Always,
        LockAfterGravity
    }

    [Header("Movement mode")]
    [SerializeField] private AirControlMode _airControlMode = AirControlMode.Always;

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
    private bool _wasGrounded = true;
    private bool _pendingLanding = false;
    private float _landingTimer = 0f;
    private const float LANDING_DELAY = 0.2f;


    private bool airControlLocked = false;
    private bool fallStartedAfterGravity = false;

    private float _jumpClickBlockTimer = 0f;
    private const float JUMPCLICKBLOCKDURATION = 0.1f;

    private float _lastLandingSfxTime = -999f;
    private const float LANDING_SFX_COOLDOWN = 0.15f;
    private const float MIN_VERTICAL_SPEED_FOR_GROUNDED = 0.05f;

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

        HandleDoubleJumpEffectTimer();
        HandleJumpClickBlockTimer();
        HandleAirControlLock();
        HandleLanding();
        QuickGroundingCheck();

        _wasGrounded = _isGrounded;
    }

    private void HandleDoubleJumpEffectTimer()
    {
        if (hasDoubleJumpEffect)
        {
            doubleJumpEffectTimer -= Time.deltaTime;
            if (doubleJumpEffectTimer <= 0f)
            {
                DisableDoubleJumpEffect();
            }
        }
    }

    private void HandleJumpClickBlockTimer()
    {
        if (_jumpClickBlockTimer > 0f)
        {
            _jumpClickBlockTimer -= Time.deltaTime;
        }
    }

    private void HandleAirControlLock()
    {
        if (!airControlLocked)
            return;

        if (_onlyFallingBoulderUnderFoot)
            return;

        if (_isGrounded)
        {
            airControlLocked = false;
        }
        else
        {
            if (!fallStartedAfterGravity && Mathf.Abs(rb.velocity.y) > 0.05f)
            {
                fallStartedAfterGravity = true;
            }

            if (fallStartedAfterGravity && Mathf.Abs(rb.velocity.y) < 0.05f)
            {
                airControlLocked = false;
            }
        }
    }

    private void HandleLanding()
    {
        if (_pendingLanding && !_isGrounded)
        {
            _pendingLanding = false;
            return;
        }

        if (!_wasGrounded && _isGrounded)
        {
            if (rb.velocity.y <= 0f)
            {
                if (!GravityController.Instance.IsActiveRotate)
                    PlayLandingSound();
            }
            else
            {
                _pendingLanding = true;
                _landingTimer = LANDING_DELAY;
            }
        }

        if (_pendingLanding)
        {
            _landingTimer -= Time.deltaTime;
            if (_landingTimer <= 0f)
            {
                if (_isGrounded && !GravityController.Instance.IsActiveRotate)
                    PlayLandingSound();
            }
        }
    }

    private void QuickGroundingCheck()
    {
        if (isJump && _isGrounded && Mathf.Abs(rb.velocity.y) < MIN_VERTICAL_SPEED_FOR_GROUNDED && !_pendingLanding)
        {
            anim.SetBool("isJump", false);
            isJump = false;
            isFall = true;
        }
    }

    private void PlayLandingSound()
    {
        if (Time.time - _lastLandingSfxTime < LANDING_SFX_COOLDOWN) return;

        landingSound.Play();
        _lastLandingSfxTime = Time.time;

        _pendingLanding = false;
        _landingTimer = 0f;

        anim.SetBool("isJump", false);
        isJump = false;
        isFall = true;
    }

    private void DetectGround()
    {
        Vector2 originLeft = (Vector2)transform.position + Vector2.left * 0.1f + Vector2.up * 0.1f;
        Vector2 originRight = (Vector2)transform.position + Vector2.right * 0.1f + Vector2.up * 0.1f;
        float rayLength = 0.6f;

        RaycastHit2D[] hitsLeft = Physics2D.RaycastAll(originLeft, Vector2.down, rayLength);
        RaycastHit2D[] hitsRight = Physics2D.RaycastAll(originRight, Vector2.down, rayLength);

        Debug.DrawRay(originLeft, Vector2.down * rayLength, Color.red);
        Debug.DrawRay(originRight, Vector2.down * rayLength, Color.red);

        bool anyStableSurface = false;
        bool anyFallingBoulder = false;

        ProcessHits(hitsLeft, ref anyStableSurface, ref anyFallingBoulder);
        ProcessHits(hitsRight, ref anyStableSurface, ref anyFallingBoulder);

        _isGrounded = anyStableSurface;
        _onlyFallingBoulderUnderFoot = !anyStableSurface && anyFallingBoulder;
    }

    private void ProcessHits(RaycastHit2D[] hits, ref bool anyStableSurface, ref bool anyFallingBoulder)
    {
        foreach (var hit in hits)
        {
            if (hit.collider == null || hit.collider.isTrigger || hit.collider.CompareTag("Player")) { continue; }

            if (hit.collider.TryGetComponent<BoulderMover>(out var boulder))
            {
                if (boulder.BoulderIsFalling)
                {
                    anyFallingBoulder = true;
                }
                else
                {
                    anyStableSurface = true;
                }
            }
            else
            {
                anyStableSurface = true;
            }
        }
    }

    public void Run()
    {
        if (GravityController.Instance.IsActiveRotate ||
            (_airControlMode == AirControlMode.LockAfterGravity && airControlLocked))
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
        //RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, Vector2.down, 0.6f);
        //_isGrounded = hits.Any(IsSolidGround);

        // ���� ����� �� �����, �� ��������� ������� "����� �����"
        if (_isGrounded && !isJump)
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        // ���� ������ ������ ������ � ����� �� ��� ��������� � �������� "����� �����"
        if ((Input.GetButtonDown("Jump") || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) && coyoteTimeCounter > 0f && _jumpClickBlockTimer <= 0f && !_onlyFallingBoulderUnderFoot)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpPower);
            anim.SetBool("isJump", true);
            isJump = true;
            isFall = false;
            _jumpClickBlockTimer = JUMPCLICKBLOCKDURATION;
            coyoteTimeCounter = 0f;
        }
        else if (hasDoubleJumpEffect && isJump && (Input.GetButtonDown("Jump") || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) && _jumpClickBlockTimer <= 0f && !_onlyFallingBoulderUnderFoot)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpPower);
            anim.SetBool("isJump", true);
            isJump = false;
            _jumpClickBlockTimer = JUMPCLICKBLOCKDURATION;
            coyoteTimeCounter = 0f;
        }

        if (hasDoubleJumpEffect && !_isGrounded && isFall && (Input.GetButtonDown("Jump") || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) && _jumpClickBlockTimer <= 0f && !_onlyFallingBoulderUnderFoot)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpPower);
            anim.SetBool("isJump", true);
            isFall = false;
            _jumpClickBlockTimer = JUMPCLICKBLOCKDURATION;
            coyoteTimeCounter = 0f;
        }

/*        if (rb.velocity.y <= 0 && _isGrounded)
        {
            anim.SetBool("isJump", false);
            if (isJump || !isFall)
            {
                landingSound.Play();
            }
            isJump = false;
            isFall = true;
        }*/
    }

    private static bool IsSolidGround(RaycastHit2D hit)
    {
        if (hit.collider == null || hit.collider.isTrigger) return false;

        var boulder = hit.collider.GetComponent<BoulderMover>();
        return boulder == null || !boulder.BoulderIsFalling;
    }

    private void OnGravityChangeFinished(GravityDirection _)
    {
        if (_airControlMode == AirControlMode.LockAfterGravity)
        {
            airControlLocked = true;          // ��������� Run()
            fallStartedAfterGravity = false;  // ���, ���� ������� ����� ������
        }

        _isGrounded = false;
        _wasGrounded = false;
        _pendingLanding = false;
        _landingTimer = 0f;

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

    public void SetAirControlMode(AirControlMode mode)
    {
        _airControlMode = mode;
    }

    private void OnPlayerDead() => DisableDoubleJumpEffect();

    private void OnDisable()
    {
        _playerController.OnPlayerDead -= OnPlayerDead;
    }
}

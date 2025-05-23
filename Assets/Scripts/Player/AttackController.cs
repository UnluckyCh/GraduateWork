using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class AttackController : MonoBehaviour
{
    public bool IsAimingOrAttackLocked => _isAttacking || _isAttackAnimationPlaying;

    [SerializeField] private AudioSource _attackEndSound;
    [SerializeField] private AudioSource _attackStartSound;

    private Animator _animator;
    private TrajectoryController _trajectoryController;
    private Transform _playerTransform;

    private bool _isAttacking = false;
    private bool _isAttackAnimationPlaying = false;
    private bool _lastAimFacingRight = true;

    // Флаг, указывающий, что пользователь отпустил кнопку во время блокировки
    private bool _attackFinishRequested = false;

    [SerializeField]
    private float attackDirectionLockTime = 0.5f;

    [SerializeField]
    private ProjectileLauncher projectileLauncher;

    private void Start()
    {
        _animator = GetComponentInParent<Animator>();
        if (!_animator)
            Debug.LogError($"Не найден компонент Animator среди родительских объектов у {name}");

        _trajectoryController = GetComponentInChildren<TrajectoryController>();
        if (!_trajectoryController)
            Debug.LogError($"Не найден компонент TrajectoryController среди дочерних объектов у {name}");

        _playerTransform = _animator.transform;
    }

    // Этот метод должен вызываться каждый кадр (например, из Update)
    public void Attack(bool actionsLocked)
    {
        // Если ранее был зафиксирован запрос на завершение, и сейчас блокировки нет — завершаем атаку
        if (_attackFinishRequested && !actionsLocked)
        {
            ExecuteAttackEnd();
            _attackFinishRequested = false;
        }

        // Если атака в процессе и кнопка отпущена...
        if (_isAttacking && Input.GetMouseButtonUp(0))
        {
            if (actionsLocked)
            {
                // Если действия заблокированы — сохраняем запрос на завершение
                _attackFinishRequested = true;
            }
            else
            {
                // Если блокировки нет — завершаем атаку сразу
                ExecuteAttackEnd();
            }
        }

        // Начинаем атаку при нажатии кнопки, если блокировки нет и курсор не над UI
        if (!actionsLocked)
        {
            if (!_isAttacking && !EventSystem.current.IsPointerOverGameObject() && Input.GetMouseButtonDown(0))
            {
                _isAttacking = true;
                _isAttackAnimationPlaying = false;
                _animator.SetTrigger("attackStart");
                if (_attackStartSound)
                {
                    _attackStartSound.Play();
                }
            }
        }
    }

    private void LateUpdate()
    {
        if (_isAttacking)
        {
            UpdateAimingRotation();
            _trajectoryController.DrawTrajectory();
        }
        else if (_isAttackAnimationPlaying)
        {
            LockAimingRotation();
        }
    }

    private void ExecuteAttackEnd()
    {
        // Если атака уже завершена, выходим (на всякий случай)
        if (!_isAttacking)
            return;

        _isAttacking = false;
        _lastAimFacingRight = (_playerTransform.localScale.x > 0);
        _isAttackAnimationPlaying = true;
        _trajectoryController.ClearTrajectory();
        _animator.SetTrigger("attackEnd");

        if (_attackEndSound)
        {
            _attackEndSound.Play();
        }

        Vector2 targetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        projectileLauncher.LaunchProjectile(targetPos);

        StartCoroutine(UnlockDirectionAfterDelay(attackDirectionLockTime));
    }

    private void UpdateAimingRotation()
    {
        Vector3 playerPosition = _playerTransform.position;
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPosition.z = playerPosition.z;

        Vector2 direction = mouseWorldPosition - playerPosition;
        if (direction.x >= 0)
        {
            _playerTransform.localScale = new Vector3(Mathf.Abs(_playerTransform.localScale.x),
                                                       _playerTransform.localScale.y,
                                                       _playerTransform.localScale.z);
        }
        else
        {
            _playerTransform.localScale = new Vector3(-Mathf.Abs(_playerTransform.localScale.x),
                                                       _playerTransform.localScale.y,
                                                       _playerTransform.localScale.z);
        }
    }

    private void LockAimingRotation()
    {
        if (_lastAimFacingRight)
        {
            _playerTransform.localScale = new Vector3(Mathf.Abs(_playerTransform.localScale.x),
                                                       _playerTransform.localScale.y,
                                                       _playerTransform.localScale.z);
        }
        else
        {
            _playerTransform.localScale = new Vector3(-Mathf.Abs(_playerTransform.localScale.x),
                                                       _playerTransform.localScale.y,
                                                       _playerTransform.localScale.z);
        }
    }

    private IEnumerator UnlockDirectionAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        _isAttackAnimationPlaying = false;
    }
}

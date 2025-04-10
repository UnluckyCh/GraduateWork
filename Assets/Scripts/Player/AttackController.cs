using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class AttackController : MonoBehaviour
{
    public bool IsAimingOrAttackLocked => _isAttacking || _isAttackAnimationPlaying;

    private Animator _animator;
    private TrajectoryController _trajectoryController;
    private Transform _playerTransform;

    private bool _isAttacking = false;
    private bool _isAttackAnimationPlaying = false;
    private bool _lastAimFacingRight = true;

    // ����, �����������, ��� ������������ �������� ������ �� ����� ����������
    private bool _attackFinishRequested = false;

    [SerializeField]
    private float attackDirectionLockTime = 0.5f;

    [SerializeField]
    private ProjectileLauncher projectileLauncher;

    private void Start()
    {
        _animator = GetComponentInParent<Animator>();
        if (!_animator)
            Debug.LogError($"�� ������ ��������� Animator ����� ������������ �������� � {name}");

        _trajectoryController = GetComponentInChildren<TrajectoryController>();
        if (!_trajectoryController)
            Debug.LogError($"�� ������ ��������� TrajectoryController ����� �������� �������� � {name}");

        _playerTransform = _animator.transform;
    }

    // ���� ����� ������ ���������� ������ ���� (��������, �� Update)
    public void Attack(bool actionsLocked)
    {
        // ���� ����� ��� ������������ ������ �� ����������, � ������ ���������� ��� � ��������� �����
        if (_attackFinishRequested && !actionsLocked)
        {
            ExecuteAttackEnd();
            _attackFinishRequested = false;
        }

        // ���� ����� � �������� � ������ ��������...
        if (_isAttacking && Input.GetMouseButtonUp(0))
        {
            if (actionsLocked)
            {
                // ���� �������� ������������� � ��������� ������ �� ����������
                _attackFinishRequested = true;
            }
            else
            {
                // ���� ���������� ��� � ��������� ����� �����
                ExecuteAttackEnd();
            }
        }

        // �������� ����� ��� ������� ������, ���� ���������� ��� � ������ �� ��� UI
        if (!actionsLocked)
        {
            if (!_isAttacking && !EventSystem.current.IsPointerOverGameObject() && Input.GetMouseButtonDown(0))
            {
                _isAttacking = true;
                _isAttackAnimationPlaying = false;
                _animator.SetTrigger("attackStart");
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
        // ���� ����� ��� ���������, ������� (�� ������ ������)
        if (!_isAttacking)
            return;

        _isAttacking = false;
        _lastAimFacingRight = (_playerTransform.localScale.x > 0);
        _isAttackAnimationPlaying = true;
        _trajectoryController.ClearTrajectory();
        _animator.SetTrigger("attackEnd");

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

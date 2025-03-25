using UnityEngine;

public class Enemy : MonoBehaviour
{
    private Rigidbody2D _rb;
    private bool _isFacingRight;
    private CapsuleCollider2D _collider;

    [SerializeField] private MenuesManager menuMan;
    private GameObject _playerReference;
    private Vector2 _vectorToPlayer;
    [SerializeField] private float _distanceX;
    private float _distanceY;
    [SerializeField] private float _maxSpeed = 3;
    [SerializeField] private Vector2 _moveVelocity;
    [SerializeField] private float _acceleration = 5;
    [SerializeField] private float _deceleration = 20;

    [SerializeField] private float _triggerAttackDistance = 1.4f;
    [SerializeField] private float _triggerMoveDistance = 3;
    [SerializeField] private float _triggerCombatIdleDistance = 7;

    // animations
    private Animator _anim;
    private float _lockedTill;
    private int _currentState;
    private bool _isAttacking;
    [SerializeField] private float _cd = 0;
    private float _attackAnimTime = 0.8f;
    private float _attack_AnimTime;
    [SerializeField] private bool _dead;
    [SerializeField] private bool _hitted;
    [SerializeField] private float _hittedAnimTime = 0.3f;

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _anim = GetComponent<Animator>();
        _collider = GetComponent<CapsuleCollider2D>();
        _playerReference = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        if (!menuMan.Started || menuMan.Paused || menuMan.Died) return;
        AnimStates();
    }

    void FixedUpdate()
    {
        if (!menuMan.Started || menuMan.Paused || menuMan.Died) return;
        // point to player
        MoveToPlayer();
        AttackChecks();
        // AnimStates();
    }

    void MoveToPlayer()
    {
        _distanceX = _playerReference.transform.position.x - transform.position.x;
        _distanceY = _playerReference.transform.position.y - transform.position.y;

        // face to player 
        if (_distanceX >= 0 && !_isFacingRight)
        {
            _isFacingRight = true;
            transform.localScale = new Vector2(-transform.localScale.x, transform.localScale.y);
        }
        else if (_distanceX < 0 && _isFacingRight)
        {
            _isFacingRight = false;
            transform.localScale = new Vector2(-transform.localScale.x, transform.localScale.y);
        }

        if (Mathf.Abs(_distanceX) <= _triggerMoveDistance && Mathf.Abs(_distanceY) <= 1.5f && Mathf.Abs(_distanceX) > _triggerAttackDistance)
        {
            Vector2 targetVelocity = new Vector2(_distanceX, 0).normalized * _maxSpeed;

            _moveVelocity = Vector2.Lerp(_moveVelocity, targetVelocity, _acceleration * Time.deltaTime);
            _rb.linearVelocity = new Vector2(_moveVelocity.x, 0);
        }
        else if (_rb.linearVelocity != Vector2.zero)
        {
            // stop the movement
            _moveVelocity = Vector2.Lerp(_moveVelocity, Vector2.zero, _deceleration * Time.deltaTime);
            _rb.linearVelocity = new Vector2(_moveVelocity.x, 0);
        }

    }

    void AttackChecks()
    {
        if (Mathf.Abs(_distanceX) <= _triggerAttackDistance && _distanceY <= 1.5 && _cd < Time.time)
        {
            _isAttacking = true;
            _cd = Time.time + _attackAnimTime;
        }
    }

    #region Animations States Handler
    private void AnimStates()
    {
        var state = GetState();

        if (state == _currentState) return;
        _anim.CrossFade(state, 0, 0);
        _currentState = state;
    }

    private int GetState()
    {
        if (Time.time < _lockedTill) { return _currentState; }

        // most to less important
        if (_dead) return EnemyAnimations.Death;

        if (_hitted)
        {
            _hitted = false;
            return LockState(EnemyAnimations.Hit, _hittedAnimTime);
        }

        if (_isAttacking)
        {
            _isAttacking = false;
            return LockState(EnemyAnimations.Attack, _attackAnimTime);
        }

        if (Mathf.Abs(_distanceX) <= _triggerMoveDistance && Mathf.Abs(_distanceY) <= 1.5 && Mathf.Abs(_distanceX) >= _triggerAttackDistance) return EnemyAnimations.Run;

        return Mathf.Abs(_distanceX) > _triggerCombatIdleDistance ? EnemyAnimations.Idle : EnemyAnimations.CombatIdle;

        int LockState(int s, float t)
        {
            _lockedTill = Time.time + t;
            return s;
        }
    }
    #endregion
}

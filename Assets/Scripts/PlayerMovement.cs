using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    public PlayerMovementStats MoveStats;
    public PlayerAttackStats AttackStats;
    [SerializeField] private Collider2D _feetCol;
    [SerializeField] private Collider2D _bodyCol;

    private Rigidbody2D _rb;

    // movement vars
    private Vector2 _moveVelocity;
    private bool _isFacingRight;

    // collision check vars
    private RaycastHit2D _groundHit;
    private RaycastHit2D _headHit;
    [SerializeField] private bool _isGrounded;
    private bool _bumpedHead;

    // jump vars
    public float VerticalVelocity { get; private set; }
    private bool _isJumping;
    private bool _isFastFalling;
    private bool _isFalling;
    private float _fastFallTime;
    private float _fastFallReleaseSpeed;
    private int _numberOfJumpsUsed;

    // apex vars
    private float _apexPoint;
    private float _timePastApexThreshold;
    private bool _isPastApexThreshold;

    // jump buffer vars
    private float _jumpBufferTimer;
    // check if player released the jump button before hitting the ground
    private bool _jumpReleasedDuringBuffer;

    // coyote time vars
    private float _coyoteTimer;

    // animations
    private Animator _anim;
    private float _lockedTill;
    private int _currentState;
    private bool _preAttacking_1;
    private bool _isAttacking_1;
    private bool _preAttacking_2;
    private bool _isAttacking_2;
    private bool _isHeavyAttacking;
    private float _attack_1_AnimTime;
    private float _attack_2_AnimTime;
    private float _attack_H_AnimTime;
    [SerializeField] private bool _dead;
    [SerializeField] private bool _hitted;
    [SerializeField] private float _hittedAnimTime = 0.3f;
    [SerializeField] private bool _criticalHitted;
    [SerializeField] private float _criticHittedAnimTime = 0.3f;
    // jump and falling already asigned

    // attack vars
    // is attacking 1 and 2 already asigned
    [SerializeField] private float _attackComboTime = -1;
    [SerializeField] private float _heavyTime;  // time to hold to trigger heavy attack

    [Header("Menu Stuff")]
    [SerializeField] private MenuesManager menuMan;

    void Awake()
    {
        _isFacingRight = true;
        _rb = GetComponent<Rigidbody2D>();
        _anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (!menuMan.Started) return;
        PauseChecks();
        if (menuMan.Paused || menuMan.Died) return;
        CountTimers();

        AttackChecks();
        AnimStates();
        // no jumps while attacking
        if (isAttacking())
        {
            return;
        }
        else { JumpChecks(); }
    }

    void FixedUpdate()
    {
        if (!menuMan.Started || menuMan.Paused || menuMan.Died) return;
        CollisionChecks();

        Jump();

        if (_isGrounded)
        {
            Move(MoveStats.GroundAcceleration, MoveStats.GroundDeceleration, InputManager.Movement);
        }
        else
        {
            Move(MoveStats.AirAcceleration, MoveStats.AirDeceleration, InputManager.Movement);
        }
    }

    #region Movement

    private void Move(float acceleration, float deceleration, Vector2 moveInput)
    {
        if (moveInput != Vector2.zero)
        {
            // cancel attacks anims while moving
            _preAttacking_1 = false;
            _preAttacking_2 = false;
            _isHeavyAttacking = false;

            // check if it needs to turn
            if (moveInput.x > 0 && !_isFacingRight)
            {
                _isFacingRight = true;
                transform.localScale = new Vector2(-transform.localScale.x, transform.localScale.y);
            }
            else if (moveInput.x < 0 && _isFacingRight)
            {
                _isFacingRight = false;
                transform.localScale = new Vector2(-transform.localScale.x, transform.localScale.y);
            }

            Vector2 targetVelocity = new Vector2(moveInput.x, 0f) * MoveStats.MaxMoveSpeed;

            _moveVelocity = Vector2.Lerp(_moveVelocity, targetVelocity, acceleration * Time.deltaTime);
            _rb.linearVelocity = new Vector2(_moveVelocity.x, _rb.linearVelocityY);
        }
        else
        {
            // stop the movement
            _moveVelocity = Vector2.Lerp(_moveVelocity, Vector2.zero, deceleration * Time.deltaTime);
            _rb.linearVelocity = new Vector2(_moveVelocity.x, _rb.linearVelocityY);
        }

    }
    
    #endregion

    #region Jump
    private void JumpChecks()
    {
        // Jump button pressed
        if (InputManager.JumpWasPressed)
        {
            _jumpBufferTimer = MoveStats.JumpBufferTime;
            _jumpReleasedDuringBuffer = false;
        }

        // Jump button released
        if (InputManager.JumpWasReleased)
        {
            if (_jumpBufferTimer > 0f) { _jumpReleasedDuringBuffer = true; }

            if (_isJumping && VerticalVelocity > 0f)
            {
                if (_isPastApexThreshold)
                {
                    _isPastApexThreshold = false;
                    _isFastFalling = true;
                    _fastFallTime = MoveStats.TimeForUpwardsCancel;
                    VerticalVelocity = 0f;
                }
                else
                {
                    _isFastFalling = true;
                    _fastFallReleaseSpeed = VerticalVelocity;
                }
            }
        }

        // Initiate jump with jump buffering and coyote time
        if (_jumpBufferTimer > 0f &&
                !_isJumping &&
                (_isGrounded || _coyoteTimer > 0f))
        {
            InitiateJump(1);

            if (_jumpReleasedDuringBuffer)
            {
                _isFastFalling = true;
                _fastFallReleaseSpeed = VerticalVelocity;
            }
        }

        // Double jump
        else if (_jumpBufferTimer > 0f &&
                _isJumping &&
                _numberOfJumpsUsed < MoveStats.NumberOfJumpsAllowed)
        {
            _isFastFalling = false;
            InitiateJump(1);
        }

        // Air jump after coyote time lapsed
        else if (_jumpBufferTimer > 0f &&
                _isFalling &&
                _numberOfJumpsUsed < MoveStats.NumberOfJumpsAllowed - 1)
        {
            // instead of using two jumps, should just rest 1
            // from the MoveStats.NumberOfJumpsAllowed
            // on this if
            InitiateJump(1);
            _isFastFalling = false;
        }

        // Land
        if ((_isJumping || _isFalling) && _isGrounded && VerticalVelocity <= 0)
        {
            _isJumping = false;
            _isFalling = false;
            _isFastFalling = false;
            _fastFallTime = 0f;
            _isPastApexThreshold = false;
            _numberOfJumpsUsed = 0;
            VerticalVelocity = Physics2D.gravity.y;
        }

    }

    private void InitiateJump(int numberOfJumpsUsed)
    {
        if (!_isJumping) { _isJumping = true; }

        _jumpBufferTimer = 0f;
        _numberOfJumpsUsed += numberOfJumpsUsed;

        VerticalVelocity = MoveStats.InitialJumpVelocity;
    }

    private void Jump()
    {
        // Apply gravity while Jumping
        if (_isJumping)
        {
            // Check for head bump
            if (_bumpedHead)
            {
                _isFastFalling = true;
            }
            // Gravity on ascending
            if (VerticalVelocity >= 0f)
            {
                // Apex controls
                _apexPoint = Mathf.InverseLerp(MoveStats.InitialJumpVelocity, 0f, VerticalVelocity);

                if (_apexPoint > MoveStats.ApexThreshold)
                {
                    if (!_isPastApexThreshold)
                    {
                        _isPastApexThreshold = true;
                        _timePastApexThreshold = 0f;
                    }

                    if (_isPastApexThreshold)
                    {
                        _timePastApexThreshold += Time.fixedDeltaTime;
                        if (_timePastApexThreshold < MoveStats.ApexHangTime)
                        {
                            VerticalVelocity = 0f;
                        }
                        else
                        {
                            VerticalVelocity = -0.01f;
                        }
                    }
                }
                
                // gravity before apex threshold
                else
                {
                    VerticalVelocity += MoveStats.Gravity * Time.fixedDeltaTime;
                    if (_isPastApexThreshold)
                    {
                        _isPastApexThreshold = false;
                    }
                }
            }

            // Gravity on descending
            else if (!_isFastFalling)
            {
                VerticalVelocity += MoveStats.Gravity * MoveStats.GravityOnReleaseMultiplier * Time.fixedDeltaTime;
            }

            else if (VerticalVelocity < 0f)
            {
                if (!_isFalling) { _isFalling = true; }
            }
        }

        // Jump cut
        if (_isFastFalling)
        {
            if (_fastFallTime >= MoveStats.TimeForUpwardsCancel)
            {
                VerticalVelocity += MoveStats.Gravity * MoveStats.GravityOnReleaseMultiplier * Time.fixedDeltaTime;
            }
            else
            {
                VerticalVelocity = Mathf.Lerp(_fastFallReleaseSpeed, 0f, (_fastFallTime / MoveStats.TimeForUpwardsCancel));
            }

            _fastFallTime += Time.fixedDeltaTime;
        }

        // Normal gravity while falling
        if (!_isGrounded && !_isJumping)
        {
            if (!_isFalling) { _isFalling = true; }

            VerticalVelocity += MoveStats.Gravity * Time.fixedDeltaTime;
        }

        // Clamp fall speed
        VerticalVelocity = Mathf.Clamp(VerticalVelocity, -MoveStats.MaxFallSpeed, 50f);

        _rb.linearVelocity = new Vector2(_rb.linearVelocityX, VerticalVelocity);
    }
    #endregion

    #region Pause
    private void PauseChecks()
    {
        if (InputManager.UiCancelPressed)
        {
            if (menuMan.Paused)
            {
                menuMan.continueGame();
            }
            else
            {
                menuMan.pauseGame();
            }
        }
    }
    #endregion

    #region Collision Checks

    private void IsGrounded()
    {
        Vector2 boxCastOrigin = new Vector2(_feetCol.bounds.center.x, _feetCol.bounds.min.y);
        Vector2 boxCastSize = new Vector2(_feetCol.bounds.size.x, MoveStats.GroundDetectionRayLength);

        _groundHit = Physics2D.BoxCast(
                boxCastOrigin,
                boxCastSize,
                0f,
                Vector2.down,
                MoveStats.GroundDetectionRayLength,
                MoveStats.GroundLayer
            );

        _isGrounded = _groundHit.collider != null ? true : false;

        #region Debug Visualization
        if (MoveStats.DebugShowIsGroundedBox)
        {
            Color rayColor = _isGrounded ? Color.green : Color.red;

            Debug.DrawRay(new Vector2(boxCastOrigin.x - boxCastSize.x / 2, boxCastOrigin.y), Vector2.down * MoveStats.GroundDetectionRayLength, rayColor);
            Debug.DrawRay(new Vector2(boxCastOrigin.x + boxCastSize.x / 2, boxCastOrigin.y), Vector2.down * MoveStats.GroundDetectionRayLength, rayColor);
            Debug.DrawRay(new Vector2(boxCastOrigin.x - boxCastSize.x / 2, boxCastOrigin.y - MoveStats.GroundDetectionRayLength), Vector2.right * boxCastSize.x, rayColor);
        }
        #endregion
    }

    private void BumpedHead()
    {
        Vector2 boxCastOrigin = new Vector2(_feetCol.bounds.center.x, _bodyCol.bounds.max.y);
        Vector2 boxCastSize = new Vector2(_feetCol.bounds.size.x * MoveStats.HeadWidth, MoveStats.HeadDetectionRayLength);

        _headHit = Physics2D.BoxCast(boxCastOrigin, boxCastSize, 0f, Vector2.up, MoveStats.HeadDetectionRayLength, MoveStats.GroundLayer);

        _bumpedHead = _headHit.collider != null ? true : false;

        #region Debug Visualization
        if (MoveStats.DebugShowHeadBumpBox)
        {
            float headWidth = MoveStats.HeadWidth;

            Color rayColor = _bumpedHead ? Color.green : Color.red;

            Debug.DrawRay(new Vector2(boxCastOrigin.x - boxCastSize.x / 2 * headWidth, boxCastOrigin.y), Vector2.up * MoveStats.HeadDetectionRayLength, rayColor);
            Debug.DrawRay(new Vector2(boxCastOrigin.x + (boxCastSize.x / 2) * headWidth, boxCastOrigin.y), Vector2.up * MoveStats.HeadDetectionRayLength, rayColor);
            Debug.DrawRay(new Vector2(boxCastOrigin.x - boxCastSize.x / 2 * headWidth, boxCastOrigin.y + MoveStats.HeadDetectionRayLength), Vector2.right * boxCastSize.x * headWidth, rayColor);
        }
        #endregion
    }

    private void CollisionChecks()
    {
        IsGrounded();
    }
    #endregion

    #region Timers
    private void CountTimers()
    {
        // jump
        _jumpBufferTimer -= Time.deltaTime;

        if (!_isGrounded)
        {
            _coyoteTimer -= Time.deltaTime;
        }
        else { _coyoteTimer = MoveStats.JumpCoyoteTime; }

        // attack combo time check
        if (_preAttacking_2 && _attackComboTime > 0)
        {
            _attackComboTime -= Time.deltaTime;
        }
    }
    #endregion

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
        if (_dead) return PlayerAnimations.Death;

        if (_hitted)
        {
            _hitted = false;
            return LockState(PlayerAnimations.Hit, _hittedAnimTime);
        }

        if (_criticalHitted)
        {
            _criticalHitted = false;
            return LockState(PlayerAnimations.CriticalHit, _criticHittedAnimTime);
        }

        if (_isHeavyAttacking)
        {
            _isHeavyAttacking = false;
            return LockState(PlayerAnimations.HeavyAttack, AttackStats.HAMinTime);
        }

        if (_isAttacking_2)
        {
            _isAttacking_2 = false;
            return LockState(PlayerAnimations.Attack_2, AttackStats.A2MinTime);
        }

        if (_isAttacking_1)
        {
            _isAttacking_1 = false;
            return LockState(PlayerAnimations.Attack_1, AttackStats.A1MinTime);
        }

        if (_isGrounded && InputManager.Movement.magnitude != 0) return PlayerAnimations.Run;

        if (_preAttacking_1) return PlayerAnimations.PreAttack_1;
        if (_preAttacking_2) return PlayerAnimations.PreAttack_2;

        if (_isGrounded) return PlayerAnimations.Idle;

        return VerticalVelocity > 0 ? PlayerAnimations.Jump : PlayerAnimations.Fall;

        int LockState(int s, float t)
        {
            _lockedTill = Time.time + t;
            return s;
        }
    }
    #endregion

    #region Attack
    private void AttackChecks()
    {
        /* as with normal attacks in genshin impact (swords),
         * you hold attack for a bit and you may trigger
         * a heavy attack, else, if you press
         * the attack button again in a short time, you trigger
         * the next normal attack
         */
        if (InputManager.Attack_1_WasPressed)
        {
            // cant attack while on air
            if (!_isGrounded) return;

            if (_preAttacking_2 && _attackComboTime > 0)
            {
                _preAttacking_2 = false;
                _isAttacking_2 = true;
                _attackComboTime = -1;
            }
            else
            {
                // initiate Attack 1
                _preAttacking_1 = true;
                _attackComboTime = AttackStats.TriggerComboTime;
                _heavyTime = Time.time + AttackStats.TriggerHeavyTime;
            }
        }

        if (InputManager.Attack_1_WasReleased)
        {
            // heavy Attack
            if (Time.time >= _heavyTime && !_isAttacking_2 && _preAttacking_1)
            {
                _preAttacking_1 = false;
                _isHeavyAttacking = true;
            }

            // normal attack 1
            else if (_preAttacking_1)
            {
                _preAttacking_1 = false;
                _isAttacking_1 = true;
                _preAttacking_2 = true;
            }
        }

        if (_attackComboTime <= 0)
        {
            _preAttacking_2 = false;
            _attackComboTime = AttackStats.TriggerComboTime;
        }
    }

    private bool isAttacking()
    {
        // is doing anything related to attack
        return _isHeavyAttacking || _preAttacking_1 || _isAttacking_1 || _preAttacking_2 || _isAttacking_2;
    }
    #endregion
}

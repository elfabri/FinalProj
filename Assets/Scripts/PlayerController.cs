using UnityEngine;

public class PlayerController : MonoBehaviour
{

    [SerializeField] private float runSpeed = 40f;
    private float horizontalInput = 0f;
    [SerializeField] private float _jumpForce = 8;
    private bool jump = false;
    [SerializeField] private float jumpBufferTime = 0.2f;
    private float jumpBufferCounter;
    [SerializeField] private bool _grounded = true;
    [SerializeField][Range(0, .3f)] private float _movementSmoothing = .1f;	// How much to smooth out the movement
    private Vector3 _velocity = Vector3.zero;
    private Rigidbody2D player;
    [SerializeField] private MenuesManager menus;
    [SerializeField] private GameObject _footPos;
    private bool _facingRight = true;  // starts facing right
    private bool _falling = false;
    private float airVel = 0.5f;
    private float xMov;

    private Animator animator;

    void Start()
    {
        player = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (menus.Paused || !menus.Started || menus.Died) {
            return;
        }

        horizontalInput = Input.GetAxisRaw("Horizontal") * runSpeed;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            jumpBufferCounter = jumpBufferTime;

        } else {
            jumpBufferCounter -= Time.deltaTime;
        }

        if (jumpBufferCounter > 0 && _grounded)
        {
            jump = true;
            jumpBufferCounter = 0;
        }
    }

    void FixedUpdate()
    {
        if (horizontalInput != 0) Move(horizontalInput);
        if (jump) Jump();
        if (player.linearVelocityY < 0) SoftDrop();
        animator.SetFloat("Speed", Mathf.Abs(player.linearVelocity.x));
        animator.SetFloat("YSpeed", player.linearVelocity.y);
    }

    public void Move(float move)
    {
        xMov = move * Time.fixedDeltaTime;

        if (!_grounded) xMov *= airVel;

        // Move the character by finding the target velocity
        Vector3 targetVelocity = new Vector2(xMov * 10f, player.linearVelocity.y);
        // And then smoothing it out and applying it to the character
        player.linearVelocity = Vector3.SmoothDamp(player.linearVelocity, targetVelocity, ref _velocity, _movementSmoothing);

        if (move > 0 && !_facingRight)
        {
			Flip();
		}
        else if (move < 0 && _facingRight)
        {
			Flip();
		}

    }

    public void Jump()
    {
        player.linearVelocityY = _jumpForce;
        _grounded = false;
        jump = false;
    }

    void SoftDrop()
    {
        if (!_falling)
        {
            _falling = true;
            player.gravityScale = 3.0f;
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Ground"))
        {
            if (col.transform.position.y < _footPos.transform.position.y)
            {
                _grounded = true;
                player.gravityScale = 1.0f;
            }
        }
    }

	private void Flip() {
		// Switch the way the player is labelled as facing.
		_facingRight = !_facingRight;

		// Multiply the player's x local scale by -1.
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}
}

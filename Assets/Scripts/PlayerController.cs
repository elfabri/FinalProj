using UnityEngine;

public class PlayerController : MonoBehaviour
{

    [SerializeField] private float runSpeed = 200f;
    private float horizontalInput = 0f;
    [SerializeField] private float m_JumpForce = 2200;
    private bool jump = false;
    [SerializeField] private float jumpBufferTime = 0.2f;
    private float jumpBufferCounter;
    [SerializeField] private bool m_Grounded = true;
    [Range(0, .3f)] private float m_MovementSmoothing = .05f;	// How much to smooth out the movement
    private Vector3 m_Velocity = Vector3.zero;
    private Rigidbody2D player;
    [SerializeField] private MenuesManager menus;

    void Start()
    {
        player = GetComponent<Rigidbody2D>();
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

        if (jumpBufferCounter > 0 && m_Grounded)
        {
            jump = true;
            jumpBufferCounter = 0;
        }
    }

    void FixedUpdate()
    {
        if (horizontalInput != 0 || jump) {
            Move(horizontalInput * Time.fixedDeltaTime, jump);
        }
        jump = false;
    }

    public void Move(float move, bool jump)
    {
        // Move the character by finding the target velocity
        Vector3 targetVelocity = new Vector2(move * 10f, player.linearVelocity.y);
        // And then smoothing it out and applying it to the character
        player.linearVelocity = Vector3.SmoothDamp(player.linearVelocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);

        if (jump) {
            Jump();
        }
    }

    public void Jump()
    {
        player.AddForce(new Vector2(0f, m_JumpForce));
        m_Grounded = false;
    }

    void OnCollisionEnter2D(Collision2D col) {
        if (col.gameObject.CompareTag("Ground"))
            m_Grounded = true;

        // if (col.gameObject.CompareTag("Enemy") && hasStarted)
            // PlayerTriggers("hit");
    }
}

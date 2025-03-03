using UnityEngine;

public class PlayerController : MonoBehaviour
{

    [SerializeField] private float runSpeed = 200f;
    private float horizontalInput = 0f;
    [SerializeField] private float m_JumpForce = 2200;
    private bool jump = false;
    [Range(0, .3f)] private float m_MovementSmoothing = .05f;	// How much to smooth out the movement
    private bool m_Grounded = true;
    private Vector3 m_Velocity = Vector3.zero;
    private Rigidbody2D player;

    void Start()
    {
        player = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal") * runSpeed;
        if (Input.GetKeyDown(KeyCode.Space) && m_Grounded) {
            m_Grounded = false;
            jump = true;
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
            // Debug.Log("We should jump!");
            Jump();
        }
    }

    public void Jump()
    {
        player.AddForce(new Vector2(0f, m_JumpForce));
    }

    void OnCollisionEnter2D(Collision2D col) {
        if (col.gameObject.CompareTag("Ground"))
            m_Grounded = true;

        // if (col.gameObject.CompareTag("Enemy") && hasStarted)
            // PlayerTriggers("hit");
    }
}

using System.Collections;
using UnityEngine;
public class playerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private bool isGrounded;

    [Header("Movement")]
    [SerializeField]private float moveSpeed = 7f;
    [SerializeField] private float Drag = 1f;

    [Header("Jumping")]
    [SerializeField] private float jumpForce = 6f;
    [SerializeField] private float fallMultiplier = 2.5f;
    [SerializeField] private float lowJumpMultiplier = 2f;

    [Header("Dash")]
    [SerializeField] private float dashSpeed = 20f;
    [SerializeField] private float dashDuration = 0.15f;
    [SerializeField] private float dashDrag = 6f;
    private bool isDashing = false;
    private bool canDash = true;
    private Vector2 dashDirection;
    private float dashTimer = 0f;

    [Header("Ground Check (Raycast)")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float rayLength = 0.1f;
    [SerializeField] private Vector2 rayOffsetLeft = new Vector2(-0.5f, -0.5f);
    [SerializeField] private Vector2 rayOffsetCenter = new Vector2(0f, -0.5f);
    [SerializeField] private Vector2 rayOffsetRight = new Vector2(0.5f, -0.5f);

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.drag = Drag;
    }

    void Update()
    {
        float xInput = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(xInput * moveSpeed, rb.velocity.y);

        Vector2 pos = transform.position;

        bool left = Physics2D.Raycast(pos + rayOffsetLeft, Vector2.down, rayLength, groundLayer);
        bool center = Physics2D.Raycast(pos + rayOffsetCenter, Vector2.down, rayLength, groundLayer);
        bool right = Physics2D.Raycast(pos + rayOffsetRight, Vector2.down, rayLength, groundLayer);

        isGrounded = left || center || right;

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (rb.velocity.y > 0 && !Input.GetKey(KeyCode.Space))
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash && !isDashing)
        {
            float x = Input.GetAxisRaw("Horizontal");
            float y = Input.GetAxisRaw("Vertical");

            // Default to facing right if no input
            if (x == 0 && y == 0) x = transform.localScale.x >= 0 ? 1 : -1;

            dashDirection = new Vector2(x, y).normalized;
            StartCoroutine(Dash());
        }
        if (isGrounded && !isDashing)
        {
            canDash = true;
        }
    }
    private IEnumerator Dash()
    {
        isDashing = true;
        canDash = false;
        rb.gravityScale = 2;
        rb.drag = dashDrag;
        rb.velocity = dashDirection * dashSpeed;

        yield return new WaitForSeconds(dashDuration);
        rb.drag = Drag;
        isDashing = false;
        if (isGrounded)
        {
            rb.gravityScale = 1;
        }
        else
        {
            yield return new WaitForSeconds(.5f);
            rb.gravityScale = 1;
        }
    }

    private void OnDrawGizmos()
    {
        Vector2 pos = transform.position;

        Vector2[] origins = new Vector2[]
        {
            pos + rayOffsetLeft,
            pos + rayOffsetCenter,
            pos + rayOffsetRight
        };

        Gizmos.color = Color.yellow;

        foreach (Vector2 origin in origins)
        {
            Gizmos.DrawLine(origin, origin + Vector2.down * rayLength);
            Gizmos.DrawSphere(origin, 0.02f);
        }
    }
}

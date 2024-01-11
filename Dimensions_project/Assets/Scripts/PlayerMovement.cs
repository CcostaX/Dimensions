using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 0f;
    private Rigidbody2D rb;
    private Vector2 moveDirection;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private int previousDirection = -1;

    //DASH
    private bool canDash = true;
    private bool isDashing = false;
    private float dashingTime = 0.2f;
    private float dashingPower = 24f;
    private float dashingCooldown = 0.5f;

    [SerializeField] private TrailRenderer tr;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        ProcessInputs();
    }

    //physics calculations
    private void FixedUpdate()
    {
        if (isDashing)
        {
            return;
        }

        Move();
    }

    void ProcessInputs()
    {
        if (isDashing)
        {
            return;
        }

        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        moveDirection = new Vector2(moveX, moveY);

        if (Input.GetKeyDown(KeyCode.X) && canDash)
        {
            StartCoroutine(Dash(moveDirection));
        }
    }

    void Move()
    {
        rb.velocity = new Vector2(moveDirection.x * moveSpeed, moveDirection.y * moveSpeed);
    }

    private void OnAnimatorMove()
    {
        animator.SetBool("isMoving", moveDirection.magnitude > 0);

        // Check for 8 directions
        if (moveDirection.magnitude > 0)
        {
            float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;

            // Adjust angle to be positive and between 0 and 360 degrees
            if (angle < 0)
            {
                angle += 360f;
            }

            // Determine the direction based on the angle (right = 0, up/right = 1, up = 2, ... )
            int direction = Mathf.RoundToInt(angle / 45f) % 8;

            // Set the direction parameter in the Animator
            animator.SetInteger("direction", direction);

            if (direction != previousDirection)
            {
                spriteRenderer.flipX = direction == 3 || direction == 4 || direction == 7 ;
                previousDirection = direction;
            }
        }
        else
        {
            animator.SetInteger("direction", -1);
        }
    }

    private IEnumerator Dash(Vector2 dashDirection)
    {
        canDash = false;
        isDashing = true;

        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;

        // Normalize the dash direction to ensure consistent speed in all directions
        dashDirection.Normalize();

        // Set the velocity based on the dash direction and power
        rb.velocity = dashDirection * dashingPower;

        tr.emitting = true;

        yield return new WaitForSeconds(dashingTime);

        tr.emitting = false;
        rb.gravityScale = originalGravity;
        isDashing = false;

        yield return new WaitForSeconds(dashingCooldown);
        canDash = true;
    }
}

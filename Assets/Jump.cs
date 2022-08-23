using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jump : MonoBehaviour
{
    [SerializeField] private InputController input = null;
    [SerializeField, Range(0f, 10f)] private float jumpHeight = 3f;
    [SerializeField, Range(0f, 5)] private int maxAirJumps = 0;
    [SerializeField, Range(0f, 5f)] private float downwardMovementMultiplier = 3f;
    [SerializeField, Range(0f, 5f)] private float upwardMovementMultiplier = 1.7f;

    private Rigidbody2D rigidBody;
    private Ground ground;
    private Vector2 velocity;

    private int jumpPhase;
    private float defaultGravityScale;

    private bool desiredJump;
    private bool onGround;

    // Start is called before the first frame update
    void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        ground = GetComponent<Ground>();

        defaultGravityScale = 1f;
    }

    // Update is called once per frame
    void Update()
    {
        desiredJump |= input.RetrieveJumpInput();
    }

    private void FixedUpdate()
    {
        onGround = ground.GetOnGround();
        velocity = rigidBody.velocity;

        if (onGround)
        {
            jumpPhase = 0;
        }

        if (desiredJump)
        {
            desiredJump = false;
            JumpAction();
        }

        if(rigidBody.velocity.y > 0)
        {
            rigidBody.gravityScale = upwardMovementMultiplier;
        }
        else if(rigidBody.velocity.y < 0)
        {
            rigidBody.gravityScale = downwardMovementMultiplier;
        }
        else if(rigidBody.velocity.y == 0)
        {
            rigidBody.gravityScale = defaultGravityScale;
        }

        rigidBody.velocity = velocity;
    }

    private void JumpAction()
    {
        if(onGround || jumpPhase < maxAirJumps)
        {
            jumpPhase += 1;
            float jumpSpeed = Mathf.Sqrt(-2f * Physics2D.gravity.y * jumpHeight);
            if(velocity.y > 0f)
            {
                jumpSpeed = Mathf.Max(jumpSpeed - velocity.y, 0f);
            }

            velocity.y += jumpSpeed;
        }
    }
}

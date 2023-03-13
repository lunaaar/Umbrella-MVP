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
    [SerializeField, Range(0f, 5f)] private float glideMovementMultiplier = .5f;


    [SerializeField, Range(0f, 1f)] private float groundCoyoteTimer = 0.2f;
    [SerializeField, Range(0f, 1f)] private float airCoyoteTimer = 0.2f;

    private Rigidbody2D rigidBody;
    private Ground ground;
    private Vector2 velocity;

    private int jumpPhase;
    private float defaultGravityScale;
    private bool falling;

    [SerializeField] private bool desiredJump;
    [SerializeField] private bool desiredGlide;
    [SerializeField] private bool onGround;
    [SerializeField] private bool inWind;

    public bool glideEnabled = false;

    private AreaEffector2D windCurrent;

    private Animator animator;

    [SerializeField] private GameObject balloon;

    // Start is called before the first frame update
    void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        ground = GetComponent<Ground>();
        animator = GetComponent<Animator>();

        defaultGravityScale = 1f;
        inWind = false;
        desiredGlide = false;
    }

    // Update is called once per frame
    void Update()
    {
        desiredJump |= input.RetrieveJumpInput();
        // |= is equal to x = x OR input. So once its set to true, it will just stay true. You would have to set in manually back to false.
        desiredGlide = input.RetrieveGlideInput();

        if (Input.GetButtonUp("Jump"))
        {
            Debug.Log("UP");
            falling = true;
            rigidBody.gravityScale = defaultGravityScale * 10;
            //rigidBody.gravityScale = downwardMovementMultiplier;
        }
    }

    private void FixedUpdate()
    {
        onGround = ground.GetOnGround();
        velocity = rigidBody.velocity;
        animator.SetInteger("direction", (int)velocity.x);

        if (desiredJump)
        {
            desiredJump = false;
            airCoyoteTimer = 0.2f;
        }
        else
        {
            if(airCoyoteTimer > -1)
            {
                airCoyoteTimer -= Time.deltaTime;
            }
        }

        if (onGround)
        {
            jumpPhase = 0;

            animator.SetBool("isGliding", false);

            groundCoyoteTimer = 0.2f;
            rigidBody.gravityScale = defaultGravityScale;
            falling = false;
        }
        else
        {
            groundCoyoteTimer -= Time.deltaTime;
        }

        if (onGround && airCoyoteTimer > 0)
        {
            JumpAction();
            airCoyoteTimer = 0;

            animator.SetBool("isGliding", false);
        }

        if (inWind)
        {

            rigidBody.gravityScale = defaultGravityScale;

            if (desiredGlide && glideEnabled)
            {
                animator.SetBool("isGliding", true);
                balloon.SetActive(true);
                windCurrent.enabled = true;
            }
            else
            {
                balloon.SetActive(false);
                animator.SetBool("isGliding", false);
            }
        }
        else if(desiredGlide && !onGround && glideEnabled)
        {
            //Regular Gliding
            animator.SetBool("isGliding", true);

            balloon.SetActive(true);

            if (rigidBody.velocity.y > 0)
            {
                rigidBody.gravityScale = defaultGravityScale;
            }
            else if (rigidBody.velocity.y < 0)
            {
                rigidBody.gravityScale = glideMovementMultiplier;
            }
            
        }
        else
        {
            //Regular Jumping
            animator.SetBool("isGliding", false);

            balloon.SetActive(false);
            if (rigidBody.velocity.y > 0 && !falling)
            {
                rigidBody.gravityScale = upwardMovementMultiplier;
            }
            /**else if (rigidBody.velocity.y < 0)
            {
                rigidBody.gravityScale = downwardMovementMultiplier;
            }*/
            else if (falling)
            {
                rigidBody.gravityScale = downwardMovementMultiplier;
            }
            else if (rigidBody.velocity.y == 0)
            {
                rigidBody.gravityScale = defaultGravityScale;
            }
        }
        rigidBody.velocity = velocity;
    }

    public void JumpAction()
    {
        if(groundCoyoteTimer > 0f || jumpPhase < maxAirJumps)
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

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.tag == "WindCurrent")
        {
            inWind = true;
            windCurrent = collision.transform.GetComponent<AreaEffector2D>();
        }
        else if(collision.name == "EnableGlide")
        {
            glideEnabled = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        
        if (collision.tag == "WindCurrent")
        {
            inWind = false;
            windCurrent.enabled = false;
            rigidBody.velocity = new Vector2(rigidBody.velocity.x, Mathf.Min(rigidBody.velocity.y, 8f));
        }
    }
}

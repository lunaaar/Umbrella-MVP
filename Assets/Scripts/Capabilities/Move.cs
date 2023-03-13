using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    [SerializeField] private InputController input = null;
    [SerializeField, Range(0f, 100f)] private float maxSpeed = 4f;
    [SerializeField, Range(0f, 100f)] private float maxAcceleration = 35f;
    [SerializeField, Range(0f, 100f)] private float maxAirAcceleration = 20f;

    private Vector2 direction;

    public Animator animator;

    private Vector2 desiredVelocity;
    private Vector2 velocity;
    private Rigidbody2D rigidBody;
    private Ground ground;

    private float maxSpeedChange;
    private float acceleration;
    private bool onGround;

    // Start is called before the first frame update
    void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        ground = GetComponent<Ground>();
        animator.GetComponent<Animator>();
        animator.SetBool("facingLeft", false);
    }

    // Update is called once per frame
    void Update()
    {
        direction.x = input.RetrieveMoveInput();
        if (direction.x > 0)
        {
            animator.SetBool("facingLeft", false);
        }
        else if (direction.x < 0)
        {
            animator.SetBool("facingLeft", true);
        }
        desiredVelocity = new Vector2(direction.x, 0f) * Mathf.Max(maxSpeed - ground.GetFriction(), 0);
    }

    private void FixedUpdate()
    {
        onGround = ground.GetOnGround();
        velocity = rigidBody.velocity;

        acceleration = onGround ? maxAcceleration : maxAirAcceleration;
        maxSpeedChange = acceleration * Time.deltaTime;

        velocity.x = Mathf.MoveTowards(velocity.x, desiredVelocity.x, maxSpeedChange);

        rigidBody.velocity = velocity;
    }
}

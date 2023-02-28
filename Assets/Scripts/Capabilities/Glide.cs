using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Glide : MonoBehaviour
{
    [SerializeField] private InputController input = null;
    [SerializeField, Range(0f, 5f)] private float glideMovementMultiplier = .5f;

    private Rigidbody2D rigidBody;
    private Ground ground;

    private float defaultGravityScale;

    private bool desiredGlide;
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
        desiredGlide = input.RetrieveGlideInput();
    }

    private void FixedUpdate()
    {
        onGround = ground.GetOnGround();

        if (desiredGlide && !onGround)
        {
            rigidBody.gravityScale = glideMovementMultiplier;
        }
        else
        {
            
            rigidBody.gravityScale = defaultGravityScale;
        }
    }
}

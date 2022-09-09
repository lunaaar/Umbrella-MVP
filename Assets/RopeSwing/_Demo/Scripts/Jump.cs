using UnityEngine;

namespace RopeSwing
{
    public class Jump : MonoBehaviour
    {
        [SerializeField] private float jumpForce = 10f;
    
        private Rigidbody2D _rb;

        private bool _didJumpThisFrame;
    
        private void Start()
        {
            _rb = GetComponent<Rigidbody2D>();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
                _didJumpThisFrame = true;
        }

        private void FixedUpdate()
        {
            if (_didJumpThisFrame)
            {
                _rb.velocity = new Vector2(_rb.velocity.x, jumpForce);
                _didJumpThisFrame = false;
            }
        }
    }
}

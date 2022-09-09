using System.Collections;
using UnityEngine;

namespace RopeSwing
{
    public class Movement : MonoBehaviour
    {
        [SerializeField] private float speed = 10f;
        [SerializeField] private float smoothTime = 0.2f;
        [SerializeField] private float smoothTimeResetDelay = 0.5f;
        private Rigidbody2D _rb;

        private float _horizontalMovement;

        private float _prevVelocity;
    
        private void Start()
        {
            _rb = GetComponent<Rigidbody2D>();
        }

        private void Update()
        {
            _horizontalMovement = Input.GetAxis("Horizontal");
        }

        private void FixedUpdate()
        {
            var targetVelocity = 
                Mathf.Approximately(0, _horizontalMovement)
                    ? 0
                    : Mathf.Max(Mathf.Abs(_horizontalMovement * speed), Mathf.Abs(_rb.velocity.x)) ;
        
            _rb.velocity = new Vector2( Mathf.SmoothDamp(_rb.velocity.x, targetVelocity * Mathf.Sign(_horizontalMovement), ref _prevVelocity, smoothTime), _rb.velocity.y);
        }

        public void SetSmoothTime(float newSmoothTime)
        {
            var oldSmoothTime = smoothTime;
            smoothTime = newSmoothTime;
            StartCoroutine(ResetSmoothTime(oldSmoothTime, smoothTimeResetDelay));
        }

        private IEnumerator ResetSmoothTime(float oldSmoothTime, float secondsToWait)
        {
            yield return new WaitForSeconds(secondsToWait);
            smoothTime = oldSmoothTime;
        }
    }
}

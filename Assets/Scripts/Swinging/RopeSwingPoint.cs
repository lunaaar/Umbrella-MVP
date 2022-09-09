using System;
using UnityEditor;
using UnityEngine;
    public class RopeSwingPoint : MonoBehaviour
    {
        [SerializeField, Tooltip("The activation radius for this swing point")] 
        private float radius;

        public float Radius => radius;
        public float SqrRadius => _sqrRadius;
    

        [SerializeField, Tooltip("How many second should we keep the max velocity for?")] 
        private float velocityStoreTime = 0.5f;
    
        [SerializeField, Tooltip("The amount to increase the gravity while swinging down. 1 is normal gravity, higher for faster speed gain")] 
        private float swingDownGravityMultiplier;
        [SerializeField, Tooltip("The amount to decrease the gravity while swinging up. 1 is normal gravity, lower for less speed loss")] 
        private float swingUpGravityMultiplier;

        private bool _isSwinging;
        private bool _inputPressedThisFrame;

        private Rigidbody2D _activeRigidbody;
        private float _gravityScale;
        private float _maxVelocity;
        private float _timer;
        private float _sqrRadius;
        private float _distanceToObject;

        public static event Action<RopeSwingPoint> SwingPointActive;
        public static event Action<RopeSwingPoint> SwingPointInactive;


        /// <summary>
        /// When the object is enabled, event function
        /// </summary>
        private void OnEnable()
        {
            // calculate the squared radius so we can use it in a sqr distance check. Slightly more efficient, especially when there are lots of rope swing points around
            _sqrRadius = radius * radius;
            // tell anything that cares that there is a new swing point
            SwingPointActive?.Invoke(this);
        }

        /// <summary>
        /// When the object is disabled, event function
        /// </summary>
        private void OnDisable()
        {
            // tell anything that cares that this swing point has been deactivated
            SwingPointInactive?.Invoke(this);
        }

        /// <summary>
        /// Every physics tick, event function
        /// </summary>
        private void FixedUpdate()
        {
            if (_isSwinging)
            {
                // keep track of the max velocity so we don't punish players for detaching at the wrong time
                StoreVelocity();
                // swing the player
                SwingUpdate();
            }
        }

        /// <summary>
        /// Start the swing
        /// </summary>
        public void ActivateSwing(Rigidbody2D rbody)
        {
            // reset the timer, so we're always starting at 0 during a swing and will capture the most recent velocity
            _timer = 0;
        
            // now we're swingin' 
            _isSwinging = true;
        
            // grab the rigidbody, and any components we want to disable
            _activeRigidbody = rbody;
        
            // store the current gravity scale so we can reset it properly when we detach
            _gravityScale = _activeRigidbody.gravityScale;
        
            ImmediateChangeVelocityToTangent();
            
            // store the distance to the object. This is the actual distance, not the squared distance
            // this is because we'll need to use the value rather than just comparing against it
            _distanceToObject = (_activeRigidbody.transform.position - transform.position).magnitude;
        }

        /// <summary>
        /// Immediately translate all the player's momentum into the swing, so we keep the speed
        /// </summary>
        private void ImmediateChangeVelocityToTangent()
        {
            // get the direction from the centre of the rope swing point to the player
            var vectorFromCentre = _activeRigidbody.transform.position - transform.position;
            vectorFromCentre.Normalize();

            // use the clockwise tangent if we're to the right of the rope swing point, and the anticlockwise tangent if we're to the left of it 
            // nb: this is probably the thing to change if you're wanting slightly different functionality
            // currently e.g. if the player is moving right and is to the left of the attach point, it will swing up
            var tangent = vectorFromCentre.x < 0 
                ? new Vector2(-vectorFromCentre.y, vectorFromCentre.x) 
                : new Vector2(vectorFromCentre.y, -vectorFromCentre.x);
        
            _activeRigidbody.velocity = tangent.normalized * _activeRigidbody.velocity.magnitude;
        }

    
        /// <summary>
        /// Deactivate the swing
        /// </summary>
        public void DeactivateSwing()
        {
            _isSwinging = false;
            // reset the gravity scale
            _activeRigidbody.gravityScale = _gravityScale;
            // increase the player's speed so it matches the max cached speed
            _activeRigidbody.velocity = _activeRigidbody.velocity.normalized * _maxVelocity;
            _timer = 0;
            // make the active rigidbody null so it throws errors properly if something goes wrong
            _activeRigidbody = null;
        }

    
        /// <summary>
        /// Actually do the swing
        /// </summary>
        private void SwingUpdate()
        {
            // change the gravity multiplier, so we're heavier while going down and lighter while going up - this way, we build up more speed
            _activeRigidbody.gravityScale = _activeRigidbody.velocity.y > 0 ? swingUpGravityMultiplier : swingDownGravityMultiplier;

            // clamp the position so it always stays the same distance away from the swing point
            // cache the velocity so we make sure it doesn't change when we change the rigidbody's position
            var velocity = _activeRigidbody.velocity;
            // get the current distance
            var currentDistance = (_activeRigidbody.transform.position - transform.position).magnitude;
            // if the current distance is less than the original distance we calculated when attaching to the swing point
            if (_distanceToObject < currentDistance)
            {
                // the new position should be:
                // old position
                // plus the vector going from the rigidbody to the swing point
                // multiplied by the difference between the distances
                _activeRigidbody.position = (_activeRigidbody.transform.position + (_activeRigidbody.transform.position - transform.position) * (_distanceToObject - currentDistance));
                // reset the velocity to the cached value, because moving a rigidbody's position _sometimes_ updates the velocity...
                _activeRigidbody.velocity = velocity;
            }
            
            // get the direction from the centre of the rope swing point to the player
            var vectorFromCentre = _activeRigidbody.transform.position - transform.position;
            vectorFromCentre.Normalize();

            // get the clockwise tangent
            var tangent = new Vector2(vectorFromCentre.y, -vectorFromCentre.x);

            // project the current velocity onto the tangent
            // this will preserve external forces like gravity!
            _activeRigidbody.velocity = Vector3.Project(_activeRigidbody.velocity, tangent.normalized);
        }
    

        /// <summary>
        /// Keep track of the max velocity for a short time
        /// </summary>
        private void StoreVelocity()
        {
            // if we're out of time, reset the max velocity
            if (_timer >= velocityStoreTime) _maxVelocity = 0;
            // if our current velocity is greater than the stored max
            if (_activeRigidbody.velocity.magnitude > _maxVelocity)
            {
                // store the current velocity
                _maxVelocity = _activeRigidbody.velocity.magnitude;
                // reset the timer, so we store this new max for the longest possible time
                _timer = 0;
            } else
            {
                _timer += Time.deltaTime;
            }
        }

#if UNITY_EDITOR
        /// <summary>
        /// Editor function for drawing things in the scene view, event function
        /// </summary>
        private void OnDrawGizmos()
        {
            Handles.DrawWireDisc(transform.position, transform.forward, radius);
            var col = Handles.color;

            if (_activeRigidbody != null && _isSwinging)
            {
                var vectorFromCentre = _activeRigidbody.transform.position - transform.position;
                Handles.DrawLine(transform.position, _activeRigidbody.position);

                Handles.color = Color.red;
                vectorFromCentre.Normalize();
                var tangent = new Vector2(-vectorFromCentre.y, vectorFromCentre.x);
                Handles.DrawLine(_activeRigidbody.position, _activeRigidbody.position + tangent);
            
                Handles.color = Color.cyan;
                Handles.DrawLine(_activeRigidbody.position, _activeRigidbody.transform.position + vectorFromCentre);
            }

            Handles.color = col;
        }
#endif
    }
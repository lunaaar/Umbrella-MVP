using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif
    /// <summary>
    /// Input controller for swinging from ropes
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class RopeSwingInputController : MonoBehaviour
    {
    [SerializeField] private InputController input = null;

#if ENABLE_LEGACY_INPUT_MANAGER
    [Header("Input"), SerializeField, Tooltip("The key used to start swinging")] 
        private KeyCode ropeAttachKey = KeyCode.LeftShift;
        [SerializeField, Tooltip("An alternate key used to start swinging")] 
        private KeyCode ropeAttachKeyAlt = KeyCode.RightShift;
#endif
        
        [SerializeField, Tooltip("Should the jump key stop the player from swinging?")] private bool jumpDetaches = true;

        [Header("Events"), SerializeField, Tooltip("An event broadcast when the object starts swinging")]
        private RopeSwingEvent onAttach;
        [SerializeField, Tooltip("An event broadcast when the object stops swinging")] 
        private RopeSwingEvent onDetach;

        public event Action<Rigidbody2D, RopeSwingPoint> OnAttach;
        public event Action<Rigidbody2D, RopeSwingPoint> OnDetach;

        private Rigidbody2D _rbody;
        private List<RopeSwingPoint> _swingPoints = new ();

        private bool _attachInputPressed;
        private bool _isSwinging;
        private RopeSwingPoint _activeSwingPoint;
    
        
        #if ENABLE_LEGACY_INPUT_MANAGER
        public void RebindKey(KeyCode attachKey, KeyCode attachKeyAlt)
        {
            ropeAttachKey = attachKey;
            ropeAttachKeyAlt = attachKeyAlt;
        }
        #endif
        
        /// <summary>
        /// When the object is created, event function
        /// </summary>
        private void Start()
        {
            // grab a reference to the rbody
            _rbody = GetComponent<Rigidbody2D>();
            
            // populate the list of swing points, a cheaper way of doing things than using Unity's collisions...
            RopeSwingPoint.SwingPointActive += RegisterSwingPoint;
            RopeSwingPoint.SwingPointInactive += DeregisterSwingPoint;
            
            // populate the list with any already enabled swing points 
            var alreadyEnabledSwingPoints = FindObjectsOfType<RopeSwingPoint>();
            foreach (var point in alreadyEnabledSwingPoints)
                RegisterSwingPoint(point);
            // make sure we don't have any duplicates in the list
            _swingPoints = _swingPoints.Distinct().ToList();
        }

        /// <summary>
        /// Every frame, event function
        /// </summary>
        private void Update()
        {
            // if we're using the old (bad) input system
            #if ENABLE_LEGACY_INPUT_MANAGER
            // capture the input
            //if (Input.GetKeyDown(ropeAttachKey) || Input.GetKeyDown(ropeAttachKeyAlt))
            if(input.RetrieveSwingInput())
                _attachInputPressed = true;

            if (jumpDetaches && _isSwinging && Input.GetButtonDown("Jump"))
                _attachInputPressed = true;
#endif
        }

        // if we're using the new (good) input system
        #if ENABLE_INPUT_SYSTEM
        // Capture the "Attach" input - assign on your PlayerInput object
        public void OnAttachInput (InputAction.CallbackContext ctx)
        {
            if (ctx.ReadValueAsButton())
                _attachInputPressed = true;
        }

        // Capture the "Jump" input - assign on your PlayerInput object
        public void OnJumpInput (InputAction.CallbackContext ctx)
        {
            if (jumpDetaches && ctx.ReadValueAsButton() && _isSwinging)
                _attachInputPressed = true;
        }
        #endif
        
        /// <summary>
        /// Every physics tick, event function
        /// </summary>
        private void FixedUpdate()
        {
            if (_attachInputPressed)
            {
                _attachInputPressed = false;
                // if we need to start swinging
                if (!_isSwinging)
                {
                    // get the closest point swing point, we don't care about any of the others
                    var (swingPoint, sqrDist)  = GetClosestSwingPoint();
                    // check if we're inside the swing point's radius
                    // n.b. getting the distance squared is much cheaper, and means we need to square the value we're checking against
                    if (sqrDist < swingPoint.SqrRadius)
                    {
                        // we're swinging - it's cheaper to use a bool here than it is to check if _isSwinging is null
                        // in Unity, null checks need to dip into native C++ code so there's a bit of a performance cost
                        _isSwinging = true;
                        // cache the found swing point
                        _activeSwingPoint = swingPoint;
                        // activate the swing, passing through our rigidbody2D
                        _activeSwingPoint.ActivateSwing(_rbody);
                        
                        // broadcast a message to any listeners
                        onAttach?.Invoke(_rbody, _activeSwingPoint);
                        OnAttach?.Invoke(_rbody, _activeSwingPoint);
                    }
                } else
                {
                    // deactivate the swing
                    _activeSwingPoint.DeactivateSwing();
                    // broadcast a message to any listeners
                    onDetach?.Invoke(_rbody, _activeSwingPoint);
                    OnDetach?.Invoke(_rbody, _activeSwingPoint);
                    // remove the active swing point reference
                    _activeSwingPoint = null;
                    // we're no longer swinging
                    _isSwinging = false;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>Both the closest swing point, and the squared distance from the current object to that swing point. The returns swingpoint is null and the distance is float.MaxValue if there are no swing points.</returns>
        private (RopeSwingPoint, float) GetClosestSwingPoint()
        {
            // default value, starts null
            RopeSwingPoint closest = null;
            // default value, starts at the maximum possible float value. Every other float is smaller than this, so the first check will always return true!
            var dist = float.MaxValue;
            // loop through the swing points
            for (var i = 0; i < _swingPoints.Count; ++i)
            {
                // grab the distance squared between our current position and the swing point's position
                // sqrMagnitude is computationally cheaper than Vector3.Distance, because it avoids the expensive sqrt function used in distance checks
                var tempDist = (transform.position - _swingPoints[i].transform.position).sqrMagnitude;
                // if the this distance is less than the current closest, this one is now the closest 
                if (tempDist < dist)
                {
                    closest = _swingPoints[i];
                    dist = tempDist;
                }
            }

            // return both the object and the square distance
            return (closest, dist);
        }

        /// <summary>
        /// Cleanup, event function
        /// </summary>
        private void OnDestroy()
        {
            // stop checking if swing points were activated/deactivated
            // always deregister your listeners, otherwise you'll get null reference exceptions in the editor!
            RopeSwingPoint.SwingPointActive -= RegisterSwingPoint;
            RopeSwingPoint.SwingPointInactive -= DeregisterSwingPoint;
        }

        /// <summary>
        /// Store any active swing points in a list when they're activated
        /// </summary>
        /// <param name="point"></param>
        private void RegisterSwingPoint(RopeSwingPoint point)
        {
            _swingPoints.Add(point);
        }

        /// <summary>
        /// Remove any disabled swing points from the list as soon as they're deactivated
        /// </summary>
        /// <param name="point"></param>
        private void DeregisterSwingPoint(RopeSwingPoint point)
        {
            _swingPoints.Remove(point);
        }
}
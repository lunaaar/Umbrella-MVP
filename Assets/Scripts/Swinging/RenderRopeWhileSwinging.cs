using UnityEngine;

    public class RenderRopeWhileSwinging : MonoBehaviour
    {
        [SerializeField] private LineRenderer line;

        private bool _isSwinging;
        private Transform _target;
    
        public void SwingStart(Rigidbody2D rbody, RopeSwingPoint rope)
        {
            line.enabled = true;
            _target = rope.transform;
            _isSwinging = true;
        }

        public void SwingEnd(Rigidbody2D rbody, RopeSwingPoint rope)
        {
            line.enabled = false;
            _target = null;
            _isSwinging = false;
        }

        private void Update()
        {
            // set the positions of the rope line renderer
            if (_isSwinging)
            {
                line.SetPosition(0, transform.position);
                line.SetPosition(1, _target.position);
            }
        }
    }
using System.Collections.Generic;
using UnityEngine;

public class DisableBehavioursOnRopeSwingStart : MonoBehaviour
    {
        [SerializeField] private List<MonoBehaviour> behaviours;
    
        public void SwingStart(Rigidbody2D rbody, RopeSwingPoint rope)
        {
            foreach (var mb in behaviours)
                mb.enabled = false;
        }

        public void SwingEnd(Rigidbody2D rbody, RopeSwingPoint rope)
        {
            foreach (var mb in behaviours)
                mb.enabled = true;
        }
    }
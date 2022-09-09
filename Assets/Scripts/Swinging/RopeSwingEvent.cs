using System;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Unity Event that takes a Rigidbody2D and a RopeSwingPoint, so we can pass values around inside the events
/// </summary>
[Serializable]
public class RopeSwingEvent : UnityEvent<Rigidbody2D, RopeSwingPoint>
{}
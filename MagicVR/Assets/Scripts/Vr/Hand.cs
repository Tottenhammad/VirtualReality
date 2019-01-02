using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
public class Hand : MonoBehaviour
{
    public XRNode NodeType;

    private void Start()
    {
        // Setup the tracking area so hand know where they can and can't go
        XRDevice.SetTrackingSpaceType(TrackingSpaceType.RoomScale);
        
    }

    private void Update()
    {
        // Moves the hand relative to the main camera and at position of the NodeType
        transform.localPosition = InputTracking.GetLocalPosition(NodeType);
        transform.localRotation = InputTracking.GetLocalRotation(NodeType);
    }
}

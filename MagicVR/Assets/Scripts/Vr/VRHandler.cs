using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRHandler : MonoBehaviour {
    void Update()
    {
        OVRInput.Update();
    }

    private void FixedUpdate()
    {
        OVRInput.FixedUpdate();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRHandler : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        OVRInput.Update();
	}

    private void FixedUpdate()
    {
        OVRInput.FixedUpdate();
    }
}

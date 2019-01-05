using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
[RequireComponent(typeof(FixedJoint))]
public class Grabbing : MonoBehaviour {
    // Fixed Joint component used to connect held object to hand at the point of grabbing
    FixedJoint joint;

    // Variables used to calulate velocities of the hand allowing for throwing to work
    Vector3 lastFramePosition;
    Vector3 lastFrameRotation;
    // Any collided with rigidbodies are stored in here
    List <Rigidbody> potential = new List<Rigidbody>();
    
    // Current held object
    public Rigidbody current;
    // The tag an object has to have inorder forit to be grabbale, defaulted to Grab
    public string grabTag = "Grab";
    // How strong the throw are
    public float throwMultiplier;
    // Which button should be pressed to grab
    public OVRInput.RawAxis1D InputName;
    public OVRInput.RawAxis1D AltInputName;
    // Which hand is it tracking
    public XRNode NodeType;
    // Reference to the other hand inorder to handle using two hands on an object
    public Grabbing otherHand;
    // CanGrab used to have a cooldown between grabbing
    public bool CanGrab = true;

    //Exchange Object For Hand
    public bool exchange = false;

    // Did it have gravity before
    public bool Gravity;
    private void Start()
    {
        // Setup the tracking area so hand know where they can and can't go
        XRDevice.SetTrackingSpaceType(TrackingSpaceType.RoomScale);
        // Actually setting the joint varibole to contain the joint on the object
        joint = GetComponent<FixedJoint>();
    }

    private void Update()
    {
        // Moves the hand relative to the main camera and at position of the NodeType
        transform.localPosition = InputTracking.GetLocalPosition(NodeType);
        transform.localRotation = InputTracking.GetLocalRotation(NodeType);

        if (current)
        {
            if (current.GetComponent<ReplaceGrab>())
            {
                current.transform.localPosition = InputTracking.GetLocalPosition(NodeType);
                current.transform.localRotation = InputTracking.GetLocalRotation(NodeType);
            }
        }
        // Input detection, If the button is pressed for than 10% down it grabs
        if ((OVRInput.Get(AltInputName) >= 0.1f || OVRInput.Get(InputName) >= 0.1f))
        {
            // Checks if it can actually grab
            if (!current && CanGrab)
                Grab();
        }
        // If it cant grab then drop the object
        else if(current)
        {
            Drop();
        }

        // Update the varibles used in velocity calculations
        lastFramePosition = transform.position;
        lastFrameRotation = transform.rotation.eulerAngles;
    }

    // Check if the hand is within an object
    private void OnTriggerEnter(Collider other)
    {
        // If it has the gabbing tag then we add it to out potential pick up list
        if (other.gameObject.tag == grabTag)
            potential.Add(other.GetComponent<Rigidbody>());

    }
    // Check if the hand has left an object
    private void OnTriggerExit(Collider other)
    {
        // If it has the gabbing tag then we rempove it to out potential pick up list
        if (other.gameObject.tag == grabTag)
            potential.Remove(other.GetComponent<Rigidbody>());
    }
    // The actually grab handler
    void Grab()
    {
        // Get the nearest object to the hand from the potential list
        current = NearestRb();
        // If their is an object there runs this
        if (current)
        {
            // Makes sure it doesnt fall out the hand
            Gravity = current.useGravity;
            current.useGravity = false;

            if (current.GetComponent<ReplaceGrab>())
            {
                Toggle(false);
                current.position = transform.position;
            }
            else
            {
                joint.connectedBody = current;
            }
        }

    }
    // Dropping and throwing of an object
    public void Drop()
    {
        // Check if an item actually is in hand
        if (current)
        {
            // Checks if the other hand has the same object in hand
            if (otherHand.current != current)
            {
                // If the other hand doesnt have same object 
                // connect the joint
                joint.connectedBody = null;
                // Turn back on gravity (Later might need to store whether we actually want gravity or not)
                current.useGravity = Gravity;
                // Calculate the velocity of the hand then divides by deltaTime to make as smooth as possible
                Vector3 CurrentVelocity = (transform.position - lastFramePosition) / Time.deltaTime;
                current.velocity = CurrentVelocity * throwMultiplier;
                // Angular velocites arethen calculated, as of the current moment a bit buggy but works alright
                Vector3 CurrentAngular = (transform.rotation.eulerAngles - lastFrameRotation) / Time.deltaTime;
                current.angularVelocity = CurrentAngular;

                // Finally stop it from being stored as the held object
                current = null;
            }
            else
            {
                // If the other hand does have the same object
                // Disconnect this hands joint
                joint.connectedBody = null;

                // Finally stop it from being stored as the held object
                current = null;
            }
            Toggle(true);
        }
    }
    // Simple way to find nearest Rb (could benefit optimisation to run a priority array instead)
    Rigidbody NearestRb()
    {
        Rigidbody nearest = null;

        float min = float.MaxValue;
        float distance = 0;

        foreach(Rigidbody p in potential)
        {
            distance = (p.gameObject.transform.position - transform.position).sqrMagnitude;

            if(distance < min)
            {
                min = distance;
                nearest = p;
            }
        }


        return nearest;
    }

    // Stops grabbing from working for x amount of seconds
    public void DisableThenEnableGrabbing(float enableTime)
    {
        CanGrab = false;
        StartCoroutine(AllowGrabbing(enableTime));
    }
    //waits x amount of seconds then re-enables Grabbing
    IEnumerator AllowGrabbing(float waitTime)
    {
        yield return new WaitForSecondsRealtime(waitTime);
        CanGrab = true;
    }


    public void Toggle(bool onOff)
    {
        GetComponent<MeshRenderer>().enabled = onOff;
    }
}

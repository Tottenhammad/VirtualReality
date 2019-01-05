using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(LineRenderer))]
public class Blade : MonoBehaviour
{
    public Collider col;
    LineRenderer line;
    public GameObject StartPoint;
    public GameObject EndPointer;
    Vector3 endPoint;

    public float Speed = 10;
    public bool Expanded = false;

    private void Awake()
    {
        endPoint = EndPointer.transform.localPosition;
        EndPointer.transform.position = StartPoint.transform.position;
        line = GetComponent<LineRenderer>();
    }
    private void Start()
    {
        line.positionCount = 2;

    }

    private void Update()
    {
        if (OVRInput.GetDown(OVRInput.RawButton.RIndexTrigger))
            Expanded = !Expanded;
        line.SetPosition(0, StartPoint.transform.position);
        line.SetPosition(1, EndPointer.transform.position);
        if(Expanded && Vector3.Distance(EndPointer.transform.localPosition, endPoint) > 0.01f)
        {
            line.enabled = true;
            EndPointer.transform.localPosition = Vector3.Lerp(EndPointer.transform.localPosition, endPoint, 1 * Speed * Time.deltaTime);
        }else if(!Expanded && Vector3.Distance(EndPointer.transform.localPosition, StartPoint.transform.localPosition) > 0.01f)
        {
            EndPointer.transform.localPosition = Vector3.Lerp(EndPointer.transform.localPosition, StartPoint.transform.localPosition, 1 * Speed * Time.deltaTime);
        }
        else if (!Expanded)
        {
            line.enabled = false;
        }
    }
}

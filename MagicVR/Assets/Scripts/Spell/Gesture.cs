using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gesture : MonoBehaviour { 
    public Transform head;
    public Transform wandTip;
    public Transform lastTracker;
    public Transform refObject;
    bool referenceStatus;
    Vector3 direction;
    public float minDist;
    public float directionTolerance;
    public List<int> spellSequence;
    public int[] fireCode = new int[3];
    void Start()
    {
        spellSequence = new List<int>();
        lastTracker = new GameObject().transform;
        lastTracker.parent = head;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            Track();
        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            ResetTracking();
        }
    }

    void ResetTracking()
    {
        Destroy(refObject.gameObject);
        Destroy(lastTracker.gameObject);
        referenceStatus = false;
        spellSequence.Clear();
        spellSequence.Add(0);
    }

    void BuildRefObjects()
    {
        refObject = new GameObject().transform;
        refObject.position = head.position;
        refObject.rotation = head.rotation;
        refObject.eulerAngles = new Vector3(0, refObject.eulerAngles.y, 0);
        lastTracker = new GameObject().transform;
        lastTracker.parent = refObject;
        referenceStatus = true;
    }

    Vector3 GetTrackerDirection(Transform tracker)
    {
        var heading = tracker.transform.localPosition - lastTracker.transform.localPosition;
        var distance = heading.magnitude;
        var dir = heading / distance;
        return dir;
    }

    void Track()
    {
        //dirNum is basically an integer designating the direciton of 
        int dirNum = 0;
        Transform tracker = new GameObject().transform;
        tracker.position = wandTip.position;

        if (!referenceStatus || lastTracker == null)
        {
            BuildRefObjects();
            lastTracker.position = tracker.position;
        }

        refObject.eulerAngles = new Vector3(0, head.eulerAngles.y, 0);
        tracker.parent = refObject;

        if (lastTracker != null)
        {
            float dist = Vector3.Distance(lastTracker.position, tracker.position);

            if (dist > minDist)
            {
                direction = GetTrackerDirection(tracker);
                lastTracker.position = tracker.position;
            }

            var dirY = direction.x;
            var dirX = direction.y;

            if (Mathf.Abs(dirX) < directionTolerance)
            {
                dirX = 0;
            }

            if (Mathf.Abs(dirY) < directionTolerance)
            {
                dirY = 0;
            }

            if (dirX > 0)
            {
                if (dirY == 0)
                {
                    //print("up");
                    dirNum = 8;
                }

                if (dirY > 0)
                {
                    //print("up right");
                    dirNum = 1;
                }

                if (dirY < 0)
                {
                    //print("up left");
                    dirNum = 7;
                }
            }

            if (dirX < 0)
            {
                if (dirY == 0)
                {
                    //print("down");
                    dirNum = 4;
                }

                if (dirY > 0)
                {
                    //print("down right");
                    dirNum = 3;
                }

                if (dirY < 0)
                {
                    //print("down left");
                    dirNum = 5;
                }
            }

            if (dirX == 0)
            {
                if (dirY == 0)
                {
                    //print("nowhere");
                    dirNum = 0;
                }

                if (dirY > 0)
                {
                    //print("right");
                    dirNum = 2;
                }

                if (dirY < 0)
                {
                    //print("left");
                    dirNum = 6;
                }
            }

            int spellCount = spellSequence.Count;

            if (spellSequence.Count > 2)
            {
                if (dirNum != spellSequence[spellCount - 1])
                {
                    spellSequence.Add(dirNum);

                    if (spellCount + 1 > 2)
                    {
                        DetectPattern();
                    }
                }
            }

            if (spellCount < 3)
            {
                spellSequence.Add(dirNum);
            }
        }

        Destroy(tracker.gameObject);
    }

    void DetectPattern()
    {
        int spellCount = spellSequence.Count;

        for (int i = 0; i < spellCount; i++)
        {
            if (spellSequence[i] == fireCode[0])
            {
                if ((i + 1 < spellCount) && (spellSequence[i + 1] == fireCode[1]))
                {
                    if ((i + 2 < spellCount) && (spellSequence[i + 2] == fireCode[2] || spellSequence[i + 2] == 8 || spellSequence[i + 2] == 2))
                    {
                        // Boom
                    }
                }
            }
        }
    }
}

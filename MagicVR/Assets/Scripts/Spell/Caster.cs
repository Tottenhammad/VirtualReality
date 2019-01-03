using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Windows.Speech;
using UnityEngine.UI;
public class Caster : MonoBehaviour
{

    // Base Stuff
    public Transform castPoint;
    public List<SOSpell> useAbleSpells = new List<SOSpell>();
    public SpellCastHandler sCH;
    void UseSpell(SOSpell spell)
    {
        sCH.SetCurrentSpell(spell, castPoint, 1);
    }
    void Cast(SOSpell spell)
    {
        GameObject gO = Instantiate(spell.spellObject, castPoint.position, castPoint.rotation);
        gO.GetComponent<Rigidbody>().AddForce(castPoint.forward * spell.spellSpeed);

        Destroy(gO, 4);
    }


    // Speech
    KeywordRecognizer keywordRecognizer;
    List<string> spellCasters = new List<string>();
    private void Start()
    {
        UpdateSpellRecog();
        sequence = new List<int>();
        tracker = new GameObject().transform;
        tracker.parent = head;


        TextAsset[] gesturesXml = Resources.LoadAll<TextAsset>("GestureSet/10-stylus-MEDIUM/");
        foreach (TextAsset gestureXml in gesturesXml)
            trainingSet.Add(DollarGestureIO.ReadGestureFromXML(gestureXml.text));
    }
    void UpdateSpellRecog()
    {
        foreach (SOSpell spell in useAbleSpells)
        {
            spellCasters.Add(spell.spellName);
        }

        keywordRecognizer = new KeywordRecognizer(spellCasters.ToArray());
        keywordRecognizer.OnPhraseRecognized += RecognizedWord;
        keywordRecognizer.Start();
    }
    void RecognizedWord(PhraseRecognizedEventArgs speech)
    {
        foreach (SOSpell spell in useAbleSpells)
        {
            if (speech.text == spell.spellName)
                UseSpell(spell);
        }
    }

    // Gesture Recog
    public Transform head;
    public Transform tracker;
    public Transform rotRefObj;
    bool refStatus;
    Vector3 currentDirection;
    public float minDist;
    public float directionTolerance;
    public List<int> sequence = new List<int>();
    public OVRInput.RawButton Draw;
    public OVRInput.RawButton Activate;


    public Text test;
    bool tracking;

    bool flip = false;
    private void Update()
    {
        if (OVRInput.GetDown(Activate))
        {
            RunRecog();
            ResetDollar();
            flip = true;
        }
        if (OVRInput.GetDown(Draw))
            strokeId += 1;
        if (Input.GetKey(KeyCode.A) || OVRInput.Get(Draw) && !flip)
        {
            DollarTest();
            //Track();
            tracking = true;

            if (sequence.Count > 10)
                sequence.RemoveAt(0);
        }
        else
        {
            if (sequence.Count > 1)
            {
                CheckForMatch();
                ResetTracking();
                tracking = false;
            }
            flip = false;
        }

        test.text = "";
        foreach(int i in sequence)
        {
            test.text += i.ToString();
        }
    }
    void CheckForMatch()
    {
        foreach(SOSpell spell in useAbleSpells)
        {
            for(var i = 0; i < sequence.Count; i++)
            {
                int correctCounter = 0;
                if (i + spell.spellCastPos.Count > sequence.Count)
                    break;
                for(var j = 0; j < spell.spellCastPos.Count; j++)
                {
                    if (sequence[i + j] == spell.spellCastPos[j])
                        correctCounter += 1;
                }
                if (correctCounter == spell.spellCastPos.Count)
                {
                    UseSpell(spell);
                    ResetTracking();
                }
                }

        }
    }

    void RefBuild()
    {
        rotRefObj = new GameObject().transform;
        rotRefObj.position = head.position;
        rotRefObj.rotation = head.rotation;
        rotRefObj.eulerAngles = new Vector3(0, rotRefObj.eulerAngles.y, 0);
        tracker = new GameObject().transform;
        tracker.parent = rotRefObj;
        refStatus = true;
    }


    Vector3 lastPoint;
    void Track()
    {
        int dir = 0;
        Transform track = new GameObject().transform;
        track.position = castPoint.position;

        if(!refStatus || tracker == null)
        {
            RefBuild();
            tracker.position = track.position;
        }
        rotRefObj.eulerAngles = new Vector3(0, head.eulerAngles.y, 0);
        track.parent = rotRefObj;
        if(tracker != null)
        {
            float distanceCheck = Vector3.Distance(tracker.position, track.position);
            if(distanceCheck > minDist)
            {
                currentDirection = GetTrackerDirection(track);
                tracker.position = track.position;
            }

            var dirX = currentDirection.x;
            var dirY = currentDirection.y;

            if(Mathf.Abs(dirX) < directionTolerance)
            {
                dirX = 0;
            }
            if(Mathf.Abs(dirY) < directionTolerance)
            {
                dirY = 0;
            }

            if (dirX > 0)
            {
                if (dirY == 0)
                {
                    // UP
                    dir = 8;
                }

                if (dirY > 0)
                {
                    // UP RIGHT
                    dir = 1;
                }

                if (dirY < 0)
                {
                    // UP LEFT
                    dir = 7;
                }
            }
            if (dirX == 0)
            {
                if (dirY == 0)
                {
                    // N/A
                    dir = 0;
                }

                if (dirY > 0)
                {
                    // RIGHT
                    dir = 2;
                }

                if (dirY < 0)
                {
                    // LEFT
                    dir = 6;
                }
            }
            if (dirX < 0)
            {
                if (dirY == 0)
                {
                    // DOWN
                    dir = 4;
                }

                if (dirY > 0)
                {
                    // DOWN RIGHT
                    dir = 3;
                }

                if (dirY < 0)      
                {
                    // DOWN LEFT
                    dir = 5;
                }
            }
            if (sequence.Count == 0)
            {
                sequence.Add(0);
                lastPoint = track.position;
            }
            if (dir != sequence[sequence.Count - 1] && Vector3.Distance(track.position, lastPoint) > 0.1f)
            {
                lastPoint = track.position;
                sequence.Add(dir);
            }
        }


        Destroy(track.gameObject);
    }

    private void ResetTracking()
    {
        Destroy(rotRefObj.gameObject);
        Destroy(tracker.gameObject);
        refStatus = false;
        sequence.Clear();
        sequence.Add(0);
    }

    Vector3 GetTrackerDirection(Transform trackee)
    {
        var headingDir = trackee.transform.localPosition - tracker.localPosition;
        var distance = headingDir.magnitude;
        return headingDir / distance;
    }

    // Dollar-P Implementation

    List<DollarGesture> trainingSet = new List<DollarGesture>();
    List<DollarPoint> points = new List<DollarPoint>();
    int strokeId = -1;

    void DollarTest()
    {
        Transform track = new GameObject().transform;
        track.position = castPoint.position;

        if (!refStatus || tracker == null)
        {
            RefBuild();
            tracker.position = track.position;
        }
        rotRefObj.eulerAngles = new Vector3(0, head.eulerAngles.y, 0);
        track.parent = rotRefObj;

        points.Add(new DollarPoint(track.localPosition.x, track.localPosition.y, strokeId));

        //Debug.Log(track.localPosition);
    }


    void ResetDollar()
    {
        points.Clear();
        strokeId = -1;
    }

    void RunRecog()
    { 
        DollarGesture prep = new DollarGesture(points.ToArray());
        DollarResult result = DollarPointCloud.Classify(prep, trainingSet.ToArray());

        Debug.Log(result.Gesture);
        test.text = result.Gesture;
    }
}

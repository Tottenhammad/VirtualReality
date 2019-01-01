using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Windows.Speech;

public class Caster : MonoBehaviour
{

    // Base Stuff
    public Transform castPoint;
    public GameObject spawnObject;
    public List<SOSpell> useAbleSpells = new List<SOSpell>();


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
                Cast(spell);
        }
    }

    // Gesture Recog
    public List<Vector3> gesturePoints = new List<Vector3>();
    public List<Vector3> gestureDirections = new List<Vector3>();
    public List<float> gestureMagnitudes = new List<float>();

    Vector3 startPoint;
    private void Update()
    {

        if (gesturePoints.Count == 0)
        {
            gesturePoints.Add(Vector3.zero);
            startPoint = transform.position;
        }
        Vector3 newPoint = (transform.position - startPoint);
        if (Vector3.Distance(newPoint, gesturePoints[gesturePoints.Count - 1]) > 0.2f)
        {
            gesturePoints.Add(newPoint);
            gestureDirections.Add(newPoint - gesturePoints[gesturePoints.Count - 2]);
            gestureMagnitudes.Add(gestureDirections[gestureDirections.Count - 1].magnitude);
        }

    }


}

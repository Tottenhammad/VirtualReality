using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Spell", menuName = "Custom/Spell")]
public class SOSpell : ScriptableObject{
    public string spellName;
    public string spellDesc;
    public float spellSpeed;
    public GameObject spellObject;
}

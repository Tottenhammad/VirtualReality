using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(ApplySpellAffects))]
public class ProjectileSpell : MonoBehaviour {
    public ApplySpellAffects aSA;
    SOSpell spell;
    private void Awake()
    {
        aSA = GetComponent<ApplySpellAffects>();
    }

    private void OnTriggerEnter(Collider other)
    {
        aSA.Affect(other.transform, other.ClosestPoint(transform.position));
    }
}

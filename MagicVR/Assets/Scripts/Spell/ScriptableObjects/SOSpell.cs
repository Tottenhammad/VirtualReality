using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Spell", menuName = "Custom/Spell")]
public class SOSpell : ScriptableObject{
    public string spellName;
    public string spellDesc;


    public List<int> spellCastPos = new List<int>();

    public List<Damages> Damage = new List<Damages>();
    public bool OneHit = true;
    public CastTypes castType;

    public float knockBackEffect = 0;
    [Header("Only If projectile")]
    public GameObject spellObject;
    public float projectileLife;
    public float spellSpeed;

}

public enum CastTypes { RAYCAST, PROJECTILE, SELF };
public enum SpellDamageTypes { FIRE, WATER, POISON, NEUTRUAL}

[System.Serializable]
public class Damages {
    public SpellDamageTypes DamageType;
    public float instantDamageAmount;
    public float overTimeDamageAmount;
    public float overTimeDamageDuration;

}

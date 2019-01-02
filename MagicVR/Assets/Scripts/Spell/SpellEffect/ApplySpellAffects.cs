using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplySpellAffects : MonoBehaviour {
    public SOSpell spell;


    public void Affect(Transform HitObj, Vector3 point)
    {
        AffectedBySpells affected = HitObj.GetComponent<AffectedBySpells>();
        Debug.Log(affected);
        if (affected)
        {
            foreach(Damages d in spell.Damage)
            {
                if(d.instantDamageAmount != 0)
                    affected.ChangeHealth(d.DamageType, d.instantDamageAmount);
                if(d.overTimeDamageAmount != 0)
                    affected.ChangeHealthOvertime(d.DamageType, d.overTimeDamageAmount, d.overTimeDamageDuration);
                if(spell.knockBackEffect != 0)
                {
                    Rigidbody hitRB = HitObj.GetComponent<Rigidbody>();
                    if (hitRB)
                        hitRB.AddForceAtPosition(point.normalized, transform.position);
                }
            }
        }


        if (spell.OneHit && spell.castType == CastTypes.PROJECTILE)
            Destroy(this.gameObject);
    }
}

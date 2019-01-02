using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellCastHandler : MonoBehaviour {

    public SOSpell currentEquiptSpell;

    Transform castPoint;
    bool CastListen;


    Vector3 LastPointPosition;
    float LastMag = 0;
    public void SetCurrentSpell(SOSpell spell, Transform CastPoint, int ListenTime = 0)
    {
        currentEquiptSpell = spell;
        castPoint = CastPoint;
        if (ListenTime != 0)
            CastListen = true;
        else
            Cast(currentEquiptSpell);
    }

    void ClearCurrent()
    {
        CastListen = false;
        currentEquiptSpell = null;
    }

    private void Update()
    {

        float magnitude = (transform.position - LastPointPosition).magnitude;
        if (CastListen && magnitude < 0.0005f && LastMag > magnitude)
        {
            Cast(currentEquiptSpell);
            currentEquiptSpell = null;
        }

        LastMag = magnitude;
        LastPointPosition = transform.position;
    }
    void Cast(SOSpell spell)
    {
        if (spell.castType == CastTypes.PROJECTILE) {
            GameObject gO = Instantiate(spell.spellObject, castPoint.position, castPoint.rotation);
            Physics.IgnoreCollision(transform.GetComponent<Collider>(), gO.GetComponent<Collider>());
            gO.GetComponent<Rigidbody>().AddForce(castPoint.forward * spell.spellSpeed);
            ProjectileSpell pS = gO.GetComponent<ProjectileSpell>();
            pS.aSA.spell = spell;
            Destroy(gO, spell.projectileLife);

        }

        ClearCurrent();
    }

}

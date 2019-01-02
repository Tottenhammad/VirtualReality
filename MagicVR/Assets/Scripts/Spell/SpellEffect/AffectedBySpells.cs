using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AffectedBySpells : MonoBehaviour {
    public float health = 100;
    public bool respawn = true;
    public float respawnTime;
    public Vector3 startPos;
    public Quaternion startRot;

    public List<SpellDamageTypes> Immunities = new List<SpellDamageTypes>();

    public void ChangeHealth(SpellDamageTypes damageType, float DamageAmount)
    {
        if (Immunities.Contains(damageType))
            return;
        else
            health += DamageAmount;
    }
    public void ChangeHealthOvertime(SpellDamageTypes damageType, float DamageAmount, float duration)
    {
        Debug.Log("In");
        if (Immunities.Contains(damageType))
            return;
        else
        {
            StartCoroutine(HealthOvertime(DamageAmount, duration));
        }
    }

    private void Update()
    {
        if(health < 0)
        {
            if (respawn)
            {
                ToggleOnOff(false);
                StartCoroutine(Respawn(respawnTime));
            }
            else{
                Destroy(gameObject);
            }
        }
    }

    public void ToggleOnOff(bool state)
    {
        GetComponent<MeshRenderer>().enabled = state;
        GetComponent<Collider>().enabled = state;
    }


    IEnumerator Respawn(float Time)
    {
        yield return new WaitForSecondsRealtime(Time);
        ToggleOnOff(true);
    }



    IEnumerator HealthOvertime(float DamageAmount, float duration)
    {
        float totalDamageDone = 0;
        float DamagePerMilisecond = DamageAmount / (duration * 10);

        while(totalDamageDone !=DamageAmount){
            if (totalDamageDone + DamagePerMilisecond > DamageAmount)
                break;
            health += DamagePerMilisecond;
            totalDamageDone += DamagePerMilisecond;
            yield return new WaitForSecondsRealtime(0.1f);
        }

        if(totalDamageDone < DamageAmount)
        {
            health += DamageAmount - totalDamageDone;
        }

    }
}

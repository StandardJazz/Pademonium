using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestDummyBot : ACreeps
{
    void Start()
    {
        CreepsStart();
        
        SetStats(300.0f, 3.0f, 45.0f, 3.0f, 2.0f,1.0f,2.0f);
        SetComponents();
        SetCoroutine();
        bAggressive = false;
    }

    void Update()
    {
        CreepsUpdate();
    }

    protected override IEnumerator MeleeAttack_CO()
    {
        while (true)
        {
            if (applyDmgOnce && creep_anim.GetBool("A"))
            {
                applyDmgOnce = false;
                objToAttack.GetComponent<AHeroes>().GetHit(attackDmg);
            }
            yield return new WaitForSeconds(0.001f);
        }
    }
}

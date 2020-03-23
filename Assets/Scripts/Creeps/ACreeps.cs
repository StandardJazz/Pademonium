using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ACreeps : MonoBehaviour
{
    //protected 
    protected Animator creep_anim = null;
    protected float max_hp;
    protected float current_hp;
    
    private bool isDead = false;
    protected IEnumerator Dying;

    // Start is called before the first frame update
    protected void CreepStart()
    {
        Dying = Dying_Co();
        isDead = false;
    }

    protected void Update()
    {

    }

    protected IEnumerator Dying_Co()
    {
        creep_anim.SetBool("isDead", true);
        isDead = true;
        print("creep Dead : " + isDead);

        creep_anim.SetTrigger("Dead");
        yield return new WaitForSeconds(2.0f);
        Destroy(this.gameObject);
    }

    public void GetHit(GameObject WhoAttack, float Damage)//cc기도 추가.
    {
        current_hp -= Damage;

        if (current_hp <= 0.0f) StartCoroutine(Dying);
        

        if (!isDead) creep_anim.SetTrigger("GetHit");


        Vector3 rot = new Vector3(WhoAttack.transform.position.x, this.transform.position.y, WhoAttack.transform.position.z);
        WhoAttack.transform.position = rot;

        this.transform.LookAt(WhoAttack.transform);
    }


    //In Canvas
    public float GetHPRatio()
    {
        return current_hp / max_hp;
    }
     
    public bool IsDead()
    {
        return isDead;
    }

    public bool CanSee()
    {
        return true;
    }
}

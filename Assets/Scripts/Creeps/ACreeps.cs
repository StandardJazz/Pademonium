using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ACreeps : MonoBehaviour
{
    //Component
    protected Animator creep_anim = null;
    protected NavMeshAgent creep_nav = null;

    //Stats
    protected float max_hp;
    protected float current_hp;

    //attack
    protected float attackDmg;
    protected float attackSpeed;
    protected float attackRange;
    protected float perceptionRange;
    protected float attackTimer;

    //move
    protected float norspeed = 1.0f;
    protected float chasespeed = 2.0f;
    protected Vector3 targetPos = Vector3.zero;
  
    //die
    private bool isDead = false;
    protected IEnumerator Dying;
    protected IEnumerator AdjustCrowdControl;

    //state
    protected CROWD_CONTROL_TYPE cc_state;


    protected void CreepsStart()
    {
        isDead = false;
        attackTimer = attackSpeed;
    }
    
    protected void CreepsUpdate()
    {

    }

    protected void SetStats(float hp, float perhp, float dmg, float attackspeed, float attackrange)
    {
        this.max_hp = hp ;
        this.current_hp = max_hp;
        this.attackDmg = dmg;
        this.attackSpeed = attackspeed;
        this.attackRange = attackrange;
    }

    protected void SetComponents()
    {
        creep_anim = GetComponent<Animator>();
        creep_nav = GetComponent<NavMeshAgent>();
        targetPos = transform.position;
    }

    protected void SetCoroutine()
    {
        
    }

    public void GetHit(float Damage)
    {
        current_hp -= Damage;

        if (current_hp <= 0.0f)
        {
            Dying = Dying_Co();
            StartCoroutine(Dying);
        }
        else
        {
            if(Damage >15.0f)
            {
                creep_anim.SetTrigger("GetHit");
            }
        }
    }

    public void GetHit(float Damage, CROWD_CONTROL_TYPE cctype, float adjustTime)//cc기도 추가.
    {
        GetHit(Damage);

        cc_state |= cctype;

        AdjustCrowdControl = AdjustCrowdControl_CO(cctype, adjustTime);
        StartCoroutine(AdjustCrowdControl);
    }

    public void GetHit(GameObject WhoAttack, float Damage)//cc기도 추가.
    {
        GetHit(Damage);

        if (WhoAttack == null) return;

        Vector3 rot = new Vector3(WhoAttack.transform.position.x, this.transform.position.y, WhoAttack.transform.position.z);
        WhoAttack.transform.position = rot;

        this.transform.LookAt(WhoAttack.transform);
    }

    public void GetHit(GameObject WhoAttack, float Damage, CROWD_CONTROL_TYPE cctype, float adjustTime)//cc기도 추가.
    {
        GetHit(Damage, cctype, adjustTime);

        Vector3 rot = new Vector3(WhoAttack.transform.position.x, this.transform.position.y, WhoAttack.transform.position.z);
        WhoAttack.transform.position = rot;

        this.transform.LookAt(WhoAttack.transform);
    }

    protected IEnumerator AdjustCrowdControl_CO(CROWD_CONTROL_TYPE type, float adjustTime)
    {
        switch(type)
        {
            case CROWD_CONTROL_TYPE.NONE:
                break;
            case CROWD_CONTROL_TYPE.SLOW:
                creep_nav.speed *= 0.5f;
                break;
            case CROWD_CONTROL_TYPE.STUN:
                creep_nav.speed = 0.0f;
                break;
        }
        yield return new WaitForSeconds(adjustTime);

        creep_nav.speed = norspeed;
        yield return null;
    }

    protected IEnumerator Dying_Co()
    {
        creep_anim.SetBool("isDead", true);
        isDead = true;
        print("creep Dead : " + isDead);
        
        yield return new WaitForSeconds(2.0f);
        Destroy(this.gameObject);
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

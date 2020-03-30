using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

abstract public class ACreeps : MonoBehaviour
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
    protected bool isAttackable;
    protected bool attackReady;
    protected bool applyDmgOnce;
    protected abstract IEnumerator MeleeAttack_CO();

    //perception
    private int layerMask = 1;
    private NavMeshPath path;
    protected bool bAggressive;
    protected GameObject objToAttack = null;

    //move
    protected float norSpeed = 1.0f;
    protected float maxSpeed = 2.0f;
    protected float speedFactor = 1.0f;
    protected bool isChasing;
    protected Vector3 targetPos = Vector3.zero;
    protected bool isMove;

    //die
    private bool isDead = false;
    protected IEnumerator Dying;
    protected IEnumerator Perception;
    protected IEnumerator AdjustCrowdControl;
    protected IEnumerator IdlePatrol;
    protected IEnumerator MeleeAttack;



    //state
    protected CROWD_CONTROL_TYPE cc_state;


    protected void CreepsStart()
    {
        isChasing = false;
        layerMask = 1;
        layerMask <<= 11;
        isDead = false;
        attackTimer = attackSpeed;
        speedFactor = 1.0f;
    }

    protected void CreepsUpdate()
    {
        isDead = current_hp <= 0.0f;
        creep_anim.SetBool("isDead", isDead);


        if (attackTimer < attackSpeed)
        {
            attackTimer += Time.deltaTime;
            attackReady = false;
        }
        else
            attackReady = true;

        if (isDead)
        {
            creep_nav.isStopped = true;
        }
        else
            Move();

    }

    protected void SetStats(float hp, float perhp, float dmg, float attackspeed, float attackrange, float norspeed, float maxspeed)
    {
        this.max_hp = hp;
        this.current_hp = max_hp;
        this.attackDmg = dmg;
        this.attackSpeed = attackspeed;
        this.attackRange = attackrange;
        this.norSpeed = norspeed;
        this.maxSpeed = maxspeed;
        perceptionRange = 8.0f;
    }

    protected void SetComponents()
    {
        creep_anim = GetComponent<Animator>();
        creep_nav = GetComponent<NavMeshAgent>();
        targetPos = transform.position;
    }

    protected void SetCoroutine()
    {
        MeleeAttack = MeleeAttack_CO();
        Perception = Perception_CO();

        StartCoroutine(MeleeAttack);
        StartCoroutine(Perception);
    }

    private void Move()
    {
        Vector2 targetPos_xz = new Vector2(targetPos.x, targetPos.z);
        Vector2 playerGroundPos = new Vector2(transform.position.x, transform.position.z);

        if()

        isMove = Vector2.Distance(targetPos_xz, playerGroundPos) > 0.1f;
        isMove &= !isAttackable;
        isMove &= creep_anim.GetBool("isMovable");
        creep_anim.SetBool("isMove", isMove);

        if (isMove)
        {
            creep_nav.isStopped = false;
            creep_nav.updateRotation = true;
            creep_nav.SetDestination(targetPos);
            norSpeed = Vector3.Distance(Vector3.zero, creep_nav.velocity) / maxSpeed;
            creep_anim.SetFloat("Speed", norSpeed);
            print("move");
        }
        else
        {
            creep_nav.isStopped = true;
            creep_anim.SetFloat("Speed", 0.0f);
        }
    }
    
    protected IEnumerator Perception_CO()
    {
        bool firstTimeRotFixed = false;

        while (true)
        {
            if (bAggressive)
            {
                Collider[] cols = Physics.OverlapSphere(this.transform.position, perceptionRange, layerMask);

                float nearHeroDistance = float.MaxValue;
                int minHeroIndex = 0;
                Vector3 nearestHeroPos = this.transform.position;

                print("cols.Length : " + cols.Length);
                //경로 중 제일 짧은 것을 선출.
                for (int i = 0; i < cols.Length; i++)
                {
                    GameObject hero = cols[i].gameObject;
                    Vector3 yFixedHeroPos = new Vector3(hero.transform.position.x, transform.position.y, hero.transform.position.z);

                    path = new NavMeshPath();
                    float distance = 0.0f;

                    if (creep_nav.CalculatePath(yFixedHeroPos, path))
                    {
                        Vector3[] corners = path.corners;
                        for (int k = 0; k < corners.Length; k++)
                        {
                            if (k + 1 > corners.Length - 1) break;
                            Vector3 temp = new Vector3(0.0f, 5.0f, 0.0f);
                            distance += Vector3.Distance(corners[k], corners[k + 1]);
                        }
                    }

                    if (nearHeroDistance > distance)
                    {
                        nearHeroDistance = distance;
                        minHeroIndex = i;
                        nearestHeroPos = yFixedHeroPos;
                    }
                }


                //해당 영웅과 거리가 공격 사거리에 들어왔을 경우
                if (nearHeroDistance < attackRange)
                {
                    objToAttack = cols[minHeroIndex].gameObject;

                    isAttackable = true;
                    creep_anim.SetBool("isAttackable", isAttackable);

                    if (!firstTimeRotFixed)
                    {
                        firstTimeRotFixed = true;
                        this.transform.LookAt(nearestHeroPos);
                    }
                }
                else
                {
                    isAttackable = false;
                    creep_anim.SetBool("isAttackable", isAttackable);

                    if (nearHeroDistance < perceptionRange)
                    {
                        isChasing = true;
                        targetPos = nearestHeroPos;
                        creep_nav.speed = maxSpeed * speedFactor;
                    }
                    else
                    {
                        isChasing = false;
                        targetPos = transform.position;
                        bAggressive = false;
                        creep_nav.speed = norSpeed * speedFactor;
                    }
                }

            }
            else //bAggressive
            {
                firstTimeRotFixed = false;
                isAttackable = false;
                creep_anim.SetBool("isAttackable", isAttackable);

                objToAttack = null;
            }

            if (isAttackable && attackReady && objToAttack != null)
            {
                attackTimer = 0.0f;
                int rd_att = (int)Random.Range(0.0f, 2.0f);
                //float clampAS = 2.0f - ((attackSpeed - 0.5f) / 1.5f);
                //creep_anim.SetFloat("AttackSpeedMultiplier", clampAS);
                creep_anim.SetInteger("AttackRandomParam", rd_att);
                creep_anim.SetTrigger("Attack");
                applyDmgOnce = true;
            }
            yield return new WaitForSeconds(0.001f);
        }
    }


    public void GetHit(float Damage)
    {
        current_hp -= Damage;

        if (current_hp <= 0.0f)
        {
            bAggressive = false;
            Dying = Dying_Co();
            StartCoroutine(Dying);
        }
        else
        {
            bAggressive = true;
            if (Damage > 15.0f)
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
        switch (type)
        {
            case CROWD_CONTROL_TYPE.NONE:
                break;
            case CROWD_CONTROL_TYPE.SLOW:
                speedFactor = 0.5f;
                break;
            case CROWD_CONTROL_TYPE.STUN:
                //speedFactor = 0.0f;
                break;
        }
        yield return new WaitForSeconds(adjustTime);
        cc_state &= ~type;
        speedFactor = 1.0f;
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

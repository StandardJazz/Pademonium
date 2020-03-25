using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public enum SpellType
{
    NONE = 0, Target, FixedNonTarget, FreeNonTarget
}

abstract public class AHeroes : MonoBehaviour
{
    //Camera
    [SerializeField] protected Camera playerCam = null;

    //NavMesh
    protected NavMeshAgent player_navMesh = null;

    //Move
    protected Vector3 currentPos;
    protected Vector3 targetPos;
    Quaternion directionRotation;   //회전 부드럽게는 됐는데, 짧은 이동때 다 돌지 않음.
    protected bool cursor_StateFind;

    protected bool isMove;
    protected bool rotationVerified;

    protected float maxSpeed = 10.0f;
    protected float norSpeed;

    //MeleeAttack
    protected bool isAttackable;  //사정거리에 들어왔을 경우 (코루틴 안에서)
    protected bool castAttack;    //A키 누른 후 마우스(0)을 누르면 공격하게끔
    protected bool isAutoAttack = false;
    public float attackSpeed;   //하나의 공격 애니메이션이 시작하고 해당 시간 후에 다시 공격 시작 
    protected float attackTimer;
    protected bool attackReady;
    protected bool applyDmgOnce;
    protected float attackDamage; // 공격 데미지
    protected GameObject objToAttack = null; //공격 대상 선별. (가장 가까운)

    //Perception
    protected float perceptionRange;
    protected float attackRange;
    private int layerMask = 1;
    //private bool isCanSee = false;
    private NavMeshPath path;

    //Skill
    protected Spell_Indicator spell_indicator = null;
    [SerializeField] protected Collider Q_Skill_Scope = null;
    [SerializeField] protected Collider W_Skill_Scope = null;
    [SerializeField] protected Collider E_Skill_Scope = null;
    [SerializeField] protected Collider R_Skill_Scope = null;

    public struct SpellInfo
    {
        public SpellType spellType;
        public float range;
        public float bound;
        public float proj_size;
    }

    private SpellInfo[] spellInfos = new SpellInfo[SKILL_NUM];

    private const int SKILL_NUM = 4;
    protected bool[] canUseSkills = new bool[SKILL_NUM];
    protected bool[] canManaSkills = new bool[SKILL_NUM];
    protected float[] sk_cds = new float[SKILL_NUM];
    protected float[] sk_manas = new float[SKILL_NUM];
    protected float[] MAX_SK_CDS = new float[SKILL_NUM];
    protected float[] ct_ratio = new float[SKILL_NUM];

    protected bool[] currentIndicatingSkills = new bool[SKILL_NUM];
    protected bool[] immediateSkills = new bool[SKILL_NUM];
    protected int indicatorIndex = -1;

    protected delegate bool SkillFunc();
    protected SkillFunc[] InvokeSkills = new SkillFunc[4];

    protected HasSkillHit[] skillHitsScripts = new HasSkillHit[SKILL_NUM];
    protected bool[] hasActualHit = new bool[SKILL_NUM];


    //Stats
    protected float currentMP;
    protected float maxMP;
    protected float perMP = 1.0f;

    protected float currentHP;
    protected float maxHP;
    protected float perHP;

    protected bool isDead;

    //Animator
    protected Animator player_anim = null;

    //IEnumerator
    protected IEnumerator Regeneration;
    protected IEnumerator Perception;
    protected IEnumerator MeleeAttack;

    protected abstract bool Use_Q_Skill();
    protected abstract bool Use_W_Skill();
    protected abstract bool Use_E_Skill();
    protected abstract bool Use_R_Skill();

    protected abstract IEnumerator MeleeAttack_CO();

    //Revive
    private bool revive = true;
    private bool isSpawning = true;

    private float deathTime = 6.5f;
    private float deathTimer = 0.0f;
    private Vector3 spawnLocation;

    //private
    private readonly float FIXED_PERCEPTION_RANGE = 9.0f;

    void Awake()
    {
        spawnLocation = new Vector3(5, 1, 5);

    }

    void Revive()
    {
        currentHP = maxHP;
        currentMP = maxMP;
        isDead = false;
        deathTimer = 0.0f;
        targetPos = spawnLocation;

        player_navMesh.speed = 3.0f;
        transform.localScale = new Vector3(1, 1, 1);

        player_anim.SetBool("StartDeadAnim", false);
        player_anim.SetBool("IsDead", false);
        player_anim.SetBool("Spawning", true);
        player_anim.SetBool("isMovable", false);

        transform.position = spawnLocation;
        player_anim.SetTrigger("Revive");
        revive = true;

        Revive_Child();
    }

    void Death()
    {
        currentMP = 0.0f;
        currentHP = 0.0f;
        deathTimer += Time.deltaTime;
        Death_Child();
        if (deathTime < deathTimer) Revive();
    }

    protected virtual void Revive_Child() { }
    protected virtual void Death_Child() { }


    protected void HeroesStart()
    {
        playerCam = Camera.main;
        perceptionRange = FIXED_PERCEPTION_RANGE;

        layerMask <<= 12;//Creep

        castAttack = false;
        cursor_StateFind = false;
        isDead = false;
        attackTimer = attackSpeed;
        applyDmgOnce = false;
    }

    void DyingTest()
    {
        currentHP = -1.0f;
    }

    protected void HeroesUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Space)) DyingTest();


        isDead = currentHP <= 0.0f;
        player_anim.SetBool("IsDead", isDead);

        if (attackTimer < attackSpeed)
        {
            attackTimer += Time.deltaTime;
            attackReady = false;
        }
        else
            attackReady = true;

        if (revive)
        {
            isSpawning = player_anim.GetBool("Spawning");
            revive = isSpawning;
        }

        if (isDead)
        {
            player_navMesh.isStopped = true;
            Death();
        }
        else
            if (!revive) Move();


        Skill();
    }

    //자식 클래스에서 호출
    protected bool SetSkill(float[] mana_costs, float[] coolTime_costs, float[] skillDmgs, bool[] immediatelySkills)
    {
        if (mana_costs.Length != SKILL_NUM
            || coolTime_costs.Length != SKILL_NUM
            || skillDmgs.Length != SKILL_NUM
            || immediatelySkills.Length != SKILL_NUM
          ) return false;

        InvokeSkills[0] = Use_Q_Skill;
        InvokeSkills[1] = Use_W_Skill;
        InvokeSkills[2] = Use_E_Skill;
        InvokeSkills[3] = Use_R_Skill;

        skillHitsScripts[0] = (Q_Skill_Scope != null) ? Q_Skill_Scope.GetComponent<HasSkillHit>() : null;
        skillHitsScripts[1] = (W_Skill_Scope != null) ? W_Skill_Scope.GetComponent<HasSkillHit>() : null;
        skillHitsScripts[2] = (E_Skill_Scope != null) ? E_Skill_Scope.GetComponent<HasSkillHit>() : null;
        skillHitsScripts[3] = (R_Skill_Scope != null) ? R_Skill_Scope.GetComponent<HasSkillHit>() : null;

        for (int i = 0; i < 4; i++)
        {
            currentIndicatingSkills[i] = false;
            hasActualHit[i] = false;

            if (skillHitsScripts[i]) skillHitsScripts[i].SetSkillDmg(skillDmgs[i]);
            sk_manas[i] = mana_costs[i];
            MAX_SK_CDS[i] = coolTime_costs[i];
            immediateSkills[i] = immediatelySkills[i];
        }


        for (int i = 0; i < SKILL_NUM; i++)
            for (int j = 0; j < SKILL_NUM; j++)
                sk_cds[i] = MAX_SK_CDS[j];

        return true;
    }

    protected void SetSpellInfos(SpellInfo[] spellInfos)
    {
        for (int i = 0; i < SKILL_NUM; i++)
        {
            this.spellInfos[i] = spellInfos[i];
        }
    }

    protected void SetStats(float hp, float perhp, float mp, float permp, float dmg, float attackspeed, float attackrange)
    {
        if (mp == 0) print("마나를 사용하지 않는 영웅");

        maxHP = hp;
        currentHP = maxHP;

        maxMP = mp;
        currentMP = maxMP;

        attackRange = attackrange;
        attackDamage = dmg;
        attackSpeed = attackspeed;
    }

    protected void SetComponents()
    {
        player_anim = this.GetComponent<Animator>();
        player_navMesh = this.GetComponent<NavMeshAgent>();
        targetPos = transform.position;
    }

    protected void SetCoroutine()
    {
        Regeneration = Regeneration_CO();
        MeleeAttack = MeleeAttack_CO();
        Perception = Perception_CO();

        StartCoroutine(Regeneration);
        StartCoroutine(Perception);
        StartCoroutine(MeleeAttack);
    }

    private void Move()
    {
        if(player_navMesh.)

        if (Input.GetKeyDown(KeyCode.A)) castAttack = true;

        if (castAttack && Input.GetMouseButtonDown(0))
        {
            isAutoAttack = true;
            targetPos = playerCam.GetComponent<FollowCam>().GetMousePoint();
        }

        if (Input.GetMouseButtonDown(1))
        {
            EndSpell();
            isAutoAttack = false;
            castAttack = false;
            targetPos = playerCam.GetComponent<FollowCam>().GetMousePoint();
        }


        Vector2 targetPos_xz = new Vector2(targetPos.x, targetPos.z);
        Vector2 playerGroundPos = new Vector2(transform.position.x, transform.position.z);

        isMove = Vector2.Distance(targetPos_xz, playerGroundPos) > 0.01f;
        isMove &= !isAttackable;
        isMove &= player_anim.GetBool("isMovable");
        player_anim.SetBool("isMove", isMove);

        if (isMove)
        {
            player_navMesh.isStopped = false;
            player_navMesh.updateRotation = true;
            player_navMesh.SetDestination(targetPos);
            norSpeed = Vector3.Distance(Vector3.zero, player_navMesh.velocity) / maxSpeed;
            player_anim.SetFloat("Speed", norSpeed);
        }
        else
        {
            player_navMesh.isStopped = true;
            player_anim.SetFloat("Speed", 0.0f);
        }
    }
    private void DetIndicator()
    {
        int skillIndex = -1;

        if (Input.GetKeyDown(KeyCode.Q))
        {
            skillIndex = 0;
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            skillIndex = 1;
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            skillIndex = 2;
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            skillIndex = 3;
        }

        if (skillIndex < 0) return;
        else
        {
            if (indicatorIndex == skillIndex) indicatorIndex = -1;
            else indicatorIndex = skillIndex;
        }
    }

    private void EndSpell()
    {
        for (int i = 0; i < SKILL_NUM; i++)
            currentIndicatingSkills[i] = false;
        indicatorIndex = -1;
    }

    private void Skill()
    {
        for (int i = 0; i < SKILL_NUM; i++)
        {
            canManaSkills[i] = currentMP > sk_manas[i];
            ct_ratio[i] = sk_cds[i] / MAX_SK_CDS[i];
            canUseSkills[i] = (ct_ratio[i] >= 1.0f);
        }

        if (!isDead && !isSpawning)
        {
            DetIndicator();

            for (int i = 0; i < SKILL_NUM; i++)
            {
                bool bCheck = true;
                bCheck &= canUseSkills[i];
                bCheck &= canManaSkills[i];
                bCheck &= !(player_anim.GetBool("isCasting"));
                bCheck &= (i == indicatorIndex);

                if (bCheck)
                {
                    currentIndicatingSkills[i] = true;

                    bool bSkilled = false;

                    if (immediateSkills[i]) bSkilled = InvokeSkills[i]();
                    else if (Input.GetMouseButtonDown(0)) bSkilled = InvokeSkills[i]();

                    if (bSkilled)
                    {
                        EndSpell();
                        sk_cds[i] = 0.0f;
                    }
                }
                else sk_cds[i] += Time.deltaTime;
            }

        }


        //실질적인 스킬 충돌 체크
        if (skillHitsScripts[0] != null) skillHitsScripts[0].SetActualHitTime(player_anim.GetBool("QSkill"));
        if (skillHitsScripts[1] != null) skillHitsScripts[1].SetActualHitTime(player_anim.GetBool("WSkill"));
        if (skillHitsScripts[2] != null) skillHitsScripts[2].SetActualHitTime(player_anim.GetBool("ESkill"));
        if (skillHitsScripts[3] != null) skillHitsScripts[3].SetActualHitTime(player_anim.GetBool("RSkill"));
    }

    protected IEnumerator Regeneration_CO()
    {
        while (true)
        {
            if (currentMP + perMP > maxMP)
                currentMP = maxMP;
            else
                currentMP += perMP;

            yield return new WaitForSeconds(0.25f);
        }
    }

    //항상 켜놓고, 죽을 때만 끄기. revive시 다시 켜야해
    protected IEnumerator Perception_CO()
    {
        bool firstTimeRotFixed = false;
        while (true)
        {
            if (isAutoAttack)
            {
                Collider[] cols = Physics.OverlapSphere(this.transform.position, perceptionRange, layerMask);
                if (cols.Length == 0) yield return new WaitForSeconds(0.001f);

                float nearCreepDistance = float.MaxValue;
                int minCreepIndex = 0;
                Vector3 nearestCreepPos = this.transform.position;

                //경로 중 제일 짧은 것을 선출.
                for (int i = 0; i < cols.Length; i++)
                {
                    GameObject creep = cols[i].gameObject;
                    Vector3 yFixedCreepPos = new Vector3(creep.transform.position.x, transform.position.y, creep.transform.position.z);

                    if (creep.GetComponent<ACreeps>().CanSee())
                    {
                        path = new NavMeshPath();
                        float distance = 0.0f;

                        if (player_navMesh.CalculatePath(yFixedCreepPos, path))
                        {
                            Vector3[] corners = path.corners;
                            for (int k = 0; k < corners.Length; k++)
                            {
                                if (k + 1 > corners.Length - 1) break;
                                Vector3 temp = new Vector3(0.0f, 5.0f, 0.0f);
                                Debug.DrawLine(corners[k] + temp, corners[k + 1], Color.green);
                                distance += Vector3.Distance(corners[k], corners[k + 1]);
                            }
                        }

                        if (nearCreepDistance > distance)
                        {
                            nearCreepDistance = distance;
                            minCreepIndex = i;
                            nearestCreepPos = yFixedCreepPos;
                        }
                    }

                }

                if (nearCreepDistance > attackRange)
                {
                    if (nearCreepDistance <= perceptionRange) targetPos = nearestCreepPos;
                }
                else
                {
                    isAttackable = true;
                    player_anim.SetBool("isAttackable", isAttackable);
                    objToAttack = cols[minCreepIndex].gameObject;

                    if (!firstTimeRotFixed)
                    {
                        firstTimeRotFixed = true;
                        this.transform.LookAt(nearestCreepPos);
                    }
                }
            }
            else
            {
                firstTimeRotFixed = false;
                isAttackable = false;
                player_anim.SetBool("isAttackable", isAttackable);

                objToAttack = null;
            }

            if (isAttackable && attackReady && objToAttack != null)
            {
                attackTimer = 0.0f;
                int rd_att = (int)Random.Range(0.0f, 2.0f);
                float clampAS = 2.0f - ((attackSpeed - 0.5f) / 1.5f);
                player_anim.SetFloat("AttackSpeedMultiplier", clampAS);
                player_anim.SetInteger("AttackRandomParam", rd_att);
                player_anim.SetTrigger("Attack");
                applyDmgOnce = true;
            }

            yield return new WaitForSeconds(0.001f);
        }
    }


    //public Function
    public bool IsMove()
    {
        return player_anim.GetBool("isMove");
    }

    public void SetAutoAttack(bool value)
    {
        this.isAutoAttack = value;
    }


    //Use In Canvas
    public float[] GetSkill_CT_Ratio()
    {
        return ct_ratio;
    }

    public float GetCurrentManaValue()
    {
        return currentMP;
    }

    public float GetMaxManaValue()
    {
        return maxMP;
    }

    public bool[] GetEnoughForSkills()
    {
        return canManaSkills;
    }

    public float GetCurrentHealthValue()
    {
        return currentHP;
    }

    public float GetMaxHealthValue()
    {
        return maxHP;
    }

    public bool IsDead()
    {
        return isDead;
    }

    //Spell public
    public void SetSpellIndicator(Spell_Indicator spell_indicator)
    {
        this.spell_indicator = spell_indicator;
    }

    public bool[] GetCurrentIndicatingSkills()
    {
        return currentIndicatingSkills;
    }

    public int GetCurrentIndicatorIndex()
    {
        return indicatorIndex;
    }

    public SpellInfo GetSpellInfo(int i )
    {
        return spellInfos[i];
    }

    public Camera GetPlayerCam()
    {
        return playerCam;
    }


}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;


public class Hero_FallenAngel : AHeroes
{
    private bool startESkill = false;
    private bool startRSkill = false;
    private bool rSkillDuration = false;
    private bool eSkillDuration = false;

    private Vector3 scale = Vector3.one;

    IEnumerator eSkillEnumerator;
    IEnumerator rSkillEnumerator;

    // Start is called before the first frame update
    void Start()
    {
        //float[] mana_costs = new float[4] { 30.0f, 70.0f, 100.0f, 120.0f };
        float[] mana_costs = new float[4] { 10.0f, 10.0f, 10.0f, 10.0f };
        //float[] coolTime_costs = new float[4] { 3.5f, 9.0f, 14.0f, 96.0f };
        float[] coolTime_costs = new float[4] { 2.0f, 2.0f, 2.0f, 10.0f };
        float[] skillDmgs = new float[4] { 120.0f, 30.0f, 250.0f, 0.0f };
        bool[] noCastSkills = new bool[4] { false, true, false, true };

        SpellInfo[] spellInfos = new SpellInfo[4];

        spellInfos[0].spellType = SpellType.FixedNonTarget;
        spellInfos[0].bound = 3.0f;
        spellInfos[0].proj_size = 2.0f;

        spellInfos[1].spellType = SpellType.NONE;
        spellInfos[1].proj_size = 2.0f;

        spellInfos[2].spellType = SpellType.FreeNonTarget;
        spellInfos[2].range = 7.5f;
        spellInfos[2].bound = 3.5f;
        spellInfos[2].proj_size = 3.5f;


        spellInfos[3].spellType = SpellType.NONE;
        spellInfos[3].proj_size = 2.0f;

        HeroesStart();

        SetSkill(mana_costs, coolTime_costs, skillDmgs, noCastSkills);
        SetStats(840.0f, 5.0f, 310.0f, 3.0f, 200.0f, 2.0f, 2.5f);
        SetComponents();
        SetCoroutine();
        SetSpellInfos(spellInfos);

        eSkillEnumerator = ESkill();
        rSkillEnumerator = RSkill();

        spell_indicator.GetComponent<Spell_Indicator>().SetPlayer(GameManager.player);

        StartCoroutine(eSkillEnumerator);
        StartCoroutine(rSkillEnumerator);
    }

    protected override void Death_Child()
    {
        if (rSkillDuration)
        {
            sk_cds[3] = 0.0f;
            rSkillDuration = false;
        }
    }

    protected override void Revive_Child()
    {
        spell_indicator.spellsize_multiplier = 1.0f;

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L)) currentHP = 0.0f;
        HeroesUpdate();
    }

    protected override IEnumerator MeleeAttack_CO()
    {
        while (true)
        {
            if (applyDmgOnce && player_anim.GetBool("A"))
            {
                applyDmgOnce = false;
                if (objToAttack != null) objToAttack.GetComponent<ACreeps>().GetHit(this.gameObject, attackDamage);
            }
            yield return new WaitForSeconds(0.001f);
        }
    }

    protected override bool Use_Q_Skill()
    {
        Vector3 target = spell_indicator.GetTargetPos();
        target.y = transform.position.y;


        player_navMesh.updateRotation = true;
        this.transform.LookAt(target);
        player_navMesh.updateRotation = false;

        player_anim.SetBool("isCasting", true);

        player_anim.SetTrigger("Q");

        currentMP -= sk_manas[0];

        return true;
    }

    protected override bool Use_W_Skill()
    {
        player_anim.SetBool("isCasting", true);
        player_anim.SetTrigger("W");

        currentMP -= sk_manas[1];
        return true;
    }

    protected override bool Use_E_Skill()
    {
        eSkillDuration = true;
        startESkill = true;

        player_anim.SetBool("isCasting", true);
        player_anim.SetTrigger("E");


        currentMP -= sk_manas[2];
        return true;
    }

    protected override bool Use_R_Skill()
    {
        rSkillDuration = true;
        startRSkill = true;

        player_anim.SetBool("isCasting", true);
        player_anim.SetTrigger("R");

        currentMP -= sk_manas[3];
        
        return false;
    }


    IEnumerator ESkill()
    {
        Vector3 target = Vector3.zero;
        Vector3 heroPos = Vector3.zero;
        float time = 0.0f;

        while (true)
        {
            if (startESkill)
            {
                startESkill = false;
                target = spell_indicator.GetTargetPos();
                target.y = transform.position.y;

                heroPos = transform.position;
                player_navMesh.updateRotation = true;
                this.transform.LookAt(target);
                useMoveSkillNow = true;
                time = 0.0f;
            }

     
            if (eSkillDuration)
            {
                float dist = Vector3.Distance(target, transform.position);
                float weight = 0.015f + time * 0.2f;
                time += Time.deltaTime;
                transform.position = Vector3.Lerp(transform.position, target, weight);

                if (dist < 0.05f)
                {
                    useMoveSkillNow = false;
                    targetPos = transform.position;
                    eSkillDuration = false;

                }
                yield return null;

            }

            yield return new WaitForSeconds(0.001f);
        }
    }


    //서서히 증가하다가 감소하는 
    IEnumerator RSkill()
    {
        float timer = 0.0f;
        while (true)
        {
            if (startRSkill)
            {
                startRSkill = false;
                indicatorIndex = -1;
                timer = 0.0f;
            }

            if (isDead)
            {
                timer = 0.0f;
                transform.localScale = Vector3.one;
                break;
            }


            

            if (rSkillDuration)
            {
                timer += Time.deltaTime;

                switch((int)timer)
                {
                    case 0:
                        transform.localScale = Vector3.one * (1 + timer * 0.75f);
                        spell_indicator.spellsize_multiplier = 1.0f;
                        break;
                    case 1:
                    case 2:
                    case 3:
                    case 4:
                    case 5:
                    case 6:
                        player_navMesh.speed = 5.0f;
                        spell_indicator.spellsize_multiplier = 1.75f;
                        break;
                    case 7:
                        transform.localScale = Vector3.one* 1.75f - Vector3.one * ((timer - 7) * 0.75f);
                        break;
                    case 8:
                    default:
                        transform.localScale = Vector3.one;
                        player_navMesh.speed = 3.0f;
                        spell_indicator.spellsize_multiplier = 1.0f;
                        indicatorIndex = -1;
                        sk_cds[3] = 0.0f;
                        rSkillDuration = false;
                        break;
                }



                yield return null;
            }

            yield return new WaitForSeconds(0.001f);
        }
    }

}

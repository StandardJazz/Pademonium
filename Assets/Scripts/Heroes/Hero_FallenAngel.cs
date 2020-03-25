using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;


public class Hero_FallenAngel : AHeroes
{
    private bool rSkillDuration = false;
    private Vector3 scale = Vector3.one;

    // Start is called before the first frame update
    void Start()
    {
        float[] mana_costs = new float[4] { 30.0f, 70.0f, 100.0f, 120.0f };
        float[] coolTime_costs = new float[4] { 3.5f, 9.0f, 14.0f, 96.0f };
        float[] skillDmgs = new float[4] { 120.0f, 30.0f, 250.0f, 0.0f };
        bool[] noCastSkills = new bool[4] { false, true, false, true };

        SpellInfo[] spellInfos = new SpellInfo[4];

        spellInfos[0].spellType = SpellType.FixedNonTarget;
        spellInfos[0].bound = 3.0f;
        spellInfos[0].proj_size = 2.0f;

        spellInfos[1].spellType = SpellType.NONE;

        spellInfos[2].spellType = SpellType.FreeNonTarget;
        spellInfos[2].range = 15.0f;
        spellInfos[2].bound = 2.5f;
        spellInfos[0].proj_size = 2.0f;


        spellInfos[3].spellType = SpellType.NONE;

        HeroesStart();

        SetSkill(mana_costs, coolTime_costs, skillDmgs, noCastSkills);
        //SetStats(840.0f, 5.0f, 310.0f, 3.0f, 56.0f, 0.5f, 2.5f);
        SetStats(840.0f, 5.0f, 310.0f, 3.0f, 200.0f, 2.0f, 2.5f);
        SetComponents();
        SetCoroutine();
        SetSpellInfos(spellInfos);
    }

    protected override void Death_Child()
    {
        if (rSkillDuration)
        {
            sk_cds[3] = 0.0f;
            rSkillDuration = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        HeroesUpdate();
    }


    //서서히 증가하다가 감소하는 
    IEnumerator RSkill()
    {
        indicatorIndex = -1;
        while (true)
        {
            if (isDead)
            {
                StopCoroutine(RSkill());
                break;
            }

            if (rSkillDuration)
            {
                if (transform.localScale.x < 1.74f)
                {
                    player_navMesh.speed = 4.5f;
                    transform.localScale += new Vector3(0.05f, 0.05f, 0.05f);
                    yield return new WaitForSeconds(0.2f);
                }
                else
                {
                    rSkillDuration = false;
                    yield return new WaitForSeconds(8.0f);
                }
            }
            else
            {
                if (transform.localScale.x > 1.0f)
                {
                    transform.localScale -= new Vector3(0.05f, 0.05f, 0.05f);
                    yield return new WaitForSeconds(0.2f);  //3초
                }
                else
                {
                    player_navMesh.speed = 2.0f;
                    transform.localScale = new Vector3(1, 1, 1);
                    indicatorIndex = -1;
                    sk_cds[3] = 0.0f;
                    break;
                }
            }
        }
        StopCoroutine(RSkill());
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
        this.transform.rotation = spell_indicator.GetSpellDirection();
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
        this.transform.rotation = spell_indicator.GetSpellDirection();
        player_navMesh.updateRotation = false;

        player_anim.SetBool("isCasting", true);
        player_anim.SetTrigger("E");

        currentMP -= sk_manas[2];
        return true;
    }

    protected override bool Use_R_Skill()
    {
        player_anim.SetBool("isCasting", true);
        player_anim.SetTrigger("R");

        currentMP -= sk_manas[3];
        rSkillDuration = true;
        StartCoroutine(RSkill());
        return false;
    }

}

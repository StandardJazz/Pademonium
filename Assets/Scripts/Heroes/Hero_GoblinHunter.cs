using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class Hero_GoblinHunter : AHeroes
{
    GameObject potion = null;
    bool isThrowWPotion = false;
    

    void Awake()
    {
        potion = transform.Find("metarig").Find("shNoj.L.").gameObject;
        potion.transform.localScale = Vector3.one * 0.005f;
    }

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
    }

    // Update is called once per frame
    void Update()
    {
        HeroesUpdate();

        //if(!isThrowWPotion && player_anim.GetBool("ThrowWPotion"))
        //{
        //    isThrowWPotion = true;
        //    potion.SetActive(false);
        //}
    }

    protected override void Death_Child()
    {
        base.Death_Child();
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

    protected override void Revive_Child()
    {
        base.Revive_Child();
    }

    protected override bool Use_E_Skill()
    {
        throw new System.NotImplementedException();
    }

    protected override bool Use_Q_Skill()
    {
        throw new System.NotImplementedException();
    }

    protected override bool Use_R_Skill()
    {
        throw new System.NotImplementedException();
    }

    protected override bool Use_W_Skill()
    {
        throw new System.NotImplementedException();
    }


}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

/*
    Q 회피 사격 : 점프한 후 뒤로 이동, -> 전방으로 화살 한 발 사격 

    W 끈적이 풀 : 해당 지점에 끈적이 풀을 던진다. 해당 지점에서 슬로우가 걸린다. 

    E 독성 화살 촉 : 화살에 독을 묻혀, 추가 데미지를 준다. 해당 독데미지는 3초간 지속된다. 

    R 화살 파티 : 전방에 원뿔 모양으로 빠르게 화살을 10번 10발씩 쏜다. 
 */

public class Hero_GoblinHunter : AHeroes
{
    GameObject potion = null;
    GameObject[] regularArrows = null;
    GameObject thrownPotion = null;

    bool readyShootPotion = false;
    bool potionTrigger = false;

    private Vector3 single_attack_targetPos = Vector3.zero;

    IEnumerator WSkill;

    void Awake()
    {
        potion = transform.Find("metarig").Find("shNoj.L.").gameObject;

        thrownPotion = Instantiate(Resources.Load("GH_StickyBomb") as GameObject, new Vector3(0.0f, 1.0f, 0.0f), Quaternion.identity);
        thrownPotion.SetActive(false);

        regularArrows = new GameObject[10];
        GameObject temp = Resources.Load("GH_RegularArrow") as GameObject;
        for (int i =0; i < regularArrows.Length;i++)
        {
            regularArrows[i] = Instantiate(temp, new Vector3 (0.0f,1.0f,0.0f),Quaternion.identity);
            regularArrows[i].SetActive(false);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        //float[] mana_costs = new float[4] { 30.0f, 70.0f, 100.0f, 120.0f };
        float[] mana_costs = new float[4] { 10.0f, 10.0f, 10.0f, 10.0f };
        //float[] coolTime_costs = new float[4] { 3.5f, 9.0f, 14.0f, 96.0f };
        float[] coolTime_costs = new float[4] { 2.0f, 2.0f, 2.0f, 10.0f };
        float[] skillDmgs = new float[4] { 120.0f, 30.0f, 250.0f, 0.0f };
        bool[] noCastSkills = new bool[4] { false, false, true, false };

        SpellInfo[] spellInfos = new SpellInfo[4];

        spellInfos[0].spellType = SpellType.FixedNonTarget;
        spellInfos[0].bound = 3.0f;
        spellInfos[0].proj_size = 2.0f;

        spellInfos[1].spellType = SpellType.FreeNonTarget;
        spellInfos[1].range = 7.5f;
        spellInfos[1].bound = 3.5f;
        spellInfos[1].proj_size = 2.0f;

        spellInfos[2].spellType = SpellType.NONE;
        spellInfos[2].proj_size = 3.5f;

        spellInfos[3].spellType = SpellType.FixedNonTarget;
        spellInfos[3].bound = 3.0f;
        spellInfos[3].proj_size = 2.0f;

        HeroesStart();

        SetSkill(mana_costs, coolTime_costs, skillDmgs, noCastSkills);
        SetStats(500.0f, 3.0f, 200.0f, 3.0f, 200.0f, 2.0f, 6.0f);
        SetComponents();
        SetCoroutine();
        SetSpellInfos(spellInfos);

        WSkill = WSkill_CO();
        StartCoroutine(WSkill);
    }

    // Update is called once per frame
    void Update()
    {
        HeroesUpdate();
        readyShootPotion = canUseSkills[1];
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

                for (int i = 0; i < regularArrows.Length; i++)
                {
                    if(!regularArrows[i].activeSelf)
                    {
                        Vector3 temp = (transform.up *1.25f + transform.forward).normalized;
                        regularArrows[i].SetActive(true);
                        regularArrows[i].GetComponent<GH_RegularArrow>().SetTarget(this.transform.position + temp * 1.5f, objToAttack, attackDamage);
                        break;
                    }
                }
                
            }
            yield return new WaitForSeconds(0.001f);
        }
    }

    protected override void Revive_Child()
    {
        base.Revive_Child();
    }

    protected override bool Use_Q_Skill()
    {
        throw new System.NotImplementedException();
    }

    protected override bool Use_W_Skill()
    {
        Vector3 target = spell_indicator.GetTargetPos();
        target.y = transform.position.y;


        player_navMesh.updateRotation = true;
        this.transform.LookAt(target);
        player_navMesh.updateRotation = false;

        player_anim.SetBool("isCasting", true);
        player_anim.SetTrigger("W");
        potionTrigger = true;

        currentMP -= sk_manas[2];
        return true;
    }

    protected override bool Use_E_Skill()
    {
        throw new System.NotImplementedException();
    }
    
    protected override bool Use_R_Skill()
    {
        throw new System.NotImplementedException();
    }


    IEnumerator WSkill_CO()
    {
        while(true)
        {
            if (potionTrigger && player_anim.GetBool("ThrowWPotion"))
            {
                Vector3 temp = (transform.up + transform.forward).normalized;
                thrownPotion.GetComponent<GH_StickyBomb>().ActivateBomb(this.transform.position + temp *  1.5f, spell_indicator.GetTargetPos());

                potionTrigger = false;
                potion.SetActive(false);
            }

            yield return new WaitForSeconds(0.001f);
        }
    }
}

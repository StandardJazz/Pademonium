using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Canvas : MonoBehaviour
{
    [SerializeField] private GameObject hero_player = null;

    [SerializeField] private Image HP_bar = null;
    [SerializeField] private Image MP_bar = null;
    [SerializeField] private Image[] skill_boxes = new Image[4];

    delegate float[] Skill_CoolTime_FuncPtr();
    Skill_CoolTime_FuncPtr GetCT_Ratio = null;

    delegate float Mana_Current_FuncPtr();
    Mana_Current_FuncPtr GetCurrentMana = null;

    delegate float Mana_Max_FuncPtr();
    Mana_Max_FuncPtr GetMaxMana = null;

    delegate bool[] Is_EnoughManForSkills();
    Is_EnoughManForSkills GetEnoughManaSkills = null;

    delegate float Health_Current_FuncPtr();
    Health_Current_FuncPtr GetCurrentHealth = null;

    delegate float Health_Max_FuncPtr();
    Health_Max_FuncPtr GetMaxHealth = null;

    void Start()
    {
        hero_player = GameManager.player;
        AHeroes hero_script = hero_player.GetComponent<AHeroes>(); 
        GetCT_Ratio = hero_script.GetSkill_CT_Ratio;

        GetCurrentMana = hero_script.GetCurrentManaValue;
        GetMaxMana = hero_script.GetMaxManaValue;
        GetEnoughManaSkills = hero_script.GetEnoughForSkills;

        GetCurrentHealth = hero_script.GetCurrentHealthValue;
        GetMaxHealth = hero_script.GetMaxHealthValue;
    }

    void Update()
    {
        for(int i = 0; i <4;i++)
        {
            skill_boxes[i].fillAmount = GetCT_Ratio()[i];
            skill_boxes[i].color = GetEnoughManaSkills()[i] ? Color.white : Color.red;
        }


        MP_bar.fillAmount = GetCurrentMana() / GetMaxMana();
        HP_bar.fillAmount = GetCurrentHealth() / GetMaxHealth();
    }
    
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HPMPBarController : MonoBehaviour
{
    [SerializeField] private GameObject pf_creep_hp = null;
    List<GameObject> CreepList = new List<GameObject>();
    List<GameObject> CreepBarList = new List<GameObject>();
    List<Image> CreepBarImgList = new List<Image>();


    [SerializeField] private GameObject pf_hero_bar = null;

    GameObject hero;
    GameObject hero_bar;
    
    private Image hero_hp = null;
    private Image hero_mp = null;
    private Text hero_lv = null;


    Camera cam;


    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
        hero = GameManager.player;

        GameObject[] creeps = GameObject.FindGameObjectsWithTag("Creep");

        foreach (GameObject creep in creeps)
        {
            CreepList.Add(creep);
            GameObject temp = Instantiate(pf_creep_hp, creep.transform.position, Quaternion.identity, transform);
            CreepBarList.Add(temp);
            CreepBarImgList.Add(temp.GetComponentsInChildren<Image>()[1]);
        }


        //hero
        hero_bar = Instantiate(pf_hero_bar, hero.transform.position, Quaternion.identity, transform);

        Image[] hero_imgs = hero_bar.GetComponentsInChildren<Image>();
        hero_lv = pf_hero_bar.GetComponentInChildren<Text>();
        hero_hp = hero_imgs[1];
        hero_mp = hero_imgs[2];
    }

    void ShowCreepBar()
    {
        for (int i = 0; i < CreepBarList.Count; i++)
        {
            if (CreepList[i] != null && CreepList[i].GetComponent<ACreeps>().IsDead())
            {
                Destroy(CreepList[i]);
                Destroy(CreepBarList[i]);
                Destroy(CreepBarImgList[i]);

                CreepList.RemoveAt(i);
                CreepBarList.RemoveAt(i);
                CreepBarImgList.RemoveAt(i);
                continue;
            }

            Vector3 viewportPt = cam.WorldToViewportPoint(CreepList[i].transform.position + new Vector3(0.0f, 3.0f, 0.0f));

            if (viewportPt.x > 0.0f && viewportPt.y > 0.0f && viewportPt.x < 1.0f && viewportPt.y < 1.0f)
            { 
                CreepBarList[i].SetActive(true);
                CreepBarList[i].transform.position = cam.ViewportToScreenPoint(viewportPt);
            }
            else
            {
                CreepBarList[i].SetActive(false);
            }

            CreepBarImgList[i].fillAmount = CreepList[i].GetComponent<ACreeps>().GetHPRatio();
        }
    }


    void ShowHeroBar()
    {
        Vector3 viewportPt = cam.WorldToViewportPoint(hero.transform.position + new Vector3(0.0f, 3.5f, 0.0f));
        AHeroes tempHeroScript = hero.GetComponent<AHeroes>();

        bool bCheck = true;
        bCheck &= viewportPt.x > 0.0f && viewportPt.y > 0.0f && viewportPt.x < 1.0f && viewportPt.y < 1.0f; //안에 있을 때,
        bCheck &= !tempHeroScript.IsDead();

        hero_bar.SetActive(bCheck);


        if (bCheck)
        {
            hero_bar.transform.position = cam.ViewportToScreenPoint(viewportPt);

            hero_hp.fillAmount = tempHeroScript.GetCurrentHealthValue() / tempHeroScript.GetMaxHealthValue();
            hero_mp.fillAmount = tempHeroScript.GetCurrentManaValue() / tempHeroScript.GetMaxManaValue();
        }
    }

   

    // Update is called once per frame
    void Update()
    {
        ShowCreepBar();
        ShowHeroBar();
    }


    public void AddCreep(GameObject creep)
    {
        CreepList.Add(creep);
    }
}

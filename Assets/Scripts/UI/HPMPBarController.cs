using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HPMPBarController : MonoBehaviour
{
    public GameObject gameManager = null;
    //CROWD_CONTROL_TYPE
    [SerializeField] private GameObject pf_creep_hp = null;
    List<GameObject> CreepList = new List<GameObject>();

    List<GameObject> NewCreepBarList = new List<GameObject>();
    List<Image> creepBarImgs = new List<Image>();
    List<Image[]> creepCCBarImgs = new List<Image[]>();
    Sprite[] cc_icons = new Sprite[4];

    [SerializeField] private GameObject pf_hero_bar = null;

    delegate List<GameObject> GetCreepList();
    GetCreepList getCreepList = null;


    GameObject hero;
    GameObject hero_bar;

    private Image hero_hp = null;
    private Image hero_mp = null;
    private Text hero_lv = null;


    Camera cam;

    void Awake()
    {

        cc_icons[0] = Resources.Load<Sprite>("Sprites/CC_Icons/non_icon");
        cc_icons[1] = Resources.Load<Sprite>("Sprites/CC_Icons/Slow_icon");
        cc_icons[2] = Resources.Load<Sprite>("Sprites/CC_Icons/Root_icon");
        cc_icons[3] = Resources.Load<Sprite>("Sprites/CC_Icons/Stun_icon");

        GameObject temp = null;

        for (int i = 0; i < 20; i++)
        {
            NewCreepBarList.Add(Instantiate(Resources.Load<GameObject>("cr_bar"), new Vector3(0, 0, 0), Quaternion.identity, transform));
            Image[] tempimgs =  NewCreepBarList[i].GetComponentsInChildren<Image>();
            creepBarImgs.Add(tempimgs[1]);
            tempimgs[2].sprite = cc_icons[0];
            tempimgs[3].sprite = cc_icons[0];
            tempimgs[4].sprite = cc_icons[0];

            creepCCBarImgs.Add(new Image[3] { tempimgs[2], tempimgs[3], tempimgs[4] });

            NewCreepBarList[i].SetActive(false);
        }

      
    }


    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
        hero = GameManager.player;
        getCreepList = gameManager.GetComponent<CreepsManager>().GetCreepList;

        int index = 0;
        foreach(var temp in getCreepList())
        {
            NewCreepBarList[index].SetActive(true);
            index++;
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
        int index = 0;
        foreach(var tempCreep in getCreepList())
        {
            if(tempCreep.GetComponent<ACreeps>().IsDead())
            {
                NewCreepBarList[index].SetActive(false);

            }
            else
            {
                Vector3 viewportPt = cam.WorldToViewportPoint(tempCreep.transform.position + new Vector3(0.0f, 3.0f, 0.0f));
                if (viewportPt.x > 0.0f && viewportPt.y > 0.0f && viewportPt.x < 1.0f && viewportPt.y < 1.0f)
                {
                    NewCreepBarList[index].SetActive(true);
                    NewCreepBarList[index].transform.position = cam.ViewportToScreenPoint(viewportPt);

                    CROWD_CONTROL_TYPE ctype = tempCreep.GetComponent<ACreeps>().GetCCType();
                    int i = 0;
                    if(ctype == CROWD_CONTROL_TYPE.NONE)
                    {
                        creepCCBarImgs[index][0].sprite = cc_icons[0];
                        creepCCBarImgs[index][1].sprite = cc_icons[0];
                        creepCCBarImgs[index][2].sprite = cc_icons[0];
                    }
                    else
                    {
                        if ((ctype & CROWD_CONTROL_TYPE.STUN) == CROWD_CONTROL_TYPE.STUN)
                            creepCCBarImgs[index][i++].sprite = cc_icons[3];
                        if ((ctype & CROWD_CONTROL_TYPE.ROOT) == CROWD_CONTROL_TYPE.ROOT)
                            creepCCBarImgs[index][i++].sprite = cc_icons[2];
                        if ((ctype & CROWD_CONTROL_TYPE.SLOW) == CROWD_CONTROL_TYPE.SLOW)
                            creepCCBarImgs[index][i++].sprite = cc_icons[1];
                    }
                }
                else
                {
                    NewCreepBarList[index].SetActive(false);
                }

                creepBarImgs[index].fillAmount = tempCreep.GetComponent<ACreeps>().GetHPRatio();
            }

            index++;
        }
        //for (int i = 0; i < NewCreepBarList.Count; i++)
        //{
        //    if (CreepList[i] != null && CreepList[i].GetComponent<ACreeps>().IsDead())
        //    {
                
        //        continue;
        //    }

        //    Vector3 viewportPt = cam.WorldToViewportPoint(CreepList[i].transform.position + new Vector3(0.0f, 3.0f, 0.0f));

        //    if (viewportPt.x > 0.0f && viewportPt.y > 0.0f && viewportPt.x < 1.0f && viewportPt.y < 1.0f)
        //    {
        //        CreepBarList[i].SetActive(true);
        //        CreepBarList[i].transform.position = cam.ViewportToScreenPoint(viewportPt);
        //    }
        //    else
        //    {
        //        CreepBarList[i].SetActive(false);
        //    }

        //    CreepBarImgList[i].fillAmount = CreepList[i].GetComponent<ACreeps>().GetHPRatio();
        //}
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

    }
}

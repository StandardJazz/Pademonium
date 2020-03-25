using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spell_Indicator : MonoBehaviour
{
    private FollowCam cam = null;
    private GameObject heroPlayer = null;
    private AHeroes heroCompo = null;

    private Projector spell_indicator = null;
    private int index = -1;

    private SpellType spellType = SpellType.NONE;
    private float spellbound = 0.0f;
    private float spellRange = 0.0f;
    private float spellProjSize = 0.0f;

    private Quaternion spell_rot;
    private Vector3 target_pos;

    void Awake()
    {
        spell_indicator = this.gameObject.GetComponentInChildren<Projector>();
    }

    // Start is called before the first frame update
    void Start()
    {
        heroCompo = heroPlayer.GetComponent<AHeroes>();
        cam = Camera.main.GetComponent<FollowCam>();
        spell_indicator.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
            spell_indicator.material.SetInt("_TextureIndex", 2);

        if (index != heroCompo.GetCurrentIndicatorIndex())
        {
            index = heroCompo.GetCurrentIndicatorIndex();

            if (index >= 0 && heroCompo.GetCurrentIndicatingSkills()[index]) 
            {
                spell_indicator.enabled = true;
                spell_indicator.material.SetInt("_TextureIndex", index);

                AHeroes.SpellInfo temp = heroCompo.GetSpellInfo(index);
                spellType = temp.spellType;
                spellbound = temp.bound;
                spellRange = temp.range;
                spellProjSize = temp.proj_size;

                spell_indicator.orthographicSize = temp.proj_size;
                print(index + "번째 indicator 출력중");
            }
            else
            {
                spell_indicator.enabled = false;
            }

        }

        if(spell_indicator.enabled)
        {
            Vector3 mousePos = cam.GetMousePoint();
            float y = mousePos.y;

            Transform tempT = this.gameObject.transform;

            Vector3 heroPos = heroPlayer.transform.position;

            mousePos.y = heroPos.y = 5.5f;
            Vector3 dir = (mousePos - heroPos).normalized;

            switch (spellType)
            {
                case SpellType.FixedNonTarget:
                    {
                        tempT.position = heroPos + (spellbound * 0.5f) * dir;
                        tempT.rotation = Quaternion.LookRotation(dir, Vector3.up);
                    }
                    break;

                case SpellType.FreeNonTarget:
                    {
                        if(Vector3.Distance(heroPos, mousePos) < spellRange)
                        {
                            print("freenontarget");
                            tempT.position = mousePos;
                            //tempT.rotation = Quaternion.LookRotation(dir, Vector3.up);
                        }
                    }
                    break;

            }
            
            target_pos = mousePos;
            target_pos.y = y;
        }

    }

    public void SetPlayer(GameObject player)
    {
        heroPlayer = player;
    }

    public Vector3 GetTargetPos()
    {
        return target_pos;
    }
}

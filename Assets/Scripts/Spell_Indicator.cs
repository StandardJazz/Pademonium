using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spell_Indicator : MonoBehaviour
{
    private FollowCam cam = null;
    private GameObject heroPlayer = null;
    private AHeroes heroCompo = null;

    private Projector spell_indicator = null;
    private Projector spell_Second_indicator = null;
    private int index = -1;

    private SpellType spellType = SpellType.NONE;
    private float spellbound = 0.0f;
    private float spellRange = 0.0f;
    private float spellProjSize = 0.0f;
    public float spellsize_multiplier = 1.0f;

    private Quaternion spell_rot;
    private Vector3 target_pos;

    void Awake()
    {
        Projector[] projectors = this.gameObject.GetComponentsInChildren<Projector>();
        spell_indicator = projectors[0];
        spell_Second_indicator = projectors[1];
    }

    // Start is called before the first frame update
    void Start()
    {
        heroPlayer = GameManager.player;
        heroCompo = heroPlayer.GetComponent<AHeroes>();
        cam = Camera.main.GetComponent<FollowCam>();
        spell_indicator.enabled = false;
        spell_Second_indicator.enabled = false;
        spellsize_multiplier = 1.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (heroCompo.IsDead())
        {
            spell_indicator.enabled = false;
            spell_Second_indicator.enabled = false;
            return;
        }

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

                spell_indicator.orthographicSize = temp.proj_size * spellsize_multiplier;

                if (spellType == SpellType.FreeNonTarget)
                {
                    spell_Second_indicator.enabled = true;
                    spell_Second_indicator.material.SetInt("_TeuxtreIndex", index);
                    spell_Second_indicator.orthographicSize = spellRange + 3.0f;
                }
            }
            else
            {
                spell_indicator.enabled = false;
                spell_Second_indicator.enabled = false;
            }

        }

        Vector3 mousePos = cam.GetMousePoint();
        float height = mousePos.y;
        Vector3 heroPos = heroPlayer.transform.position;
        mousePos.y = heroPos.y = 5.5f;
        Vector3 dir = (mousePos - heroPos).normalized;


        if (spell_indicator.enabled)
        {
            this.gameObject.transform.rotation = Quaternion.LookRotation(dir, Vector3.up);

            switch (spellType)
            {
                case SpellType.FixedNonTarget:
                    spell_indicator.transform.position = heroPos + (spellbound * 0.5f) * dir;
                    break;
                case SpellType.FreeNonTarget:
                    if (Vector3.Distance(heroPos, mousePos) < spellRange)
                        spell_indicator.transform.position = mousePos;
                    else
                        spell_indicator.transform.position = heroPos + spellRange * dir;
                    break;

            }
            target_pos = spell_indicator.transform.position;
            target_pos.y = height;


        }

        if (spell_Second_indicator.enabled)
            spell_Second_indicator.transform.position = heroPos;
        

    }

    public Vector3 GetTargetPos()
    {
        return target_pos;
    }
}

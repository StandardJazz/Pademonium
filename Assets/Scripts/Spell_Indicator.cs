using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spell_Indicator : MonoBehaviour
{
    private GameObject heroPlayer = null;
    private AHeroes heroCompo = null;

    private Projector spell_indicator = null;
    private int index = -1;

    void Awake()
    {
        spell_indicator = this.gameObject.GetComponent<Projector>();
    }

    // Start is called before the first frame update
    void Start()
    {
        heroPlayer = GameManager.player;
        heroCompo = heroPlayer.GetComponent<AHeroes>();
    }

    // Update is called once per frame
    void Update()
    {
        if(index != heroCompo.GetCurrentIndicatorIndex())
        {
            index = heroCompo.GetCurrentIndicatorIndex();

            if (index < 0)
                spell_indicator.enabled = false;
            else
            {
                spell_indicator.enabled = true;
                spell_indicator.material.SetInt("_TextureIndex", index);
            }
        }
    }
}

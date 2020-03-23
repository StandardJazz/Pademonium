using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroSpotLight : MonoBehaviour
{
    [SerializeField] private AHeroes go_hero = null;

    void Start()
    {
        go_hero = GameManager.player.GetComponent<AHeroes>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!go_hero.IsDead())
        {
            this.transform.position = new Vector3(go_hero.transform.position.x, 5.0f, go_hero.transform.position.z);
            this.gameObject.GetComponent<Light>().enabled = true;
        }
        else
        {
            this.gameObject.GetComponent<Light>().enabled = false;
        }
    }
}

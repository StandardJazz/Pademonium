using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestDummyBot : ACreeps
{
    void Start()
    {
        CreepStart();
        max_hp = 1000.0f;
        creep_anim = this.GetComponent<Animator>();
        current_hp = max_hp;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestDummyBot : ACreeps
{
    void Start()
    {
        CreepsStart();
        
        SetStats(300.0f, 3.0f, 45.0f, 3.0f, 2.0f);
        SetComponents();
        SetCoroutine();
    }

}
